using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Move : MonoBehaviour
{
    public Transform player; // Referencia al transform del jugador
    public float speed = 5f; // Velocidad a la que el enemigo se moverá hacia el jugador
    public float minDistance = 1f; // Distancia mínima para activar los colliders
    public float retreatDistance = 5f; // Distancia a la que se alejará el enemigo después de activar los colliders
    public float retreatSpeed = 3f; // Velocidad a la que el enemigo se alejará
    public Collider2D collider1; // Primer collider a activar
    public Collider2D collider2; // Segundo collider a activar
    public int maxCollisionCount = 3; // Cantidad máxima de colisiones permitidas antes de destruirse
    public float waitBeforeActivatingColliders = 2f; // Tiempo de espera antes de activar los colliders

    private bool followingPlayer = true; // Variable para controlar si el enemigo sigue al jugador o no
    private int collisionCount = 0; // Contador de colisiones
    private Vector2 lastDirection; // Dirección en la que el enemigo se acercó al jugador

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

    private void Update()
    {
        if (player != null)
        {
            // Calcula la dirección hacia la posición del jugador
            Vector2 direction = (player.position - transform.position).normalized;

            // Si el enemigo sigue al jugador
            if (followingPlayer)
            {
                // Guarda la última dirección en la que se movió hacia el jugador
                lastDirection = direction;

                // Mueve al enemigo hacia el jugador con una velocidad constante
                transform.Translate(direction * speed * Time.deltaTime);

                // Verifica la posición relativa del jugador
                if (player.position.x < transform.position.x)
                {
                    // Jugador está a la izquierda, enemigo debe estar normal
                    if (transform.localScale.x < 0)
                    {
                        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    }
                }
                else if (player.position.x > transform.position.x)
                {
                    // Jugador está a la derecha, enemigo debe girar 180 grados
                    if (transform.localScale.x > 0)
                    {
                        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    }
                }
            }

            // Calcula la distancia entre el enemigo y el jugador
            float distance = Vector2.Distance(transform.position, player.position);

            // Si el enemigo está lo suficientemente cerca del jugador
            if (distance <= minDistance && followingPlayer)
            {
                // Activa los colliders secuencialmente con una espera antes
                StartCoroutine(WaitAndActivateColliders());
                followingPlayer = false; // Deja de seguir al jugador
            }
            // Si el enemigo ha retrocedido lo suficiente
            else if (!followingPlayer && distance >= retreatDistance)
            {
                followingPlayer = true; // Vuelve a seguir al jugador
            }
        }
    }

    // Corrutina para esperar antes de activar los colliders secuencialmente
    private IEnumerator WaitAndActivateColliders()
    {
        yield return new WaitForSeconds(waitBeforeActivatingColliders);
        yield return StartCoroutine(ActivateCollidersSequentially());
    }

    // Corrutina para activar los colliders secuencialmente
    private IEnumerator ActivateCollidersSequentially()
    {
        // Activa el primer collider
        collider1.enabled = true;
        yield return new WaitForSeconds(0.5f);

        // Activa el segundo collider
        collider2.enabled = true;
        yield return new WaitForSeconds(0.5f);

        // Desactiva ambos colliders
        collider1.enabled = false;
        collider2.enabled = false;

        // Mueve al enemigo hacia la dirección contraria
        StartCoroutine(MoveAway());
    }

    // Corrutina para mover al enemigo en la dirección opuesta a la última dirección conocida hacia el jugador
    private IEnumerator MoveAway()
    {
        Vector2 startPos = transform.position;
        Vector2 endPos;

        // Determina la dirección de alejamiento (opuesta a la última dirección conocida hacia el jugador)
        if (lastDirection.x > 0)
        {
            endPos = startPos + Vector2.left * retreatDistance; // Alejarse a la izquierda
        }
        else
        {
            endPos = startPos + Vector2.right * retreatDistance; // Alejarse a la derecha
        }

        float elapsedTime = 0f;
        float moveTime = retreatDistance / retreatSpeed; // Tiempo de movimiento basado en la velocidad de retroceso

        while (elapsedTime < moveTime)
        {
            transform.position = Vector2.Lerp(startPos, endPos, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos; // Asegúrate de que el enemigo esté en la posición final

        // Una vez que se haya movido a la derecha o izquierda, vuelve a seguir al jugador
        followingPlayer = true;
    }
}






