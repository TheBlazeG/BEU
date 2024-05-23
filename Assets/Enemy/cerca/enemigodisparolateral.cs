using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemigodisparolateral : MonoBehaviour
{
    public Transform player; // Referencia al transform del jugador
    public float speed = 5f; // Velocidad a la que el enemigo se moverá hacia el jugador
    public float waitBeforeFirstSummoning = 2f; // Tiempo de espera antes del primer disparo
    public float waitBeforeSummoning = 2f; // Tiempo de espera antes de disparar de nuevo
    public int maxCollisionCount = 3; // Cantidad máxima de colisiones permitidas antes de destruirse
    public GameObject summonedObject; // Objeto a invocar
    public float waitAfterSummoning = 2f; // Tiempo de espera después de disparar

    private int collisionCount = 0; // Contador de colisiones

    private void Start()
    {
        StartCoroutine(WaitBeforeFirstSummoning());
    }

    private void Update()
    {
        if (player != null)
        {
            // Calcula la dirección en el eje Y hacia la posición del jugador
            Vector2 direction = new Vector2(0, player.position.y - transform.position.y).normalized;

            // Mueve al enemigo hacia el jugador en la coordenada Y con una velocidad constante
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si la colisión proviene del jugador
        if (collision.CompareTag("hitplayer"))
        {
            // Detiene al enemigo y espera un segundo si es la primera o segunda colisión
            if (collisionCount < 2)
            {
                StopEnemyForOneSecond();
            }

            // Incrementa el contador de colisiones
            collisionCount++;

            // Si el contador llega a la cantidad máxima, destruye el objeto enemigo
            if (collisionCount >= maxCollisionCount)
            {
                Destroy(gameObject);
            }
            else
            {
                // Si no ha alcanzado el límite, detiene completamente al enemigo
                StopEnemy();
            }
        }
    }

    private void StopEnemyForOneSecond()
    {
        // Detiene al enemigo por un segundo
        StopEnemy();
        StartCoroutine(ResumeEnemyAfterDelay(1f));
    }

    private void StopEnemy()
    {
        // Detiene cualquier movimiento del enemigo
        speed = 0f;
    }

    private IEnumerator ResumeEnemyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        speed = 5f; // Vuelve a asignar la velocidad original
    }

    // Corrutina para esperar antes del primer disparo
    private IEnumerator WaitBeforeFirstSummoning()
    {
        yield return new WaitForSeconds(waitBeforeFirstSummoning);
        StartCoroutine(SummonObjectAndWait());
    }

    // Corrutina para esperar antes de disparar de nuevo
    private IEnumerator WaitBeforeNextSummoning()
    {
        yield return new WaitForSeconds(waitBeforeSummoning);
        StartCoroutine(SummonObjectAndWait());
    }

    // Corrutina para invocar el objeto y esperar después de disparar
    private IEnumerator SummonObjectAndWait()
    {
        // Invoca el objeto en la posición actual del enemigo
        Instantiate(summonedObject, transform.position, Quaternion.identity);

        // Detiene al enemigo temporalmente
        StopEnemy();

        // Espera después de disparar
        yield return new WaitForSeconds(waitAfterSummoning);

        // Vuelve a seguir al jugador
        speed = 5f; // Vuelve a asignar la velocidad original

        // Espera antes de disparar de nuevo
        StartCoroutine(WaitBeforeNextSummoning());
    }
}
