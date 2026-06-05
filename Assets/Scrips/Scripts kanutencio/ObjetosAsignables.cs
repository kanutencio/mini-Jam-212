using UnityEngine;

public class ObjetosAsignables : MonoBehaviour
{
    public IntermediarioObjetos InterObj;
    void Start()
    {
        
    }

    // Update is called once per frame
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
        }
    }
}
