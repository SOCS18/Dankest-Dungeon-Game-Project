using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENEMY_TYPE {KNIGHT, ARCHER, DEMON, SKULL, SKULLBOSS, HELLBEAST};
public class Enemy : MonoBehaviour
{
    // Stats
    [Header("Stats")]
    public ENEMY_TYPE enemytype = ENEMY_TYPE.KNIGHT;
    public float maxHealth = 100.0f;
    float currentHealth;
    //private bool jumped = false;
    [SerializeField] private int damage = 10;
    public float AttackRange = 0.5f;
    [SerializeField] float MoveSpeed = 10;
    [SerializeField] public float attackRate = 0.7f;
    [SerializeField] private int JumpForce;
    float nextAttackTime = 0f;

    [Header("Drop Table")]
    [SerializeField] private int dropAmount;
    [SerializeField] private List<Drop> drops;

    [Header("Behaviour")]
    [SerializeField] private bool IsGrounded = false;
    [SerializeField] float Stoppingdistance;
    [SerializeField] float attractdistance;
    public LayerMask PlayerLayer;
    [SerializeField] bool isIgnoringAboveAndBelow = false; //Enemy doesn't chase the Player if the Player is too high up or down.
    [SerializeField] float verticalLimit = 2f; // If we are ignoring, how far before we ignore?
    [SerializeField] float dieTime = 3f; // Time to die, mostly for skull;

    [Header("Patrolling")]
    bool isPatrolling = false;
    [SerializeField] float patrolTimer = 5f; // How long before we patrol if the enemy is doing nothing.
    float currentPatrolTimer;
    [SerializeField] float patrolMoveTimer = 2f;
    float currentPatrolMoveTimer;
    float patrolDirection = 1;
    [SerializeField] private List<Transform> patrolPoints;
    [SerializeField] private Transform selectedPatrolPoint;
    [SerializeField] private Transform lastSelectedPatrolPoint;
    [SerializeField] int patrolPointsReached = 0;

    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] public GameObject arrow;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private PlayerController player;
    public ParticleHandler particleHandler;
    public Animator animator;
    [SerializeField] private PlayerController pc;
    [SerializeField] private Hitbox hitbox;
    Rigidbody2D rb2d;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] attackSounds;
    [SerializeField] private AudioClip[] hurtSounds;
    [SerializeField] private AudioClip[] idleSounds;
    [SerializeField] private AudioClip bowArrowSound;

    [Header("Misc")]
    public float xscale;
    public float yscale;
    private float _angle;
    public Vector3 center;
    public Transform master;
    bool chasing = false;
    public bool isMinion;
    public EnemySpawner spawner;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>());
        
        if (enemytype == ENEMY_TYPE.DEMON || enemytype == ENEMY_TYPE.SKULL || enemytype == ENEMY_TYPE.SKULLBOSS) {
            GetComponent<Collider2D>().isTrigger = true;
            rb2d.gravityScale = 0;
            currentPatrolMoveTimer = patrolMoveTimer;
            patrolPoints = new List<Transform>();
            foreach(GameObject _gameObject in GameObject.FindGameObjectsWithTag("PatrolPoint"))
            {
                patrolPoints.Add(_gameObject.transform);
            }
        }

        if(enemytype == ENEMY_TYPE.HELLBEAST)
        {
            patrolPoints = new List<Transform>();
            foreach (GameObject _gameObject in GameObject.FindGameObjectsWithTag("PatrolPoint"))
            {
                patrolPoints.Add(_gameObject.transform);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // For some reason particles wont render unless z = -1
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        xscale = transform.localScale.x;
        yscale = transform.localScale.y;

        currentHealth = maxHealth;

        currentPatrolTimer = patrolTimer + Random.Range(-2f, 2f);
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        player = playerTransform.gameObject.GetComponent<PlayerController>();
        if(!isMinion)
            center = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0) // Just in case
            return;

        if (player.IsDead())
        {
            Patrol();
            return;
        }

        switch (enemytype)
        {
            case ENEMY_TYPE.DEMON:
                DemonBehaviour();
                return;

            case ENEMY_TYPE.SKULL:
                SkullBehaviour();
                return;

            case ENEMY_TYPE.SKULLBOSS:
                SkullBossBehaviour();
                return;

            case ENEMY_TYPE.HELLBEAST:
                HellBeastBehaviour();
                return;
        }

        float Distancetoplayer = (transform.position. x - playerTransform.position.x);
        if(Mathf.Abs(Distancetoplayer) >= Stoppingdistance &&
            (!isIgnoringAboveAndBelow || !(playerTransform.position.y > transform.position.y + verticalLimit || playerTransform.position.y < transform.position.y - verticalLimit)))
        { // If the player is too high up or below and we are ignoring above and below, don't chase
            ChasePlayer();
            currentPatrolTimer = patrolTimer;
        }
        else{
            if(enemytype == ENEMY_TYPE.KNIGHT){
                StopChasingPlayer();
            }
            else{
                ShootBowAtPlayer();
            }
        }

        // Enemy has done nothing long enough. Time to Patrol.
        if (currentPatrolTimer <= 0)
            Patrol();
    }

    public void ChasePlayer(){
       //If player is to the left
       animator.SetBool("Isrunning", true);
        if(transform.position.x < playerTransform.position.x){
            rb2d.velocity = new Vector2(MoveSpeed, 0);
            /*Vector2 vecloc = rb2d.velocity; //Also commented out code for the jump in case I feel like coming back to it
            vecloc.x = MoveSpeed;*/
            transform.localScale = new Vector2(-xscale, transform.localScale.y);
        }
        //If player is to the right
        else if(transform.position.x > playerTransform.position.x){
            rb2d.velocity = new Vector2(-MoveSpeed, 0);
            /*Vector2 vecloc = rb2d.velocity;
            vecloc.x = -MoveSpeed;*/
            transform.localScale = new Vector2(xscale, transform.localScale.y);
        }

        if (IsTouchingEnemyBlocker()) // If enemy can't reach player.
        {
            animator.SetBool("Isrunning", false);
            animator.SetBool("Attacking", true);
        }
        else
        {
            animator.SetBool("Attacking", false);
        }

        //Commented out jump function in case I feel like coming back to it lol
        /*//If player is above the enemy, jump
        if(rb2d.velocity.y != 0 && !IsGrounded){
            animator.SetTrigger("Jump");
            animator.SetBool("Isrunning", false);
        }
        else{
            animator.SetBool("Isrunning", true);
        }
        if(transform.position.y < (player.position.y - .8) && IsGrounded){
            rb2d.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        }*/
    }

    //Stop chasing player and start attacking
    public void StopChasingPlayer(){
        Vector2 vecloc = rb2d.velocity;
        vecloc.x = 0;
        animator.SetBool("Isrunning", false);
        if (Time.time > nextAttackTime && IsGrounded == true)
        {
            if (hitbox.IsTouchingPlayer()) // Check Hitbox range
            {
                animator.SetBool("Attacking", true);
                StartCoroutine("Attack");
                animator.SetTrigger("Attack");
                PlaySound(attackSounds[Random.Range(0, attackSounds.Length)]);
                //Debug.Log("Player hit");
                nextAttackTime = Time.time + 1f / attackRate;
                currentPatrolTimer = patrolTimer;
            }
            else // The enemy is doing nothing here.
            {
                currentPatrolTimer -= Time.deltaTime;
            }
        }
    }

    public void ShootBowAtPlayer(){
        Vector2 vecloc = rb2d.velocity;
        vecloc.x = 0;
        animator.SetBool("Isrunning", false);
        if(Time.time > nextAttackTime && IsGrounded == true){
            //If player is to the right
            if(transform.position.x > playerTransform.position.x){
            transform.localScale = new Vector2(xscale * 1, yscale);
            }
            //If player is to the left
            if(transform.position.x < playerTransform.position.x){
            transform.localScale = new Vector2(xscale * -1, yscale);
            }

            //StartCoroutine("Shoot");
            animator.SetTrigger("Attack");
            StartCoroutine("Shoot");
            /*GameObject fire = Instantiate(arrow) as GameObject;
            arrow.GetComponent<ArrowTravel>().damage = 10;
            arrow.GetComponent<ArrowTravel>().target = playerTransform.transform;
            Vector2 arrowpos = transform.position * 1;
            arrowpos.y += .25f;
            fire.transform.position = arrowpos;
            fire.transform.rotation = transform.rotation;
            Rigidbody2D arrowrb = fire.GetComponent<Rigidbody2D>();
            arrowrb.AddForce((arrowpos - (Vector2)playerTransform.transform.position) * -35);
            Destroy(fire, 5f);*/
            nextAttackTime = Time.time + 1f/attackRate;
        }
        else
        {
            animator.ResetTrigger("Attack");
        }
    }

    // For Demon Enemy
    public void DemonBehaviour()
    {
        // Face Player
        //If player is to the left
        if (transform.position.x < playerTransform.position.x)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, 180, transform.rotation.z);
        }
        //If player is to the right
        else if (transform.position.x > playerTransform.position.x)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);
        }

        if (patrolPointsReached <= 5) // Go to a random patrol point 6 times
        {
            while (selectedPatrolPoint == null)
            {
                selectedPatrolPoint = patrolPoints[Random.Range(0, patrolPoints.Count)];
                if (selectedPatrolPoint == lastSelectedPatrolPoint)
                    selectedPatrolPoint = null;
            }
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(selectedPatrolPoint.position.x, selectedPatrolPoint.position.y, -1), MoveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, selectedPatrolPoint.position) < 1f)
            {
                patrolPointsReached++;
                lastSelectedPatrolPoint = selectedPatrolPoint;
                selectedPatrolPoint = null;

                if (patrolPointsReached == 6) // At the 6th patrol point, play a sound to warn the player.
                {
                    PlaySound(idleSounds[Random.Range(0, idleSounds.Length)]);
                    currentPatrolTimer = patrolTimer;
                }
            }
        }
        else // After the sixth Chase the Player for a duration equal to the patrol timer.
        {
            Vector3 target = new Vector3(playerTransform.position.x + transform.localScale.x, playerTransform.position.y + -.5f + transform.localScale.y, -1);
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.x, target.y, -1), (MoveSpeed * .5f) * Time.deltaTime);
            if (Vector2.Distance(transform.position, target) < 2f)
            {
                currentPatrolTimer -= Time.deltaTime;
                if (currentPatrolTimer <= 0)
                {
                    selectedPatrolPoint = null;
                    patrolPointsReached = 0;
                }
            }
        }

        if (Time.time > nextAttackTime) // Attack the player if we are in range.
        {
            if (hitbox.IsTouchingPlayer()) // Check Hitbox range
            {
                StartCoroutine("Attack");
                animator.SetTrigger("Attack");
                PlaySound(attackSounds[Random.Range(0, attackSounds.Length)]);
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    public void SkullBehaviour()
    {
        // Face Player
        //If player is to the left
        if (transform.position.x < playerTransform.position.x)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, 180, transform.rotation.z);
        }
        //If player is to the right
        else if (transform.position.x > playerTransform.position.x)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);
        }

        Vector3 target = new Vector3(playerTransform.position.x + transform.localScale.x, playerTransform.position.y + transform.localScale.y, -1);
        if (Vector2.Distance(transform.position, target) < attractdistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.x, target.y, -1), (MoveSpeed * .75f) * Time.deltaTime);
            chasing = true;
        }
        else
        {
            if(chasing == true)
            {
                chasing = false;
                center = transform.position;
            }

            _angle += MoveSpeed * Time.deltaTime;

            var offset = new Vector3(Mathf.Sin(_angle), Mathf.Cos(_angle), -1) * .5f;
            transform.position = center + offset;
        }

        if (Vector2.Distance(transform.position, target) < 1.5f) // Attack the player if we are in range.
        {
            if (!isPatrolling) // Que Detonation
            {
                isPatrolling = true;
                currentPatrolTimer = patrolTimer;
                animator.SetBool("Attacking", true);
                particleHandler.Emit(true);
            }
            else
            {
                if (currentPatrolTimer > 0)
                {
                    currentPatrolTimer -= Time.deltaTime;
                }
                else // Detonate
                {
                    PlaySound(attackSounds[Random.Range(0, attackSounds.Length)]);
                    particleHandler.Explode();
                    player.TakeDamage(damage);
                    Die();
                }
            }

        }
        else
        {
            isPatrolling = false;
            animator.SetBool("Attacking", false);
            particleHandler.Emit(false);
        }
    }

    public void SkullBossBehaviour()
    {
        // Face Player
        //If player is to the left
        if (transform.position.x < playerTransform.position.x)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, 180, transform.rotation.z);
        }
        //If player is to the right
        else if (transform.position.x > playerTransform.position.x)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);
        }

        if (isMinion)
        {
            Vector3 target = new Vector3(playerTransform.position.x + transform.localScale.x, playerTransform.position.y + transform.localScale.y, -1);
            if (Vector2.Distance(transform.position, target) < attractdistance && currentPatrolMoveTimer <= 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.x, target.y, -1), (MoveSpeed * .75f) * Time.deltaTime);
                chasing = true;
            }
            else
            {
                _angle += MoveSpeed * Time.deltaTime;

                var offset = new Vector3(Mathf.Sin(_angle), Mathf.Cos(_angle), -1) * 2f;
                transform.position = master.position + offset;

                currentPatrolMoveTimer -= Time.deltaTime;
            }

            if (Vector2.Distance(transform.position, target) < 1.5f) // Attack the player if we are in range.
            {
                if (!isPatrolling) // Que Detonation
                {
                    isPatrolling = true;
                    currentPatrolTimer = patrolTimer;
                    animator.SetBool("Attacking", true);
                    particleHandler.Emit(true);
                }
                else
                {
                    if (currentPatrolTimer > 0)
                    {
                        currentPatrolTimer -= Time.deltaTime;
                    }
                    else // Detonate
                    {
                        PlaySound(attackSounds[Random.Range(0, attackSounds.Length)]);
                        particleHandler.Explode();
                        player.TakeDamage(damage);
                        Die();
                    }
                }

            }
            else
            {
                isPatrolling = false;
                animator.SetBool("Attacking", false);
                particleHandler.Emit(false);
            }
        }
        else
        {
            animator.SetBool("Attacking", true);
            particleHandler.Emit(true);
            while (selectedPatrolPoint == null)
            {
                selectedPatrolPoint = patrolPoints[Random.Range(0, patrolPoints.Count)];
                if (selectedPatrolPoint == lastSelectedPatrolPoint && lastSelectedPatrolPoint != null)
                    selectedPatrolPoint = null;
            }
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(selectedPatrolPoint.position.x, selectedPatrolPoint.position.y, -1), MoveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, selectedPatrolPoint.position) < 1f)
            {
                lastSelectedPatrolPoint = selectedPatrolPoint;
                selectedPatrolPoint = null;
            }

            if (Time.time > nextAttackTime) // Spawn Enemy every attack Time
            {
                nextAttackTime = Time.time + 1f / attackRate;
                PlaySound(idleSounds[Random.Range(0, idleSounds.Length)]);
                GameObject minion = Instantiate(gameObject);
                minion.transform.position = transform.position;
                Enemy minionE = minion.GetComponent<Enemy>();
                minionE.isMinion = true;
                minionE.maxHealth = Mathf.Floor(maxHealth * .1f);
                minionE.currentHealth = maxHealth;
                minionE.master = transform;
                minion.transform.localScale = new Vector2(1, 1);
                minionE.xscale = 1;
                minionE.yscale = 1;
                minionE.dropAmount = 0;
                minionE.MoveSpeed = 6;
            }
        }
    }

    public void HellBeastBehaviour()
    {
        // Face Player
        //If player is to the left
        if (transform.position.x < playerTransform.position.x)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, 180, transform.rotation.z);
        }
        //If player is to the right
        else if (transform.position.x > playerTransform.position.x)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);
        }

        if (Time.time > nextAttackTime && IsGrounded == true && currentPatrolTimer > 0)
        {
            //StartCoroutine("Shoot");
            animator.SetTrigger("Attack");
            StartCoroutine("Shoot");
            nextAttackTime = Time.time + 1f / attackRate;
            currentPatrolTimer += .7f;
        }
        else
        {
            if(currentPatrolTimer <= 0)
            {
                particleHandler.Explode();
                animator.SetTrigger("Teleport");
                if(currentPatrolMoveTimer <= 0)
                {
                    while (selectedPatrolPoint == null)
                    {
                        selectedPatrolPoint = patrolPoints[Random.Range(0, patrolPoints.Count)];
                        if (selectedPatrolPoint == lastSelectedPatrolPoint)
                            selectedPatrolPoint = null;
                    }

                    transform.position = selectedPatrolPoint.position;
                    lastSelectedPatrolPoint = selectedPatrolPoint;
                    selectedPatrolPoint = null;
                    animator.ResetTrigger("Teleport");
                    particleHandler.Emit(false);
                    currentPatrolTimer = patrolTimer;
                }
                else
                {
                    currentPatrolMoveTimer -= Time.deltaTime;
                }
                
            }
            else
            {
                currentPatrolMoveTimer = patrolMoveTimer;
                currentPatrolTimer -= Time.deltaTime;
            }
        }
    }

    public void Patrol()
    {
        animator.SetBool("Attacking", false);
        animator.SetBool("Isrunning", true);

        if (rb2d.velocity.x == 0)
            patrolDirection = -patrolDirection;

        if (!isPatrolling)
        {
            isPatrolling = true;

            if (Random.Range(0f, 1f) > .5f)
                patrolDirection = 1;
            else
                patrolDirection = -1;

            currentPatrolMoveTimer = patrolMoveTimer;
        }

        if (currentPatrolMoveTimer > 0)
        {
            currentPatrolMoveTimer -= Time.deltaTime;
        }
        else
        {
            isPatrolling = false;
            currentPatrolTimer = patrolTimer + Random.Range(-2f, 2f); ;
            return;
        }

        rb2d.velocity = new Vector2(MoveSpeed * patrolDirection, 0);
        transform.localScale = new Vector2(-patrolDirection * xscale, yscale);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        nextAttackTime += .55f;
        animator.SetTrigger("Hurt");

        PlaySound(hurtSounds[Random.Range(0, hurtSounds.Length)]);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //Determine if Enemy is grounded
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            IsGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision){
        if(collision.gameObject.tag == "Platform")
        {
            IsGrounded = false;
        }
    }

    public void DamageLocation(Vector2 attackPos, Collider2D collider)
    {
        //Vector2 rot = new Vector2();
        Vector2 closestPoint = collider.ClosestPoint(attackPos);

        RaycastHit2D hit;

        hit = Physics2D.Raycast(attackPos, closestPoint);

        particleHandler.SetParticlePosAndRot(transform.InverseTransformPoint(closestPoint), hit.normal);
        particleHandler.PlayParticle();
    }

    void Die()
    {
        animator.SetBool("IsDead", true);

        // this will make it to where, when the enemy dies, it's layer will change to default so the player can't
        // attack the body anymore...
        gameObject.layer = LayerMask.NameToLayer("Default");

        // this will make it to where, when the enemy dies, the player won't collide with the body.

        // disable the enemy script when the enemy dies.
        DropLoot();
        spawner.currentSpawns--;
        this.enabled = false;
        Destroy(gameObject, dieTime);
    }

    public bool IsTouchingEnemyBlocker()
    {
        Collider2D collider = GetComponent<BoxCollider2D>();
        return collider.IsTouching(GameObject.FindGameObjectWithTag("EnemyBlocker").GetComponent<UnityEngine.Tilemaps.TilemapCollider2D>());
    }

    IEnumerator Attack(){
        float counter = 0.5f;
        while(counter > 0){
            yield return new WaitForSeconds(.3f);
            hitbox.TryToHitPlayer(damage);
            yield return new WaitForSeconds(.5f);
            counter = 0;
        }
    }

    IEnumerator Shoot(){
        float counter = .5f;
        while(counter > 0){
            yield return new WaitForSeconds(.3f);
            //Attack
            GameObject fire = Instantiate(arrow);
            fire.transform.rotation = new Quaternion(fire.transform.rotation.x * (transform.localScale.x < 0 ? 180 : 0), fire.transform.rotation.y, fire.transform.rotation.z, fire.transform.rotation.w);
            PlaySound(bowArrowSound);
            arrow.GetComponent<ArrowTravel>().damage = 10;
            arrow.GetComponent<ArrowTravel>().target = playerTransform.transform;
            fire.transform.position = firePoint.position;
            fire.transform.rotation = transform.rotation;
            Rigidbody2D arrowrb = fire.GetComponent<Rigidbody2D>();
            arrowrb.AddForce(((Vector2)firePoint.position - (Vector2)playerTransform.transform.position) * -35);
            Destroy(fire, 5f);
            yield return new WaitForSeconds(.5f);
            counter = 0;

        }
    }

    public void DropLoot()
    {
        for (int d = 0; d < dropAmount; d++)
        {
            if (drops.Count == 0)
                return;
            float totalWeight = 0;

            foreach (Drop drop in drops)
            {
                totalWeight += drop.dropChance;
            }

            float r = Random.Range(0, totalWeight);

            float weight = 0;
            Drop chosenDrop = null;
            foreach (Drop drop in drops)
            {
                weight += drop.dropChance;
                if (r <= weight)
                {
                    chosenDrop = drop;
                    break;
                }
            }

            if (chosenDrop == null)
                return;

            for (int i = 0; i < chosenDrop.amount; i++)
            {
                GameObject go = Instantiate(chosenDrop.item);
                go.transform.position = new Vector2(transform.position.x, transform.position.y + 1);
                go.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-5f, 5f), 5f), ForceMode2D.Impulse);
            }
        }
    }

    private void PlaySound(AudioClip audioClip)
    {
        audioSource.pitch = Random.Range(.75f, 1.25f); // Pitch variation
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
