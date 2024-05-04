using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class BarraDeVida : MonoBehaviour
{

    private Slider slider;
    public FloatValue playerHealth;
    [SerializeField] public float health;
    [SerializeField] PlayerMovement player;

    private void Start()
    {
        slider = GetComponent<Slider>();
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    public void CambiarVidaMaxima(float playerHealth)
    {
        slider.maxValue = playerHealth;
    }

    public void CambiarVidaActual(float health)
    {
        if (player.health <= 0)
        {
            SceneManager.LoadScene("muerte");
        } else
        {
            slider.value = player.health;
        }

        
    }

    public void InicializarBarraDeVida(float health)
    {
        CambiarVidaMaxima(player.health);
        CambiarVidaActual(player.health);
    }
}
