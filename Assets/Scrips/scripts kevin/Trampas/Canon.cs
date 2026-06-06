using UnityEngine;

public class Canon : MonoBehaviour
{
    [Header("Stats")]
    public float daño = 30f;
    public float velocidadBala = 6f;
    public float tiempoEntreDisparos = 2f;
    public float rango = 5f;

    [Header("Referencias")]
    public GameObject balaPrefab;
    public Transform puntoDisparo;
    [Tooltip("Asigna aquí el objeto hijo que tiene el sprite del cañón (la parte que gira), para que la base se quede quieta.")]
    public Transform cabezaDelCañon;

    private float timer = 0f;
    private GameObject heroeActivo;

    void Update()
    {
        heroeActivo = HeroSpawner.Instance != null ? HeroSpawner.Instance.GetHeroeActivo() : null;

        // El cañón solo dispara si hay un heroe en el rango
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
        if (balaPrefab == null || puntoDisparo == null) return;

        Vector3 dir = (heroeActivo.transform.position - puntoDisparo.position).normalized;
        GameObject bala = Instantiate(balaPrefab, puntoDisparo.position, Quaternion.identity);

        Bala balaScript = bala.GetComponent<Bala>();
        if (balaScript != null)
        {
            balaScript.Inicializar(heroeActivo.transform, dir, velocidadBala, daño);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rango);
    }
}