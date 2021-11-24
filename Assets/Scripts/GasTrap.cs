using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasTrap : MonoBehaviour
{

    public GameObject gasExplosion;
    private bool triggerGas;
    [SerializeField] private float Delay;
    [SerializeField] private float GasTime;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            triggerGas = true;
        }
    }

    public void Update()
    {
        if (triggerGas)
        {
            triggerGas = false;
            StartCoroutine("GasTimer");
        }
    }

    IEnumerator GasTimer()
    {
        yield return new WaitForSeconds(Delay);
        gasExplosion.SetActive(true);
        yield return new WaitForSeconds(GasTime);
        gasExplosion.SetActive(false);
    }
}
