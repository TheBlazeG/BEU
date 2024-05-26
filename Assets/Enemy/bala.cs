using System.Collections;
using UnityEngine;

public class Bala : MonoBehaviour
{
    public string playerTag = "Player"; // Etiqueta del jugador en la escena
    public float speed = 10f; // Velocidad de la bala
    private Vector3 targetPosition; // Posición objetivo (posición actual del jugador)

    void Start()
    {
        // Inicia la corrutina para destruir la bala después de 15 segundos
        StartCoroutine(DestroyAfterTime(15f));

        // Busca el GameObject del jugador en la escena por etiqueta
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            // Establece la posición objetivo como la posición actual del jugador
            targetPosition = playerObject.transform.position;
        }
        else
        {
            Debug.LogWarning("No se encontró ningún objeto con la etiqueta " + playerTag + " en la escena.");
        }
    }

    void Update()
    {
        // Mueve la bala en línea recta hacia la posición objetivo
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Verifica si la bala ha alcanzado la posición objetivo
        if (transform.position == targetPosition)
        {
            Destroy(gameObject); // Destruye la bala
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si la bala colisiona con el jugador
        if (collision.CompareTag(playerTag))
        {
            Destroy(gameObject); // Destruye la bala al impactar con el jugador
        }
    }

    private IEnumerator DestroyAfterTime(float time)
    {
        // Espera un tiempo específico antes de destruir la bala
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}

