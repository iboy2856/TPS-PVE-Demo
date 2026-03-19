using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("手枪组件")]
    public Transform gunModel;
    public Transform barrelLocation;      // 枪口位置
    public Animator gunAnimator;

    [Header("对象池")]
    public ObjectPool objectPool;          // 拖入 PoolManager

    [Header("弹壳设置")]
    public Transform casingExitLocation;   // 弹壳出口
    public float ejectPower = 5f;           // 抛出力

    [Header("射击参数")]
    public float fireRate = 0.2f;
    private float nextFireTime = 0f;

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Fire()
    {
        // 触发射击动画
        if (gunAnimator != null)
            gunAnimator.SetTrigger("Fire");

        // 从对象池获取子弹
        if (objectPool != null && barrelLocation != null)
        {
            GameObject bullet = objectPool.GetBullet();
            bullet.transform.position = barrelLocation.position;
            bullet.transform.rotation = barrelLocation.rotation;
            // 速度在 Bullet 脚本的 OnEnable 中自动设置
        }

        // 从对象池获取弹壳并施加力
        if (objectPool != null && casingExitLocation != null)
        {
            GameObject casing = objectPool.GetCasing();
            casing.transform.position = casingExitLocation.position;
            casing.transform.rotation = casingExitLocation.rotation;

            Rigidbody rb = casing.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 抛出力方向：向右上方
                Vector3 ejectDir = casingExitLocation.right * ejectPower + casingExitLocation.up * ejectPower * 0.5f;
                rb.AddForce(ejectDir, ForceMode.Impulse);
                // 添加随机旋转
                rb.AddTorque(Random.insideUnitSphere * ejectPower, ForceMode.Impulse);
            }
        }
    }
}