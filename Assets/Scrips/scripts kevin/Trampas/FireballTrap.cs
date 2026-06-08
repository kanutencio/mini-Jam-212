using UnityEngine;

public class FireballTrap : MonoBehaviour
{
    [Header("Stats")]
    public float daño = 25f;
    public float dañoFuegoPoTick = 5f;
    public float duracionFuego = 3f;
    public float tiempoEntreDisparos = 2f;
    public float velocidadBola = 6f;
    public float rango = 5f;

    [Header("Referencias")]
    public GameObject fireballPrefab;
    public Transform cabeza;        // hijo con el sprite que rota
    public Transform puntoDisparo;  // boca del cañón

    [Header("Sonido")]
    [SerializeField] private AudioSource audioSource;

    private float timer;
    private GameObject objetivoLockeado;

    private void Awake()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (cabeza == null) cabeza = transform;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (objetivoLockeado == null ||
            Vector2.Distance(transform.position, objetivoLockeado.transform.position) > rango)
        {
            objetivoLockeado = ObtenerEnemigoMasCercano();
        }

        if (objetivoLockeado == null) return;

        ApuntarA(objetivoLockeado.transform.position);

        if (timer >= tiempoEntreDisparos)
        {
            timer = 0f;
            Disparar(objetivoLockeado.transform);
        }
    }

    private void ApuntarA(Vector3 posObjetivo)
    {
        if (puntoDisparo == null)
        {
            Vector2 dirSimple = posObjetivo - cabeza.position;
            cabeza.rotation = Quaternion.Euler(0f, 0f,
                Mathf.Atan2(dirSimple.y, dirSimple.x) * Mathf.Rad2Deg);
            return;
        }

        Vector2 dirObjetivo = posObjetivo - cabeza.position;
        if (dirObjetivo.sqrMagnitude < 0.0001f) return;
        float anguloObjetivo = Mathf.Atan2(dirObjetivo.y, dirObjetivo.x) * Mathf.Rad2Deg;

        Vector2 bocaLocal = puntoDisparo.localPosition;
        if (bocaLocal.sqrMagnitude < 0.0001f)
        {
            cabeza.rotation = Quaternion.Euler(0f, 0f, anguloObjetivo);
            return;
        }

        float anguloBoca = Mathf.Atan2(bocaLocal.y, bocaLocal.x) * Mathf.Rad2Deg;
        cabeza.rotation = Quaternion.Euler(0f, 0f, anguloObjetivo - anguloBoca);
    }

    private void Disparar(Transform objetivo)
    {
        if (fireballPrefab == null || puntoDisparo == null) return;
        if (audioSource != null) audioSource.Play();

        Vector3 dir = (objetivo.position - puntoDisparo.position).normalized;
        GameObject bola = Instantiate(fireballPrefab, puntoDisparo.position, Quaternion.identity);

        FireballProjectile script = bola.GetComponent<FireballProjectile>();
        if (script != null)
            script.Inicializar(objetivo, dir, velocidadBola, daño, dañoFuegoPoTick, duracionFuego);
    }

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rango);

        if (cabeza != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, cabeza.right * rango);
        }

        if (puntoDisparo != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(puntoDisparo.position, 0.15f);
        }
    }
}
