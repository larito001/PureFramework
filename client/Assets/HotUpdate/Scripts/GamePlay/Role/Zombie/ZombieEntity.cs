using System.Collections;
using System.Collections.Generic;
using ProjectDawn.Navigation;
using ProjectDawn.Navigation.Hybrid;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using YOTO;

public class ZombieEntity : ObjectBase, PoolItem<Vector3>
{
    public static DataObjPool<ZombieEntity, Vector3> pool = new DataObjPool<ZombieEntity, Vector3>("ZombieEntity", 50);
    public Transform zombieBase;
    private ZombieAnimatorCtrl ZombieAnimator;
    private ZombieNavCtrl zombieNav;
    public float HP = 100;
    private Transform target;
    private CrowdGroupAuthoring group;
    public bool isInit = false;
    public bool isDie { get; private set; }
    private float atkTimer = 0;

    public void SetTarget(Transform t)
    {
        this.target = t;
    }

    protected override void AfterInstanceGObj()
    {
        objTrans.gameObject.SetActive(true);
        isDie = false;
        zombieBase = objTrans;
        ZombieAnimator = objTrans.GetComponent<ZombieAnimatorCtrl>();
        ZombieAnimator.Init(this);
        zombieBase.gameObject.SetActive(true);
        zombieNav = zombieBase.GetComponent<ZombieNavCtrl>();
        zombieNav.Init(this);
        zombieBase.GetComponent<ZombieColliderCtrl>().entityId = this._entityID;
        ZombieAnimator.EnemyIdel();
        var colliderCtrl = zombieBase.GetComponent<ZombieColliderCtrl>();
        colliderCtrl.Init(this);
        colliderCtrl.Run();
        HP = 100;
        var auth = zombieBase.GetComponent<AgentCrowdPathingAuthoring>();
        auth.Group = group;
        isInit = true;
    }

    public void SetGroup(CrowdGroupAuthoring group)
    {
        this.group = group;
        if (zombieBase)
        {
            zombieBase.GetComponent<AgentCrowdPathingAuthoring>().Group = group;
        }
    }

    public void StopNav()
    {
        zombieNav.Stop();
    }

    public void StartNav()
    {
        zombieNav.StartMove();
    }

    public void Hurt(float hurt)
    {
        HP -= hurt;
        int rand = Random.Range(0, 2); // 0 或 1
        FlyTextMgr.Instance.AddText(hurt.ToString(), this.objTrans.position, (FlyTextType)rand);
        if (HP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDie) return;
        isDie = true;
        StopNav();
        EnemyManager.Instance.RemoveZombie(_entityID);
        YOTOFramework.timeMgr.DelayCall(() => { Free(); }, 2.2f);
        ZombieAnimator.EnemyDie();
        zombieBase.GetComponent<ZombieColliderCtrl>().Stop();
    }

    protected override void YOTOOnload()
    {
    }

    public override void YOTOStart()
    {
    }

    private float timer = 0;

    public override void YOTOUpdate(float deltaTime)
    {
        if (HP > 0 && zombieNav)
        {
            timer += deltaTime;
            if (timer > 1)
            {
                timer = 0;
                zombieNav.SetTarget(target);
            }
        }
    }

    public override void YOTONetUpdate()
    {
    }

    public void ZombieMoveStopTrigger()
    {
        if (isDie) return;
        var pPos = PlayerManager.Instance.GetPlayerPos();
        var zPos = zombieBase.transform.position;
        var dis = (pPos - zPos).magnitude;
        if (dis < 3)
        {
            if (atkTimer >= 2.4f)
            {
                if (!isDie)
                {
                    ZombieAnimator.EnemyAtk();
                }

                atkTimer = 0;
            }

            atkTimer += Time.fixedDeltaTime;
        }
        else
        {
            atkTimer = 2.4f;
            ZombieAnimator.EnemyIdel();
        }
    }

    public void GenerateZombieBullet()
    {
        if (isDie) return;
        var bullet = NormalZombieBullet.pool.GetItem(zombieBase);
        bullet.Location = zombieBase.position;
        bullet.InstanceGObj();
        bullet.FireFromTo(bullet.Location, zombieBase.forward);
    }

    public void ZombieMovingTrigger()
    {
        if (isDie) return;
        atkTimer = 2.4f;
        ZombieAnimator.EnemyRun();
    }

    public override void YOTOFixedUpdate(float deltaTime)
    {
    }

    public override void YOTOOnHide()
    {
    }

    //如果继承，注意重写
    public override void Free()
    {
        base.Free();
        objTrans.gameObject.SetActive(false);
        RecoverObject();
        pool.RecoverItem(this);
    }

    public void AfterIntoObjectPool()
    {
        isInit = false;
    }


    public void TriggerEnter(Collider other)
    {
    }

    public void TriggerExit(Collider other)
    {
    }

    public void SetData(Vector3 serverData)
    {
        SetInVision(true);
        SetPrefabBundlePath("Assets/HotUpdate/prefabs/Zombies/NormalZombie.prefab");
    }
}