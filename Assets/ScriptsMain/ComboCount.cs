using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class ComboCount : MonoBehaviour
{
    [SerializeField] public Slider barraCombo;
    [SerializeField] PlayerMovement playerMovement;
    
    int combo;
    float comboTimer;
    float meter;
    float comboMultiplier=0;
    // Start is called before the first frame update
    void Awake()
    {
        barraCombo.value = 0;
        barraCombo.maxValue = 100;
        
       
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Time.time-comboTimer);

        if ((Time.time - comboTimer) > 2.5)
        {
            combo = 0;
            comboMultiplier = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !playerMovement.inRage) {
            Debug.Log(comboMultiplier);
            comboTimer= Time.time;
            combo++;
            meter +=1+comboMultiplier;
            comboMultiplier += .25f;
            barraCombo.value=meter;

        }
    }

   
    //meter=10

    //meterup+= 1*combo
}
/*




   if(cantidad de barra>checkpoint && superActive=false)
   Mathf.Clamp(cantidad de barra, 15, 30);

   if (Input.GetButtonDown("super") && cantidad de barra!=maxcantidad de barra)
    {
   superActive=true;
    cantidad de barra-=15;
}

   if (Input.GetButtonDown("super") && cantidad de barra>=maxcantidad de barra)
    superActive=true;
    cantidad de barra=0;
   superMode=Active;







    */
