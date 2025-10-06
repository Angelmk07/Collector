using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletUi : MonoBehaviour
{
    [SerializeField] private Image[] images;
    [SerializeField] private GameObject Panel;
    private int count;
    private void Awake()
    {
        count = images.Length-1;
    }
    public void ShowPanel()
    {
        Panel.SetActive(true);
    }
    public void HidePanel()
    {
        Panel.SetActive(false);
    }
    public void BulletRemove()
    {
        images[count].gameObject.SetActive(false);
        count--;
    }
    public void ResetUi()
    {
        for(int i =0;i<images.Length - 1; i++)
        {
            images[i].gameObject.SetActive(true);
        }
    }
}
