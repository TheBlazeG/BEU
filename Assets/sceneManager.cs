using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class sceneManager : MonoBehaviour
{
    private bool isPaused = false;

    // Referencia al men� de pausa (un GameObject que se activa/desactiva)
    public GameObject pauseMenuUI;

    void Update()
    {
        // Detectar si el jugador presiona la tecla "Escape" para pausar/reanudar
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    // M�todo para reanudar el juego
    public void Resume()
    {
        pauseMenuUI.SetActive(false);  // Desactivar el men� de pausa
        Time.timeScale = 1f;           // Reanudar el tiempo del juego
        isPaused = false;
    }

    // M�todo para pausar el juego
    void Pause()
    {
        pauseMenuUI.SetActive(true);   // Activar el men� de pausa
        Time.timeScale = 0f;           // Detener el tiempo del juego
        isPaused = true;
    }

    // M�todo para cargar la escena del men� principal
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;           // Asegurarse de que el tiempo del juego est� reanudado antes de cargar la escena
        SceneManager.LoadScene("MainMenu");  // Reemplazar "MainMenu" con el nombre de la escena del men� principal
    }

    // M�todo para salir del juego (solo funciona en compilaciones, no en el editor)
    public void QuitGame()
    {
        Application.Quit();
    }
}
