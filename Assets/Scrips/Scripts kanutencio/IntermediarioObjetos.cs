using UnityEngine;

public class IntermediarioObjetos : MonoBehaviour
{
    public GameObject Objeto;
    public int Nobjetos;


    public void LessTrampa()
    {
        Nobjetos--;
    }
    public void SumarTRampa7()
    {
        Nobjetos = 7;
    }
    public void SumarTRampa5()
    {
        Nobjetos = 5;
    }
}
