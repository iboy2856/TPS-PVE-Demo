using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 注意：你原代码拼写是traget，建议改回target
    public Vector3 offset = new Vector3(0, 1.5f, -4f);
    public float mouseSensitivity = 2f;

    public Vector3 shoulderOffset = new Vector3(0.5f, 0.2f, -1.5f);
    public float transitionSpeed = 10f; // 只用于切换时的位置过渡

    private float currentX = 0f;
    private float currentY = 0f;
    private bool isAiming = false;

    // 关键：当前实际使用的偏移量，用于在两种模式间插值
    private Vector3 currentOffset;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentOffset = offset; // 初始化为普通模式
    }

    void Update()
    {
        currentX += Input.GetAxis("Mouse X") * mouseSensitivity;
        currentY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        currentY = Mathf.Clamp(currentY, -30f, 80f);
        isAiming = Input.GetButton("Fire2");
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 旋转：始终直接计算，无平滑，避免眩晕
        Quaternion targetRot = Quaternion.Euler(currentY, currentX, 0);

        // 确定目标偏移（根据是否肩射）
        Vector3 targetOffset = isAiming ? shoulderOffset : offset;

        // 关键逻辑：如果当前偏移已经接近目标，直接设为精确值（硬锁定）
        // 如果距离较远（正在切换模式），则平滑过渡
        if (Vector3.Distance(currentOffset, targetOffset) < 0.01f)
        {
            currentOffset = targetOffset; // 精确锁定，无漂移
        }
        else
        {
            // 正在进入或退出肩射，平滑过渡位置
            currentOffset = Vector3.Lerp(currentOffset, targetOffset, transitionSpeed * Time.deltaTime);
        }

        // 位置：使用 currentOffset（稳定时是精确值，切换时是过渡值）
        transform.position = target.position + targetRot * currentOffset;

        // 旋转：直接赋值，无Slerp，鼠标晃多快都不滞后
        transform.rotation = targetRot;
    }
}