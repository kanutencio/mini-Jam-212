using UnityEngine;

public class CamaraScript : MonoBehaviour
{
    [Header("Puntos de la cámara")]
    [SerializeField] private GameObject Punto1;
    [SerializeField] private GameObject Punto2;
    [SerializeField] private GameObject Punto3;
    [SerializeField] private GameObject Punto4;
    [SerializeField] private GameObject Punto5;
    [SerializeField] private GameObject Punto6;
    [SerializeField] private GameObject Punto7;
    [SerializeField] private GameObject Punto8;

    [Header("Configuración")]
    [SerializeField] private float duracionMovimiento = 2f;

    [SerializeField] private int puntoActual;

    private Vector3 posicionInicial;
    private Vector3 destino;
    private float tiempo;
    private bool moviendose;

    void Update()
    {
        if (moviendose)
        {
            tiempo += Time.deltaTime;

            float t = tiempo / duracionMovimiento;

            t = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(
                posicionInicial,
                destino,
                t
            );

            if (tiempo >= duracionMovimiento)
            {
                transform.position = destino;
                moviendose = false;
            }
        }
    }

    public void CambiarPunto()
    {
        puntoActual++;

        if (puntoActual > 8)
        {
            puntoActual = 1;
        }
            

        posicionInicial = transform.position;
        destino = ObtenerDestino();
        tiempo = 0f;
        moviendose = true;
    }

    private Vector3 ObtenerDestino()
    {
        switch (puntoActual)
        {
            case 1: return Punto1.transform.position;
            case 2: return Punto2.transform.position;
            case 3: return Punto3.transform.position;
            case 4: return Punto4.transform.position;
            case 5: return Punto5.transform.position;
            case 6: return Punto6.transform.position;
            case 7: return Punto7.transform.position;
            case 8: return Punto8.transform.position;
            default: return transform.position;
        }
    }


}
