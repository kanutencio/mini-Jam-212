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
        // Ensure we have a reference to the part that should rotate.
        // If not assigned in the inspector, rotate the whole object.
        if (cabezaDelCañon == null)
            cabezaDelCañon = transform;
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
        // Use world space direction directly for simplicity.
        Vector3 dirWorld = (objetivo - transform.position).normalized;
        if (dirWorld.sqrMagnitude < 0.001f) return;

        // Angle in degrees where 0 points to the right (+X).
        float angle = Mathf.Atan2(dirWorld.y, dirWorld.x) * Mathf.Rad2Deg;
        // Apply any sprite offset (e.g., 180 if the sprite points left by default).
        angle += spriteOffset;

        Quaternion objetivoRot = Quaternion.Euler(0f, 0f, angle);

        // Rotate the cannon head (or the whole object if head not set).
        Transform rotTarget = cabezaDelCañon != null ? cabezaDelCañon : transform;

        if (apuntadoInstantaneo)
        {
            rotTarget.localRotation = objetivoRot;
        }
        else
        {
            rotTarget.localRotation = Quaternion.Slerp(
                rotTarget.localRotation,
                objetivoRot,
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