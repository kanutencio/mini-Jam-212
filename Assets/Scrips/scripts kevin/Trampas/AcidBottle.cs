using UnityEngine;

public class AcidBottle : MonoBehaviour
{
    [Header("Charco")]
    public GameObject acidPuddlePrefab;   // arrastra aquí el prefab del charco de ácido

    [Header("Daño de impacto (opcional)")]
    public float dañoImpacto = 0f;        // daño directo al caer encima del héroe (0 = solo charco)

    private Vector3 origen;
    private Vector3 destino;
    private float alturaArco;
    private float duracion = 1.2f;        // segundos que tarda en llegar
    private float tiempoTranscurrido = 0f;
    private bool llegó = false;

    /// <summary>
    /// Llámalo justo después de Instantiate.
    /// destinoPos = posición del héroe en el momento del lanzamiento.
    /// </summary>
    public void Inicializar(Vector3 destinoPos, float altura)
    {
        origen = transform.position;
        destino = destinoPos;
        alturaArco = altura;
    }

    void Update()
    {
        if (llegó) return;

        tiempoTranscurrido += Time.deltaTime;
        float t = Mathf.Clamp01(tiempoTranscurrido / duracion);

        // Parábola: interpolación lineal en X/Y + arco en Y
        Vector3 posLineal = Vector3.Lerp(origen, destino, t);
        float arcoY = alturaArco * Mathf.Sin(Mathf.PI * t);
        transform.position = new Vector3(posLineal.x, posLineal.y + arcoY, posLineal.z);

        // Rotar la botella siguiendo la trayectoria
        if (t < 0.99f)
        {
            Vector3 siguiente = Vector3.Lerp(origen, destino, t + 0.01f);
            siguiente.y += alturaArco * Mathf.Sin(Mathf.PI * (t + 0.01f));
            Vector3 dir = siguiente - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        if (t >= 1f)
        {
            Impactar();
        }
    }

    void Impactar()
    {
        llegó = true;

        // Daño directo si el héroe está muy cerca del punto de impacto
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

        // Crear el charco de ácido donde cayó la botella
        if (acidPuddlePrefab != null)
        {
            Instantiate(acidPuddlePrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.4f);
    }
}
