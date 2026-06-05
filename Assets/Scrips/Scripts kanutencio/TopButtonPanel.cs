using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopButtonPanel : MonoBehaviour
{
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject buttonPrefab;

    private List<GameObject> buttons = new List<GameObject>();

    public void AddButton(GameObject source, GameObject target)
    {
        GameObject btn = Instantiate(buttonPrefab, buttonContainer);
        btn.GetComponent<Button>().onClick.AddListener(() =>
        {
            source.transform.SetParent(target.transform, false);
            source.transform.localPosition = Vector3.zero;
        });
        buttons.Add(btn);
    }
}