using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    private Tilemap tm;
    public Tile tile;

    private HealthAddon health;
    private Rigidbody2D rb2d;

    [SerializeField] private float actionTimer = 2f;
    private float m_time;

    private string[] actions;
    // Start is called before the first frame update
    void Start()
    {
        //note an enemy does not 'have' to have health so health may be null.
        health = this.GetComponent<HealthAddon>();
        rb2d = GetComponent<Rigidbody2D>();
        tm = GameObject.Find("/Grid").transform.GetChild(0).GetComponent<Tilemap>();

        m_time = 0f;
        actions = new string[]{"Move", "Stay"};
    }   

    // Update is called once per frame
    void Update()
    {
        if (m_time > actionTimer) {
            m_time = 0;
            //chose action
            int rand = (int)UnityEngine.Random.Range(0, actions.Length);
            Invoke("Action_" + actions[rand], 0f);
        }
        m_time += Time.deltaTime;

        //Put this somewhere else after proto
        Vector3Int pos = tm.WorldToCell(this.transform.position);
        TileBase c_tile = tm.GetTile(pos);
        if (c_tile != null) {
            tm.SetTile(pos, tile);
        }
    }

    //Action Methods should only be called from itself
    protected virtual void Action_Stay ()
    {
        Debug.Log("doing nothing");
        //do nothing
        return;
    }
    protected virtual void Action_Move ()
    {
        //Get closest 'player'
        GameObject player = GameObject.Find("/Player");
        if (player == null) {
            Debug.Log("no player detected");
            //no player detected
            return;
        }

        Vector3 dir = player.transform.position - this.transform.position;
        rb2d.AddForce(dir * 10);
        //do nothing
        return;
    }
    
}
