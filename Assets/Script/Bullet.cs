using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 3f;      // 最大生存时间
    public float speed = 30f;         // 子弹飞行速度（可在预制体上调整）

    private Rigidbody rb;
    private ObjectPool pool;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        // 每次从池中取出时，设置速度
        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
        }

        // 启动生命周期计时
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    void OnDisable()
    {
        CancelInvoke();               // 防止回池后还调用返回
        // 重置速度，避免下次取出时残留
        if (rb != null)
            rb.velocity = Vector3.zero;
    }

    void ReturnToPool()
    {
        if (pool == null)
            pool = FindObjectOfType<ObjectPool>();

        if (pool != null)
            pool.ReturnBullet(gameObject);
        else
            Destroy(gameObject);       // 保底
    }

    void OnTriggerEnter(Collider other)
    {
        // 击中任何物体时立即归还（可加入击中特效、伤害逻辑）
        ReturnToPool();
    }
}