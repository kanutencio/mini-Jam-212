using UnityEngine;

/// <summary>
/// Ponlo en el GameObject del sarcófago.
/// Tiene su propia vida. Cuando llega a 0 (o el héroe lo toca),
/// reproduce una animación/efecto y se destruye.
/// </summary>
public class SarcofagoVida : MonoBehaviour
{
    [Header("Stats")]
    public float vidaMaxima = 200f;

    [Header("Efectos (opcional)")]
    [Tooltip("Prefab de partículas al destruirse.")]
    public GameObject prefabExplosion;

    [Header("UI (opcional)")]
    public UnityEngine.UI.Image barraVida;

    private float vidaActual;

    void Awake()
    {
        vidaActual = vidaMaxima;
        ActualizarBarra();
    }

    public void RecibirDaño(float cantidad)
    {
        vidaActual -= cantidad;
        ActualizarBarra();

        if (vidaActual <= 0f)
            Destruir();
    }

    public void Destruir()
    {
        if (prefabExplosion != null)
            Instantiate(prefabExplosion, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    void ActualizarBarra()
    {
        if (barraVida != null)
            barraVida.fillAmount = Mathf.Clamp01(vidaActual / vidaMaxima);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}
