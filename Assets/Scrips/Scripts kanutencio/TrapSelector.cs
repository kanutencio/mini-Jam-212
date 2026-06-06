using System.Collections.Generic;
using UnityEngine;

public class TrapSelector : MonoBehaviour
{
    public OrganizacionLista OGList;
    public Transform padre;
    private List<GameObject> botones = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < 3; i++) {
            AddButton(OGList.ListaBotonesDesordenados[i], padre);
        }
    }

    public void AddButton(GameObject button, Transform parent)
    {
        Debug.Log("Lista inicial");
        GameObject btn = Instantiate(button, parent);
        botones.Add(btn);
    }
}
