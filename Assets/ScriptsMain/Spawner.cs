using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] SpawnerActivate spawner;
    [SerializeField] GameObject Enemy;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if(spawner.canSpawn)
        { Instantiate(Enemy);
            spawner.canSpawn = false;
        }
    }
}
