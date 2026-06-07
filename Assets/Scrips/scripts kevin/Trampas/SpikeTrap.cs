using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [Header("Referencia al Pivote")]
    [Tooltip("Arrastra aquí el GameObject 'Pivot' que contiene la cadena y la bola.")]
    public Transform pivote;

    [Header("Rotación")]
    [Tooltip("Grados por segundo. Positivo = antihorario. Negativo = horario.")]
    public float rotationSpeed = 90f;

    [Header("Daño (se aplica al SpikeHitbox de la bola)")]
    [Tooltip("Daño que recibe el héroe al tocar la bola.")]
    public float daño = 15f;

    void Start()
    {
        if (pivote != null)
        {
            foreach (SpikeHitbox hitbox in pivote.GetComponentsInChildren<SpikeHitbox>())
            {
                hitbox.daño = daño;
            }
        }
    }

    void Update()
    {
        if (pivote != null)
        {
            // Rotar la cadena y la bola
            pivote.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (pivote == null) return;

        foreach (SpikeHitbox hitbox in pivote.GetComponentsInChildren<SpikeHitbox>())
        {
            float radio = Vector3.Distance(transform.position, hitbox.transform.position);
            Gizmos.color = new Color(1f, 0.3f, 0f, 0.5f);
            DrawWireCircle(transform.position, radio);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, hitbox.transform.position);
        }
    }

    void DrawWireCircle(Vector3 center, float radius, int segments = 36)
    {
        float step = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            float a1 = i * step * Mathf.Deg2Rad;
            float a2 = (i + 1) * step * Mathf.Deg2Rad;
            Vector3 p1 = center + new Vector3(Mathf.Cos(a1) * radius, Mathf.Sin(a1) * radius, 0f);
            Vector3 p2 = center + new Vector3(Mathf.Cos(a2) * radius, Mathf.Sin(a2) * radius, 0f);
            Gizmos.DrawLine(p1, p2);
        }
    }
}
