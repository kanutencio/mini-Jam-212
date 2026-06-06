using System.Collections.Generic;
using UnityEngine;

public class OrganizacionLista : MonoBehaviour
{
    [Header("ordenados")]
    public List<GameObject> ListaBotonesOrdenados;
    [Header("desordenados")]
    public List<GameObject> ListaBotonesDesordenados;

    void Start()
    {
        ListaBotonesDesordenados = Desordenar(new List<GameObject>(ListaBotonesOrdenados));
    }

    public static List<T> Desordenar<T>(List<T> lista)
    {
        int n = lista.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = lista[i];
            lista[i] = lista[j];
            lista[j] = temp;
        }
        return lista;
    }
}