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
    [SerializeField] private float velocidad = 3f;

    [SerializeField] private int puntoActual;
    private Vector3 destino;

    void Update()
    {
        switch (puntoActual)
        {
            case 1:
                destino = Punto1.transform.position;
                break;

            case 2:
                destino = Punto2.transform.position;
                break;

            case 3:
                destino = Punto3.transform.position;
                break;

            case 4:
                destino = Punto4.transform.position;
                break;

            case 5:
                destino = Punto5.transform.position;
                break;

            case 6:
                destino = Punto6.transform.position;
                break;

            case 7:
                destino = Punto7.transform.position;
                break;

            case 8:
                destino = Punto8.transform.position;
                break;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            destino,
            velocidad * Time.deltaTime
        );
    }

    public void CambiarPunto()
    {
        puntoActual++;

        if (puntoActual > 8)
        {
            puntoActual = 1;
        }
    }


}
