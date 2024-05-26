using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Move : MonoBehaviour
{
    public Transform player; // Referencia al transform del jugador
    public float speed = 5f; // Velocidad a la que el enemigo se mover� hacia el jugador
    public float minDistance = 1f; // Distancia m�nima para activar la animaci�n de ataque
    public float retreatDistance = 5f; // Distancia a la que se alejar� el enemigo despu�s de atacar
    public float retreatSpeed = 3f; // Velocidad a la que el enemigo se alejar�
    public int maxCollisionCount = 3; // Cantidad m�xima de colisiones permitidas antes de destruirse
    public float attackDelay = 1f; // Tiempo de espera antes de iniciar la animaci�n de ataque
    public float postAttackDelay = 1f; // Tiempo de espera despu�s de la animaci�n de ataque
    int hitsToKb = 0;
    float timeForKb = 0;
    public SpawnerActivate spawner; // Referencia al spawner
    Animator animator;

    private bool followingPlayer = true; // Variable para controlar si el enemigo sigue al jugador o no
    private int collisionCount = 0; // Contador de colisiones
    private Vector2 lastDirection; // Direcci�n en la que el enemigo se acerc� al jugador

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si la colisi�n proviene del jugador
        if (collision.CompareTag("hitplayer"))
        {
            // Detiene al enemigo y espera un segundo si es la primera o segunda colisi�n
            if (collisionCount < 2)
            {
                StopEnemyForOneSecond();
            }
            if (hitsToKb >= 3)
            {
                // Puedes agregar l�gica adicional aqu� si lo deseas
            }

            // Incrementa el contador de colisiones
            collisionCount++;
            hitsToKb++;
            timeForKb = Time.time;

            // Si el contador llega a la cantidad m�xima, destruye el objeto enemigo
            if (collisionCount >= maxCollisionCount)
            {
                Destroy(gameObject);
                spawner.enemiesSlain++;
            }
            else
            {
                // Si no ha alcanzado el l�mite, detiene completamente al enemigo
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
        if (Time.time - timeForKb > 0.6f)
        {
            hitsToKb = 0;
        }

        if (player != null)
        {
            // Calcula la direcci�n hacia la posici�n del jugador
            Vector2 direction = (player.position - transform.position).normalized;

            // Si el enemigo sigue al jugador
            if (followingPlayer)
            {
                // Guarda la �ltima direcci�n en la que se movi� hacia el jugador
                lastDirection = direction;

                // Mueve al enemigo hacia el jugador con una velocidad constante
                transform.Translate(direction * speed * Time.deltaTime);

                // Verifica la posici�n relativa del jugador
                if (player.position.x < transform.position.x)
                {
                    // Jugador est� a la izquierda, enemigo debe estar normal
                    if (transform.localScale.x < 0)
                    {
                        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    }
                }
                else if (player.position.x > transform.position.x)
                {
                    // Jugador est� a la derecha, enemigo debe girar 180 grados
                    if (transform.localScale.x > 0)
                    {
                        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    }
                }
            }

            // Calcula la distancia entre el enemigo y el jugador
            float distance = Vector2.Distance(transform.position, player.position);

            // Si el enemigo est� lo suficientemente cerca del jugador
            if (distance <= minDistance && followingPlayer)
            {
                // Deja de seguir al jugador y se prepara para atacar
                followingPlayer = false;
                StartCoroutine(WaitAndAttack());
            }
            // Si el enemigo ha retrocedido lo suficiente
            else if (!followingPlayer && distance >= retreatDistance)
            {
                followingPlayer = true; // Vuelve a seguir al jugador
            }
        }
    }

    // Corrutina para esperar antes de iniciar la animaci�n de ataque y luego moverse
    private IEnumerator WaitAndAttack()
    {
        yield return new WaitForSeconds(attackDelay); // Espera antes de iniciar la animaci�n de ataque

        animator.SetBool("Attacking", true); // Activa la animaci�n de ataque

        // Espera a que termine la animaci�n de ataque
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Desactiva la animaci�n de ataque
        animator.SetBool("Attacking", false);

        // Espera un tiempo despu�s de la animaci�n de ataque antes de moverse
        yield return new WaitForSeconds(postAttackDelay);

        // Mueve al enemigo hacia la direcci�n contraria
        StartCoroutine(MoveAway());
    }

    // Corrutina para mover al enemigo en la direcci�n opuesta a la �ltima direcci�n conocida hacia el jugador
    private IEnumerator MoveAway()
    {
        Vector2 startPos = transform.position;
        Vector2 endPos;

        // Determina la direcci�n de alejamiento (opuesta a la �ltima direcci�n conocida hacia el jugador)
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

        transform.position = endPos; // Aseg�rate de que el enemigo est� en la posici�n final

        // Una vez que se haya movido a la derecha o izquierda, vuelve a seguir al jugador
        followingPlayer = true;
    }
}









