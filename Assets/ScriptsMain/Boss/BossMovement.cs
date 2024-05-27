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

    private int launchingLasersYDirection = 1, launchedLasers = 0;
    private float timerLaunchALaser = 0, timerLaunchingLasers = 0, timerMovingBossLasers = 0;
    private bool hiting = false, hitNearCoolDown = true, launchingLasers = false, launchingALaser = false, takeTimeTimerMovingBossLasers = true, takeTimeTimerLaunchingLasers = true, bossInXPlace = false;

    Vector2 playerPositionReference;

    private void Start()
    {
        player = GameObject.Find("Player");
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Debug.Log(bossInXPlace);

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

        

        if (currentHealth < 70 && currentHealth > 30 && launchedLasers <= 0)
        {
            launchedLasers++;
            launchingLasers = true;
        }

        if (launchingLasers)
        {
            animator.SetBool("launchlaser", true);
            MoveBossLaunchingLasers();
        }
        else
        {
            animator.SetBool("launchlaser", false);
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
    private void MoveBossLaunchingLasers()
    {


        if (takeTimeTimerLaunchingLasers)
        {
            timerLaunchingLasers = Time.time;
            takeTimeTimerLaunchingLasers = false;
        }

        if (Time.time - timerLaunchingLasers > 20)
        {
            launchingLasers = false;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.2f && animator.GetCurrentAnimatorStateInfo(0).IsName("BossLaunchLaser") && bossInXPlace)
        {
            Debug.Log("move");
            animator.SetBool("movinglaser", false);
            launchingALaser = true;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && animator.GetCurrentAnimatorStateInfo(0).IsName("LaunchLaser"))
        {
            animator.SetBool("movinglaser", true);
            launchingALaser = false;
        }
        
        if (transform.position.x < launchingLasersPlaceX)
        {
            bossInXPlace = false;
            transform.Translate((walkingSpeed * 4) * Time.deltaTime, 0, 0);
        }
        else
        {
            bossInXPlace = true;
            if(!launchingALaser) 
            {
                transform.Translate(0, (walkingSpeed * 20) * launchingLasersYDirection * Time.deltaTime, 0);
            }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && (hiting || launchingLasers))
        {
            HitPlayer(hitNearDamage);
        }

        if (collision.CompareTag("hitplayer"))
        {
            //takedamage;
        }

        if (collision.CompareTag("BossLimitLasers"))
        {
            launchingLasersYDirection *= -1;
        }
    }
}
