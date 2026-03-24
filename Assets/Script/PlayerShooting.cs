using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    [Header("音效")]
    public AudioSource audioSource;
    public AudioClip fireSound;
    public AudioClip emptySound;
    public float fireSoundVolume = 1f;
    public float emptySoundVolume = 0.8f;

    [Header("枪口特效")]
    public GameObject muzzleFlashPrefab;
    public float flashDuration = 0.1f;

    [Header("后坐力")]
    public float recoilAmount = 5f;
    public float recoilSpeed = 20f;
    private float currentRecoil;

    [Header("手部IK")]
    public Transform rightHandBone;
    public Vector3 handRotationOffset;

    [Header("手枪组件")]
    public Transform gunModel;
    public Transform barrelLocation;
    public Animator gunAnimator;

    [Header("对象池")]
    public ObjectPool objectPool;

    [Header("弹壳设置")]
    public Transform casingExitLocation;
    public float ejectPower = 5f;

    [Header("射击参数")]
    public float fireRate = 0.2f;
    private float nextFireTime = 0f;

    [Header("弹药系统")]
    public int maxAmmo = 30;
    public int currentAmmo;
    public int totalReserveAmmo = 120;
    public float reloadTime = 1.5f;
    private bool isReloading = false;

    private Animator animator;
    private bool isAiming = false;
    private Camera mainCamera;

    private bool isFiring = false;  // 是否正在射击

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        mainCamera = Camera.main;
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        // 平滑恢复后坐力
        currentRecoil = Mathf.Lerp(currentRecoil, 0f, recoilSpeed * Time.deltaTime);

        // 射击输入
        if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }

        // 瞄准输入
        isAiming = Input.GetButton("Fire2");
        int aimingLayer = animator.GetLayerIndex("AimingLayer");
        if (aimingLayer != -1)
        {
            animator.SetLayerWeight(aimingLayer, isAiming ? 1f : 0f);
        }

        // 换弹输入
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < maxAmmo && totalReserveAmmo > 0)
        {
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        // 可选：播放换弹动画或音效
        yield return new WaitForSeconds(reloadTime);

        int needed = maxAmmo - currentAmmo;
        int take = Mathf.Min(needed, totalReserveAmmo);
        currentAmmo += take;
        totalReserveAmmo -= take;
        isReloading = false;
        Debug.Log($"Reloaded. Current ammo: {currentAmmo}, Reserve: {totalReserveAmmo}");
    }

    void LateUpdate()
    {
        // 右键瞄准 或 左键射击时，手部跟随准星
        if ((isAiming || isFiring) && rightHandBone != null && mainCamera != null)
        {
            Quaternion targetRot = mainCamera.transform.rotation * Quaternion.Euler(handRotationOffset);
            Quaternion recoilRot = Quaternion.Euler(-currentRecoil, 0f, 0f);
            rightHandBone.rotation = targetRot * recoilRot;
        }
    }

    void Fire()
    {
        // 开始射击，标记为正在射击
        isFiring = true;
        Invoke(nameof(StopFiring), 1f);  // 1秒后复位

        // 换弹时不能射击
        if (isReloading) return;

        // 弹药耗尽：只播放空枪音效，不执行后续
        if (currentAmmo <= 0)
        {
            if (audioSource != null && emptySound != null)
                audioSource.PlayOneShot(emptySound, emptySoundVolume);
            return;
        }

        // ===== 正常射击逻辑 =====
        // 播放开火音效
        if (audioSource != null && fireSound != null)
            audioSource.PlayOneShot(fireSound, fireSoundVolume);

        // 触发射击动画
        if (gunAnimator != null)
            gunAnimator.SetTrigger("Fire");

        // 消耗弹药
        currentAmmo--;

        // 后坐力
        currentRecoil += recoilAmount;
        currentRecoil = Mathf.Clamp(currentRecoil, 0f, recoilAmount * 2f);

        // 枪口火焰
        if (muzzleFlashPrefab != null && barrelLocation != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);
            Destroy(flash, flashDuration);
        }

        // 计算目标点（射线从准星发出）
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

        // 设置 LayerMask，忽略 Player 层
        int playerLayer = LayerMask.NameToLayer("Player");
        int layerMask = ~(1 << playerLayer);  // 忽略 Player 层，其他层都检测

        if (Physics.Raycast(ray, out hit, 1000f, layerMask))
        {
            targetPoint = hit.point;
            // 可选：调试用，在控制台输出击中点
            // Debug.Log("击中点: " + hit.point + " 物体: " + hit.collider.name);
        }
        else
        {
            // 没有击中任何物体，取射线前方 1000 米处的点
            targetPoint = ray.GetPoint(1000f);
        }


        // 生成子弹
        if (objectPool != null && barrelLocation != null)
        {
            GameObject bullet = objectPool.GetBullet();
            bullet.transform.position = barrelLocation.position;

            // 计算子弹方向：从枪口指向目标点
            Vector3 direction = (targetPoint - barrelLocation.position).normalized;
            bullet.transform.rotation = Quaternion.LookRotation(direction);
            bullet.SetActive(true);
        }

        // 生成弹壳
        if (objectPool != null && casingExitLocation != null)
        {
            GameObject casing = objectPool.GetCasing();
            casing.transform.position = casingExitLocation.position;
            casing.transform.rotation = casingExitLocation.rotation;

            Rigidbody rb = casing.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 ejectDir = casingExitLocation.right * ejectPower + casingExitLocation.up * ejectPower * 0.5f;
                rb.AddForce(ejectDir, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * ejectPower, ForceMode.Impulse);
            }
        }
    }
    void StopFiring()
    {
        isFiring = false;
    }
}