using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweepRoomUI : MonoBehaviour
{
    [SerializeField] RoomManager RoomManager = default;
    public GameObject ClickButton;
    public GameObject SweepTextUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        StartCoroutine(SweepRoomStart());
        //throw new NotImplementedException();
    }


    IEnumerator SweepRoomStart()
    {
        ClickButton.SetActive(true);
        SweepTextUI.SetActive(true);
        yield return new WaitForSeconds(4f);
        SweepTextUI.SetActive(false);
        
        yield return new WaitForSeconds(3f);
        ClickButton.SetActive(false);
    }
    
}
