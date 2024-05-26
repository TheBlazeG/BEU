using UnityEngine;

public class CameraController : MonoBehaviour
{
    private bool playerDetected = false;

    private void Start()
    {
        PlayerDetector.OnPlayerDetected += FreezeCameraAndBlockEdges;
    }

    private void FreezeCameraAndBlockEdges()
    {
        playerDetected = true;
        // Congelar la cámara
        // Bloquear los extremos con colliders
    }

    private void Update()
    {
        if (playerDetected)
        {
            // Lógica para mantener la cámara congelada
            // por ejemplo, manteniendo su posición fija
        }
    }
}

