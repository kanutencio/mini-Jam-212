using UnityEngine;
using System;
using System.Collections;

public class HeroMover : MonoBehaviour
{
    [Header("Stats")]
    public float velocidad = 3f;
    public float vidaMaxima = 100f;

    [Header("Eventos")]
    public Action onReachedEnd;
    public Action onDeath;

    [Header("Movimiento Casilla por Casilla")]
    [Tooltip("Permite pausar o reanudar el movimiento desde código o inspector.")]
    public bool puedeMoverse = true;
    [Tooltip("Tiempo de pausa en segundos que hace el personaje al llegar a cada casilla antes de seguir.")]
    public float tiempoPausaEnCasilla = 0f;

    private float _timerPausa = 0f;

    [Header("Efectos Visuales")]
    [Tooltip("Prefab del sistema de partículas para cuando el personaje da un paso en una casilla.")]
    public GameObject prefabParticulasPaso;

    public Action onPaso; // Evento que se dispara en cada paso por si quieres programar más cosas de sonido o lógica.

    [Header("UI")]
    [Tooltip("El objeto padre de la barra de vida (para evitar que rote con el héroe)")]
    public Transform healthBarContainer;
    [Tooltip("La imagen de UI (Image) que representa la vida actual")]
    public UnityEngine.UI.Image barraVidaRelleno;

    private float vidaActual;
    private WaypointPath currentPath;
    private int targetWaypointIndex = 1;
    private bool isMoving = false;

    public float Progress { get; private set; } = 0f;

    private Vector3 _healthBarOffset;

    void Awake()
    {
        vidaActual = vidaMaxima;
        if (healthBarContainer != null)
        {
            // Guardamos la distancia inicial (local) a la que estaba la barra
            _healthBarOffset = healthBarContainer.localPosition;
        }
        ActualizarBarraVida();
    }

    public void SetPath(WaypointPath path)
    {
        currentPath = path;
        targetWaypointIndex = 1;
        Progress = 0f;

        if (path != null && path.WaypointCount > 0)
        {
            Vector3 start = path.GetWaypoint(0).position;
            transform.position = new Vector3(start.x, start.y, 0f);
            isMoving = true;
        }
    }

    void Update()
    {
        // CONDICIONAL: Si no tiene permiso de moverse, no avanza ni procesa movimiento
        if (!puedeMoverse) return;

        if (!isMoving || currentPath == null) return;

        // Si está haciendo una pausa en la casilla actual
        if (_timerPausa > 0f)
        {
            _timerPausa -= Time.deltaTime;
            return;
        }

        Mover();
        UpdateProgress();
    }

    void LateUpdate()
    {
        if (healthBarContainer != null)
        {
            // Bloqueamos la rotación para que siempre esté horizontal
            healthBarContainer.rotation = Quaternion.identity;
            // Bloqueamos la posición en el mundo para que siempre esté justo arriba del héroe
            healthBarContainer.position = transform.position + _healthBarOffset;
        }
    }

    void Mover()
    {
        if (targetWaypointIndex >= currentPath.WaypointCount)
        {
            isMoving = false;
            onReachedEnd?.Invoke();
            return;
        }

        Vector3 targetPos = currentPath.GetWaypoint(targetWaypointIndex).position;
        targetPos.z = 0f;
        Vector3 current = new Vector3(transform.position.x, transform.position.y, 0f);

        Vector3 dir = (targetPos - current).normalized;
        transform.position = Vector3.MoveTowards(current, targetPos, velocidad * Time.deltaTime);

        if (dir != Vector3.zero)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            targetWaypointIndex++;
            _timerPausa = tiempoPausaEnCasilla; // Activa la pausa al tocar la casilla

            // Lanzar efectos del paso
            onPaso?.Invoke();
            if (prefabParticulasPaso != null)
            {
                Instantiate(prefabParticulasPaso, transform.position, Quaternion.identity);
            }
        }
    }

    void UpdateProgress()
    {
        float totalLength = currentPath.GetTotalLength();
        if (totalLength <= 0f) return;

        float accumulated = 0f;
        for (int i = 0; i < targetWaypointIndex - 1 && i < currentPath.WaypointCount - 1; i++)
            accumulated += Vector3.Distance(
                currentPath.GetWaypoint(i).position,
                currentPath.GetWaypoint(i + 1).position);

        if (targetWaypointIndex < currentPath.WaypointCount)
        {
            Transform prev = currentPath.GetWaypoint(targetWaypointIndex - 1);
            Transform next = currentPath.GetWaypoint(targetWaypointIndex);
            float segLen = Vector3.Distance(prev.position, next.position);
            float distInSeg = segLen - Vector3.Distance(transform.position, next.position);
            accumulated += Mathf.Max(0f, distInSeg);
        }

        Progress = accumulated / totalLength;
    }

    public void RecibirDaño(float cantidad)
    {
        vidaActual -= cantidad;
        ActualizarBarraVida();

        if (vidaActual <= 0f)
        {
            isMoving = false;
            onDeath?.Invoke();
            Destroy(gameObject);
        }
    }

    void ActualizarBarraVida()
    {
        if (barraVidaRelleno != null)
        {
            barraVidaRelleno.fillAmount = Mathf.Clamp01(vidaActual / vidaMaxima);
        }
    }

    public void Ralentizar(float factor, float duracion)
    {
        StartCoroutine(EfectoRalentizar(factor, duracion));
    }

    IEnumerator EfectoRalentizar(float factor, float duracion)
    {
        float velOriginal = velocidad;
        velocidad *= factor;
        yield return new WaitForSeconds(duracion);
        velocidad = velOriginal;
    }

    public float GetVidaPorcentaje() => vidaActual / vidaMaxima;
}