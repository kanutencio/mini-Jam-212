using UnityEngine;
using UnityEngine.UI;

public class ObjetosAsignables : MonoBehaviour
{
    public IntermediarioObjetos InterObj;
    public GameObject OBInterObj;
    private Button Boton;

    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        OBInterObj = GameObject.Find("Intermediario de asignacion");
        InterObj = OBInterObj.GetComponent<IntermediarioObjetos>();
        Boton = GetComponent<Button>();
    }

    
    void Update()
    {

    }

    public void PonerObjeto()
    {
        if (InterObj == null)
        {
            Debug.LogError("no asignaste ningun objeto");
        }

        if (InterObj.Nobjetos > 0)
        {
            audioSource.Play();
            Instantiate(InterObj.Objeto, transform.position, Quaternion.identity);
            InterObj.LessTrampa();
            Boton.interactable = false;
        }
    }
}
