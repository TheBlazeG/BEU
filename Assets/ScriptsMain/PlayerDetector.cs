using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    // Evento que se activará cuando el jugador entre en el trigger
    public delegate void PlayerDetected();
    public static event PlayerDetected OnPlayerDetected;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // El jugador ha sido detectado
            OnPlayerDetected?.Invoke();
        }
    }
}

