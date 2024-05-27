using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pause : MonoBehaviour
{
    private bool paused = false;
  
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (paused)
            {
                Time.timeScale = 1.0f;
                paused = false;
            }
            else
            {
                paused = true;
                Time.timeScale = 0.0f;
            }

        }
    }
}
