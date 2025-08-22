using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using YOTO;

public class CameraCtrl
{
    CinemachineVirtualCamera startVCamera;
    CinemachineVirtualCamera vCamera;
    Vector3 moveDirection;
    Vector3 currentVelocity; 
    float moveSpeed = 3f; 
    float drag = 0.95f; 
    bool isDragging = false; 
    GraphicRaycaster graphicRaycaster;
    PointerEventData pointerEventData;
    private Vector3 touchPosition;
    private GameObject startCameraDir;
    private GameObject cameraDir;
    private float xRotation = 0f;
    private float yRotation = 0f;

    private float xRotationVelocity = 0f; // 用于SmoothDamp平滑垂直角度
    private float yRotationVelocity = 0f; // 用于SmoothDamp平滑水平角度

    public float verticalClamp = 5f;
    public float horizontalClamp = 7f;

    public float sensitivity = 100f;
    public float smoothTime = 0.35f; // 平滑时间，调节缓启动和缓减速效果

    private Vector2 lookInput = new Vector2(0, 0);

    public CameraCtrl()
    {
        startVCamera = YOTOFramework.cameraMgr.getVirtualCamera("StartCameraVirtual");
        startVCamera.m_Lens.OrthographicSize = 50;
        startCameraDir = GameObject.Find("StartCameraDir");
        startVCamera.transform.position = startCameraDir.transform.position;
        startVCamera.transform.rotation = startCameraDir.transform.rotation;
        YOTOFramework.eventMgr.AddEventListener<Vector2>(YOTO.YOTOEventType.Look,
            (lookInput) => { this.lookInput = lookInput; });
        vCamera = YOTOFramework.cameraMgr.getVirtualCamera("MainCameraVirtual");
        cameraDir = GameObject.Find("CameraDir");
        vCamera.transform.position = cameraDir.transform.position;
        vCamera.transform.rotation = cameraDir.transform.rotation;


        vCamera.m_Lens.FieldOfView = 30;
        vCamera.m_Lens.OrthographicSize = 40;
        YOTOFramework.eventMgr.AddEventListener<Vector2>(YOTO.YOTOEventType.Touch, Touch);
        YOTOFramework.eventMgr.AddEventListener(YOTO.YOTOEventType.PressLeftMouse, Press);

        graphicRaycaster = YOTOFramework.uIMgr.GetLayer(UILayerEnum.RayCast).layerRoot.GetComponent<GraphicRaycaster>();
        pointerEventData = new PointerEventData(EventSystem.current);
    }

  
    private bool isShaking=false;
    public void AddShake(float time)
    {
        if (isShaking) return;
        isShaking=true;   
        var perlin = vCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    
        if (perlin == null)
        {
            // 如果没有 Perlin Noise 组件，添加一个
            perlin = vCamera.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
        // perlin.m_NoiseProfile=
        // 设置抖动参数（你可以根据需要调整这两个值）
        perlin.m_AmplitudeGain = 1.5f;
        perlin.m_FrequencyGain = 2.0f;

        // 延迟 time 秒后关闭抖动
        YOTOFramework.timeMgr.DelayCall(() =>
        {
            isShaking = false;
            perlin.m_AmplitudeGain = 0f;
            perlin.m_FrequencyGain = 0f;
        }, time);
    }
    public void UseStartCamera()
    {
        startVCamera.Priority = 999;
        vCamera.Priority = 0;
    }
    public void UsePlayerCamera()
    {
        startVCamera.Priority = 0;
        vCamera.Priority = 999;

    }

    private void CameraMove(Vector2 dir)
    {
        isDragging = true;
        Vector3 forward = new Vector3(vCamera.transform.forward.x, 0, vCamera.transform.forward.z).normalized;
        Vector3 right = new Vector3(vCamera.transform.right.x, 0, vCamera.transform.right.z).normalized;
        moveDirection = -forward * dir.y - right * dir.x;
        currentVelocity = moveDirection * moveSpeed;
        vCamera.transform.position += currentVelocity * Time.deltaTime;
    }
    
    private void Press()
    {
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);
        if (results.Count > 0)
        {
    
            Debug.Log("点到UI"+results[0].gameObject.name);
        }
        else
        {
   
            Vector3 dir = new Vector3(touchPosition.x, touchPosition.y, vCamera.m_Lens.NearClipPlane);
            Ray ray = YOTOFramework.cameraMgr.getMainCamera().ScreenPointToRay(dir);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 1000, LayerMask.GetMask("Test")))
            {
                //todo:点击
            }
            else
            {
              
            }
        }
    }

    private void Touch(Vector2 pos)
    {
        touchPosition = pos;
    }

    private void CameraSclae(float sclale)
    {
        if (sclale >= 0)
        {
            vCamera.m_Lens.FieldOfView -= 1;
            
        }
        else
        {
            vCamera.m_Lens.FieldOfView += 1;
        }
    }

    private float rayTimer = 0;
    public void Update(float dt)
    {
        if (rayTimer >= 0.02f)
        {
            Vector3 dir = new Vector3(touchPosition.x, touchPosition.y, vCamera.m_Lens.NearClipPlane);
            // Debug.Log("TOUCH" + dir);
            Ray ray = YOTOFramework.cameraMgr.getMainCamera().ScreenPointToRay(dir);
            Ray uiRay = YOTOFramework.cameraMgr.getUICamera().ScreenPointToRay(dir);
            RaycastHit hitInfo; //

            pointerEventData.position = touchPosition;
            List<RaycastResult> results = new List<RaycastResult>();

            // ִUI
            // graphicRaycaster.Raycast(pointerEventData, results);
            // if (results.Count > 0)
            // {
            //     //Debug.Log("TOUCH" + hitInfo.point);
            //
            //     //BattleSystem.Instance.playerEntity?.MoveTarget(hitInfo.point);
            //     Debug.Log("�㵽UI��");
            // }
            // else
            {
                if (Physics.Raycast(ray, out hitInfo, 1000, LayerMask.GetMask("Ground")))
                {
                    // Debug.Log("Ground hit");
                    YOTOFramework.eventMgr.TriggerEvent<Vector3>(YOTO.YOTOEventType.RefreshMousePos, hitInfo.point);
                }
            }
            rayTimer -= 0.02f;
        }

        rayTimer += dt;
      
        if (!isDragging)
        {
            if (currentVelocity.magnitude > 0.01f)
            {
                vCamera.transform.position += currentVelocity * dt;
                currentVelocity *= drag;
            }
        }
        else
        {
            isDragging = false;
        }

        {
            float mouseX = lookInput.x * sensitivity * Time.deltaTime;
            float mouseY = lookInput.y * sensitivity * Time.deltaTime;

            float targetXRotation = xRotation - mouseY;
            float targetYRotation = yRotation + mouseX;
            // 获取目标朝向并直接计算欧拉角
            Vector3 desiredEuler = Quaternion.LookRotation(startCameraDir.transform.forward).eulerAngles;
            // 计算并限制角度偏差
            float angleDiffX = Mathf.Clamp(Mathf.DeltaAngle(desiredEuler.x, targetXRotation), -verticalClamp,
                verticalClamp);
            float angleDiffY = Mathf.Clamp(Mathf.DeltaAngle(desiredEuler.y, targetYRotation), -horizontalClamp,
                horizontalClamp);
            // 计算最终目标角度（目标朝向+允许的偏差）
            targetXRotation = desiredEuler.x + angleDiffX;
            targetYRotation = desiredEuler.y + angleDiffY;
            // 平滑过渡并应用旋转
            xRotation = Mathf.SmoothDampAngle(xRotation, targetXRotation, ref xRotationVelocity, smoothTime);
            yRotation = Mathf.SmoothDampAngle(yRotation, targetYRotation, ref yRotationVelocity, smoothTime);
            startVCamera.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }
}