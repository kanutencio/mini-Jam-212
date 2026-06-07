using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrapSelector : MonoBehaviour
{
    public OrganizacionLista OGList;
    public Transform padre;

    public AudioSource Audio;

    private List<GameObject> botones = new List<GameObject>();

    [SerializeField] private float fadeDuration = 0.5f;

    private void OnEnable()
    {
        Audio.Play();
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
        botones.Clear();

        for (int i = padre.childCount - 1; i >= 0; i--)
        {
            Destroy(padre.GetChild(i).gameObject);
        }
    }

    public void AddButton(GameObject button, Transform parent)
    {
        GameObject btn = Instantiate(button, parent);
        botones.Add(btn);

        StartCoroutine(FadeIn(btn));
    }

    public void CerrarPanel()
    {
        Audio.Play();
        StartCoroutine(FadeOutPanel());
    }

    private IEnumerator FadeIn(GameObject button)
    {
        Graphic[] graphics = button.GetComponentsInChildren<Graphic>();

        foreach (Graphic graphic in graphics)
        {
            Color color = graphic.color;
            color.a = 0;
            graphic.color = color;
        }

        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            float alpha = Mathf.Lerp(0, 1, t / fadeDuration);

            foreach (Graphic graphic in graphics)
            {
                Color color = graphic.color;
                color.a = alpha;
                graphic.color = color;
            }

            yield return null;
        }

        foreach (Graphic graphic in graphics)
        {
            Color color = graphic.color;
            color.a = 1;
            graphic.color = color;
        }
    }

    private IEnumerator FadeOutPanel()
    {
        Graphic[] graphics = padre.GetComponentsInChildren<Graphic>();

        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            float alpha = Mathf.Lerp(1, 0, t / fadeDuration);

            foreach (Graphic graphic in graphics)
            {
                if (graphic == null) continue;

                Color color = graphic.color;
                color.a = alpha;
                graphic.color = color;
            }

            yield return null;
        }

        gameObject.SetActive(false);
    }
}