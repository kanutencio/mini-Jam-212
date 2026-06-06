using System.Collections.Generic;
using UnityEngine;

public class TrapSelector : MonoBehaviour
{
    public OrganizacionLista OGList;
    public Transform padre;
    private List<GameObject> botones = new List<GameObject>();


    private void OnEnable()
    {
        for (int i = 0; i < 3; i++) {
            if (OGList.ListaBotonesDesordenados[i] == null) {
                return;
            } 
            else
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
        Debug.Log("Lista inicial");
        GameObject btn = Instantiate(button, parent);
        botones.Add(btn);
    }
}
