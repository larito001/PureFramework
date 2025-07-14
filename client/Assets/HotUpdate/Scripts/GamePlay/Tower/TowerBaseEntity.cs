using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YOTO;



public enum TowerEnum
{
    Fire,
    NormalBullet,
}
public class TowerBaseEntity : ObjectBase, PoolItem<Vector3>
{
    
    
    public static DataObjPool<TowerBaseEntity, Vector3> pool =
        new DataObjPool<TowerBaseEntity, Vector3>("TowerBaseEntity", 20);

    public static TowerBaseEntity selectTower;
    private bool isHaveTower = false;
    private ObjectBase tower; 
    protected override void YOTOOnload()
    {
    }

    public override void YOTOStart()
    {
    }



    public void GenerateTower(TowerEnum towerType)
    {
        if (towerType==TowerEnum.NormalBullet)
        {
            tower = NormalTower.pool.GetItem(this);
            tower.Location=this.Location+new Vector3(0,0.5f,0);
            tower.InstanceGObj();

        }else if (towerType == TowerEnum.Fire)
        {
            tower= FireStreamTower.pool.GetItem(this);
            tower.Location=this.Location+new Vector3(0,0.5f,0);
            tower.InstanceGObj();
        }
        else
        {
            tower = NormalTower.pool.GetItem(this);
            tower.Location=this.Location;
            tower.InstanceGObj();
    
        }
        isHaveTower = true;
        //todo:选择防御塔生产
   
    }

    public override void YOTOUpdate(float deltaTime)
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
    }


    protected override void AfterInstanceGObj()
    {
        var towerBase = ObjTrans.GetComponent<TowerBase>();
        towerBase.SetId(this._entityID);
    }

    public void AfterIntoObjectPool()
    {
        isHaveTower = false;
    }


    public void SetData(Vector3 serverData)
    {
        SetInVision(true);
        SetPrefabBundlePath("Assets/HotUpdate/prefabs/TowerBase/BaseOfTower.prefab");
    }

    public void OnEnter()
    {
        if (!isHaveTower)
        {
            YOTOFramework.uIMgr.Show(UIEnum.TowerCreateUI);
            selectTower = this;
        }
    }

    public void OnExit()
    {
        YOTOFramework.uIMgr.Hide(UIEnum.TowerCreateUI);
        selectTower = null;
    }

    public override void Free()
    {
        base.Free();
        tower?.Free();
        RecoverObject();
        pool.RecoverItem(this);
    }
}