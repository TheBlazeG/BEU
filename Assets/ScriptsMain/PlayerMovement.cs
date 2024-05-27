using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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
    public float maxPlayerHealth=30;
    public float damage = 1;
    private Rigidbody2D myRigidbody;
    private Vector3 change;
    private Vector3 ardir;
    private Animator animator;
    public PlayerState currentState;
    [SerializeField] public Vector2 direction;
    [SerializeField] BarraDeVida barra;
     int numberOfAttacks;
    float lastAttackedTime=0;
    float lastDodgeTime=0;
    float maxComboDelay = .6f;
    private float nextFireTime = 0;
    bool canDash=true;
    [SerializeField] public bool inRage=false;
    bool isInChange=false;
    [SerializeField]ComboCount comboCount;
    
    PlayerMovement player;
    SpriteRenderer playerRenderer;

    // PlayerDash pd;
    void Start()
    {
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        ardir.y = -1;
        playerRenderer = GetComponent<SpriteRenderer>();
      
        //pd = GetComponent<PlayerDash>();

    }
    private void Update()
    {
        Debug.Log(currentState);

       
        

        if (Mathf.Abs(myRigidbody.velocity.x) > 0 || Mathf.Abs(myRigidbody.velocity.y) > 0) 
        {
            animator.SetBool("moving", true);
        }
        else
        {
            animator.SetBool("moving", false);
        }

        if (myRigidbody.velocity.x > 0 && currentState != PlayerState.attack)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        if(myRigidbody.velocity.x < 0 && currentState != PlayerState.attack)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (numberOfAttacks > 0)
        {
            currentState = PlayerState.attack;
        }
        else if(currentState != PlayerState.stagger && !inRage)
        {
            currentState = PlayerState.walk;
        }

        if (numberOfAttacks == 0)
        { animator.SetBool("attack1", false);
        animator.SetBool("Attack2",false);
        animator.SetBool("attack3",false);}
            

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime>.7 && animator.GetCurrentAnimatorStateInfo(0).IsName("Punch"))
        {
            animator.SetBool("attack1", false);
           
        }
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > .8 && animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerPunch2"))
        {
            
            animator.SetBool("Attack2", false);
           
        }
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >.99 && animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerPunch3"))
        {
            
            animator.SetBool("attack3", false);
            numberOfAttacks = 0;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && numberOfAttacks==3 )
        {
            
            numberOfAttacks = 0;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && numberOfAttacks == 3)
        {

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


        if (Input.GetButtonDown("attack") && currentState != PlayerState.stagger)
        {
           Rage();
        
        }

        if (Input.GetButtonDown("attack2")  && currentState != PlayerState.stagger)//attaaaaaaaack
        {
            attack();
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

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > .99 && animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerSuper1"))
        {
            animator.SetBool("Super", false);
            currentState=PlayerState.walk;
        }

    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void Awake()
    {
        health = maxPlayerHealth;
        barra.CambiarVidaMaxima();
    }

    private void TakeDamage(float damage)
    {
        health -= damage;
        barra.CambiarVidaActual();
        if (health <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }

    private IEnumerator KnockCo(float knockTime)
    {
        if (currentState != PlayerState.stagger)
        {
            currentState = PlayerState.stagger;
            yield return new WaitForSeconds(knockTime);
            myRigidbody.velocity = Vector2.zero;
            currentState = PlayerState.idle;
            myRigidbody.velocity = Vector2.zero;
        }
    }
    public void Knock(float damage)
    {

        StartCoroutine(KnockCo(.3f));
        Debug.Log("Knock");
        TakeDamage(damage);
        barra.CambiarVidaActual();
    }

    void MovePlayer()
    {
        Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        

        if (currentState != PlayerState.stagger && currentState != PlayerState.attack) 
        {
            myRigidbody.velocity = direction * Time.fixedDeltaTime * speed;
        }
        else
        {
            myRigidbody.velocity = new Vector2(0, 0);
        }

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
        if (numberOfAttacks >= 2 && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.2 )

        {
            animator.SetBool("attack1", false);
            animator.SetBool("Attack2", true);
        }
        if (numberOfAttacks >= 3 && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.3 )
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

    void Super()
    {
        if (inRage)
        {
            animator.SetBool("Super", true);
            currentState = PlayerState.attack;
        }
        
        else if (comboCount.barraCombo.value >= 10)
        {
            comboCount.barraCombo.value -= 10;
            animator.SetBool("Super", true);
            currentState = PlayerState.attack;
        }

    }
    public void Rage()
    {
        if (comboCount.barraCombo.value >= 20 && !inRage)
        {
            speed *= 1.5f;
            StartCoroutine(Ragetime());
            comboCount.barraCombo.value -= 20;
            Super();
            StartCoroutine(Colorchange(playerRenderer));


        }
        else
        {
            Super();
        }
    }
    IEnumerator Ragetime()
    {
        Debug.Log("monke");
        inRage = true;
        yield return new WaitForSeconds(20);
        speed /= 1.5f;
        inRage = false;
        

    }
    IEnumerator Colorchange(SpriteRenderer playerRenderer )
    {
        while (inRage)
        {
            playerRenderer.color = Color.blue;
            yield return new WaitForSeconds(.3f);
            playerRenderer.color = Color.red;
            yield return new WaitForSeconds(.3f);
            playerRenderer.color = Color.green;
            yield return new WaitForSeconds(.3f);
            playerRenderer.color = Color.yellow;
            yield return new WaitForSeconds(.3f);
            playerRenderer.color = Color.magenta;
            yield return new WaitForSeconds(.3f);
        }
           




        
        
        playerRenderer.color = Color.white;
        yield return null;

    }

}
