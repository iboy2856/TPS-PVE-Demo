using UnityEngine;

public class Casing : MonoBehaviour
{
    public float lifeTime = 5f;
    private ObjectPool pool;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    void OnDisable()
    {
        CancelInvoke();
        if (rb != null)
            rb.velocity = Vector3.zero;
    }

    void ReturnToPool()
    {
        if (pool == null)
            pool = FindObjectOfType<ObjectPool>();
        if (pool != null)
            pool.ReturnCasing(gameObject);
        else
            Destroy(gameObject);
    }
}