using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireStreamBullet : BulletEntity
{
      public static  DataObjPool<FireStreamBullet,Transform> pool=new DataObjPool<FireStreamBullet, Transform>("FireStreamBullet", 20);
    private int needFireCount = 0;
    private Vector3 lastPos;
    private Vector3 lastDir;
    public override void FireFromTo(Vector3 pos, Vector3 dir)
    {
        destroyTime = 3000;
        SetStartTime();
        if (!bulletBase)
        {
            lastPos = pos;
            lastDir +=dir;
            needFireCount++;
            return;
        } 
        bulletBase.SetTrailEnabled(false);
        // 设置子弹位置：角色前方 + Y轴偏移（比如 +1.0f 高一点）
        bulletBase.transform.position = pos;
        bulletBase.ClearTrail();
        bulletBase.SetTrailTime(0.1f);
        // 设置子弹朝向
        bulletBase.transform.rotation = Quaternion.LookRotation(dir);
        bulletBase.SetTrailEnabled(true);
        // 添加刚体
        Rigidbody temp;
        if (!bulletBase.gameObject.TryGetComponent<Rigidbody>(out  temp))
        {
            temp =bulletBase.gameObject.AddComponent<Rigidbody>();
            temp.useGravity = false;
        }
    }

    protected override void AfterInstanceGObj()
    {
        base.AfterInstanceGObj();
        if (needFireCount!=0)
        {
            FireFromTo(lastPos,lastDir);
            lastPos = new Vector3();
            lastDir = new Vector3();
            needFireCount = 0;
        }
    }

    //主动移除
    public override void Remove()
    {
        atkEnemy.Clear();
        // Debug.Log("回收");
        base.Remove();
        pool.RecoverItem(this);
    }
    Dictionary<int ,ZombieColliderCtrl> atkEnemy = new Dictionary<int ,ZombieColliderCtrl>();
    public override void TriggerEnter(Collider other)
    {
        ZombieColliderCtrl ctrl;
        if ( other.TryGetComponent<ZombieColliderCtrl>(out ctrl))
        {
            // EnemyManager.Instance.Hurt(ctrl.entityId,999);
            // Debug.Log("僵尸进入火焰");
            atkEnemy.Add(ctrl.entityId,ctrl);
        }
      
    }

    public override void TriggerExit(Collider other)
    {
        ZombieColliderCtrl ctrl;
        if ( other.TryGetComponent<ZombieColliderCtrl>(out ctrl))
        {
            atkEnemy.Remove(ctrl.entityId);
        }
    }

    public override void SetBulletData(Transform parent)
    {
        SetInVision(true);
        SetPrefabBundlePath("Assets/HotUpdate/prefabs/Bullet/FireSteam.prefab");
    }

    public override void BulletAfterIntoObjectPool()
    {
 
    }
    List<int >removeList = new List<int>();
    private float timer = 0;
    public override void YOTOUpdate(float deltaTime)
    {
        base.YOTOUpdate(deltaTime);
        if (timer >=0.5f)
        {
            foreach (var zombieColliderCtrl in atkEnemy)
            {
                if (!EnemyManager.Instance.CheckZombieAlive(zombieColliderCtrl.Value.entityId))
                {
                    removeList.Add(zombieColliderCtrl.Value.entityId);
                
                }
                else
                {
                    EnemyManager.Instance.Hurt(zombieColliderCtrl.Value.entityId,20);
                }
            }
            for (var i = 0; i < removeList.Count; i++)
            {
                atkEnemy.Remove(removeList[i]);

            }

            timer -= 0.5f;
        }

        timer += deltaTime;


    }
}
