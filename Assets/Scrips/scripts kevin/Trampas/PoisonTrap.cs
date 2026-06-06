using UnityEngine;

public class PoisonTrap : MonoBehaviour
{
    [Header("Stats")]
    public float dañoImpacto = 10f; // Daño de la bala al golpear
    public float velocidadBala = 6f;
    public float tiempoEntreDisparos = 3f;
    public float rango = 5f;

    [Header("Referencias")]
    public GameObject poisonProjectilePrefab;
    public Transform puntoDisparo;
    [Tooltip("Asigna aquí el objeto hijo que tiene el sprite del cañón (la parte que gira), para que la base se quede quieta.")]
    public Transform cabezaDelCañon;
    [Tooltip("Si el cañón mira hacia atrás o de lado, ajusta este valor (ej. 180, 90, -90)")]
    public float offsetRotacion = 0f;

    private float timer = 0f;
    private GameObject heroeActivo;

    void Update()
    {
        heroeActivo = HeroSpawner.Instance != null ? HeroSpawner.Instance.GetHeroeActivo() : null;

        timer += Time.deltaTime;

        if (heroeActivo == null) return;

        float distancia = Vector3.Distance(transform.position, heroeActivo.transform.position);

        if (distancia <= rango)
        {
            Apuntar(heroeActivo.transform.position);

            if (timer >= tiempoEntreDisparos)
            {
                Disparar();
                timer = 0f;
            }
        }
    }

    void Apuntar(Vector3 objetivo)
    {
        Vector3 dir = (objetivo - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angle += offsetRotacion;
        
        if (cabezaDelCañon != null)
        {
            cabezaDelCañon.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    void Disparar()
    {
        if (poisonProjectilePrefab == null || puntoDisparo == null) return;

        Vector3 dir = (heroeActivo.transform.position - puntoDisparo.position).normalized;
        GameObject proyectil = Instantiate(poisonProjectilePrefab, puntoDisparo.position, Quaternion.identity);

        PoisonProjectile scriptProyectil = proyectil.GetComponent<PoisonProjectile>();
        if (scriptProyectil != null)
        {
            scriptProyectil.Inicializar(heroeActivo.transform, dir, velocidadBala, dañoImpacto);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rango);
    }
}
