using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrller : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    private Transform cameraMain;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        cameraMain = Camera.main.transform;
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 camForward = cameraMain.forward;
        Vector3 camRight = cameraMain.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // 计算相对于相机的移动方向
        Vector3 moveDirection = camRight * horizontal + camForward * vertical;


        if (moveDirection.magnitude > 0.1f)
        {
            // 平滑旋转到移动方向
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // 移动
            transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;
        }
        float currentSpeed = moveDirection.magnitude; // 范围 0~1
        if (currentSpeed > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;
        }
        if (animator != null)
        {
            animator.SetFloat("Speed", currentSpeed);
        }
    }
}
