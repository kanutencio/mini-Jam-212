using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopButtonPanel : MonoBehaviour
{
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject buttonPrefab;

    private List<GameObject> buttons = new List<GameObject>();

    public void AddButton(GameObject button, Transform parent)
    {
        GameObject btn = Instantiate(buttonPrefab, buttonContainer);
        buttons.Add(btn);
    }
}