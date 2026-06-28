using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance {get; private set;}

    [Header("Stats")]
    public float maxHp = 100f;

    private float currentHP;

//Make some events for flexibility sake
    public UnityEvent onHealthChanged; //current, max
    [SerializeField] private RawImage hpBar;
    [SerializeField] private Image turtle;
    [SerializeField] private Sprite[] emotes;
    [SerializeField] private ParticleSystem gunShot;

    [SerializeField] private GameObject gameOverScreen;

    bool isDead = false;

    void Awake()
    {
        //Singelton stuff
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    void Start()
    {
        currentHP = maxHp;
        onHealthChanged.AddListener(UpdateHPBar);
        onHealthChanged.Invoke();
    }

    public void TakeDamage(float amount)
    {
        if (currentHP <= 0f) return;

        currentHP = Mathf.Max(0f, currentHP - amount);
        StartCoroutine(Emote(1));
        onHealthChanged.Invoke();

        if (currentHP <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHP = Mathf.Min(maxHp, currentHP + amount);
        onHealthChanged.Invoke();
    }

    void Die()
    {
        
        StartCoroutine(Emote(2));
        Debug.Log("You lose!! idiot..");
        //game over screen tbd V
    }

    void UpdateHPBar()
    {
        float newWidth = currentHP/maxHp * 490f; //490 is the base size of the HP bar
        hpBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
    }

    public void DoAbility(Ability a, int teethSize)
    {
        if (isDead) return;
        //Pick a target
        Enemy target = WaveManager.instance.spawnedEnemies[0];
        if (target == null) return;
        for (int i = 1; i < WaveManager.instance.spawnedEnemies.Count; i++)
        {
            if (Vector3.Distance(transform.position, target.transform.position) > Vector3.Distance(transform.position, WaveManager.instance.spawnedEnemies[i].transform.position))
            {
                target = WaveManager.instance.spawnedEnemies[i];
            }
        }

        //Do the ability
        if (target == null) return;
        Debug.Log(target.gameObject.name);

        //calc amount of damage
        if (Vector3.Distance(transform.position, target.transform.position) <= 45f)
        {
            if (a.ability == AbilityName.Blap)
            {
                int baseDamage = 5;
                float finalDamage = baseDamage * (1f + (0.1f * (int)a.rarity)) + teethSize/10f;
                target.TakeDamage(finalDamage);
            }
            else if (a.ability == AbilityName.Smash)
            {
                //AOE in front of us
                List<Enemy> smashTargets = new List<Enemy>();
                for (int i = 0; i < WaveManager.instance.spawnedEnemies.Count; i++)
                {
                    //Check all enemies within 10 units of us
                    if (Vector3.Distance(transform.position, WaveManager.instance.spawnedEnemies[i].transform.position) < 10f)
                    {
                        smashTargets.Add(WaveManager.instance.spawnedEnemies[i]);
                    }
                }

                for (int i = 0; i < smashTargets.Count; i++)
                {   int baseDamage = 5;
                    float finalDamage = baseDamage * (0.6f* (1f + (0.1f * (int)a.rarity)) + teethSize/10f);
                    smashTargets[i].TakeDamage(finalDamage);
                }

            }
            else if (a.ability == AbilityName.Stagger)
            {
                //Staggers enemy for a sec
                target.Stagger(teethSize/10f);
            }
            else if (a.ability == AbilityName.Skewer)
            {
                //TBD
                int baseDamage = 5;
                float finalDamage = baseDamage * ( 0.5f* (1f + (0.1f * (int)a.rarity)) + teethSize/10f);
                target.TakeDamage(finalDamage);
                Heal(finalDamage * .25f);
            }
            else if (a.ability == AbilityName.Sling)
            {
                //Ranged aoe
                //Hits all enemies within 5 units of the target
                List<Enemy> slingTargets = new List<Enemy>();
                for (int i = 0; i < WaveManager.instance.spawnedEnemies.Count; i++)
                {
                    if (Vector3.Distance(transform.position, target.transform.position) < 5f)
                    {
                        slingTargets.Add(WaveManager.instance.spawnedEnemies[i]);
                    }
                }

                for (int i = 0; i < slingTargets.Count; i++)
                {   int baseDamage = 5;
                    float finalDamage = baseDamage * ( 0.4f* (1f + (0.1f * (int)a.rarity)) + teethSize/10f);
                    slingTargets[i].TakeDamage(finalDamage);
                }
            }
            else
            {
                //target.TakeDamage(10);
            }
            gunShot.Play();
            
        }
        
        Debug.Log(a.ability);
    }

    IEnumerator Emote(int id)
    {
        if (id == 0)
        {
            yield return null;
        }
        else if (id == 1 && !isDead) //Take damage
        {
            turtle.sprite = emotes[1];
            yield return new WaitForSeconds(0.5f);
            turtle.sprite = emotes[0];
        }
        else if (id == 2)
        {
            turtle.sprite = emotes[2];
            yield return new WaitForSeconds(1f);
            gameOverScreen.SetActive(true);
        }
    }

    public void NewGame()
    {
        SceneManager.LoadScene(0);
    }
}
