using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YOTO;

public class NormalTowerBulletEntity : BulletEntity
{
     public static  DataObjPool<NormalTowerBulletEntity,Transform> pool=new DataObjPool<NormalTowerBulletEntity, Transform>("NormalTowerBulletEntity", 20);
    private int needFireCount = 0;
    private Vector3 lastPos;
    private Vector3 lastDir;
    public override void FireFromTo(Vector3 pos, Vector3 dir)
    {
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
        temp.velocity = dir.normalized * 50f;
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
        // Debug.Log("回收");
        base.Remove();
        pool.RecoverItem(this);
    }

    public override void TriggerEnter(Collider other)
    {
        ZombieColliderCtrl ctrl;
        if (other.TryGetComponent<ZombieColliderCtrl>(out ctrl))
        {
            if (objTrans!=null)
            {
                Vector3 zpos = objTrans.position;
                Vector3 pos = other.ClosestPoint(zpos);
                EnemyManager.Instance.Hurt(ctrl.entityId, 44);
   
                var fvx = BloodFVXEntity.pool.GetItem(objTrans.position);
                // Debug.Log("生成血+"+fvx._entityID+pos);
                fvx.Location = pos;
                fvx.StartPlay();
                fvx.InstanceGObj();
            

                Remove(); 
            }
            
        }
        else if (other.gameObject.layer == 6)
        {
            Remove();
        }
    }
    
    public override void TriggerExit(Collider other)
    {
        
    }

    public override void SetBulletData(Transform parent)
    {
        SetInVision(true);
        SetPrefabBundlePath("Assets/HotUpdate/prefabs/Bullet/Bullet.prefab");
    }

    public override void BulletAfterIntoObjectPool()
    {
 
    }


}
