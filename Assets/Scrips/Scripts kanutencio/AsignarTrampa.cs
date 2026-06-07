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

    [Header("Sonido")]
    private Button Boton;
    private AudioSource AS;

    [Header("Eliminar Trampa")]
    private GameObject ObjOrganizacionList;
    private OrganizacionLista OL;

    [Header("Modificacion Camara")]
    private GameObject ObjetoCamara;
    private CamaraScript ComponentCamara;

    [SerializeField] private float velocidadRotacion = 90f;
    private Quaternion rotacionObjetivo;

    private GameObject ObInter;
    public IntermediarioObjetos InterObj;

    private GameObject ObTrapSelec;
    private TrapSelector TrapSelec;

    private void Awake()
    {
        //componentes de este objeto
        AS = GetComponent<AudioSource>();
        Boton = GetComponent<Button>();

        //componentes de objetos externos
        ObInter = GameObject.Find("Intermediario de asignacion");
        InterObj= ObInter.GetComponent<IntermediarioObjetos>();

        ObTrapSelec = GameObject.Find("DownPanel");
        TrapSelec = ObTrapSelec.GetComponent<TrapSelector>();

        ObjetoCamara = GameObject.Find("Main Camera");
        ComponentCamara = ObjetoCamara.GetComponent<CamaraScript>();

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
        if (InterObj.Nobjetos <= 0)
        {
            Boton.interactable = false;
        }

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
        AS.Play();
        InterObj.Objeto = Trampa;
    }

    public void AsigBoton()
    {
        Boton.interactable = false;
        StartCoroutine("Sonido");
        TBP.AddButton(ContraparteBoton, TranPadre);
        OL.ListaBotonesDesordenados.RemoveAll(obj => obj.name == gameObject.name.Replace("(Clone)", ""));
        ComponentCamara.CambiarPunto();
        TrapSelec.CerrarPanel();
    }

    IEnumerator Sonido()
    {
        AS.Play();
        yield return new WaitForSeconds(0.4f);
        Destroy(gameObject);
        
    }
}
