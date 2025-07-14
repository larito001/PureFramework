using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using YOTO;

public class MeleeEntity : BaseEntity
{
    private const string WeaponPath = "Assets/Fire Axe/Prefab/PBR/Fire_Axe_LODA.prefab";
    private HandRoot handRoot;
    private ParticleSystem fire;
    public float fireRate =1f; // 每发子弹的时间间隔
    private float fireTimer = 0f;

    public bool canFire = true; // 是否允许发射
    private bool isFiring = false; // 当前是否处于开火状态

    public Transform firePos { get; private set; }
    private Vector3 _gunForward;

    public void SetForward(Vector3 forward)
    {
        _gunForward = forward;
    }
    public MeleeEntity(HandRoot handRoot)
    {
        this.handRoot = handRoot;
    }

    private GameObject melee;
    private Transform _firePosParent;

    public void SetFirePosParent(Transform firePosParent)
    {
        _firePosParent = firePosParent;
        if (firePos != null)
        {
            firePos.SetParent(_firePosParent);
            firePos.localPosition = new Vector3(0, 0, 0);
        }
    }
    protected override void YOTOOnload()
    {
        YOTOFramework.resMgr.LoadGameObject(WeaponPath, WeaponLoadComplete);
    }

    private void WeaponLoadComplete(GameObject obj)
    {
        melee = UnityEngine.Object.Instantiate(obj,handRoot.transform);
        melee.transform.localPosition = Vector3.zero;
        firePos = melee.transform.Find("FirePos");
        firePos.parent = _firePosParent;
        firePos.localPosition = Vector3.zero;
    }
    
    public void TryShot(UnityAction shootCallBack)
    {
        if (!canFire) return;

        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            shootCallBack();
            SpawnBullet();
        }
    }

    public override void Free()
    {
        GameObject.Destroy(melee);
        base.Free();
    }

    // 子弹生成接口（你来实现）
    protected virtual void SpawnBullet()
    {
        if (fire != null)
        {
            fire.Stop();
            fire.Play();   
        }
        YOTOFramework.timeMgr.DelayCall(DelayFire,0.4f);
        
    }

    private void DelayFire()
    {
        // 获取子弹对象
        var bullet = MeleeBullet.pool.GetItem(firePos);
        bullet.InstanceGObj();
        bullet.FireFromTo(firePos.position, _gunForward);
    }
    

    public override void YOTOStart()
    {
    }

    public override void YOTOUpdate(float deltaTime)
    {
        // 计时器增加
        if (fireTimer < fireRate)
        {
            fireTimer += Time.deltaTime;
        }
    }

    public override void YOTONetUpdate()
    {
    }

    public override void YOTOFixedUpdate(float deltaTime)
    {
    }

    public override void YOTOOnHide()
    {
    }
    public void UnuseWeapon()
    {
        canFire = false;
        melee.SetActive(false);
    }
    public void UseWeapon()
    {
        canFire = true;
        melee.SetActive(true);
        
    }
}