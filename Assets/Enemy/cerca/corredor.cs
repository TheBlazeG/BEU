using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class corredor : MonoBehaviour
{
    public float speed = 5f; // Velocidad a la que el enemigo se mover�
    private int collisionCount = 0; // Contador de colisiones
    private bool movingRight = true; // Variable para controlar la direcci�n del enemigo
    public int maxCollisionCount = 3; // Cantidad m�xima de colisiones permitidas antes de destruirse

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Cambia la direcci�n al colisionar con un objeto con la etiqueta "fin"
        if (collision.CompareTag("fin"))
        {
            ChangeDirection();
        }
        // Incrementa el contador de colisiones al colisionar con un objeto con la etiqueta "hitplayer"
        else if (collision.CompareTag("hitplayer"))
        {
            collisionCount++;
            // Si el contador llega a la cantidad m�xima, destruye el objeto enemigo
            if (collisionCount >= maxCollisionCount)
            {
                Destroy(gameObject);
            }
            else
            {
                // Detiene al enemigo por un segundo si no ha alcanzado el l�mite de colisiones
                StopEnemyForOneSecond();
            }
        }
    }

    private void ChangeDirection()
    {
        // Cambia la direcci�n del movimiento
        movingRight = !movingRight;
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

    private void Update()
    {
        // Mueve al enemigo de derecha a izquierda seg�n la direcci�n actual
        if (movingRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }
    }

}
