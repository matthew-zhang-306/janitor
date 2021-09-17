using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunkBulletPooler : MonoBehaviour
{
   

    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool;

    public static GunkBulletPooler SharedInstance;

    #region Singleton
    public static GunkBulletPooler Instance { get; private set; }

    private void Awake()
    {
        SharedInstance = this;
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    #endregion  

    

    // Start is called before the first frame update
    void Start()
    {

        pooledObjects = new List<GameObject>();
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject obj = (GameObject)Instantiate(objectToPool);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }


    }

    public GameObject GetPooledObject()
    {
        //1
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            //2
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        //3   
        return null;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
