using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTurret : MonoBehaviour
{

    public GameObject QuadTurretSprite;
    private Rigidbody2D rb;
    bool canRotate;
    private Animator QuadAnim;

    // Start is called before the first frame update
    void Start()
    {
        //QuadTurretFiring.CanShootTrue(true);
        canRotate = false;
        rb = QuadTurretSprite.GetComponent<Rigidbody2D>();
        QuadAnim = QuadTurretSprite.GetComponent<Animator>();
        StartCoroutine("Cooldown");
    }

    // Update is called once per frame
    void Update()
    {
        if (canRotate == true)
        {
            rb.rotation += .25f;
        }
        
    }

    IEnumerator Firing()
    {
        QuadTurretFiring.canShoot = true;
        QuadAnim.SetTrigger("Firing");
        //SoundManager.PlaySound(SoundManager.Sound.Quad, 0.5f);
        canRotate = true;
        yield return new WaitForSeconds(5);
        StartCoroutine("Cooldown");
    }
    IEnumerator Cooldown()
    {
        QuadAnim.SetTrigger("Reload");
        QuadTurretFiring.canShoot = false;
        canRotate = false;
        yield return new WaitForSeconds(3);
        StartCoroutine("Firing");


    }
}
