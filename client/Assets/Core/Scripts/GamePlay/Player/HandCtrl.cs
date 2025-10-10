using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using YOTO;

public class HandCtrl : MonoBehaviour
{
    [SerializeField] private float moveDuration = 0.5f; // 动作时间，可在 Inspector 设置
    [SerializeField] private Ease moveEase = Ease.InQuad; // 缓动类型

    private PlayerEntity playerEntity;
    public Transform leftHand;
    public Transform rightHand;
    public LineRenderer lineRenderer;
    private Vector3 leftHandOriginalPos;
    private Quaternion leftHandOriginalRot;
    private Vector3 rightHandOriginalPos;
    private Quaternion rightHandOriginalRot;

    private FoodEntity leftTarget;
    private FoodEntity rightTarget;

    public void Init(PlayerEntity playerEntity)
    {
        this.playerEntity = playerEntity;

        if (leftHand != null)
        {
            leftHandOriginalPos = leftHand.position;
            leftHandOriginalRot = leftHand.rotation;
        }

        if (rightHand != null)
        {
            rightHandOriginalPos = rightHand.position;
            rightHandOriginalRot = rightHand.rotation;
        }
        lineRenderer.enabled = false;
    }

    /// <summary>
    /// 左手伸出去抓目标
    /// </summary>
    public void ExtendLeftHand(FoodEntity target)
    {
        if (leftHand == null || target == null) return;

        playerEntity.leftHandDoing = true;
        leftTarget = target;

        // Kill 旧的 tween，并强制触发 OnComplete 避免丢失回调

        leftHand.DOKill(true);
        Debug.Log("调用回收左手" + playerEntity.isSelf);
        leftHand.DOMove(leftTarget.ObjTrans.position, moveDuration)
            .SetEase(moveEase)
            .OnKill(() => Debug.Log("LeftHand tween killed"))
            .OnComplete(() =>
            {
                Debug.Log("LeftHand tween complete" + playerEntity.isSelf);
                if (leftTarget!=null&&leftTarget.ObjTrans!=null)
                {
                    lineRenderer.enabled = true;
                    leftTarget.OnCatch();
                    leftTarget.ObjTrans.SetParent(leftHand);   
                }
        
            });
    }

    /// <summary>
    /// 右手伸出去抓目标
    /// </summary>
    public void ExtendRightHand(FoodEntity target)
    {
        if (rightHand == null || target == null) return;

        playerEntity.rightHandDoing = true;
        rightTarget = target;

        rightHand.DOKill(true);

        rightHand.DOMove(rightTarget.ObjTrans.position, moveDuration)
            .SetEase(moveEase)
            .OnKill(() => Debug.Log("RightHand tween killed"))
            .OnComplete(() =>
            {
                if (rightTarget != null&&rightTarget.ObjTrans!=null)
                {
                    Debug.Log("RightHand tween complete");
                    rightTarget.OnCatch();
                    rightTarget.ObjTrans.SetParent(rightHand); 
                }
             
            });
    }

    /// <summary>
    /// 左手收回
    /// </summary>
    public void RetractLeftHand()
    {
        if (leftHand == null) return;

        leftHand.DOKill(true);
        Debug.Log("开始收" + playerEntity.isSelf);
        Sequence seq = DOTween.Sequence();
        if (leftTarget != null && leftTarget.ObjTrans != null)
        {
            leftTarget.StopCatch(); 
            lineRenderer.enabled = false;
        }
  
        seq.Join(leftHand.DOMove(leftHandOriginalPos, moveDuration).SetEase(moveEase));
        seq.Join(leftHand.DORotateQuaternion(leftHandOriginalRot, moveDuration).SetEase(moveEase));
        seq.OnComplete(() =>
        {
            Debug.Log("完成收回" + playerEntity.isSelf);
            playerEntity.leftHandDoing = false;
            if (leftTarget != null&&leftTarget.ObjTrans!=null)
            {
                YOTOFramework.soundMgr.PlaySFX("Sound/Eat");
                leftTarget.StopCatch();  
                var food = leftTarget.ObjTrans.GetComponent<FoodBase>();
                StagePlugin.Instance.RemoveFood(food.foodId);
            }
            leftTarget = null;
            lineRenderer.enabled = false;
        });
    }


    /// <summary>
    /// 右手收回
    /// </summary>
    /// <summary>
    /// 右手收回
    /// </summary>
    public void RetractRightHand(bool releaseTarget = false)
    {
        if (rightHand == null) return;

        rightHand.DOKill(true);

        if (rightTarget != null && rightTarget.ObjTrans != null)
        {
            rightTarget.StopCatch();
        }

        Sequence seq = DOTween.Sequence();
        seq.Join(rightHand.DOMove(rightHandOriginalPos, moveDuration).SetEase(moveEase));
        seq.Join(rightHand.DORotateQuaternion(rightHandOriginalRot, moveDuration).SetEase(moveEase));
        seq.OnComplete(() =>
        {
            playerEntity.rightHandDoing = false;
            if (rightTarget != null&&rightTarget.ObjTrans!=null)
            {
                YOTOFramework.soundMgr.PlaySFX("Sound/Eat");
                rightTarget.StopCatch();
                var food = rightTarget.ObjTrans.GetComponent<FoodBase>();
                StagePlugin.Instance.RemoveFood(food.foodId);
          
            }
           
            rightTarget = null;
    
        });
    }

    public void OnUnLoad()
    {
        if (leftHand!=null)
        {
            leftHand.position = leftHandOriginalPos;
            leftHand.rotation = leftHandOriginalRot;
            leftHand.DOKill();
        }

        if (rightHand != null)
        {
            rightHand.position = rightHandOriginalPos;
            rightHand.rotation = rightHandOriginalRot;
            rightHand.DOKill();
        }  
    }
}