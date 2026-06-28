using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] Slider healthBar;
    Transform enemyTransform;
    Vector3 offset = new Vector3(0, 0, 0);

    // Update is called once per frame
    void Update()
    {
        if (enemyTransform != null)
        {
            transform.position = enemyTransform.position + offset;

            transform.LookAt(transform.position + Camera.main.transform.forward);
        }
    }

    public void SetUp(GameObject parent)
    {
        enemyTransform = parent.GetComponent<Transform>(); //grab the enemy transform
        offset = new Vector3(0, parent.GetComponentInChildren<Renderer>().bounds.size.y + .25f, 0); //position it a little over the top of the enemy
        healthBar.value = 1;
    }

    public void CalcHealthBar(float curHP, float maxHP)
    {
        healthBar.value = curHP / maxHP;
    }
}
