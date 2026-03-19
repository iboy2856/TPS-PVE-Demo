using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;                // 跟随的目标
    public Vector3 offset = new Vector3(0, 2f, -6f); // 初始偏移量
    public float mouseSensitivity = 3f;      // 鼠标灵敏度
    public float scrollSensitivity = 2f;     // 滚轮缩放灵敏度
    public float smoothSpeed = 5f;            // 跟随平滑度
    public float distanceMin = 2f;            // 最小距离
    public float distanceMax = 10f;           // 最大距离

    private float currentX = 0f;               // 绕Y轴的角度
    private float currentY = 0f;                // 绕X轴的角度（俯仰）
    private float currentDistance;               // 当前距离

    void Start()
    {
        // 初始化角度（可选：根据初始offset计算）
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;

        // 初始化距离
        currentDistance = -offset.z; // 假设offset.z为负

        // 锁定光标并隐藏
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 获取鼠标输入
        currentX += Input.GetAxis("Mouse X") * mouseSensitivity;
        currentY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        currentY = Mathf.Clamp(currentY, -30f, 80f); // 限制俯仰角度

        // 滚轮缩放
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentDistance -= scroll * scrollSensitivity;
        currentDistance = Mathf.Clamp(currentDistance, distanceMin, distanceMax);

        // 计算相机位置（球坐标系）
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 desiredPosition = target.position + rotation * new Vector3(0, 0, -currentDistance);

        // 平滑移动
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // 始终看向目标（可以添加向上偏移，让视线对准角色中心偏上）
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}