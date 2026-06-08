using UnityEngine;

public class CatapultTrap : MonoBehaviour
{
    [Header("Stats")]
    public float rango = 5f;
    public float tiempoEntreDisparos = 3f;

    [Header("Referencias")]
    public Transform brazo;                  // hijo con el sprite del brazo
    public Transform puntoLanzamiento;       // punto al extremo del brazo donde aparece la botella
    public GameObject bottlePrefab;          // prefab de la botella

    [Header("Arco")]
    [Tooltip("Altura máxima del arco de la parábola")]
    public float alturaArco = 3f;

    [Tooltip("Offset de rotación del brazo si el sprite mira para otro lado")]
    public float offsetRotacionBrazo = -90f;

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
            // Rotar el brazo hacia el héroe
            if (brazo != null)
            {
                Vector3 dir = (heroeActivo.transform.position - transform.position).normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + offsetRotacionBrazo;
                brazo.rotation = Quaternion.Euler(0f, 0f, angle);
            }

            if (timer >= tiempoEntreDisparos)
            {
                Lanzar();
                timer = 0f;
            }
        }
    }

    void Lanzar()
    {
        if (bottlePrefab == null || heroeActivo == null) return;

        Vector3 origen = puntoLanzamiento != null ? puntoLanzamiento.position : transform.position;
        GameObject botella = Instantiate(bottlePrefab, origen, Quaternion.identity);

        AcidBottle script = botella.GetComponent<AcidBottle>();
        if (script != null)
        {
            script.Inicializar(heroeActivo.transform.position, alturaArco);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rango);
    }
}
