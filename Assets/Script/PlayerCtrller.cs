using System.Collections;
using UnityEngine;

public class PlayerCtrller : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    private Transform cameraMain;
    private Animator animator;
    private int aimingLayerIndex;
    private bool isAiming = false;

    private bool isTempAiming = false;  // 新增标志
    void Start()
    {
        cameraMain = Camera.main.transform;
        animator = GetComponentInChildren<Animator>();
        aimingLayerIndex = animator.GetLayerIndex("AimingLayer");
    }

    void Update()
    {
        // 移动输入
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 camForward = cameraMain.forward;
        Vector3 camRight = cameraMain.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = camRight * horizontal + camForward * vertical;
        float currentSpeed = moveDirection.magnitude;

        // 移动
        if (currentSpeed > 0.1f)
        {
            transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;
        }

        // 动画参数
        if (animator != null)
        {
            animator.SetFloat("Speed", currentSpeed);
        }

        // ========= 射击输入（按左键短暂激活瞄准层）=========
        if (Input.GetButtonDown("Fire1")) // 按下左键的瞬间
        {
            StartCoroutine(ActivateAimingTemporarily());
        }

        // 瞄准输入
        isAiming = Input.GetButton("Fire2"); // 鼠标右键
        if (aimingLayerIndex != -1 && !isTempAiming)
        {
            animator.SetLayerWeight(aimingLayerIndex, isAiming ? 1f : 0f);
        }

        // 旋转处理：根据是否瞄准分开
        if (isAiming)
        {
            // 瞄准时：角色面向相机前方（水平方向），不转向移动方向
            Vector3 targetForward = cameraMain.forward;
            targetForward.y = 0;
            if (targetForward.sqrMagnitude > 0.001f)
            {
                targetForward.Normalize();
                Quaternion targetRotation = Quaternion.LookRotation(targetForward);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            // 非瞄准时：如果有移动输入，转向移动方向
            if (currentSpeed > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
    IEnumerator ActivateAimingTemporarily()
    {
        isTempAiming = true;
        if (aimingLayerIndex != -1)
        {
            animator.SetLayerWeight(aimingLayerIndex, 1f);
        }
        yield return new WaitForSeconds(1f);
        if (aimingLayerIndex != -1)
        {
            animator.SetLayerWeight(aimingLayerIndex, isAiming ? 1f : 0f);  // 恢复右键控制
        }
        isTempAiming = false;
    }
       

}