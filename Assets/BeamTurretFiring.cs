using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamTurretFiring : MonoBehaviour
{

    public bool canShoot;
    public Vector2 Offset = new Vector2(0, 0);

    public GameObject BeamSprite;
    private Animator BeamAnim;
    private bool Stupid = true;

    // Start is called before the first frame update
    void Start()
    {
        BeamAnim = BeamSprite.GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.GamePaused)
        {
            if (canShoot && Stupid)
            {
                Stupid = false;
                BeamAnim.SetBool("Firing", true);

            }
            if (!canShoot)
            {
                BeamAnim.SetBool("Firing", false);
                Stupid = true;
                
            }
        }
    }

}
