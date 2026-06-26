using NUnit.Framework;
using UnityEngine;

//Class for managing combat opponents in game.
public class Enemy: MonoBehaviour
{
    [Header("Stats")]
    public float maxHp = 30f;
    public float moveSPeed = 1.5f;
    public float attackRange = 1.2f;
    public float attackDamage = 5f;
    public float attackCooldown = 1.5f;
    public int xpReward = 10;

    private float currentHP;
    private float attackTimer = 0f;
    private bool isDead = false;

    private Transform target; //Player position, probs never changes

    void Start()
    {
        currentHP = maxHp;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        transform.LookAt(target);
    }

    void Update()
    {
        if (isDead || target == null) return;

        float distToTarget = Vector2.Distance(transform.position, target.position);

        if (distToTarget > attackRange)
        {
            //Move towards player
            Vector2 dir = (target.position - transform.position).normalized;
            transform.position += (Vector3)(dir * moveSPeed * Time.deltaTime);
        }
        else
        {
            //In range - attack on cooldown
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                Attack();
                attackTimer = attackCooldown;
            }
        }
    }

    void Attack()
    {
        PlayerHealth player = target.GetComponent<PlayerHealth>();
        if (player != null)
        {
            //Later we can add some sort of feedback, UI or red flash
            player.TakeDamage(attackDamage);
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHP -= amount;
        if (currentHP <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        //Tell wavemanager and xpsystem about the tragic news
        WaveManager.instance?.OnEnemyKilled();
        XPSystem.instance?.AddXP(xpReward);

        Destroy(gameObject);
    }
}


