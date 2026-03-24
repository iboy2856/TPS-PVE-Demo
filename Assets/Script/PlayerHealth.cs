using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHealth : MonoBehaviour
{
    [Header("生命值设置")]
    public int maxHelath = 100;
    private int currentHealth;

    [Header("UI")]
    public Text healthText;

    [Header("受伤效果")]
    public float invincibleDuration = 1f;//受击后有一秒的无敌时间
    private bool isInvincible = false;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHelath;
        UpdateUI();
    }
    public void TakeDamage(int damage)
    {
        //无敌状态或已死亡，不受伤害
        if (isInvincible || currentHealth <= 0) return;

        currentHealth -= damage;
        Debug.Log($"玩家受到{damage}伤害，剩余生命值:{currentHealth}");
        //触发受伤无敌
        StartCoroutine(InvincibleCoroutine());

        UpdateUI();
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    System.Collections.IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
    }

    void UpdateUI()
    {
        if (healthText != null)
        {
            healthText.text = $"生命:{currentHealth}";
        }
    }
    void Die()
    {
        Debug.Log("玩家死亡！游戏结束");
    }
    public int GetCurrentHelath()
    {
        return currentHealth;
    }
}
