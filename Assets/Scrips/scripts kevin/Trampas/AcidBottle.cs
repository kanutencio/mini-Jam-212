using UnityEngine;

public class AcidBottle : MonoBehaviour
{
    [Header("Charco")]
    public GameObject acidPuddlePrefab;

    [Header("Daño de impacto (opcional)")]
    public float dañoImpacto = 0f;

    private Vector3 origen;
    private Vector3 destino;
    private float alturaArco;
    private float duracion = 1.2f;
    private float tiempoTranscurrido = 0f;
    private bool llegó = false;

    /// <summary>
    /// duracionVuelo permite que CatapultTrap controle el tiempo de vuelo.
    /// </summary>
    public void Inicializar(Vector3 destinoPos, float altura, float duracionVuelo = 1.2f)
    {
        origen = transform.position;
        destino = destinoPos;
        alturaArco = altura;
        duracion = duracionVuelo;
    }

    void Update()
    {
        if (llegó) return;

        tiempoTranscurrido += Time.deltaTime;
        float t = Mathf.Clamp01(tiempoTranscurrido / duracion);

        Vector3 posLineal = Vector3.Lerp(origen, destino, t);
        float arcoY = alturaArco * Mathf.Sin(Mathf.PI * t);
        transform.position = new Vector3(posLineal.x, posLineal.y + arcoY, posLineal.z);

        // Rotar la botella siguiendo la dirección del arco
        if (t < 0.99f)
        {
            float tSig = Mathf.Clamp01(t + 0.02f);
            Vector3 posSig = Vector3.Lerp(origen, destino, tSig);
            posSig.y += alturaArco * Mathf.Sin(Mathf.PI * tSig);
            Vector3 dir = posSig - transform.position;
            if (dir != Vector3.zero)
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }

        if (t >= 1f)
        {
            Impactar();
        }
    }

    void Impactar()
    {
        llegó = true;

        if (dañoImpacto > 0f)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.4f);
            foreach (var hit in hits)
            {
                HeroMover heroe = hit.GetComponent<HeroMover>();
                if (heroe != null)
                    heroe.RecibirDaño(dañoImpacto);
            }
        }

        if (acidPuddlePrefab != null)
            Instantiate(acidPuddlePrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}