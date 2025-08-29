using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using YOTO;

public class CameraCtrl
{
    private CinemachineVirtualCamera vCamera;
    private Vector3 moveDirection;
    private Vector3 currentVelocity;
    private float moveSpeed = 1f; // 调整移动速度
    private float drag = 0.9f;    // 阻尼
    private bool isDragging = false;
    private GraphicRaycaster graphicRaycaster;
    private PointerEventData pointerEventData;
    private Vector3 touchPosition;
    public GameObject cameraDir;
    private float xRotation = 0f;
    private float yRotation = 0f;

    public float verticalClamp = 90f;
    public float horizontalClamp = 90f;
    public float sensitivity = 10f;
    public float smoothTime = 0.5f; // 平滑时间

    private Vector2 lookInput = Vector2.zero;
    private float rayTimer = 0f;
    private bool isShaking = false;

    public CameraCtrl()
    {


        YOTOFramework.eventMgr.AddEventListener<Vector2>(YOTO.YOTOEventType.Look, (input) =>
        {
            if (StagePlugin.Instance.GameStart)
                lookInput = input;
        });

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

    public void AddShake(float time)
    {
        if (isShaking) return;
        isShaking = true;
        var perlin = vCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        if (perlin == null)
        {
            perlin = vCamera.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        perlin.m_AmplitudeGain = 1.5f;
        perlin.m_FrequencyGain = 2.0f;

        YOTOFramework.timeMgr.DelayCall(() =>
        {
            isShaking = false;
            perlin.m_AmplitudeGain = 0f;
            perlin.m_FrequencyGain = 0f;
        }, time);
    }



    public void UsePlayerCamera()
    {
        vCamera.Priority = 999;
    }

    private void CameraMove(Vector2 dir, float dt)
    {
        isDragging = true;

        Vector3 forward = new Vector3(vCamera.transform.forward.x, 0, vCamera.transform.forward.z).normalized;
        Vector3 right = new Vector3(vCamera.transform.right.x, 0, vCamera.transform.right.z).normalized;

        moveDirection = -forward * dir.y - right * dir.x;
        currentVelocity = moveDirection * moveSpeed;

        vCamera.transform.position += currentVelocity * dt;
    }

    private void Press()
    {
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);

        if (results.Count > 0)
        {
            Debug.Log("点到UI: " + results[0].gameObject.name);
        }
        else
        {
            Vector3 dir = new Vector3(touchPosition.x, touchPosition.y, vCamera.m_Lens.NearClipPlane);
            Ray ray = YOTOFramework.cameraMgr.getMainCamera().ScreenPointToRay(dir);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, 1000, LayerMask.GetMask("Food")))
            {
                // todo: 点击逻辑
                Debug.Log("点击到了食物"+hitInfo.transform.gameObject.name);
                if (hitInfo.transform.TryGetComponent<FoodBase>(out FoodBase food))
                {
                    PlayerPlugin.Instance.CatchFood(food.foodId);
                }
            }
        }
    }

    private void Touch(Vector2 pos)
    {
        touchPosition = pos;
    }

    private void CameraScale(float scale)
    {
        if (scale >= 0)
            vCamera.m_Lens.FieldOfView -= 1;
        else
            vCamera.m_Lens.FieldOfView += 1;
    }
    
    public void Update(float dt)
    {
        PlayerPlugin.Instance.RotateSelfPlayerEyes(lookInput);
        rayTimer += dt;
        if (rayTimer >= 0.02f)
        {
            Vector3 dir = new Vector3(touchPosition.x, touchPosition.y, vCamera.m_Lens.NearClipPlane);
            Ray ray = YOTOFramework.cameraMgr.getMainCamera().ScreenPointToRay(dir);
            RaycastHit hitInfo;

            pointerEventData.position = touchPosition;

            if (Physics.Raycast(ray, out hitInfo, 1000, LayerMask.GetMask("Ground")))
            {
                YOTOFramework.eventMgr.TriggerEvent<Vector3>(YOTO.YOTOEventType.RefreshMousePos, hitInfo.point);
            }

            rayTimer -= 0.02f;
        }

        // 平滑移动阻尼
        if (!isDragging)
        {
            if (currentVelocity.magnitude > 0.01f)
            {
                vCamera.transform.position += currentVelocity * dt;
                currentVelocity *= Mathf.Pow(drag, dt * 60f);
            }
        }
        else
        {
            isDragging = false;
        }

        // 鼠标平滑视角
        float mouseX = lookInput.x * sensitivity * dt;
        float mouseY = lookInput.y * sensitivity * dt;

        xRotation -= mouseY;
        yRotation += mouseX;

        xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);
        yRotation = Mathf.Clamp(yRotation, -horizontalClamp, horizontalClamp);

        Quaternion baseRotation = Quaternion.LookRotation(cameraDir.transform.forward);
        Quaternion targetRotation = baseRotation * Quaternion.Euler(xRotation, yRotation, 0f);

        float lerpFactor = 1 - Mathf.Exp(-smoothTime * 60f * dt);
        vCamera.transform.rotation = Quaternion.Slerp(vCamera.transform.rotation, targetRotation, lerpFactor);
        
        lookInput = Vector2.zero;
   
    }
}
