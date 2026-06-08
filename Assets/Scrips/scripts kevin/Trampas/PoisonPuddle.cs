using UnityEngine;

public class PoisonPuddle : MonoBehaviour
{
    [Header("Stats del Charco")]
    public float duracionDelCharco = 5f;
    public float dañoPorTick = 5f;
    public float tiempoEntreTicks = 0.5f;

    private float timerDamage = 0f;
    private float tiempoVivido = 0f;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        // NO llamamos Destroy con timer — lo manejamos manualmente con el fade
    }

    void Update()
    {
        tiempoVivido += Time.deltaTime;
        float t = tiempoVivido / duracionDelCharco;

        // Fade: alpha va de 1 a 0 a lo largo de la vida del charco
        if (sr != null)
        {
            Color c = sr.color;
            c.a = Mathf.Lerp(1f, 0f, t);
            sr.color = c;
        }

        if (tiempoVivido >= duracionDelCharco)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        HeroMover heroe = other.GetComponent<HeroMover>();
        if (heroe != null)
        {
            timerDamage += Time.deltaTime;
            if (timerDamage >= tiempoEntreTicks)
            {
                heroe.RecibirDaño(dañoPorTick);
                timerDamage = 0f;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        HeroMover heroe = other.GetComponent<HeroMover>();
        if (heroe != null)
        {
            timerDamage = 0f;
        }
    }
}