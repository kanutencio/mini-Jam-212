using UnityEngine;

public class InfernoTrap : MonoBehaviour
{
    [Header("Stats de la Torre Infernal")]
    public float rango = 5f;
    [Tooltip("El daño inicial por segundo al enganchar al enemigo")]
    public float dañoBasePorSegundo = 10f;
    [Tooltip("Cuánto daño por segundo extra se suma cada segundo que pasa (¡Efecto Clash Royale!)")]
    public float aumentoDañoPorSegundo = 20f;
    [Tooltip("El límite máximo de daño por segundo para que no sea infinito")]
    public float dañoMaximoPorSegundo = 100f;

    [Header("Referencias")]
    [Tooltip("De dónde sale el rayo láser")]
    public Transform puntoDisparo;
    [Tooltip("El LineRenderer que dibujará el láser")]
    public LineRenderer lineRenderer;

    private GameObject objetivoActual;
    private float dañoActual;
    private float tiempoObjetivo;

    void Start()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
            lineRenderer.positionCount = 2; 
        }
    }

    void Update()
    {
        
        GameObject heroe = HeroSpawner.Instance != null ? HeroSpawner.Instance.GetHeroeActivo() : null;

        
        if (heroe == null)
        {
            PerderObjetivo();
            return;
        }

        // se mide la distancia
        float distancia = Vector3.Distance(transform.position, heroe.transform.position);

        // Si se sale del rango, el rayo se corta
        if (distancia > rango)
        {
            PerderObjetivo();
            return;
        }


        if (objetivoActual != heroe)
        {
            objetivoActual = heroe;
            dañoActual = dañoBasePorSegundo;
            tiempoObjetivo = 0f;
            if (lineRenderer != null) lineRenderer.enabled = true;
        }

        // Incrementamos el daño a lo largo del tiempo
        tiempoObjetivo += Time.deltaTime;
        dañoActual = dañoBasePorSegundo + (aumentoDañoPorSegundo * tiempoObjetivo);
        
        // Lo limitamos al daño máximo
        if (dañoActual > dañoMaximoPorSegundo)
        {
            dañoActual = dañoMaximoPorSegundo;
        }

        // Le aplicamos el daño progresivamente
        HeroMover heroScript = heroe.GetComponent<HeroMover>();
        if (heroScript != null)
        {
            heroScript.RecibirDaño(dañoActual * Time.deltaTime);
        }

        // Actualizamos el dibujo del rayo láser
        if (lineRenderer != null && puntoDisparo != null)
        {
            lineRenderer.SetPosition(0, puntoDisparo.position); // Comienza en la punta de la torre
            lineRenderer.SetPosition(1, heroe.transform.position); // Termina en el enemigo

            float grosor = Mathf.Lerp(0.05f, 0.25f, dañoActual / dañoMaximoPorSegundo);
            lineRenderer.startWidth = grosor;
            lineRenderer.endWidth = grosor;
        }
    }

    void PerderObjetivo()
    {
        objetivoActual = null;
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false; // Apagamos el láser visualmente
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f); // Color naranja
        Gizmos.DrawWireSphere(transform.position, rango);
    }
}
