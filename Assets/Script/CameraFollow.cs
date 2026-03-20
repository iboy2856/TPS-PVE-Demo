using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 1f, -4f);      // 轨道模式下的偏移（仅用于初始距离）
    public float mouseSensitivity = 150f;                 // 注意：已乘 Time.deltaTime，需要较大值
    public float scrollSensitivity = 2f;
    public float smoothSpeed = 10f;
    public float distanceMin = 2f;
    public float distanceMax = 10f;

    // 肩射模式参数
    public bool useShoulderAim = true;                    // 启用肩射
    public Vector3 shoulderOffset = new Vector3(0.5f, 0.2f, -1.5f); // 相对于角色的右肩偏移
    public float aimTransitionSpeed = 10f;                 // 模式切换平滑速度

    // 瞄准相关
    public bool autoAimZoom = true;                        // 是否启用瞄准拉近（仅在轨道模式下有效）
    public float aimZoomFactor = 0.6f;                     // 瞄准时距离比例 (0.6 = 拉近40%)

    private float currentX = 0f;
    private float currentY = 0f;
    private float currentDistance;
    private float targetBaseDistance;                      // 不受瞄准影响的基础距离（由滚轮控制）
    private bool isAiming;

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;

        currentDistance = -offset.z;
        targetBaseDistance = currentDistance;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 鼠标旋转（帧率独立）
        currentX += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        currentY -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        currentY = Mathf.Clamp(currentY, -30f, 80f);

        // 滚轮缩放（增加死区）
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.1f)
        {
            targetBaseDistance -= scroll * scrollSensitivity;
            targetBaseDistance = Mathf.Clamp(targetBaseDistance, distanceMin, distanceMax);
        }

        // 检测瞄准状态
        isAiming = Input.GetButton("Fire2"); // 鼠标右键

        if (useShoulderAim && isAiming)
        {
            // ========= 肩射模式 =========
            // 位置：角色本地偏移 -> 世界坐标
            Vector3 desiredShoulderPos = target.position + target.TransformDirection(shoulderOffset);
            targetPosition = desiredShoulderPos;

            // 旋转：直接由鼠标控制（相机朝向准星方向）
            targetRotation = Quaternion.Euler(currentY, currentX, 0);

            // 肩射模式：直接赋值，无延迟
            transform.position = Vector3.Lerp(transform.position, targetPosition, aimTransitionSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, aimTransitionSpeed * Time.deltaTime);
        }
        else
        {
            // ========= 轨道模式 =========
            // 距离：考虑瞄准拉近
            float targetDistance = targetBaseDistance;
            if (autoAimZoom && isAiming) // 注意：这里 isAiming 可能为 true，但 useShoulderAim 为 false 时会进入此分支
            {
                targetDistance *= aimZoomFactor;
            }
            currentDistance = Mathf.Lerp(currentDistance, targetDistance, smoothSpeed * Time.deltaTime);

            // 计算轨道位置
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            Vector3 desiredPos = target.position + rotation * new Vector3(0, 0, -currentDistance);
            targetPosition = desiredPos;

            // 旋转：看向角色（加上高度偏移）
            targetRotation = Quaternion.LookRotation(target.position + Vector3.up * 1.5f - targetPosition);

            // 轨道模式：平滑过渡
            transform.position = Vector3.Lerp(transform.position, targetPosition, aimTransitionSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, aimTransitionSpeed * Time.deltaTime);
        }
    }
}