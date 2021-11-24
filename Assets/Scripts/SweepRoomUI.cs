using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweepRoomUI : MonoBehaviour
{
    [SerializeField] RoomManager RoomManager = default;
    public GameObject ClickButton;
    public GameObject SweepTextUI;

    public GameObject MobileButton;
    bool PCBuild = true;

    // Start is called before the first frame update
    void Start()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            PCBuild = true;

        }
        if (Application.isMobilePlatform)
        {
            PCBuild = false;
        }
    }


    private void OnEnable()
    {
        RoomManager.AllEnemiesDefeatedEvent.AddListener(AllEnemiesDead);
    }

    private void OnDisable()
    {
        RoomManager.AllEnemiesDefeatedEvent.RemoveListener(AllEnemiesDead);
    }

    private void AllEnemiesDead()
    {
        if (PCBuild)
        {
            StartCoroutine(SweepRoomPC());
           
        }
        if (PCBuild == false)
        {
            StartCoroutine(SweepRoomMobile());
        }
        
       
    }


    IEnumerator SweepRoomPC()
    {
        ClickButton.SetActive(true);
        SweepTextUI.SetActive(true);
        yield return new WaitForSeconds(4f);
        SweepTextUI.SetActive(false);
        
        yield return new WaitForSeconds(3f);
        ClickButton.SetActive(false);
    }

    IEnumerator SweepRoomMobile()
    {
        MobileButton.SetActive(true);
        yield return new WaitForSeconds(7f);
        MobileButton.SetActive(false);
    }

}
