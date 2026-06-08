using UnityEngine;
using System.Collections;

/// <summary>
/// GameManager — uno por nivel en la escena.
///
/// SETUP EN UNITY:
/// ─────────────────────────────────────────────────────────────
/// • Crea un GameObject "GameManager_Nivel1" en el nivel 1 de la escena.
/// • Crea un GameObject "GameManager_Nivel2" en el nivel 2, etc.
/// • Cada uno tiene este script con su propio:
///     - nivelIndex  : el número de nivel que representa (0, 1, 2...)
///     - camino      : el WaypointPath de ese nivel
///     - delayInicio : segundos que esperan antes de spawner el primer enemigo
/// • El script estático Instance apunta al GameManager del nivel ACTIVO.
/// ─────────────────────────────────────────────────────────────
/// Sólo el GameManager cuyo nivel coincida con el nivel actual del juego
/// inicializará y spawneará enemigos. Los demás se ignoran.
/// ─────────────────────────────────────────────────────────────
/// </summary>
public class GameManager : MonoBehaviour
{
    // ─── Singleton (apunta al nivel activo) ──────────────────────────────
    public static GameManager Instance { get; private set; }

    // ─── Configuración de este nivel ─────────────────────────────────────
    [Header("Este nivel")]
    [Tooltip("Índice de nivel que representa este GameManager (0 = nivel 1, 1 = nivel 2, etc.)")]
    public int nivelIndex = 0;

    [Tooltip("Camino (WaypointPath) de este nivel. Déjalo vacío para usar el que ya tiene HeroSpawner.")]
    public WaypointPath camino;

    [Tooltip("Segundos de espera antes de que empiecen a spawnear los enemigos.")]
    public float delayInicio = 3f;

    // ─── Eventos estáticos ────────────────────────────────────────────────
    /// <summary>
    /// Se dispara cuando un soldado (no el héroe) llega al final del camino.
    /// Suscríbete desde CamaraScript para avanzar la cámara al siguiente nivel.
    ///   GameManager.OnSoldadoLlegoAlFinal += MiMetodo;
    /// </summary>
    public static System.Action OnSoldadoLlegoAlFinal;

    /// <summary>Se dispara cuando el héroe final escapa.</summary>
    public static System.Action OnHeroeEscapo;

    // ─── Estado global (compartido entre todos los GameManagers) ─────────
    [Header("Estado global (edita sólo en el GameManager del nivel 0)")]
    [Tooltip("Nivel actualmente activo. Se puede ver/cambiar desde el Inspector del GM activo.")]
    public static int nivelActual = 0;

    [HideInInspector]
    public int heroesEliminados = 0;

    // ─── Estado interno ───────────────────────────────────────────────────
    private bool oleadaEnCurso = false;

    // ─────────────────────────────────────────────────────────────────────
    private void Awake()
    {
        // El primero que despierta se registra como Instance.
        // Como los demás niveles pueden estar inactivos, el activo
        // siempre sobrescribirá.
        Instance = this;
    }

    private bool _pendienteInicio = false;

    private void OnEnable()
    {
        // Registrarse como el GameManager activo
        Instance = this;
        oleadaEnCurso = false;
        _pendienteInicio = true; // se procesará en Start o en el primer frame
    }

    private void Start()
    {
        // Start() garantiza que todos los Awake() ya corrieron
        // → HeroSpawner.Instance ya existe aquí
        if (_pendienteInicio && nivelIndex == nivelActual)
        {
            _pendienteInicio = false;
            IniciarNivel();
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        oleadaEnCurso = false;
        _pendienteInicio = false;
    }

    // ─── Iniciar el nivel ────────────────────────────────────────────────
    private void IniciarNivel()
    {
        if (HeroSpawner.Instance == null)
        {
            Debug.LogError("[GameManager] No se encontró HeroSpawner.Instance. " +
                           "Asegúrate de que HeroSpawner está en la escena y activo.");
            return;
        }

        // Asignar el camino del nivel al HeroSpawner
        if (camino != null)
            HeroSpawner.Instance.CambiarCamino(camino);
        else
            Debug.LogWarning($"[GameManager] Nivel {nivelIndex}: el campo 'camino' no está asignado en el Inspector.");

        // Esperar el delay antes de spawnear
        StartCoroutine(DelaySpawn());
    }

    private IEnumerator DelaySpawn()
    {
        Debug.Log($"[GameManager] Nivel {nivelIndex} — esperando {delayInicio}s antes de spawnear...");
        yield return new WaitForSeconds(delayInicio);

        if (HeroSpawner.Instance == null) yield break;
        oleadaEnCurso = true;
        HeroSpawner.Instance.SpawnHeroe();
    }

    // ─── Callbacks del HeroSpawner ────────────────────────────────────────
    public void HeroeMurio()
    {
        heroesEliminados++;
        Debug.Log($"[GameManager] Héroe eliminado. Total: {heroesEliminados}");
    }

    public void HeroeEscapo()
    {
        Debug.Log("[GameManager] El héroe escapó.");
        OnHeroeEscapo?.Invoke();
    }

    /// <summary>
    /// Llamado desde HeroSpawner cuando un SOLDADO (no el héroe) llega al final.
    /// Dispara el evento que mueve la cámara al siguiente nivel.
    /// </summary>
    public void SoldadoEscapo()
    {
        Debug.Log($"[GameManager] Soldado escapó del nivel {nivelIndex} → avanzando nivel.");
        OnSoldadoLlegoAlFinal?.Invoke();
    }

    // ─── Cambiar de nivel (llamado externamente, ej. desde la cámara) ─────
    /// <summary>
    /// Activa el siguiente nivel. Llama a esto cuando la cámara llegue al siguiente nivel.
    /// </summary>
    public static void AvanzarNivel()
    {
        nivelActual++;
        Debug.Log($"[GameManager] Avanzando al nivel {nivelActual}.");
        // El GameManager del nivel siguiente debe activarse externamente
        // (activando su GameObject). Al activarse, OnEnable() inicia el nivel.
    }

    /// <summary>
    /// Activa el nivel con el índice indicado. Llama esto desde CamaraScript
    /// cuando la cámara llega a un nuevo nivel.
    /// </summary>
    public static void SetNivelActual(int indice)
    {
        nivelActual = indice;
        Debug.Log($"[GameManager] Nivel activo cambiado a: {indice}");
    }

    // ─── Inspector debug ─────────────────────────────────────────────────
    private void OnDrawGizmosSelected()
    {
        // Muestra el índice del nivel en la Scene view para identificar fácilmente
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f,
            $"GM Nivel {nivelIndex}\nDelay: {delayInicio}s");
        #endif
    }
}