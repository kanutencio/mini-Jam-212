using UnityEngine;

public class PoisonProjectile : MonoBehaviour
{
    [Header("Charco (Puddle)")]
    public GameObject poisonPuddlePrefab;

    private Transform objetivo;
    private Vector3 direccion;
    private float velocidad;
    private float daño;
    private float distanciaImpacto = 0.3f;

    public void Inicializar(Transform obj, Vector3 dirInicial, float vel, float dan)
    {
        objetivo = obj;
        direccion = dirInicial.normalized;
        velocidad = vel;
        daño = dan;

        float angle = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Se destruye solo después de 4s si no golpeó nada
        Destroy(gameObject, 4f);
    }

    void Update()
    {
        if (objetivo == null)
        {
            transform.position += direccion * velocidad * Time.deltaTime;
            return;
        }

        direccion = (objetivo.position - transform.position).normalized;
        transform.position += direccion * velocidad * Time.deltaTime;

        float angle = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        float distancia = Vector3.Distance(transform.position, objetivo.position);
        if (distancia <= distanciaImpacto)
        {
            Impactar();
        }
    }

    void Impactar()
    {
        // Dañar al héroe si es necesario en el impacto
        if (objetivo != null)
        {
            HeroMover heroe = objetivo.GetComponent<HeroMover>();
            if (heroe != null && daño > 0)
                heroe.RecibirDaño(daño);
        }

        // Crear el charco
        if (poisonPuddlePrefab != null)
        {
            Instantiate(poisonPuddlePrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
