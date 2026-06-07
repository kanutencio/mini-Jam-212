using UnityEngine;
using UnityEngine.UI;

public class ObjetosAsignables : MonoBehaviour
{
    public IntermediarioObjetos InterObj;
    public GameObject OBInterObj;
    private Button Boton;
    void Start()
    {
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
            Instantiate(InterObj.Objeto, transform.position, Quaternion.identity);
            InterObj.LessTrampa();
            Boton.interactable = false;
        }
    }
}
