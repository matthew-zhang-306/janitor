using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;

public class LevelCleanBar : MonoBehaviour
{
    public FloorController floor;
    public Slider cleanBar;

    // this value is currently set in the inspector
    // but it should be equal to whatever threshold the room manager has
    public float cleanThreshold;

    public Transform flagParent;
    public Image flagPrefab;
    public Image clearFlagPrefab;
    [SerializeField] private TextMeshProUGUI main;

    private static NumberFormatInfo nfi = new CultureInfo( "en-US", false ).NumberFormat;


    void Awake ()
    {
        nfi.PercentDecimalDigits = 0;
    }

    // Update is called once per frame
    void Update()
    {
        cleanBar.value = Mathf.Clamp01(floor.GetCleanPercent());

        main.text = cleanBar.value.ToString("P", nfi);
    }

    public void AddGoal(float thresh) 
    {
        if (thresh > 1 || thresh < 0) {
            Debug.Log("invalid wave thresh");
            return;
        }
        var flag = Instantiate(clearFlagPrefab, flagParent);
        
        flag.rectTransform.anchoredPosition += new Vector2 (0, thresh * 36);
    }

    public void AddWave(float thresh)
    {
        if (thresh > 1 || thresh < 0) {
            Debug.Log("invalid wave thresh");
            return;
        }

        var flag = Instantiate(flagPrefab, flagParent);
        
        flag.rectTransform.anchoredPosition += new Vector2 (0, thresh * 36);
    }
    public void Clear ()
    {
        
        //Clear previous wave (use for when resetting)
        foreach (Transform flags in flagParent) {
            Destroy(flags.gameObject);
        }
    }

}
