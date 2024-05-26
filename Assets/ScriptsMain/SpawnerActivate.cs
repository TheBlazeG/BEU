using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnerActivate : MonoBehaviour
{
   [SerializeField] GameObject spawners;
    public int enemiesSlain=0;
    public bool canSpawn=true;
    public int determineSpawn=3;
    // Start is called before the first frame update
    void Start()
    {
       
    
    }

    // Update is called once per frame
    void Update()
    {
        if (enemiesSlain==determineSpawn) { canSpawn = true; }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
    spawners.SetActive(true);
    }
}
