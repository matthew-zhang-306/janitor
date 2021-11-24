using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTurretBoss : BaseEnemy
{
    private bool canRotate;
    public Transform rotatingContainer;

    public bool canShoot { get; private set; }
    public float rotationSpeed = .3f;
    public float startTime = 1.0f;
    public float cooldownTime = 3.0f;
    public float firingTime = 3.0f;

    public GameObject bossHealthBarPrefab;
    private BossHealthBar healthBar;

    public GameObject BeamContainer;
    BeamTurretFiring[] beams;

    protected override void Start()
    {
        base.Start();
        canRotate = false;
        canShoot = false;
        beams = BeamContainer.GetComponentsInChildren<BeamTurretFiring>();
    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (CanAct && canRotate)
        {
            rotatingContainer.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
    }

    public override void OnInit() {
        StartCoroutine(Cooldown(startTime));
        
        healthBar = Instantiate(bossHealthBarPrefab).GetComponent<BossHealthBar>();
        healthBar.Init(this, "Quad Turret");
    }


    protected override void Die() {
        canShoot = false;
        canRotate = false;
        StopAllCoroutines();

        Helpers.Invoke(healthBar, healthBar.Destroy, 2f);
        base.Die();
    }

    protected override void OnPlayerDied(PlayerController player) {
        base.OnPlayerDied(player);

        canShoot = false;
        canRotate = false;
        StopAllCoroutines();

        Helpers.Invoke(this, () => {
            healthBar?.Destroy();
            healthBar = null;
        }, 1f);
    }


    IEnumerator Firing()
    {
        canShoot = true;
        animator.SetTrigger("Firing");
        foreach (BeamTurretFiring Beam in beams)
        {
            Beam.StartFiring();
        }
        SoundManager.PlaySound(SoundManager.Sound.Quad, 0.5f);
        canRotate = true;
        yield return new WaitForSeconds(firingTime);
        StartCoroutine(Cooldown(cooldownTime));
    }
    IEnumerator Cooldown(float time)
    {
        animator.SetTrigger("Reload");
        foreach (BeamTurretFiring Beam in beams)
        {
            Beam.StopFiring();
        }
        canShoot = false;
        canRotate = false;
        yield return new WaitForSeconds(time);
        StartCoroutine("Firing");
    }


}
