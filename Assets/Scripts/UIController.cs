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

    public GameObject ShowElement(string name)
    {
        for (int i = 0; i < UIElements.Length; i++)
        {
            if (UIElements[i] != null && UIElements[i].name == name)
            {
                UIElements[i].SetActive(true);
                return UIElements[i];
            }
        }
        return null;
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
