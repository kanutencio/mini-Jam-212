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

    [Header("Escenas")]
    [SerializeField] private GameObject escena1;
    [SerializeField] private GameObject escena2;
    [SerializeField] private GameObject escena3;

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
                escena1.SetActive(true);
                break;

            case 3:
                destino = Punto3.transform.position;
                break;

            case 4:
                escena1.SetActive(false);
                destino = Punto4.transform.position;
                escena2.SetActive(true);
                break;

            case 5:
                destino = Punto5.transform.position;
                break;

            case 6:
                escena2.SetActive(false);
                destino = Punto6.transform.position;
                escena3.SetActive(true);
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
