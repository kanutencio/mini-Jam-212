using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HeroDestructor : MonoBehaviour
{
    [Header("Configuración")]
    public float tiempoGameOver = 3f;
    public string nombreEscenaMenu = "Menu";
    public float dañoAlSarcofago = 100f;

    [Header("Radio de destrucción")]
    public float radioDestruccion = 2f;

    [Header("Game Over UI (opcional)")]
    public GameObject pantallaGameOver;

    private bool gameOverActivado = false;

    void Update()
    {
        if (gameOverActivado) return;

        // Buscar todos los objetos con tag "Trampa" en la escena
        GameObject[] trampas = GameObject.FindGameObjectsWithTag("Trampa");

        foreach (var trampa in trampas)
        {
            if (trampa == null) continue;

            float distancia = Vector2.Distance(transform.position, trampa.transform.position);
            if (distancia <= radioDestruccion)
            {
                Debug.Log($"[HeroDestructor] Trampa destruida: {trampa.name}");
                Destroy(trampa);
            }
        }

        // Sarcófago aparte por tag o nombre
        GameObject sarcofago = GameObject.FindGameObjectWithTag("Sarcofago");
        if (sarcofago != null)
        {
            float dist = Vector2.Distance(transform.position, sarcofago.transform.position);
            if (dist <= radioDestruccion)
            {
                SarcofagoVida sv = sarcofago.GetComponent<SarcofagoVida>();
                if (sv != null) sv.RecibirDaño(dañoAlSarcofago);
            }
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioDestruccion);
    }
}