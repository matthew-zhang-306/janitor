using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (Button))]
[RequireComponent(typeof (Image))]
public class ShopItemButton : MonoBehaviour
{
    public delegate void ButtonDelegate (string name);

    public Sprite im;    

    public ButtonDelegate onClickEvent;

    void Start()
    {
        this.GetComponent<Image>().sprite = im;
 
    }

    public void OnClick()
    {
        
        onClickEvent?.Invoke(this.name);

    }
}