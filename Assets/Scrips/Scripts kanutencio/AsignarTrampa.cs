using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AsignarTrampa : MonoBehaviour
{
    [Header("Trampa")]
    public Sprite ImagenTrampa;
    [SerializeField] private Image imagen;
    public GameObject Trampa;
    public GameObject ContraparteBoton;

    [Header("Asignar esta trampa")]
    private TopButtonPanel TBP;
    private GameObject ObjTBP;
    private GameObject Objpadre;
    private Transform TranPadre;


    [Header("Eliminar Trampa")]
    private GameObject ObjOrganizacionList;
    private OrganizacionLista OL;

    [SerializeField] private float velocidadRotacion = 90f;
    private Quaternion rotacionObjetivo;

    private GameObject ObInter;
    public IntermediarioObjetos InterObj;

    private void Awake()
    {
        ObInter = GameObject.Find("Intermediario de asignacion");
        InterObj= ObInter.GetComponent<IntermediarioObjetos>();

        Objpadre = GameObject.Find("ButtonContainer");
        TranPadre = Objpadre.GetComponent<Transform>();

        ObjTBP = GameObject.Find("TopPanel"); 
        TBP = ObjTBP.GetComponent<TopButtonPanel>();

        ObjOrganizacionList = GameObject.Find("Organizacion de Trampas");
        OL = ObjOrganizacionList.GetComponent<OrganizacionLista>();
    }
    void Start()
    {
        imagen.sprite = ImagenTrampa;
        rotacionObjetivo = Quaternion.Euler(0, 0, 0);
    }

    void Update()
    {
        if (Trampa == InterObj.Objeto)
        {
            rotacionObjetivo = Quaternion.Euler(0, 0, 60);
        }
        else
        {
            rotacionObjetivo = Quaternion.Euler(0, 0, 0);
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation , rotacionObjetivo , velocidadRotacion * Time.deltaTime);
    }

    public void AsigTrampa()
    {
        InterObj.Objeto = Trampa;
    }

    public void AsigBoton()
    {
        Destroy(gameObject);
        TBP.AddButton(ContraparteBoton, TranPadre);
        OL.ListaBotonesDesordenados.RemoveAll(obj => obj.name == gameObject.name.Replace("(Clone)", "")
);
    }
}
