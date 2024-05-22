using System.Collections;
using UnityEngine;

public class enemigo_disparo : MonoBehaviour
{
    public Transform player; // Referencia al transform del jugador
    public float speed = 5f; // Velocidad a la que el enemigo se moverá hacia el jugador
    public float minDistance = 1f; // Distancia mínima para mantenerse del jugador
    public float waitBeforeFirstSummoning = 2f; // Tiempo de espera antes del primer disparo
    public float waitBeforeSummoning = 2f; // Tiempo de espera antes de disparar de nuevo
    public int maxCollisionCount = 3; // Cantidad máxima de colisiones permitidas antes de destruirse
    public GameObject summonedObject; // Objeto a invocar
    public float summoningDistance = 3f; // Distancia a la que se invocará el objeto
    public float waitAfterSummoning = 2f; // Tiempo de espera después de disparar

    private bool followingPlayer = true; // Variable para controlar si el enemigo sigue al jugador o no
    private Vector2 lastDirection; // Dirección en la que el enemigo se acercó al jugador
    private int collisionCount = 0; // Contador de colisiones

    private void Start()
    {
        StartCoroutine(WaitBeforeFirstSummoning());
    }

    private void Update()
    {
        if (player != null)
        {
            // Calcula la dirección hacia la posición del jugador
            Vector2 direction = (player.position - transform.position).normalized;

            // Calcula la distancia entre el enemigo y el jugador
            float distance = Vector2.Distance(transform.position, player.position);

            // Si el enemigo sigue al jugador
            if (followingPlayer)
            {
                // Guarda la última dirección en la que se movió hacia el jugador
                lastDirection = direction;

                // Si el enemigo está demasiado cerca del jugador
                if (distance < minDistance)
                {
                    // Se aleja del jugador
                    transform.Translate(-direction * speed * Time.deltaTime);
                }
                else
                {
                    // Mueve al enemigo hacia el jugador con una velocidad constante
                    transform.Translate(direction * speed * Time.deltaTime);
                }
            }
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
        followingPlayer = false;
    }

    private IEnumerator ResumeEnemyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        followingPlayer = true;
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
        followingPlayer = false;

        // Espera después de disparar
        yield return new WaitForSeconds(waitAfterSummoning);

        // Vuelve a seguir al jugador
        followingPlayer = true;

        // Espera antes de disparar de nuevo
        StartCoroutine(WaitBeforeNextSummoning());
    }
}



















