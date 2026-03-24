using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ZombieBase : MonoBehaviour
{
    [Header("攻击设置")]
    public float attackRange = 1.5f;      // 攻击范围
    public float attackCooldown = 2f;     // 攻击冷却
    private float nextAttackTime = 0f;
    [Header("基础属性")]
    public string zombieName;
    [TextArea] public string backstory;

    protected int currentHealth;
    protected Transform player;
    protected Animator animator;

    //Start里初始化（子类可以override）
    protected virtual void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        animator = GetComponent<Animator>();
        currentHealth = GetMaxHealth();//抽象方法，子类决定具体值
    }
    protected abstract int GetMaxHealth();//子类必须实现

    //每帧更新 寻路逻辑共用
    protected virtual void Update()
    {
        if (currentHealth <= 0) return;
        if (player == null) return;
        // 检查是否在播放攻击动画
        bool isAttacking = animator != null &&
                           animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");

        //计算与玩家距离
        float distance = Vector3.Distance(transform.position, player.position);
        

        // 攻击条件：在范围内、冷却结束、且不在攻击动画中
        if (distance <= attackRange && Time.time >= nextAttackTime && !isAttacking)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
        // 移动条件：距离大于攻击范围，且不在攻击动画中
        else if (distance > attackRange && !isAttacking)
        {
            MoveTowardsPlayer();
        }
    }
    void MoveTowardsPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= attackRange) return;
        // 防止穿过玩家：如果距离太近（比如 1.5 米），也停止
        float minDistance = 1.5f;
        if (distance <= minDistance) return;
        //共性：朝玩家移动（所有丧尸都一样）
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
        transform.position += transform.forward * GetMoveSpeed() * Time.deltaTime;
        //动画控制 （共性）
        animator.SetFloat("Speed", GetMoveSpeed());
    }
    protected abstract float GetMoveSpeed();//子类决定速度

    //受击（共性逻辑）
    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();//抽象方法，子类决定怎么死
        }
    }
    protected abstract void Die();//必须实现：死亡方式

    //攻击触发（通过动画事件调用）
    public abstract void Attack();//必须实现：攻击方式
}
