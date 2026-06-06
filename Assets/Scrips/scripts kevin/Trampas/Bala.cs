using UnityEngine;

public class Bala : MonoBehaviour
{
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

        Destroy(gameObject, 4f);
    }

    void Update()
    {
        // Si el héroe murió la bala sigue en línea recta y desaparece
        if (objetivo == null)
        {
            transform.position += direccion * velocidad * Time.deltaTime;
            return;
        }

        // Avanza hacia el héroe
        direccion = (objetivo.position - transform.position).normalized;
        transform.position += direccion * velocidad * Time.deltaTime;

        float angle = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Si está suficientemente cerca, impacta
        float distancia = Vector3.Distance(transform.position, objetivo.position);
        if (distancia <= distanciaImpacto)
        {
            HeroMover heroe = objetivo.GetComponent<HeroMover>();
            if (heroe != null)
                heroe.RecibirDaño(daño);

            Destroy(gameObject);
        }
    }
}