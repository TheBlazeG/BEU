using System.Collections;
using System.Collections.Generic;
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
    public GameObject projectile;
    private Animator animator;
    public PlayerState currentState;
    [SerializeField] public Vector2 direction;
    [SerializeField] BarraDeVida barra;
    [SerializeField] Slider barraCombo;
    public int numberOfAttacks;
    float lastAttackedTime=0;
    float lastDodgeTime=0;
    float maxComboDelay = .6f;
    private float nextFireTime = 0;
    bool canDash=true;
    public bool inRage=true;
    bool isInChange=false;
    ComboCount comboCount;
    
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

        if (inRage)
        {
            if (!isInChange)
            {
                StartCoroutine(Colorchange(playerRenderer, isInChange));
            }
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
        else if(currentState != PlayerState.stagger)
        {
            currentState = PlayerState.walk;
        }


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


        if (Input.GetButtonDown("attack") && currentState != PlayerState.attack)
        {
           StartCoroutine(Colorchange(playerRenderer,isInChange));
        
        }

        if (Input.GetButtonDown("attack2")  && currentState != PlayerState.stagger)//attaaaaaaaack
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
        if (barraCombo.value >= 10)
        {
            barraCombo.value -= 10;
            animator.SetBool("Super", true);
        }
    }
    public void Rage()
    {
        if (barraCombo.value == 20)
        {
            barraCombo.value = 0;
            animator.SetBool("Super", true);
            StartCoroutine(Ragetime());
            
        }
    }
    IEnumerator Ragetime()
    {
        player.inRage = true;
        new WaitForSeconds(20);
        player.inRage = false;
        yield return null;

    }
    IEnumerator Colorchange(SpriteRenderer playerRenderer, bool changing)
    {
        changing = true;
            playerRenderer.color = Color.blue;
            yield return new WaitForSeconds(.1f);
            playerRenderer.color = Color.red;
            yield return new WaitForSeconds(.1f);
            playerRenderer.color = Color.green;
        yield return new WaitForSeconds(.1f);
            playerRenderer.color = Color.yellow;
        yield return new WaitForSeconds(.1f);
            playerRenderer.color = Color.magenta;
        yield return new WaitForSeconds(.1f);




        
        playerRenderer.color = Color.white;
        changing = false;
        yield return null;

    }

}
