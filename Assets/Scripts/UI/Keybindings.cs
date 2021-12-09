using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity;
using UnityEngine.InputSystem;

public class Keybindings : MonoBehaviour
{
    public PlayerInputMap pim;

    public BindingButton prefab;

    void Awake()
    {
        if (pim == null) {
            pim = GameObject.Find("InputMap")?.GetComponent<PlayerInputMap>();
            if (pim == null) {
                //if still null, find the player
                pim = GameObject.Find("Player")?.GetComponent<PlayerInputMap>();
            }
        }
    }
    void Start()
    {
        //generate keybinds here
        int itr = 0;
        foreach (var act in PlayerInputMap.sInputMap.actions) {
            if (!act.name.StartsWith("Debug")) {
                
                var go = Instantiate(prefab, transform);
                go.GetComponent<RectTransform>().anchoredPosition -= new Vector2 (0, (++itr) * 100);
                go.bindingAction = act;
                go.SetString();
                // if (act.bindings[0].isComposite) {
                //     //Iterate through composite binding (move gets 4 different bindings)
                //     for (int i = 1; i < act.bindings.Count; i++) {
                //         var cgo = Instantiate(prefab, transform);
                //         cgo.GetComponent<RectTransform>().anchoredPosition -= new Vector2 (0, (++itr) * 100);
                //         cgo.bindingAction = act;
                //         cgo.index = i;
                //         cgo.SetString();
                //     }
                // }
            }
            
        }
        
    }
    void OnEnable ()
    {
        PlayerInputMap.sInputMap.Disable();
    }
    void OnDisable()
    {
        PlayerInputMap.sInputMap.Enable();
    }
}