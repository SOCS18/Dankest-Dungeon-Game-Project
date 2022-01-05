using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    public bool isGrounded = true;
    public destroyPickUps healthCounter;
    public float maxSpeed = 20;
    public float jumpHeight = 20;
    [SerializeField] private float leftRight; // left and right movement
    public Animator animator; // used to play the player animations
    public float attackDamage = 10.0f; // damage the player can deal
    public float attackRate = 1.0f; // how fast the player can attack (how long you have to wait before you can attack again)
    public int Money
    {
        get
        {
            return money;
        }
        set
        {
            money = value;
            UIHandler.Instance.OnMoneyChanged(money);
        }
    }
    [SerializeField] private int money = 0;
    
    bool attackInAir = false;
    float nextAttackTime = 0f; // keep this at 0
    int attackRotation = 1;

    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;

    public int currentJump = 0;
    public int jumpAmount = 1;

    public Transform attackPoint; // the hitbox for the player's attack
    public float attackRange = 0.5f; // the SIZE of the hitbox
    public LayerMask enemyLayers; // used for determining what will happen when a player hits an object that has the layer "Enemies"

    private Camera mainCamera;
    // Current camera position.
    private Vector3 camPos;

    Rigidbody2D rb;

    [SerializeField] private GameController gameController;

    #region Audio
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip healthSound;
    [SerializeField] private AudioClip[] slashSounds;
    [SerializeField] private AudioClip hurtSound;
    public float volume = 0.5f;

    private GameObject[] ignoreBox;

    #endregion

    #region Particles

    [SerializeField] private ParticleHandler particleHandler;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if (GameController.Instance.hasReloaded)
            GameController.Instance.OnReloadEnter(gameObject);

        healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<HealthBar>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(maxHealth);

        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        //healthCounter = GameObject.Find("HealthCounter").GetComponent<HealthCounter>();
        ignoreBox = GameObject.FindGameObjectsWithTag("EnemyBlocker");

        for (int i = 0; i < ignoreBox.Length; i++)
            Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), ignoreBox[i].GetComponent<TilemapCollider2D>(), true); // Ignore enemy blocker 
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseController.isPaused)
        {
            leftRight = Input.GetAxis("Horizontal") * Time.deltaTime * maxSpeed;

            //transform.Translate(leftRight, 0, 0);

            animator.SetFloat("Speed", Mathf.Abs(leftRight)); // used for the running animation

            if (Input.GetAxis("Horizontal") < 0)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.x, 180, transform.rotation.z);
                transform.Translate(-leftRight, 0, 0);
            }
            else if (Input.GetAxis("Horizontal") > 0)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);
                transform.Translate(leftRight, 0, 0);
            }

            // this if statement controls when the player will be allowed to attack. Notice how the player needs to be "GROUNDED" to attack
            if (Time.time >= nextAttackTime)
            {
                if (Input.GetKeyDown(KeyCode.J))
                {
                    Debug.Log("Attack");
                    Attack();
                    nextAttackTime = Time.time + 1f / attackRate; // the math function for determining how long to wait until the next attack
                }
            }

            if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || currentJump < jumpAmount))
            {
                // Play Jumping Sound Here
                PlaySound(jumpSound);
                rb.AddForce(Vector3.up * jumpHeight, ForceMode2D.Impulse);
                currentJump++;
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                TakeDamage(20);
            }

            //Debug.Log("velocity of y is " + rb.velocity.y);
            //Debug.Log("isGrounded is " + isGrounded);
            if(!isGrounded && rb.velocity.y < 0 && !attackInAir)
            {
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsFalling", true);
            }
            else
            {
                animator.SetBool("IsFalling", false);
            }
        }
    }

    internal bool IsDead()
    {
        return currentHealth <= 0;
    }

    // the attack function
    void Attack()
    {
        if (attackRotation == 1)
        {
            PlaySound(slashSounds[0]);
            animator.SetTrigger("Attack"); // trigger the attack animation
        }

        if (attackRotation == 2)
        {
            PlaySound(slashSounds[1]);
            animator.SetTrigger("Attack2"); // trigger the attack animation
        }

        if (attackRotation == 3)
        {
            PlaySound(slashSounds[2]);
            animator.SetTrigger("Attack3"); // trigger the attack animation
        }

// determining the hitbox position, hitbox size, and what layers will be affected by the circle hitbox
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach(Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name);
            Enemy enemyRef;
            enemy.TryGetComponent(out enemyRef);
            if (enemyRef == null)
                continue;
            enemyRef.TakeDamage(attackDamage);
            // For Drawing particles
            enemyRef.DamageLocation(attackPoint.position, enemy);
        }
        
        attackRotation += 1;

        if (attackRotation == 4)
            attackRotation = 1;
    }

    private void PlaySound(AudioClip audioClip)
    {
        audioSource.pitch = UnityEngine.Random.Range(.75f, 1.25f); // Pitch variation
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    // this is used for the game developers, not the players. It is for us to see the hitbox when we click on it in the unity DE
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void TakeDamage(int damage)
    {
        PlaySound(hurtSound);
        //particleHandler.PlayParticle();
        animator.SetTrigger("Hurt"); // trigger the attack animation
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }

    }

    void Die()
    {
        // disable the player script when the enemy dies.
        this.enabled = false;
        particleHandler.PlayParticle();
        particleHandler.PlayParticle();
        particleHandler.PlayParticle();
        animator.SetTrigger("Death"); // trigger the attack animation
    }

    // For Particle systems
    public void DamageLocation(Vector2 attackPos, Collider2D collider)
    {
        Vector2 closestPoint = collider.ClosestPoint(attackPos);

        RaycastHit2D hit;

        hit = Physics2D.Raycast(attackPos, closestPoint);

        particleHandler.SetParticlePosAndRot(transform.InverseTransformPoint(closestPoint), hit.normal);
        particleHandler.PlayParticle();
    }
}