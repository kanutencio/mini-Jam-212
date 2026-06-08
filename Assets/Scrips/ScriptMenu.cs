using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScriptMenu : MonoBehaviour
{

    [Header("Play")]
    [SerializeField] private float velocidad;
    [SerializeField] private GameObject Punto0;
    [SerializeField] private GameObject Punto1;
    [SerializeField] private GameObject Punto2;
    [SerializeField] private int puntoActual;
    private Vector3 destino;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(
        transform.position,
        destino,
        velocidad * Time.deltaTime
        );
    }

    public void Jugar()
    {
        velocidad = 7;
        destino = Punto1.transform.position;
        StartCoroutine("CambioScena");
    }

    IEnumerator CambioScena()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Pruebas Kanutencio");
    }

    public void Instruciones()
    {
        velocidad = 12;
        destino = Punto2.transform.position;
    }

    public void SalirDelJuego()
    {
        // Cierra la aplicación (Funciona en compilaciones de PC/Android)
        Application.Quit();

        // Si estás probando en el Editor de Unity, esto detiene el modo juego
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
