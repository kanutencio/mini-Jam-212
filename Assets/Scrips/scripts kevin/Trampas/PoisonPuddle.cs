using UnityEngine;

public class PoisonPuddle : MonoBehaviour
{
    [Header("Stats del Charco")]
    public float duracionDelCharco = 5f; // duracion del charco
    public float dañoPorTick = 5f;      // cuanto daño hace
    public float tiempoEntreTicks = 0.5f; // cada cuanto tiempo hace daño

    private float timerDamage = 0f;

    void Start()
    {
        // se destruye despues de un tiempito
        Destroy(gameObject, duracionDelCharco);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        HeroMover heroe = other.GetComponent<HeroMover>();
        if (heroe != null)
        {
            timerDamage += Time.deltaTime;
            
            // aplica daño
            if (timerDamage >= tiempoEntreTicks)
            {
                heroe.RecibirDaño(dañoPorTick);
                timerDamage = 0f;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Reinicia el tiempo para no hacer daño
        HeroMover heroe = other.GetComponent<HeroMover>();
        if (heroe != null)
        {
            timerDamage = 0f;
        }
    }
}
