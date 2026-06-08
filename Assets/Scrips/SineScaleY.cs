using UnityEngine;

public class SineScaleY : MonoBehaviour
{
    [SerializeField] private float amplitud = 0.2f;
    [SerializeField] private float velocidad = 1f;

    private Vector3 escalaInicial;

    private void Start()
    {
        escalaInicial = transform.localScale;
    }

    private void Update()
    {
        transform.localScale = new Vector3(
            escalaInicial.x,
            escalaInicial.y + Mathf.Sin(Time.time * velocidad) * amplitud,
            escalaInicial.z
        );
    }
}