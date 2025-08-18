using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CameraMgr
{
    private Camera mainCamera;
    private Camera uiCamera;

    private bool isInit = false;
    private Dictionary<string, CinemachineVirtualCamera> cameraMap = new Dictionary<string, CinemachineVirtualCamera>(2);
    private Dictionary<string, CinemachineFreeLook> cameraMapFreeLook = new Dictionary<string, CinemachineFreeLook>(2);
    
    public Camera getMainCamera()
    {
        return mainCamera;
    }
    public Camera getUICamera()
    {
        return uiCamera;
    }
    public void Init()
    {
        GameObject cameraObject = GameObject.Find("MainCamera");
        GameObject.DontDestroyOnLoad(cameraObject);
        mainCamera = cameraObject.GetComponent<Camera>();
        
        //mainCamera.tag = "MainCamera";
        //mainCamera.clearFlags = CameraClearFlags.SolidColor;
        //mainCamera.cullingMask = ~(1 << LayerMask.NameToLayer("UI"));
        mainCamera.gameObject.AddComponent<CinemachineBrain>().m_UpdateMethod = CinemachineBrain.UpdateMethod.FixedUpdate;

        GameObject UIcamera = GameObject.Find("UICamera");
        GameObject.DontDestroyOnLoad(UIcamera);
        uiCamera = UIcamera.GetComponent<Camera>();
        //uiCamera.clearFlags = CameraClearFlags.Depth;
        //uiCamera.orthographic = true;
        //uiCamera.cullingMask = (1 << LayerMask.NameToLayer("UI"));

        getVirtualCamera("MainCameraVirtual");
       
        //getVirtualCameraFreeLook("MainCameraVirtual");
        //����ui���
        isInit = true;
    }
    public CinemachineVirtualCamera getVirtualCamera(string name)
    {
        if (!cameraMap.ContainsKey(name))
        {
            cameraMap[name] = CreateCinemachineCamera(name, position: new Vector3(0, 0, 0));
        }
        return cameraMap[name];
    }
    public CinemachineFreeLook getVirtualCameraFreeLook(string name)
    {
        if (!cameraMapFreeLook.ContainsKey(name))
        {
            cameraMapFreeLook[name] = CreateCinemachineCameraFreeLook(name, position: new Vector3(0, 0, 0));
        }
        return cameraMapFreeLook[name];
    }
    public static CinemachineVirtualCamera CreateCinemachineCamera(string name, Vector3 position)
    {
        
        GameObject cameraObject;
        cameraObject = GameObject.Find(name);
        CinemachineVirtualCamera vcam;
        if (!cameraObject)
        {
            Debug.Log("未找到虚拟相机"+name);
            cameraObject = new GameObject(name);
            vcam = cameraObject.AddComponent<CinemachineVirtualCamera>();
        }
        else
        {
            vcam = cameraObject.GetComponent<CinemachineVirtualCamera>();
        }
        cameraObject.transform.position = position;

        return vcam;
    }

    public static CinemachineFreeLook CreateCinemachineCameraFreeLook(string name, Vector3 position)
    {
        // ����һ���µ��������
        GameObject cameraObject;
        cameraObject= GameObject.Find(name);
        CinemachineFreeLook vcam;
        if (!cameraObject)
        {
            Debug.Log("δ�ҵ����õ��������"+name);
            cameraObject = new GameObject(name);
            vcam= cameraObject.AddComponent<CinemachineFreeLook>();
        }
        else
        {
            vcam = cameraObject.GetComponent<CinemachineFreeLook>();
        }
        cameraObject.transform.position = position;

        return vcam;
    }

    public void LateUpdate()
    {
        if (isInit)
        {
            uiCamera.transform.position = mainCamera.transform.position;
            uiCamera.transform.rotation = mainCamera.transform.rotation;
        }
       
    }
}
