using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YOTO;

public class TrainEntity : BaseEntity
{
    private Transform train;
    private Transform AttackPos;
    private const string PerfabPath = "Assets/HotUpdate/prefabs/Train/Tain.prefab";
    protected override void YOTOOnload()
    {
        YOTOFramework.resMgr.LoadGameObject(PerfabPath, OnLoadComplete);
    }

    private void OnLoadComplete(GameObject obj)
    {
        train = UnityEngine.Object.Instantiate(obj).transform;
        train.gameObject.SetActive(true);
        AttackPos= train.Find("AttackPos");
    }

    public Transform GetAttackPos()
    {
        return AttackPos;
    }
    public override void YOTOStart()
    {
        
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
}
