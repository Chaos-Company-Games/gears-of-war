using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance {get; private set;}

    [Header("Stats")]
    public float maxHp = 100f;

    private float currentHP;

//Make some events for flexibility sake
    public UnityEvent onDeath;
    public UnityEvent onHealthChanged; //current, max
    [SerializeField] private RawImage hpBar;
    [SerializeField] private Image turtle;
    [SerializeField] private Sprite[] emotes;

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
        onHealthChanged?.Invoke();
    }

    public void TakeDamage(float amount)
    {
        if (currentHP <= 0f) return;

        currentHP = Mathf.Max(0f, currentHP - amount);
        StartCoroutine(Emote(1));
        onHealthChanged?.Invoke();

        if (currentHP <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHP = Mathf.Min(maxHp, currentHP + amount);
        onHealthChanged?.Invoke();
    }

    void Die()
    {
        onDeath?.Invoke();
        StartCoroutine(Emote(2));
        Debug.Log("You lose!! idiot..");
        //game over screen tbd V
    }

    void UpdateHPBar()
    {
        float newWidth = currentHP/maxHp * 490f; //490 is the base size of the HP bar
        hpBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
    }

    IEnumerator Emote(int id)
    {
        if (id == 0)
        {
            yield return null;
        }
        else if (id == 1) //Take damage
        {
            turtle.sprite = emotes[1];
            yield return new WaitForSeconds(0.5f);
            turtle.sprite = emotes[0];
        }
        else if (id == 2)
        {
            turtle.sprite = emotes[2];
            yield return null;
        }
    }
}
