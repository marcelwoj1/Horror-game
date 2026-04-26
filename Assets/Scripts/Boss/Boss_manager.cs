using UnityEngine;
using System.Collections;

public class Boss_manager : MonoBehaviour
{
    [Header("Variables")]
    public int health = 21;
    public int HitsTaken = 0;
    public bool isAggressive = true;
    public bool isDead = false;
    public bool isStunned = false;
    public bool isInvincible = false;

    private SpriteAnimator _animator;
    private Rigidbody2D rb;
    private ShadowGrab shadowGrab;
    private QuestService questService;

    [Header("Components")]
    public GameObject shadow;
    public GameObject Cat;
    private Transform player;
    private BossSlamAttack bossSlamAttack;
    private BossChase bossChase;

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

        if(isStunned == true)
        {
            _animator.Play("Stunned");
        }

        if (bossSlamAttack.GroundPoundAttacking == false)
        {
            if (HitsTaken == 3)
            {
                if(isStunned == true)
                {
                    HitsTaken = 0;
                    return;
                }
                
                int attack = Random.Range(0, 3); // 0, 1, or 2

                switch (attack)
                {
                    case 0:
                        SpawnShadows();
                        break;

                    case 1:
                        bossSlamAttack.StartSlamAttack();
                        break;

                    case 2:
                        shadowGrab.StartLiftAttack();
                        break;
                }
                HitsTaken = 0;
            }
        }
    }

    public void TakeDamage(int damage, Vector2 knockback)
    {
        if (isDead) return;
        if (isInvincible) return;
        
        if(isStunned == false) isAggressive = true;
        else isAggressive = false;

        HitsTaken++;

        health -= damage;

        if (health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(KnockbackRoutine(knockback));

            _animator.Play("Hurt");

            GetComponent<Rigidbody2D>().AddForce(knockback, ForceMode2D.Impulse);
        }

    }

    IEnumerator KnockbackRoutine(Vector2 knockback)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockback, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.35f);
    }

    public void BossBegin()
    {
        isAggressive = true;
        isStunned = false;
        isInvincible = false;
    }
    
    public void BossStunned()
    {
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
        yield return new WaitForSeconds(2f);
        isStunned = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        isAggressive = true;
        bossChase.isChasing = true;
    }

    public void Die()
    {
        Instantiate(Cat, new Vector3(transform.position.x, transform.position.y - 1.6f, transform.position.z), Quaternion.identity);
        questService.SatisfyQuest("FinalBoss");
        Destroy(gameObject);
    }

    private void SpawnShadows()
    {
        Instantiate(shadow, new Vector3(player.position.x, 28.32f, 0), Quaternion.identity);
    }
}