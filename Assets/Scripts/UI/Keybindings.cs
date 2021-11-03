using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity;

public class Keybindings : MonoBehaviour
{
    public PlayerInputMap pim;

    public BindingButton prefab;

    void Start()
    {
        //generate keybinds here
        int itr = 0;
        foreach (var act in pim.inputMap.actions) {
            if (!act.name.StartsWith("Debug")) {
                var go = Instantiate(prefab, transform);
                go.GetComponent<RectTransform>().anchoredPosition -= new Vector2 (0, (++itr) * 100);
                go.bindingAction = act;
                go.SetString();
            }
            
        }
        
    }
    void OnEnable ()
    {
        pim.inputMap.Disable();
    }
    void OnDisable()
    {
        pim.inputMap.Enable();
    }
}