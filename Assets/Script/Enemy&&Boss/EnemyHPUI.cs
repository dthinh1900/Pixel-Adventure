using UnityEngine;
using UnityEngine.UI;

public class EnemyHPUI : MonoBehaviour
{
    public Image hpFill;
    public Vector3 offset = new Vector3(0, 1.5f, 0);

    Transform target;
    int maxHP;
    int currentHP;

    public void Setup(Transform owner, int hp)
    {
        target = owner;
        maxHP = hp;
        currentHP = hp;
    }

    public void UpdateHP(int hp)
    {
        currentHP = hp;

        hpFill.fillAmount =
            (float)currentHP / maxHP;
    }

    void LateUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position =
            target.position + offset;
    }
}