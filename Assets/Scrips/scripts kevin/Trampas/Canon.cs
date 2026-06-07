using UnityEngine;

public class Canon : MonoBehaviour
{
    [Header("Stats")]
    public float daño = 30f;
    public float velocidadBala = 6f;
    public float tiempoEntreDisparos = 2f;
    public float rango = 5f;

    [Header("Sonido")]
    [SerializeField] private AudioSource AS;

    [Header("Referencias")]
    public GameObject balaPrefab;
    public Transform puntoDisparo;
    [Tooltip("Asigna aquí el objeto hijo que tiene el sprite del cañón (la parte que gira), para que la base se quede quieta.")]
    public Transform cabezaDelCañon;

    [Header("Giro e Imagen")]
    [Tooltip("Si está activo, apuntará de golpe al enemigo. Si no, girará poco a poco.")]
    public bool apuntadoInstantaneo = true;
    [Tooltip("Velocidad de giro del cañón si el apuntado instantáneo está desactivado.")]
    public float velocidadGiro = 20f;
    [Tooltip("Offset en grados si tu sprite original está girado (ej: 180 si mira a la izquierda de base, 0 si mira a la derecha).")]
    public float spriteOffset = 180f;

    private float timer = 0f;
    private GameObject heroeActivo;

    private void OnEnable()
    {
        AS = GetComponent<AudioSource>();
    }
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
        // Forzamos el cálculo en 2D (Vector2) para ignorar cualquier desfase en el eje Z
        Vector2 targetPos2D = new Vector2(objetivo.x, objetivo.y);
        Vector2 myPos2D = new Vector2(transform.position.x, transform.position.y);
        
        Vector2 dir = (targetPos2D - myPos2D).normalized;
        
        // Evitamos calcular si están en el mismo punto
        if (dir.sqrMagnitude < 0.001f) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotacionObjetivo = Quaternion.Euler(0f, 0f, angle + spriteOffset);

        Transform objetoARotar = cabezaDelCañon != null ? cabezaDelCañon : transform;

        if (apuntadoInstantaneo)
        {
            // Apuntado instantáneo
            objetoARotar.rotation = rotacionObjetivo;
        }
        else
        {
            // Apuntado suave con Slerp
            objetoARotar.rotation = Quaternion.Slerp(
                objetoARotar.rotation,
                rotacionObjetivo,
                Time.deltaTime * velocidadGiro
            );
        }
    }

    void Disparar()
    {
        if (balaPrefab == null || puntoDisparo == null) return;
        AS.Play();
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