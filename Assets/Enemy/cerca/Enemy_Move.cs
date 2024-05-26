using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Move : MonoBehaviour
{
    public Transform player; // Referencia al transform del jugador
    public float speed = 5f; // Velocidad a la que el enemigo se moverá hacia el jugador
    public float minDistance = 1f; // Distancia mínima para activar la animación de ataque
    public float retreatDistance = 5f; // Distancia a la que se alejará el enemigo después de atacar
    public float retreatSpeed = 3f; // Velocidad a la que el enemigo se alejará
    public int maxCollisionCount = 3; // Cantidad máxima de colisiones permitidas antes de destruirse
    public float attackDelay = 1f; // Tiempo de espera antes de iniciar la animación de ataque
    public float postAttackDelay = 1f; // Tiempo de espera después de la animación de ataque
    int hitsToKb = 0;
    float timeForKb = 0;
    public SpawnerActivate spawner; // Referencia al spawner
    Animator animator;

    private bool followingPlayer = true; // Variable para controlar si el enemigo sigue al jugador o no
    private int collisionCount = 0; // Contador de colisiones
    private Vector2 lastDirection; // Dirección en la que el enemigo se acercó al jugador

    private void Start()
    {
        animator = GetComponent<Animator>();
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
            if (hitsToKb >= 3)
            {
                // Puedes agregar lógica adicional aquí si lo deseas
            }

            // Incrementa el contador de colisiones
            collisionCount++;
            hitsToKb++;
            timeForKb = Time.time;

            // Si el contador llega a la cantidad máxima, destruye el objeto enemigo
            if (collisionCount >= maxCollisionCount)
            {
                Destroy(gameObject);
                spawner.enemiesSlain++;
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
        if (Time.time - timeForKb > 0.6f)
        {
            hitsToKb = 0;
        }

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

    // Corrutina para esperar antes de iniciar la animación de ataque y luego moverse
    private IEnumerator WaitAndAttack()
    {
        yield return new WaitForSeconds(attackDelay); // Espera antes de iniciar la animación de ataque

        animator.SetBool("Attacking", true); // Activa la animación de ataque

        // Espera a que termine la animación de ataque
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Desactiva la animación de ataque
        animator.SetBool("Attacking", false);

        // Espera un tiempo después de la animación de ataque antes de moverse
        yield return new WaitForSeconds(postAttackDelay);

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









