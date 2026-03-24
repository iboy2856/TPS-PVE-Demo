using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalZombie : ZombieBase
{
    protected override int GetMaxHealth() => 30;
    protected override float GetMoveSpeed() => 3.5f;

    protected override void Start()
    {
        base.Start();//先执行基类Start
        zombieName = "夜魔";
        backstory = "生前本身一个怨气极重的上班族，却死在了阴气最盛的子时，死后在夜晚能力大幅提升...";
    }
   public override void Attack()
    {
        // 触发攻击动画
        if (animator != null)
            animator.SetTrigger("Attack");

        Debug.Log($"{zombieName}玩家受击！造成10点伤害");
        if (player != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10);
            }
        }
    }
    protected override void Die()
    {
        animator.SetTrigger("Dead");
        Destroy(gameObject, 3f);//死亡3秒后后销毁尸体
    }
}
