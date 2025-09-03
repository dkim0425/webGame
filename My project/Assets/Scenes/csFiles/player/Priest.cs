using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerHealingUnit : BaseUnit
{
    public int healAmount = 20;
    public float healCooldown = 1.5f;
    public float healRange = 1.0f;

    private float healTimer = 0f;
    private Animator animator;
    private Transform healTarget;
    private bool isBusy = false;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    protected override void FixedUpdate()
    {
        if (isBusy) return;

        healTimer += Time.deltaTime;
        SearchClosestAlly();

        if (currentTarget != null)
        {
            float distance = Vector2.Distance(transform.position, currentTarget.position);

            if (distance > healRange)
            {
                animator.SetBool("isMoving", true);
                MoveTowardsTarget();
            }
            else
            {
                animator.SetBool("isMoving", false);

                if (healTimer >= healCooldown)
                {
                    var unit = currentTarget.GetComponent<BaseUnit>();
                    if (unit != null && unit.CurrentHP < unit.MaxHP)
                    {
                        healTarget = currentTarget;
                        animator.SetTrigger("Heal"); // 단 한 번만 트리거
                        isBusy = true;
                        healTimer = 0f;
                    }
                }
            }
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    // Priest_healing1 끝에서 호출
    public void PerformHeal()
    {
        if (healTarget != null)
        {
            var unit = healTarget.GetComponent<BaseUnit>();
            if (unit != null && unit.CurrentHP < unit.MaxHP)
            {
                unit.TakeHeal(healAmount);
            }
        }

        healTarget = null;
    }


    // Priest_healing2 끝에서 호출
    public void EndHeal()
    {
        isBusy = false;
    }

    private void SearchClosestAlly()
    {
        GameObject[] allAllies = GameObject.FindGameObjectsWithTag(gameObject.tag);

        List<BaseUnit> wounded = new List<BaseUnit>();
        List<BaseUnit> valid = new List<BaseUnit>();

        foreach (var obj in allAllies)
        {
            if (obj == gameObject) continue;

            var unit = obj.GetComponent<BaseUnit>();
            if (unit != null && !(unit is PlayerHealingUnit) && !(unit is EnemyHealingUnit))
            {
                valid.Add(unit);
                if (unit.CurrentHP < unit.MaxHP)
                    wounded.Add(unit);
            }
        }

        if (wounded.Count > 0)
        {
            currentTarget = wounded.OrderBy(u => u.CurrentHP).First().transform;
        }
        else if (valid.Count > 0)
        {
            currentTarget = valid.OrderBy(u => Vector2.Distance(transform.position, u.transform.position)).First().transform;
        }
        else
        {
            currentTarget = null;
        }
    }
}
