using UnityEngine;
using UnityEngine.SceneManagement;

public class botonFinal : MonoBehaviour
{
    public AudioSource Sonido;
    private void Start()
    {
        
    }
    public void Jugar()
    {
        Sonido.Play();
        SceneManager.LoadScene("Main Manu");
    }

}
