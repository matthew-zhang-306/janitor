using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashUI : MonoBehaviour
{

    public GameObject Player;
    public Slider dashBar;
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = Player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        dashBar.value = playerController.DashCooldownUI();
    }
}
