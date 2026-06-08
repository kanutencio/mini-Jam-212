using UnityEngine;

public class CatapultTrap : MonoBehaviour
{
    [Header("Stats")]
    public float rango = 5f;
    public float tiempoEntreDisparos = 3f;

    [Header("Referencias")]
    public GameObject bottlePrefab;

    [Tooltip("Hijo con el sprite del brazo (la parte que rota).")]
    public Transform brazoDeLaCatapulta;

    [Tooltip("Punto vacío en el EXTREMO del brazo donde aparece la botella. " +
             "Debe estar en posición local X > 0 (o Y > 0) respecto al brazo.")]
    public Transform puntoLanzamiento;

    [Header("Arco")]
    public float alturaArco = 3f;
    [Tooltip("Duración del vuelo de la botella en segundos.")]
    public float duracionVuelo = 1.2f;

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
            ApuntarAlReves(heroeActivo.transform.position);

            if (timer >= tiempoEntreDisparos)
            {
                Vector3 destinoPredicho = PredecirPosicion(heroeActivo);
                Lanzar(destinoPredicho);
                timer = 0f;
            }
        }
    }

    /// <summary>
    /// Igual que Canon.ApuntarA pero con 180° de diferencia:
    /// la BOCA (puntoLanzamiento) queda mirando OPUESTA al enemigo.
    /// </summary>
    private void ApuntarAlReves(Vector3 posObjetivo)
    {
        if (brazoDeLaCatapulta == null) return;

        if (puntoLanzamiento == null)
        {
            // Fallback: apunta el eje -X al objetivo (opuesto a +X)
            Vector2 dir = posObjetivo - brazoDeLaCatapulta.position;
            float angulo = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            brazoDeLaCatapulta.rotation = Quaternion.Euler(0f, 0f, angulo + 180f);
            return;
        }

        // 1. Dirección al objetivo
        Vector2 dirObjetivo = posObjetivo - brazoDeLaCatapulta.position;
        if (dirObjetivo.sqrMagnitude < 0.0001f) return;
        float anguloObjetivo = Mathf.Atan2(dirObjetivo.y, dirObjetivo.x) * Mathf.Rad2Deg;

        // 2. Ángulo local del punto de lanzamiento respecto al brazo
        Vector2 bocaLocal = puntoLanzamiento.localPosition;
        if (bocaLocal.sqrMagnitude < 0.0001f)
        {
            brazoDeLaCatapulta.rotation = Quaternion.Euler(0f, 0f, anguloObjetivo + 180f);
            return;
        }
        float anguloBoca = Mathf.Atan2(bocaLocal.y, bocaLocal.x) * Mathf.Rad2Deg;

        // 3. Igual que Canon pero + 180°: la boca queda opuesta al enemigo
        brazoDeLaCatapulta.rotation = Quaternion.Euler(0f, 0f, anguloObjetivo - anguloBoca + 180f);
    }

    Vector3 PredecirPosicion(GameObject heroe)
    {
        HeroMover mover = heroe.GetComponent<HeroMover>();
        float velocidadHeroe = mover != null ? mover.velocidad : 2f;
        // Héroe siempre va a la derecha
        return heroe.transform.position + Vector3.right * velocidadHeroe * duracionVuelo;
    }

    void Lanzar(Vector3 destino)
    {
        if (bottlePrefab == null) return;

        Vector3 origen = puntoLanzamiento != null ? puntoLanzamiento.position : transform.position;
        GameObject botella = Instantiate(bottlePrefab, origen, Quaternion.identity);

        AcidBottle script = botella.GetComponent<AcidBottle>();
        if (script != null)
        {
            script.Inicializar(destino, alturaArco, duracionVuelo);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rango);

        // Muestra hacia dónde apunta la boca del brazo
        if (brazoDeLaCatapulta != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(brazoDeLaCatapulta.position, brazoDeLaCatapulta.right * 1.5f);
        }
    }
}