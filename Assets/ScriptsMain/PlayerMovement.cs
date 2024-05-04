using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum PlayerState
{
    walk,
    attack,
    interact,
    stagger,
    idle
}

public class PlayerMovement : MonoBehaviour
{
    //[SerializeField] Rigidbody rb;
    // Start is called before the first frame update
    [SerializeField] public float health;
    public float speed;
    public FloatValue playerHealth;
    private Rigidbody2D myRigidbody;
    private Vector3 change;
    private Vector3 ardir;
    public GameObject projectile;
    private Animator animator;
    public PlayerState currentState;
    [SerializeField] public Vector2 direction;
    [SerializeField] BarraDeVida barra;
    // PlayerDash pd;
    void Start()
    {
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        ardir.y = -1;
        //pd = GetComponent<PlayerDash>();
    }
    private void Update()
    {
        // if (isDashing)
        // {
        //   return;
        // }
        if (change.x == 0 && change.y == 0)
        {

        }
        else
        {
            ardir.x = change.x;
            ardir.y = change.y;
        }
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal") * Time.deltaTime ;
        change.y = Input.GetAxisRaw("Vertical") * Time.deltaTime ;
        

        if (Input.GetButtonDown("attack") && currentState != PlayerState.attack)
        {
            Debug.Log("attack button prssed");
            StartCoroutine(AttackCo());
        }
        else if (Input.GetButtonDown("attack2") && currentState != PlayerState.attack && currentState != PlayerState.stagger)//attaaaaaaaack
        {
            Debug.Log("sword button prssed");
            StartCoroutine(AttackSword());
        }
        else if (currentState == PlayerState.walk || currentState == PlayerState.idle)
        {
            //UpdateAnimationAndMove();
            currentState = PlayerState.walk;
        }
        //if (Input.GetButtonDown("Dash"))
        //{

        //    if (canDash)
        //    {

        //        StartCoroutine(Dash());
        //    }
        //}
        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");
        
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void Awake()
    {
        health = playerHealth.initialValue;
    }

    private void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }
    void UpdateAnimationAndMove()
    {
        if (change != Vector3.zero)
        {

            transform.Translate(new Vector3(change.x, change.y));
            animator.SetFloat("moveX", change.x);
            animator.SetFloat("moveY", change.y);
            animator.SetBool("moving", true);

        }
        else
        {
            animator.SetBool("moving", false);
        }
        MoveCharacter();

    }
    private IEnumerator AttackCo()
    {
        currentState = PlayerState.attack;
        MakeArrow();
        yield return new WaitForSeconds(.3f);
        currentState = PlayerState.walk;
        //if (currentState != PlayerState.interact)
        //{

        // }
    }

    private IEnumerator AttackSword()
    {
        animator.SetBool("attacking", true);
        currentState = PlayerState.attack;
        yield return new WaitForSeconds(.8f);
        animator.SetBool("attacking", false);
        currentState = PlayerState.walk;
    }


    //[SerializeField] float dashSpeed = 20f;
    //[SerializeField] float dashDuration = .2f;
    //[SerializeField] float dashCooldown = 1f;
    //bool isDashing;
    //bool canDash = true;
    /***
    private IEnumerator Dash()
    {
        
        
        canDash = false;
        isDashing = true;
        myRigidbody.velocity=direction.normalized*dashSpeed;
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    */
    void MoveCharacter()
    {

        myRigidbody.MovePosition(transform.position + change.normalized * speed * Time.deltaTime
        );
        Physics.SyncTransforms();

    }
    private void MakeArrow()
    {
        Vector2 temp = new Vector2(ardir.x, ardir.y);
        Arrow arrow = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Arrow>();
        //arrow.Setup(temp, ChooseArrowDirection());
    }

    Vector3 ChooseArrowDirection()
    {
        float temp = Mathf.Atan2(ardir.x, ardir.y) * Mathf.Rad2Deg;
        return new Vector3(0, 0, temp);
    }
    private IEnumerator KnockCo(float knockTime)
    {
        if (myRigidbody != null || currentState != PlayerState.stagger)
        {
            yield return new WaitForSeconds(knockTime);
            myRigidbody.velocity = Vector2.zero;
            currentState = PlayerState.idle;
            myRigidbody.velocity = Vector2.zero;
        }
    }
    public void Knock(float knockTime, float damage)
    {

        StartCoroutine(KnockCo(knockTime));
        Debug.Log("Knock");
        TakeDamage(damage);
        barra.CambiarVidaActual(health);
    }

    void MovePlayer()
    {
        Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        
        myRigidbody.velocity = direction * Time.fixedDeltaTime * speed;
    }
}
