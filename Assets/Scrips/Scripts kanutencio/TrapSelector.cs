using System.Collections.Generic;
using UnityEngine;

public class TrapSelector : MonoBehaviour
{
    public OrganizacionLista OGList;
    public Transform padre;
    private List<GameObject> botones = new List<GameObject>();


    private void OnEnable()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i >= OGList.ListaBotonesDesordenados.Count)
                break;

            if (OGList.ListaBotonesDesordenados[i] != null)
            {
                AddButton(OGList.ListaBotonesDesordenados[i], padre);
            }
        }
    }

    private void OnDisable()
    {
        for (int i = padre.childCount - 1; i >= 0; i--)
        {
            Destroy(padre.GetChild(i).gameObject);
        }
    }
    public void AddButton(GameObject button, Transform parent)
    {
        GameObject btn = Instantiate(button, parent);
        botones.Add(btn);
    }
}
