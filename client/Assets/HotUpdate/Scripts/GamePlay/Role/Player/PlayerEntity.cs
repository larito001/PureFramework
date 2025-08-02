using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;
using YOTO;
using EventType = YOTO.EventType;

public class PlayerEntity : CharacterBase
{
    #region  属性

    private const float waitTime = 0.5f; //抬枪时间
    private const string playerPerfabPath = "Assets/PolygonApocalypse/Prefabs/Characters/SM_Chr_Teen_Male_01.prefab";

    
    //控制器
    private PlayerAnimatorCtrl animatorCtrl;

    public PlayerMoveCtrl moveCtrl;

    //外部组件
    public Camera camera; //相机,用于获取方向
    private Transform headTarget; //头朝向
    public RigBuilder builder; //控制骨骼IK的，当前只控制头
    private HandRoot handPos; //放枪的位置
    private Transform charCameraPos; //相机跟随的点，设置瞄准镜头偏移
    private Outline outline; //描边
    public GunEntity gun;
    public MeleeEntity melee;

    //玩家状态信息
    private bool isAiming = false;
    private bool canMove = true; //玩家是否可以移动
    private bool isShooting = false; //是否正在开火
    private bool isInit = false; //是否初始化完成
    private bool isAimEnd = false; //是否完成抬枪动作（控制枪械红点）
    private int currentWeapon = PlayerAnimatorCtrl.NONE_LAYER; //当前武器
    private bool isWaiting = false; //是否正在切枪
    private bool isTouching = false; //是否正在触摸屏幕、点击左键、右键
    private bool isFireing = false; //是否按下开火
    //输入信息
    private Vector2 moveInput; //移动输入
    private Vector3 playerMovement; //玩家移动方向
    private Vector3 orgPosition; //玩家复活点位置
    private Vector3 mousePoint; //鼠标的位置
    private Vector3 lookPos; //玩家看向的位置
    //私有计算temp
    private float currentTime = 0; //当前抬枪进度
    #endregion
    
    #region 输入

    private void OnPlayerLoadComplete(GameObject player)
    {
        character = UnityEngine.Object.Instantiate(player);
        character.SetActive(true);
        character.transform.position = orgPosition;
        camera = YOTOFramework.cameraMgr.getMainCamera();
        AddComponent();
        builder = character.GetComponent<RigBuilder>();
        handPos = character.GetComponentInChildren<HandRoot>();
        headTarget = character.gameObject.transform.Find("HeadTarget");
        builder.Build();
        outline = character.GetComponent<Outline>();
        isInit = true;
        SwitchWeapon(1);
    }

    private void OnMove(Vector2 move)
    {
        if (!isInit||!canMove) return;
        moveInput = move;
    }

    private void OnTouchScreen()
    {
        if (!isInit||!canMove) return;
        isTouching = true;
        isAiming = isTouching;
    }

    private void TryFire()
    {
        if (!isInit||!canMove) return;
        isAiming = true;
        isFireing = true;
    }

    private void RefrshMousePos(Vector3 pos)
    {
        if (!isInit||!canMove) return;
        mousePoint = pos;
    }

    public Vector3 GetPlayerPos()
    {
        if (character == null)
        {
            return new Vector3(9999, 9999, 9999);
        }

        return character.transform.position;
    }

    public Transform GetPlayerTrans()
    {
        return character.transform;
    }
    private void TryReload()
    {
        if (!isInit||!canMove) return;
        if (gun != null)
        {
            gun.Reload();
        }

        animatorCtrl.ReLoad(() =>
        {
            if (gun != null)
                gun.ReloadEnd();
        });
    }

    private void FireRelease()
    {
        if (!isInit||!canMove) return;
        if (!isTouching)
        {
            isAiming = false;
        }

        isFireing = false;
    }


    private void TouchReless()
    {
        if (!isInit||!canMove) return;
        mousePoint = Vector3.zero;
        isTouching = false;
        isAiming = isTouching;
    }

    private void SwitchWeapon(int index)
    {
        if (isWaiting||!canMove) return;

        isWaiting = true;
        YOTOFramework.timeMgr.DelayCall(() => { isWaiting = false; }, 0.7f);
        if (index == 1)
        {
            if (gun == null)
            {
                gun = new GunEntity(handPos);
                gun.Init();
            }
            else
            {
                gun.UseWeapon();
                animatorCtrl.UseGun();
            }

            if (melee != null)
            {
                melee.UnuseWeapon();
            }

            currentWeapon = PlayerAnimatorCtrl.GUN_LAYER;
        }
        else if (index == 2)
        {
            if (melee == null)
            {
                melee = new MeleeEntity(handPos);
                melee.Init();
                melee.SetFirePosParent(this.character.transform);
            }
            else
            {
                melee.UseWeapon();
                animatorCtrl.UseMelee();
            }

            if (gun != null)
            {
                gun.UnuseWeapon();
            }

            currentWeapon = PlayerAnimatorCtrl.MELEE_LAYER;
        }
    }

    #endregion

    #region 对外：初始化，是否移动，受击

    public override void Free()
    {
        gun?.Free();
        melee?.Free();
        GameObject.Destroy(character);
        base.Free();
    }

    public override void Init(Vector3 pos)
    {
        orgPosition = pos;
        AddEvent();
    }

    public void DontMove()
    {
        canMove = false;
    }

    public void CanMove()
    {
        canMove = true;
    }

    public void Hurt(float hurt)
    {
        HP -= hurt;
        if (HP <= 0)
        {
            HP = 0;
        }

        float healthPercentage = HP / maxHP;
        outline.OutlineColor = Color.Lerp(Color.red, Color.green, healthPercentage);
        YOTOFramework.sceneMgr.cameraCtrl.AddShake(0.1f);
        FlyTextMgr.Instance.AddText(hurt.ToString(), character.transform.position, FlyTextType.PlayerHurt);
    }

    #endregion
    
    #region 私有

    private void CulculateDir()
    {
        if (!isInit) return;

        if (!canMove)
        {
            playerMovement = Vector3.zero;
            animatorCtrl.SetMoveDir(playerMovement);
            return;
        }

        Vector3 cameraForwordProjection =
            new Vector3(camera.transform.forward.x, 0, camera.transform.forward.z).normalized;
        playerMovement = cameraForwordProjection * moveInput.y + camera.transform.right * moveInput.x;

        playerMovement = character.transform.InverseTransformVector(playerMovement);
        animatorCtrl.SetMoveDir(playerMovement);
    }

    private void AddComponent()
    {
        Debug.Log("AddComponent");
        animatorCtrl = character.AddComponent<PlayerAnimatorCtrl>();
        moveCtrl = character.AddComponent<PlayerMoveCtrl>();
        animatorCtrl.Init(this);
        animatorCtrl.SetAnimator(character.GetComponent<Animator>());
        moveCtrl.Init(this);
        moveCtrl.SetRig(character.GetComponent<Rigidbody>());
        Debug.Log("AddComponent Finish");

        var mCamera = YOTOFramework.cameraMgr.getVirtualCamera("MainCameraVirtual");
        mCamera.m_Lens.FieldOfView = 60;
        charCameraPos = GameObject.Find("CharCameraPos").transform;

        mCamera.m_Follow = charCameraPos;
        mCamera.m_LookAt = charCameraPos;
        var body = mCamera.AddCinemachineComponent<CinemachineFramingTransposer>();
        body.m_TrackedObjectOffset = new Vector3(0, 2, 0);
        body.m_DeadZoneWidth = 0f;
        body.m_DeadZoneHeight = 0f;
        body.m_CameraDistance = 10;
    }

    #endregion
    
    #region 生命周期

    protected override void AddEvent()
    {
        YOTOFramework.eventMgr.AddEventListener<int>(YOTO.EventType.KeyBoardNumClick, SwitchWeapon);
        YOTOFramework.eventMgr.AddEventListener(YOTO.EventType.TryReload, TryReload);
        YOTOFramework.eventMgr.AddEventListener<Vector3>(YOTO.EventType.RefreshMousePos, RefrshMousePos);
        YOTOFramework.eventMgr.AddEventListener(YOTO.EventType.Fire, TryFire);
        YOTOFramework.eventMgr.AddEventListener(YOTO.EventType.FireRelease, FireRelease);
        YOTOFramework.eventMgr.AddEventListener(YOTO.EventType.TouchPress, OnTouchScreen);
        YOTOFramework.eventMgr.AddEventListener(YOTO.EventType.TouchRelease, TouchReless);
        YOTOFramework.eventMgr.AddEventListener<Vector2>(YOTO.EventType.Move, OnMove);
        YOTOFramework.resMgr.LoadGameObject(playerPerfabPath, OnPlayerLoadComplete);
    }

    protected override void RemoveEvent()
    {
        YOTOFramework.eventMgr.RemoveEventListener<int>(YOTO.EventType.KeyBoardNumClick, SwitchWeapon);
        YOTOFramework.eventMgr.RemoveEventListener(YOTO.EventType.TryReload, TryReload);
        YOTOFramework.eventMgr.RemoveEventListener<Vector3>(YOTO.EventType.RefreshMousePos, RefrshMousePos);
        YOTOFramework.eventMgr.RemoveEventListener(YOTO.EventType.Fire, TryFire);
        YOTOFramework.eventMgr.RemoveEventListener(YOTO.EventType.FireRelease, FireRelease);
        YOTOFramework.eventMgr.RemoveEventListener(YOTO.EventType.TouchPress, OnTouchScreen);
        YOTOFramework.eventMgr.RemoveEventListener(YOTO.EventType.TouchRelease, TouchReless);
        YOTOFramework.eventMgr.RemoveEventListener<Vector2>(YOTO.EventType.Move, OnMove);
    }

    public override void YOTOUpdate(float deltaTime)
    {
        if (!isInit) return;
        CulculateDir();
        animatorCtrl.SetAimingState(isAiming);
        animatorCtrl.SetCurrentWeapon(currentWeapon);
        moveCtrl.SetVolocity(this.animationVelocity);
        moveCtrl.SetRotation(this.animationRotate);
        if (gun != null)
        {
            gun.SetIsAinEnd(isAimEnd);
            gun.SetIsWaiting(isWaiting);
        }
        headTarget.rotation = Quaternion.Euler(0f, character.transform.eulerAngles.y, 0f);
        if (isAiming)
        {
            Vector3 cameraPos = camera.transform.position;
            Vector3 dir = mousePoint - cameraPos;
            Vector3 dirNormalized = dir.normalized;
            float angleRad = Vector3.Angle(dir, Vector3.up) * Mathf.Deg2Rad;
            float height = 1.5f;
            float len = height / Mathf.Cos(angleRad);
            lookPos = mousePoint + dirNormalized * len;
            animatorCtrl.SetLookPos(lookPos);
            if (currentWeapon == PlayerAnimatorCtrl.GUN_LAYER)
            {
                charCameraPos.position = Vector3.Lerp(charCameraPos.position,
                    character.transform.position + character.transform.forward * 3, Time.deltaTime * 10);
            }
            else
            {
                charCameraPos.position = Vector3.Lerp(charCameraPos.position, character.transform.position,
                    Time.deltaTime * 10);
            }


            if (currentTime <= waitTime)
            {
                currentTime += deltaTime;
                isAimEnd = false;
            }

            if (isWaiting)
            {
                currentTime = 0;
            }

            if (currentTime >= waitTime)
            {
                isAimEnd = true;
                gun?.SetForward(character.transform.forward);
            }
        }
        else
        {
            charCameraPos.position =
                Vector3.Lerp(charCameraPos.position, character.transform.position, Time.deltaTime * 10);
            currentTime = 0;
            isAimEnd = false;
        }


        isShooting = false;

        if (isFireing)
        {
            if (currentTime >= waitTime)
            {
                isShooting = true;
                if (gun != null)
                {
                    
                    gun.TryShot(animatorCtrl.TryShoot);
                }
                  
                if (melee != null)
                {
                    melee.TryShot(animatorCtrl.TryUseMelee);
                }
            }
        }

        if (animatorCtrl)
        {
            animatorCtrl.YOTOUpdate(deltaTime);
        }

        if (moveCtrl)
        {
            moveCtrl.YOTOUpdate(deltaTime);
        }
    }

    protected override void YOTOOnload()
    {
    }

    public override void YOTOStart()
    {
    }

    public override void YOTONetUpdate()
    {
    }

    public override void YOTOFixedUpdate(float deltaTime)
    {
    }

    public override void YOTOOnHide()
    {
        //todo:释放调角色包
        RemoveEvent();
        character.SetActive(false);
    }

    #endregion
}