using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{

    [Header("Stats")]
    public float maxHp = 100f;

    private float currentHP;

//Make some events for flexibility sake
    public UnityEvent onDeath;
    public UnityEvent<float, float> onHealthChanged; //current, max
    void Start()
    {
        currentHP = maxHp;
        onHealthChanged?.Invoke(currentHP, maxHp);
    }

    public void TakeDamage(float amount)
    {
        if (currentHP <= 0f) return;

        currentHP = Mathf.Max(0f, currentHP - amount);
        onHealthChanged?.Invoke(currentHP, maxHp);

        if (currentHP <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHP = Mathf.Min(maxHp, currentHP + amount);
        onHealthChanged?.Invoke(currentHP, maxHp);
    }

    void Die()
    {
        onDeath?.Invoke();
        Debug.Log("You lose!! idiot..");
        //game over screen tbd V
    }
}
