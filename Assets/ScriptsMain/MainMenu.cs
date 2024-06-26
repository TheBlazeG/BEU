using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Este método se llama cuando se presiona el botón Jugar
    public void PlayGame()
    {
        // Asegúrate de que la escena del juego esté agregada en la lista de escenas en la configuración de Build
        SceneManager.LoadScene("Boss");
    }

    public void LoadMainMenu()
    {
        // Asegúrate de que la escena del juego esté agregada en la lista de escenas en la configuración de Build
        SceneManager.LoadScene("MainMenu");
    }

    public void Retry()
    {
        // Asegúrate de que la escena del juego esté agregada en la lista de escenas en la configuración de Build
        SceneManager.LoadScene("Boss");
    }

    // Este método se llama cuando se presiona el botón Salir
    public void QuitGame()
    {
        // Esto funcionará en una build del juego. En el editor no tendrá efecto.
        Debug.Log("Salir presionado");
        Application.Quit();
    }
}

