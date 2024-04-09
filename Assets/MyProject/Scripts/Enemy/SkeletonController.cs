using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonController : EnemyController
{
    public GameObject shield;
    public WeaponAttackController weapon;

    public RandomAudioPlayer attackPlayer;
    public RandomAudioPlayer hurtPlayer;
    public RandomAudioPlayer deathPlayer;

    public override void Attack()
    {
        ChangeDirection();
        base.Attack();
        animator.SetTrigger("attack");
    }

    //修正攻击方向
    public void ChangeDirection()
    {
        if (target != null) 
        {
            Vector3 direction = target.transform.position - transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void AttackStart()
    {
        weapon.BeginAttack();
        attackPlayer.PlayRandomAudio();
    }

    public void AttackEnd()
    {
        weapon.EndAttack();
    }

    public void OnHurt(DamageAble dmageAble, DamageMessage data)
    {
        Vector3 direction = data.damagePosition - transform.position;
        direction.y = 0;
        Vector3 localDirection = transform.InverseTransformDirection(direction);
        animator.SetFloat("hurtX", localDirection.x);
        animator.SetFloat("hurtY", localDirection.z);
        animator.SetTrigger("hurt");
        transform.LookAt(data.damagePosition);
        hurtPlayer.PlayRandomAudio();
    }

    public override void OnDeath(DamageAble damageAble, DamageMessage data)
    {
        base.OnDeath(damageAble, data);
        deathPlayer.PlayRandomAudio();
        meshAgent.isStopped = true;
        animator.SetTrigger("death");
        Destroy(gameObject,2);
    }
}
