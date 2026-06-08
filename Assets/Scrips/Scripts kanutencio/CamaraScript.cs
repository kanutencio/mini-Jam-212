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

    [Header("Game Manager")]
    [SerializeField] private GameObject GameManager1;
    [SerializeField] private GameObject GameManager2;
    [SerializeField] private GameObject GameManager3;

    [Header("Escenas")]
    [SerializeField] private GameObject ruleta; //no sabia como nombrarlo

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
                if (transform.position.x == Punto1.transform.position.x)
                {
                    ruleta.SetActive(true);
                }

                break;

            case 2:
                destino = Punto2.transform.position;
                if (transform.position.x == Punto2.transform.position.x)
                { 
                    escena1.SetActive(true);
                    GameManager1.SetActive(true);
                }
                

                break;

            case 3:
                destino = Punto3.transform.position;
                if (transform.position.x == Punto3.transform.position.x)
                {
                    ruleta.SetActive(true);
                }


                break;

            case 4:

                destino = Punto4.transform.position;

                if (transform.position.x == Punto4.transform.position.x)
                {
                    escena2.SetActive(true);
                    GameManager2.SetActive(true);
                    escena1.SetActive(false);        
                }

                break;

            case 5:

                destino = Punto5.transform.position;
                if (transform.position.x == Punto5.transform.position.x)
                {
                    ruleta.SetActive(true);
                }
                break;

            case 6:

                destino = Punto6.transform.position;

                if (transform.position.x == Punto6.transform.position.x)
                {
                    escena3.SetActive(true);
                    GameManager3.SetActive(true);
                    escena2.SetActive(false);
                }

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
