using UnityEngine;

//kanuto therian

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

    [Header("Game Managers")]
    [SerializeField] private GameObject GameManager1;
    [SerializeField] private GameObject GameManager2;
    [SerializeField] private GameObject GameManager3;

    [Header("Ruleta")]
    [SerializeField] private GameObject ruleta;

    [Header("Configuración")]
    [SerializeField] private float velocidad = 3f;
    [SerializeField] private int puntoActual = 1;

    private Vector3 destino;
    private bool nivelActivado = false;
    private bool nivelCambiando = false;

    private void OnEnable()
    {
        GameManager.OnSoldadoLlegoAlFinal += OnSoldadoLlegoAlFinal;
        GameManager.OnHeroeEscapo += OnHeroeEscapo;
    }

    private void OnDisable()
    {
        GameManager.OnSoldadoLlegoAlFinal -= OnSoldadoLlegoAlFinal;
        GameManager.OnHeroeEscapo -= OnHeroeEscapo;
    }

    private void OnSoldadoLlegoAlFinal()
    {
        if (nivelCambiando) return;
        nivelCambiando = true;
        ruleta.SetActive(false);
        CambiarPunto();
    }

    private void OnHeroeEscapo()
    {
        if (nivelCambiando) return;
        nivelCambiando = true;
        ruleta.SetActive(false);
        CambiarPunto();
    }

    void Update()
    {
        switch (puntoActual)
        {
            case 1:
                destino = Punto1.transform.position;
                if (LlegoAlDestino())
                {
                    if (!nivelActivado)
                    {
                        nivelActivado = true;
                        ruleta.SetActive(true);
                    }
                }
                break;

            case 2:
                destino = Punto2.transform.position;
                if (LlegoAlDestino())
                {
                    if (!nivelActivado)
                    {
                        nivelActivado = true;
                        escena1.SetActive(true);
                        GameManager.SetNivelActual(0);
                        GameManager1.SetActive(true);
                    }
                }
                break;

            case 3:
                destino = Punto3.transform.position;
                if (LlegoAlDestino())
                {
                    if (!nivelActivado)
                    {
                        nivelActivado = true;
                        GameManager1.SetActive(false);
                        ruleta.SetActive(true);
                    }
                }
                break;

            case 4:
                destino = Punto4.transform.position;
                if (LlegoAlDestino())
                {
                    if (!nivelActivado)
                    {
                        nivelActivado = true;
                        ruleta.SetActive(false);
                        escena2.SetActive(true);
                        escena1.SetActive(false);
                        GameManager.SetNivelActual(1);
                        GameManager2.SetActive(true);
                    }
                }
                break;

            case 5:
                destino = Punto5.transform.position;
                if (LlegoAlDestino())
                {
                    if (!nivelActivado)
                    {
                        nivelActivado = true;
                        GameManager2.SetActive(false);
                        ruleta.SetActive(true);
                    }
                }
                break;

            case 6:
                destino = Punto6.transform.position;
                if (LlegoAlDestino())
                {
                    if (!nivelActivado)
                    {
                        nivelActivado = true;
                        ruleta.SetActive(false);
                        escena3.SetActive(true);
                        escena2.SetActive(false);
                        GameManager.SetNivelActual(2);
                        GameManager3.SetActive(true);
                    }
                }
                break;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            destino,
            velocidad * Time.deltaTime
        );
    }

    private bool LlegoAlDestino()
    {
        return Mathf.Abs(transform.position.x - destino.x) < 0.05f;
    }

    public void CambiarPunto()
    {
        nivelCambiando = false;
        puntoActual++;
        nivelActivado = false;

        if (puntoActual > 6)
            puntoActual = 1;
    }
}