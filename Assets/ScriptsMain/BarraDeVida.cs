using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class BarraDeVida : MonoBehaviour
{

    [SerializeField]private Slider slider;
    [SerializeField] public float health;
    [SerializeField] PlayerMovement player;

    private void Start()
    {
        slider = GetComponent<Slider>();
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    public void CambiarVidaMaxima()
    {
        slider.maxValue = player.maxPlayerHealth;
        slider.value = player.maxPlayerHealth;
    }

    public void CambiarVidaActual()
    {
        if (player.health <= 0)
        {
            //SceneManager.LoadScene("muerte");
        } else
        {
            slider.value = player.health;
        }

        
    }

    public void InicializarBarraDeVida()
    {
        CambiarVidaMaxima();
        CambiarVidaActual();
    }
}
