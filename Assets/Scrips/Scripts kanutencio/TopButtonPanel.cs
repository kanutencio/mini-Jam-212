using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopButtonPanel : MonoBehaviour
{

    private List<GameObject> buttons = new List<GameObject>();

    public void AddButton(GameObject button, Transform parent)
    {
        Debug.Log("Lista Final");

        GameObject btn = Instantiate(button, parent);
        buttons.Add(btn);
    }
}