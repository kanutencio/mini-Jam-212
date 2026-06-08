using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HeroDestructor : MonoBehaviour
{
    [Header("Configuración")]
    public float tiempoGameOver = 3f;
    public string nombreEscenaMenu = "Menu";
    public float dañoAlSarcofago = 100f;

    [Header("Game Over UI (opcional)")]
    public GameObject pantallaGameOver;

    private static readonly string[] nombresTrampa = new string[]
    {
        "canon", "fireballtrap", "poisontrap", "spiketrap", "infernotrap", "catapult"
    };

    private bool gameOverActivado = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameOverActivado) return;

        string nombre = other.gameObject.name.ToLower();

        foreach (var t in nombresTrampa)
        {
            if (nombre.Contains(t))
            {
                Debug.Log($"[HeroDestructor] Trampa destruida: {other.gameObject.name}");
                Destroy(other.gameObject);
                return;
            }
        }

        if (nombre.Contains("sarcofago"))
        {
            SarcofagoVida sarcofago = other.GetComponent<SarcofagoVida>();
            if (sarcofago != null)
            {
                sarcofago.RecibirDaño(dañoAlSarcofago);
                // Solo activa Game Over cuando el sarcófago muere (lo maneja SarcofagoVida)
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameOverActivado) return;

        string nombre = collision.gameObject.name.ToLower();

        foreach (var t in nombresTrampa)
        {
            if (nombre.Contains(t))
            {
                Destroy(collision.gameObject);
                return;
            }
        }

        if (nombre.Contains("sarcofago"))
        {
            SarcofagoVida sarcofago = collision.gameObject.GetComponent<SarcofagoVida>();
            if (sarcofago != null)
                sarcofago.RecibirDaño(dañoAlSarcofago);
        }
    }

    public void ActivarGameOver()
    {
        if (gameOverActivado) return;
        gameOverActivado = true;

        HeroMover mover = GetComponent<HeroMover>();
        if (mover != null) mover.puedeMoverse = false;

        if (pantallaGameOver != null)
            pantallaGameOver.SetActive(true);

        StartCoroutine(VolverAlMenu());
    }

    private IEnumerator VolverAlMenu()
    {
        yield return new WaitForSeconds(tiempoGameOver);
        SceneManager.LoadScene(nombreEscenaMenu);
    }
}