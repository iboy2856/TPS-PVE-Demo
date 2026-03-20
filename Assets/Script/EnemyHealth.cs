using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public GameObject hitEffectPrefab;
    public AudioClip hitSound;
    public AudioSource audioSource; // 可选，如果没有则用 PlayClipAtPoint
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        if (hitSound != null)
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
    }

    void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
        // 后续可添加死亡特效、音效、掉落物等
    }
}