using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    public PlayerController player;

    //Main money display which is changed via tweening
    [SerializeField] private TextMeshProUGUI main;
    //Sub display that goes under main display
    [SerializeField] private TextMeshProUGUI sub;
    private CanvasGroup subgroup;
    [SerializeField] private float fadeTime = 2f;
    private int _main = 0;
    //buffer should hold 'true' value
    private int _buffer = 0;

    private int _batch = 0;
    private int _before = 0;

    private Tween batchFade;
    void Awake()
    {
        subgroup = sub.GetComponent<CanvasGroup>();
    }

    void Update()
    {
        _buffer = player.inventory.money;
        if (_buffer != _main) {
            if (batchFade.IsActive()) {
                batchFade.Kill();
                batchFade = null;
            }
            subgroup.alpha = 1f;
            if (_batch == 0) {
                _before = _main;
            }
            
            _batch = _buffer - _before;
            Debug.Log (_buffer + " " + _batch + " " + _before);
            
            
            _main += (int) Mathf.Sign(_buffer - _main);
        }
        else {
            if ((!batchFade.IsActive() || !batchFade.IsPlaying()) && _batch != 0) {
                batchFade = DOTween.To((x) => subgroup.alpha = x, 1f, 0f, fadeTime)
                        .SetLink(gameObject).SetTarget(this)
                        .OnComplete(()=> {_batch = 0;});
            }
            else {
                subgroup.alpha = 0f;
            }
        }
        main.text = String.Format("{0}", _main);
        sub.text = String.Format ("{0:+#;-#;+0}", _batch);
    }
}