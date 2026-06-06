using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AsignarTrampa : MonoBehaviour
{
    [Header("Trampa")]
    public Sprite ImagenTrampa;
    [SerializeField] private Image imagen;
    public GameObject Trampa;


    [SerializeField] private float velocidadRotacion = 90f;
    private Quaternion rotacionObjetivo;

    private GameObject ObInter;
    public IntermediarioObjetos InterObj;

    private void Awake()
    {
        ObInter = GameObject.Find("Intermediario de asignacion");
        InterObj= ObInter.GetComponent<IntermediarioObjetos>();
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

}
