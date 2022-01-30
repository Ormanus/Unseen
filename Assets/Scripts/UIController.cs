using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour
{
    public static UIController Singleton;

    public GameObject[] UIElements;

    private void Awake()
    {
        if (Singleton != null)
        {
            Destroy(Singleton);
        }
        Singleton = this;
    }

    public void ShowElement(string name)
    {
        for (int i = 0; i < UIElements.Length; i++)
        {
            if (UIElements[i] != null && UIElements[i].name == name)
            {
                UIElements[i].SetActive(true);
                return;
            }
        }
        Debug.LogWarning($"UI element '{name}' not found.");
        return;
    }

    public void HideElement(string name)
    {
        for (int i = 0; i < UIElements.Length; i++)
        {
            if (UIElements[i] != null && UIElements[i].name == name)
            {
                UIElements[i].SetActive(false);
                return;
            }
        }
    }

    public void HideAll()
    {
        for (int i = 0; i < UIElements.Length; i++)
        {
            if (UIElements[i] != null)
            {
                UIElements[i].SetActive(false);
            }
        }
    }
}
