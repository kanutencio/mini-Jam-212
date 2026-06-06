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

    [Header("UI")]
    [Tooltip("El objeto padre de la barra de vida (para evitar que rote con el héroe)")]
    public Transform healthBarContainer;
    [Tooltip("El sprite o imagen que representa la vida actual (se escalará en X)")]
    public Transform barraVidaRelleno;

    private float vidaActual;
    private WaypointPath currentPath;
    private int targetWaypointIndex = 1;
    private bool isMoving = false;

    public float Progress { get; private set; } = 0f;

    void Awake()
    {
        vidaActual = vidaMaxima;
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
        // Evitar que la barra de vida rote con el personaje
        if (healthBarContainer != null)
        {
            healthBarContainer.rotation = Quaternion.identity;
        }

        if (!isMoving || currentPath == null) return;
        Mover();
        UpdateProgress();
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
            targetWaypointIndex++;
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
            // Escala el objeto en el eje X según el porcentaje de vida restante
            Vector3 scale = barraVidaRelleno.localScale;
            scale.x = Mathf.Clamp01(vidaActual / vidaMaxima);
            barraVidaRelleno.localScale = scale;
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