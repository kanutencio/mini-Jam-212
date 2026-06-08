using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    private Transform objetivo;
    private Vector3 direccion;
    private float velocidad;
    private float dañoImpacto;
    private float dañoFuegoTick;
    private float duracionFuego;

    private float distanciaImpacto = 0.3f;
    private bool impactado = false;

    public void Inicializar(Transform obj, Vector3 dirInicial, float vel,
                            float danImpacto, float danFuego, float durFuego)
    {
        objetivo     = obj;
        direccion    = dirInicial.normalized;
        velocidad    = vel;
        dañoImpacto  = danImpacto;
        dañoFuegoTick = danFuego;
        duracionFuego = durFuego;

        float angle = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (impactado) return;

        if (objetivo != null)
        {
            direccion = (objetivo.position - transform.position).normalized;
            float angle = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        transform.position += direccion * velocidad * Time.deltaTime;

        if (objetivo != null &&
            Vector3.Distance(transform.position, objetivo.position) <= distanciaImpacto)
        {
            Impactar();
        }
    }

    void Impactar()
    {
        impactado = true;

        if (objetivo != null)
        {
            HeroMover heroe = objetivo.GetComponent<HeroMover>();
            if (heroe != null)
            {
                // Daño de impacto inmediato
                heroe.RecibirDaño(dañoImpacto);

                // Iniciar DoT de fuego en el proyectil antes de destruirse
                StartCoroutine(DoTFuego(heroe));
                return; // no destruir aún, esperar el DoT
            }
        }

        Destroy(gameObject);
    }

    private System.Collections.IEnumerator DoTFuego(HeroMover heroe)
    {
        // Ocultamos el sprite para que no se vea flotando
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        float tiempoVivido = 0f;
        float tickInterval = 0.5f;
        float timerTick = 0f;

        while (tiempoVivido < duracionFuego)
        {
            if (heroe == null) break; // el héroe murió antes

            tiempoVivido += Time.deltaTime;
            timerTick    += Time.deltaTime;

            if (timerTick >= tickInterval)
            {
                heroe.RecibirDaño(dañoFuegoTick);
                timerTick = 0f;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
