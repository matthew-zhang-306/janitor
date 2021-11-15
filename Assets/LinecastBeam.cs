using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinecastBeam : MonoBehaviour
{
    public bool laserOn;
    public LineRenderer line;
    public float distance;
    public Collider2D Walls;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (laserOn)
        {
            StartCoroutine("FireLaser");
        }
    }


    IEnumerator FireLaser()
    {
        line.enabled = true;

        while (laserOn)
        {
            Ray2D ray = new Ray2D(transform.position, transform.right);
            RaycastHit2D hit;

            line.SetPosition(0, ray.origin);

            hit = Physics2D.Raycast(ray.origin, Vector2.right, distance);

            if (hit.collider)
            {
                line.SetPosition(1, hit.point);
            }
            else
                line.SetPosition(1, ray.GetPoint(distance));

            yield return null;
        }
        line.enabled = false;
    }

}
