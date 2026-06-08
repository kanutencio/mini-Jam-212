using UnityEngine;

/// <summary>
/// CAÑON — Sistema de apuntado y disparo totalmente reescrito.
///
/// SETUP EN UNITY:
/// ─────────────────────────────────────────────────────────────────
/// Canon (GameObject vacío — este script va aquí)
/// ├── Base          ← sprite de la base/pedestal (no rota, optional)
/// └── CabezaDelCanon ← sprite del cañón (la parte que gira)
///     └── PuntoDisparo  ← GameObject vacío en la BOCA del cañón,
///                          en posición local X > 0 (hacia la derecha del sprite)
/// ─────────────────────────────────────────────────────────────────
///
/// El sprite del cañón debe estar dibujado apuntando a la DERECHA (+X local).
/// Si tu sprite apunta en otra dirección, róta el SpriteRenderer dentro de
/// CabezaDelCanon sin mover el objeto padre.
///
/// El script rota CabezaDelCanon en world-space, por lo que no importa
/// la rotación del GameObject padre "Canon".
/// </summary>
public class Canon : MonoBehaviour
{
    // ─── Stats ───────────────────────────────────────────────────────────
    [Header("Stats")]
    [Tooltip("Daño que hace cada bala.")]
    public float daño = 30f;

    [Tooltip("Velocidad de la bala en unidades/segundo.")]
    public float velocidadBala = 8f;

    [Tooltip("Tiempo en segundos entre disparos.")]
    public float tiempoEntreDisparos = 2f;

    [Tooltip("Distancia máxima de detección del enemigo.")]
    public float rango = 5f;

    // ─── Referencias ─────────────────────────────────────────────────────
    [Header("Referencias")]
    [Tooltip("Prefab de la bala a instanciar.")]
    public GameObject balaPrefab;

    [Tooltip("Hijo que contiene el sprite del cañón. ESTE objeto rota para apuntar.")]
    public Transform cabezaDelCanon;

    [Tooltip("Punto vacío en la boca del cañón (hijo de CabezaDelCanon).")]
    public Transform puntoDisparo;

    // ─── Sonido ───────────────────────────────────────────────────────────
    [Header("Sonido")]
    [SerializeField] private AudioSource audioSource;

    // ─── Privado ──────────────────────────────────────────────────────────
    private float timer;
    private GameObject objetivoLockeado; // enemigo al que estamos apuntando actualmente

    // ─────────────────────────────────────────────────────────────────────
    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (cabezaDelCanon == null)
            cabezaDelCanon = transform;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // 1. Si el objetivo actual murió o salió del rango → buscar el más cercano
        if (objetivoLockeado == null ||
            Vector2.Distance(transform.position, objetivoLockeado.transform.position) > rango)
        {
            objetivoLockeado = ObtenerEnemigoMasCercano();
        }

        // 2. Sin objetivo en rango → no hacer nada
        if (objetivoLockeado == null) return;

        // 3. Seguir al objetivo cada frame (tracking continuo)
        ApuntarA(objetivoLockeado.transform.position);

        // 4. Disparar según el timer
        if (timer >= tiempoEntreDisparos)
        {
            timer = 0f;
            Disparar(objetivoLockeado.transform);
        }
    }

    // ─── Lógica de apuntado ───────────────────────────────────────────────
    /// <summary>
    /// Apunta la BOCA del cañón (puntoDisparo) directamente al objetivo.
    /// Detecta automáticamente la dirección de la boca usando su posición local
    /// → no importa cómo esté orientado el sprite.
    /// </summary>
    private void ApuntarA(Vector3 posObjetivo)
    {
        if (puntoDisparo == null)
        {
            // Fallback: apunta el eje +X al objetivo
            Vector2 dirSimple = posObjetivo - cabezaDelCanon.position;
            cabezaDelCanon.rotation = Quaternion.Euler(0f, 0f,
                Mathf.Atan2(dirSimple.y, dirSimple.x) * Mathf.Rad2Deg);
            return;
        }

        // 1. Dirección al objetivo en world-space
        Vector2 dirObjetivo = posObjetivo - cabezaDelCanon.position;
        if (dirObjetivo.sqrMagnitude < 0.0001f) return;
        float anguloObjetivo = Mathf.Atan2(dirObjetivo.y, dirObjetivo.x) * Mathf.Rad2Deg;

        // 2. Posición LOCAL del puntoDisparo respecto a cabezaDelCanon
        //    (ignoramos la rotación actual → usamos localPosition directamente)
        Vector2 bocaLocal = puntoDisparo.localPosition;
        if (bocaLocal.sqrMagnitude < 0.0001f)
        {
            // El punto de disparo está en el mismo lugar que la cabeza → fallback
            cabezaDelCanon.rotation = Quaternion.Euler(0f, 0f, anguloObjetivo);
            return;
        }

        // 3. Ángulo que forma la boca en el espacio local del objeto
        float angulosBoca = Mathf.Atan2(bocaLocal.y, bocaLocal.x) * Mathf.Rad2Deg;

        // 4. La cabeza debe rotar tal que:
        //    rotacion_mundo + anguloBoca_local = anguloObjetivo
        //    → rotacion_mundo = anguloObjetivo - anguloBoca_local
        cabezaDelCanon.rotation = Quaternion.Euler(0f, 0f, anguloObjetivo - angulosBoca);
    }

    // ─── Disparo ──────────────────────────────────────────────────────────
    private void Disparar(Transform objetivo)
    {
        if (balaPrefab == null)
        {
            Debug.LogWarning("[Canon] balaPrefab no asignado.", this);
            return;
        }

        if (puntoDisparo == null)
        {
            Debug.LogWarning("[Canon] puntoDisparo no asignado. Asigna un Transform vacío en la boca del cañón.", this);
            return;
        }

        if (audioSource != null) audioSource.Play();

        // Instanciar bala en la boca del cañón
        Vector3 dirBala = (objetivo.position - puntoDisparo.position).normalized;
        GameObject bala = Instantiate(balaPrefab, puntoDisparo.position, Quaternion.identity);

        Bala balaScript = bala.GetComponent<Bala>();
        if (balaScript != null)
        {
            balaScript.Inicializar(objetivo, dirBala, velocidadBala, daño);
        }
        else
        {
            Debug.LogWarning("[Canon] El balaPrefab no tiene el componente Bala.", this);
        }
    }

    // ─── Selección de objetivo ────────────────────────────────────
    /// <summary>
    /// Devuelve el enemigo activo más cercano dentro del rango, o null si no hay ninguno.
    /// </summary>
    private GameObject ObtenerEnemigoMasCercano()
    {
        if (HeroSpawner.Instance == null) return null;
        var lista = HeroSpawner.Instance.GetTodosLosEnemigosActivos();
        if (lista == null || lista.Count == 0) return null;

        GameObject masCercano = null;
        float menorDistancia = float.MaxValue;

        foreach (var enemigo in lista)
        {
            if (enemigo == null) continue;
            float dist = Vector2.Distance(transform.position, enemigo.transform.position);
            if (dist <= rango && dist < menorDistancia)
            {
                menorDistancia = dist;
                masCercano = enemigo;
            }
        }

        return masCercano;
    }

    // ─── Gizmos ───────────────────────────────────────────────────────────
    private void OnDrawGizmosSelected()
    {
        // Rango de detección (rojo)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rango);

        // Dirección de apuntado actual (amarillo)
        if (cabezaDelCanon != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, cabezaDelCanon.right * rango);
        }

        // Punto de disparo (cyan)
        if (puntoDisparo != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(puntoDisparo.position, 0.15f);
        }
    }
}