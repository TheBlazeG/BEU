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
    public static int numberOfAttacks;
    float lastAttackedTime=0;
    float lastDodgeTime=0;
    float maxComboDelay = 1;
    private float nextFireTime = 0;
    bool canDash=true;
   
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

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime>.7 && animator.GetCurrentAnimatorStateInfo(0).IsName("Punch"))
        {
            animator.SetBool("attack1", false);
            numberOfAttacks = 0;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > .7 && animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerPunch2"))
        {
            Debug.Log("punch2");
            animator.SetBool("Attack2", false);
            numberOfAttacks = 0;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > .7 && animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerPunch3"))
        {
            Debug.Log("punch3");
            animator.SetBool("attack3", false);
            numberOfAttacks = 0;
        }

        if (Time.time - lastAttackedTime > maxComboDelay)
            numberOfAttacks = 0;
        if (Time.time - lastDodgeTime > .7)
            canDash = true;

        if (Time.time - lastDodgeTime > .3)
            speed=300;

        if (Time.time>nextFireTime)

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
        

        /*if (Input.GetButtonDown("attack") && currentState != PlayerState.attack)
        {
            lastAttackedTime = Time.time;
            numberOfAttacks++;
            Debug.Log("attack button prssed");
            StartCoroutine(AttackCo());
        
        }*/
        if(Time.time > nextFireTime)
        if (Input.GetButtonDown("attack2") && currentState != PlayerState.attack && currentState != PlayerState.stagger)//attaaaaaaaack
        {
            attack();
        }
        else if (currentState == PlayerState.walk || currentState == PlayerState.idle)
        {
            //UpdateAnimationAndMove();
            currentState = PlayerState.walk;
        }
        if (Input.GetButtonDown("Dash"))
        {

            if (canDash)
            {

                Dash(canDash);
            }
        }

        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");
        animator.SetFloat("LeftorRight",ardir.x);
       

       
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void Awake()
    {
        //health = playerHealth.initialValue;
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
        if (numberOfAttacks == 1)
        { animator.Play("Punch");
            animator.SetBool("attack1", true);
        }
        else if (numberOfAttacks == 2) 
        {
            animator.SetBool("Attack2", true);
        }
        else if (numberOfAttacks == 3) 
        {
            animator.SetBool("attack3", true);
        
        }

        currentState = PlayerState.attack;
        yield return new WaitForSeconds(.015f);
     
        currentState = PlayerState.walk;
    }


 
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
    void attack()
    {
        lastAttackedTime = Time.time;
        numberOfAttacks++;
        //StartCoroutine(AttackSword());
        if (numberOfAttacks == 1)
        {

            animator.SetBool("attack1", true);
        }
        numberOfAttacks = Mathf.Clamp(numberOfAttacks, 0, 3);
        if (numberOfAttacks >= 2 && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.04f && animator.GetCurrentAnimatorStateInfo(0).IsName("Punch"))

        {
            animator.SetBool("attack1", false);
            animator.SetBool("Attack2", true);
        }
        if (numberOfAttacks >= 3 && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.04f && animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerPunch2"))
        {
            animator.SetBool("Attack2", false);
            animator.SetBool("attack3", true);

        }


    }

    void Dash(bool Dash)
    {
        lastDodgeTime = Time.time;  
        speed=600;
    }
    /*
     agregamos nueva barra

    if(cantidad de barra>checkpoint && superActive=false)
    Mathf.Clamp(cantidad de barra, 15, 30);

    if (Input.GetButtonDown("super") && cantidad de barra!=maxcantidad de barra)
     {
    superActive=true;
     cantidad de barra-=15;
}

    if (Input.GetButtonDown("super") && cantidad de barra>=maxcantidad de barra)
     superActive=true;
     cantidad de barra=0;
    superMode=Active;
     
     
     
     
     
     
     
     */
}
