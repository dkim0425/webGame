using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EnemyHealingUnit : BaseUnit
{
    public int healAmount = 20;
    public float healCooldown = 1.5f;
    private float healTimer = 0f;
    public float healRange = 1.0f;

    private bool IsHealer(BaseUnit unit)
    {
        return unit is PlayerHealingUnit || unit is EnemyHealingUnit;
    }

    protected override void FixedUpdate()
    {
        healTimer += Time.deltaTime;

        SearchClosestAlly();

        if (currentTarget != null)
        {
            float dist = Vector2.Distance(transform.position, currentTarget.position);

            if (dist > healRange)
            {
                MoveTowardsTarget();
            }
            else
            {
                if (healTimer >= healCooldown)
                {
                    var unit = currentTarget.GetComponent<BaseUnit>();
                    if (unit != null && unit.CurrentHP < unit.MaxHP)
                    {
                        unit.TakeHeal(healAmount);
                    }
                    healTimer = 0;
                }
            }
        }
    }

    private void SearchClosestAlly()
    {
        GameObject[] allAllies = GameObject.FindGameObjectsWithTag(gameObject.tag);

        List<BaseUnit> woundedAllies = new List<BaseUnit>();
        List<BaseUnit> allAllyUnits = new List<BaseUnit>();

        foreach (var obj in allAllies)
        {
            if (obj == gameObject) continue;

            var unit = obj.GetComponent<BaseUnit>();
            if (unit != null && !IsHealer(unit)) // ❗ 힐러는 제외
            {
                allAllyUnits.Add(unit);
                if (unit.CurrentHP < unit.MaxHP)
                    woundedAllies.Add(unit);
            }
        }

        if (woundedAllies.Count > 0)
        {
            currentTarget = woundedAllies.OrderBy(u => u.CurrentHP).First().transform;
        }
        else if (allAllyUnits.Count > 0)
        {
            currentTarget = allAllyUnits.OrderBy(u => Vector2.Distance(transform.position, u.transform.position)).First().transform;
        }
        else
        {
            currentTarget = null;
        }
    }

}
