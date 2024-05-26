using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class BossMovement : MonoBehaviour
{
    [SerializeField] Animator animator;

    [SerializeField] GameObject player;
    [SerializeField] int hitNearDamage, maxHealth, currentHealth;
    [SerializeField] float walkingSpeed, maxDistanceToPlayerX, launchingLasersPlaceX;

    private int launchingLasersYDirection = 1;
    private bool hiting = false, hitNearCoolDown = true, launchingLasers = false, launchingALaser = false;

    Vector2 playerPositionReference;

    private void Start()
    {
        player = GameObject.Find("Player");
        animator = GetComponent<Animator>();
    }

    private void Update()
    {

        if(playerPositionReference.x > 0) 
        {
            transform.localScale = new Vector3(-2, 2, 2);
        }
        else
        {
            transform.localScale = new Vector3(2, 2, 2);
        }


        MoveBoss();

        HitPlayerNear();

        

        if (currentHealth < 70 && currentHealth > 30)
        {
            StartCoroutine(LaunchLasers());
        }
    }

    private void MoveBoss()
    {
        playerPositionReference = (player.transform.position - transform.position);

        if (Mathf.Abs(playerPositionReference.x) > maxDistanceToPlayerX && !hiting && !launchingLasers)
        {
            transform.Translate(playerPositionReference.normalized.x * walkingSpeed * Time.deltaTime, 0, 0);
        }

        if (Mathf.Abs(playerPositionReference.y) != 0 && !hiting && !launchingLasers)
        {
            transform.Translate(0, playerPositionReference.normalized.y * walkingSpeed * Time.deltaTime, 0);
        }
    }

    private void HitPlayer(int Damage)
    {
        Debug.Log(Damage);
    }

    private void HitPlayerNear()
    {
        if (Mathf.Abs(playerPositionReference.y) < 1 && Mathf.Abs(playerPositionReference.x) < 2 && !hiting && hitNearCoolDown && !launchingLasers)
        {
            hitNearCoolDown = false;
            hiting = true;
            StartCoroutine(HitPlayerAnimation());
        }
    }

    private IEnumerator HitPlayerAnimation()
    {
        animator.SetBool("hitplayer", true);
        yield return new WaitForSeconds(2);
        animator.SetBool("hitplayer", false);
        hiting = false;
        yield return new WaitForSeconds(6);
        hitNearCoolDown = true;
    }

    private IEnumerator LaunchLasers()
    {
        launchingLasers = true;
        while(launchingLasers && !launchingALaser)
        {
            if (transform.position.x < launchingLasersPlaceX)
            {
                transform.Translate((walkingSpeed * 4) * Time.deltaTime, 0, 0);
            }
            else
            {
                transform.Translate(0, (walkingSpeed * 20) * launchingLasersYDirection * Time.deltaTime, 0);
            }
        }

        if (launchingALaser)
        {
            StartCoroutine(LaunchALaser());
        }
        yield return new WaitForSeconds(3);
        launchingALaser = true;
        yield return new WaitForSeconds(15);
        launchingLasers = false;
    }

    private IEnumerator LaunchALaser()
    {
        animator.SetBool("launchlaser", true);
        yield return new WaitForSeconds(3);
        launchingALaser = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            HitPlayer(hitNearDamage);
        }

        if(collision.CompareTag("BossLimitLasers"))
        {
            launchingLasersYDirection *= -1;
        }
    }
}
