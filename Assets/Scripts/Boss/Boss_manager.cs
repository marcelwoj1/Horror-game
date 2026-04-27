using UnityEngine;
using System.Collections;

public class Boss_manager : MonoBehaviour
{
    [Header("Boss Stats")]
    public int health = 21;
    private int HitsTaken;
    public bool isAggressive = true;
    public bool isDead = false;
    public bool isStunned = false;
    public bool isInvincible = false;

    [Header("Components")]
    private SpriteAnimator _animator;
    private Rigidbody2D rb;
    private QuestService questService;

    [Header("Prefabs")]
    public GameObject shadow;
    public GameObject Cat;

    [Header("References")]
    private Transform player;
    private BossSlamAttack bossSlamAttack;
    private BossChase bossChase;
    private ShadowGrab shadowGrab;

    void Start()
    {
        _animator = GetComponent<SpriteAnimator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        bossSlamAttack = GetComponent<BossSlamAttack>();
        bossChase = GetComponent<BossChase>();
        shadowGrab = GetComponent<ShadowGrab>();
        questService = FindAnyObjectByType<QuestService>();
    }

    void Update()
    {
        //Will always go back to stunned look if true
        if(isStunned == true)
        {
            _animator.Play("Stunned");
        }

        if (bossSlamAttack.GroundPoundAttacking == false)
        {   
            //Perform special attack after player hits it 3 times
            if (HitsTaken == 3)
            {   //Wont start special attack if stunned
                if(isStunned == true)
                {
                    HitsTaken = 0;
                    return;
                }
                
                int attack = Random.Range(0, 3);

                //Random generator for what special attack to perform
                switch (attack)
                {
                    case 0:
                        //Shadow spike attack
                        SpawnShadows();
                        break;

                    case 1:
                        //Ground pound attack
                        bossSlamAttack.StartSlamAttack();
                        break;

                    case 2:
                        //Shadow grab attack
                        shadowGrab.StartLiftAttack();
                        break;
                }
                HitsTaken = 0;
            }
        }
    }

    //Take damage
    public void TakeDamage(int damage, Vector2 knockback)
    {
        //Cant take damage if dead or invincible
        if (isDead) return;
        if (isInvincible) return;
        
        if(isStunned == false) isAggressive = true;
        else isAggressive = false;//Wont become agressive if it is stunned

        HitsTaken++;
        health -= damage;

        //Die if health is 0 or less
        if (health <= 0)
        {
            Die();
        }
        else
        {
            //Perform knockback
            StartCoroutine(KnockbackRoutine(knockback));
            //Play hurt animation
            _animator.Play("Hurt");
            //Apply knockback force
            GetComponent<Rigidbody2D>().AddForce(knockback, ForceMode2D.Impulse);
        }

    }

    //Applies knockback force
    IEnumerator KnockbackRoutine(Vector2 knockback)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockback, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.35f);
    }

    public void BossBegin()
    {
        //For boss to start attacking player
        isAggressive = true;
        isStunned = false;
        isInvincible = false;
    }
    
    public void BossStunned()
    {
        //Boss cant attack or chase player while stunned (2 seconds)
        isAggressive = false;
        isStunned = true;
        bossChase.isChasing = false;
        isInvincible = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        
        StartCoroutine(StunnedRoutine());
        _animator.Play("Stunned");
    }
    IEnumerator StunnedRoutine()
    {
        //Return to normal after 2 seconds
        yield return new WaitForSeconds(2f);
        isStunned = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        isAggressive = true;
        bossChase.isChasing = true;
    }

    public void Die()
    {
        //Spawn Cat for ending cutscene
        Instantiate(Cat, new Vector3(transform.position.x, transform.position.y - 1.6f, transform.position.z), Quaternion.identity);
        //Satisfy quest
        questService.SatisfyQuest("FinalBoss");
        //Destroy boss
        Destroy(gameObject);
    }

    //Shadow spike attack
    private void SpawnShadows()
    {
        Instantiate(shadow, new Vector3(player.position.x, 28.32f, 0), Quaternion.identity);
    }
}