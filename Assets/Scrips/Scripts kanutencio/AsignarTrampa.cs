using UnityEngine;

public class AsignarTrampa : MonoBehaviour
{
    [Header("Trampa")]
    public Sprite ImagenTrampa;
    public GameObject Trampa;


    public IntermediarioObjetos InterObj;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void AsigTrampa()
    {
        InterObj.Objeto = Trampa;
    }

}
