using System.Collections;
using NUnit.Framework;
using UnityEditor.ShaderGraph;
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

    private Animator anim;
    float animState = 0; //0 - idle, 1 - move, 2 - attack
    private Material material;

    private Transform target; //Player position, probs never changes

    private EnemyHealthBar healthBar;

    void Start()
    {
        currentHP = maxHp;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        transform.LookAt(target);
        anim = GetComponent<Animator>();
        material = GetComponentInChildren<Renderer>().material;

        healthBar = Instantiate(Resources.Load<EnemyHealthBar>("EnemyHealthBar"));
        healthBar.SetUp(this.gameObject);
    }

    void Update()
    {
        if (isDead || target == null) return;

        float distToTarget = Vector2.Distance(transform.position, target.position);

        if (distToTarget > attackRange)
        {
            //Move towards player
            Vector2 dir = new Vector2((target.position.x - transform.position.x), 0).normalized;
            transform.position += (Vector3)(dir * moveSPeed * Time.deltaTime);
            anim.SetFloat("animState", 1);
        }
        else
        {
            //In range - attack on cooldown
            attackTimer -= Time.deltaTime;
            anim.SetFloat("animState", 0);
            if (attackTimer <= 0f)
            {
                anim.Play("Attack");
                //Attack(); //Trigger attack from animation
                attackTimer = attackCooldown;
            }
        }
    }

    public void Attack()
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
        StartCoroutine(DamageFlash());
        if (currentHP <= 0f)
        {
            isDead = true;
            anim.Play("Die");
            //Die(); //This gets triggered by the animation instead
        }
        else
        {
            //anim.Play("Damage");
            //Play damage animation if stunned
        }
        healthBar.CalcHealthBar(currentHP, maxHp);
    }

    public void Die()
    {
        Debug.Log(gameObject.name + " died!");
        //Tell wavemanager and xpsystem about the tragic news
        WaveManager.instance?.OnEnemyKilled();
        XPSystem.instance?.AddXP(xpReward);
        WaveManager.instance?.spawnedEnemies.Remove(this);

        Destroy(gameObject);
        Destroy(healthBar.gameObject);
    }

    IEnumerator DamageFlash()
    {
        material.SetColor("_Base_Color", Color.red);
        yield return new WaitForSeconds(0.2f);
        material.SetColor("_Base_Color", Color.black);
    }
    
}


