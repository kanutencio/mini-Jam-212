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
        // Se calcula la dirección desde la posición GLOBAL del cañón (transform raíz),
        // ignorando cualquier rotación heredada del padre para evitar el temblado.
        Vector3 dir = (objetivo - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Offset del sprite: 180° si el sprite mira a la izquierda, 0° si mira a la derecha.
        float spriteOffset = 180f;

        Quaternion rotacionObjetivo = Quaternion.Euler(0f, 0f, angle + spriteOffset);

        if (cabezaDelCañon != null)
        {
            // Se aplica la rotación en espacio mundo directamente (sin herencia del padre)
            // con un Slerp suave para eliminar el temblado frame a frame.
            cabezaDelCañon.rotation = Quaternion.Slerp(
                cabezaDelCañon.rotation,
                rotacionObjetivo,
                Time.deltaTime * 20f   // velocidad de giro: sube si quieres más instantáneo
            );
        }
        else
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotacionObjetivo,
                Time.deltaTime * 20f
            );
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