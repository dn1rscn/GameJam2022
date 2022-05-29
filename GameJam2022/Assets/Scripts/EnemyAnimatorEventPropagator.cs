using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyAnimationReceiver
{
    void EnableAttackCollider();
    void DisableAttackCollider();
    void AttackEnd();
}

public class EnemyAnimatorEventPropagator : MonoBehaviour, IEnemyAnimationReceiver
{
    private IEnemyAnimationReceiver recev;

    public void AttackEnd()
    {
        recev.AttackEnd();
    }

    public void DisableAttackCollider()
    {
        recev.DisableAttackCollider();
    }

    public void EnableAttackCollider()
    {
        recev.EnableAttackCollider();
    }

    private void Start()
    {
        recev = transform.parent.gameObject.GetComponentInParent<IEnemyAnimationReceiver>();
    }
}
