using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HandCtrl : MonoBehaviour
{
    [SerializeField] private float moveDuration = 0.5f; // 动作时间，可在 Inspector 设置
    [SerializeField] private Ease moveEase = Ease.OutBack; // 缓动类型

    private PlayerEntity playerEntity;
    private Transform leftHand;
    private Transform rightHand;

    private Vector3 leftHandOriginalPos;
    private Quaternion leftHandOriginalRot;
    private Vector3 rightHandOriginalPos;
    private Quaternion rightHandOriginalRot;

    private FoodEntity leftTarget;
    private FoodEntity rightTarget;
    public void Init(PlayerEntity playerEntity)
    {
      
   
        this.playerEntity = playerEntity;
        leftHand = this.transform.Find("LeftHand_L");
        rightHand = this.transform.Find("RightHand_R");

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
    }

    /// <summary>
    /// 左手伸出去抓目标
    /// </summary>
    public void ExtendLeftHand(FoodEntity target,bool success)
    {
        if (leftHand == null || target == null) return;
        PlayerPlugin.Instance.leftHandDoing = true;
        leftTarget = target;
        
        leftHand.DOMove(leftTarget.ObjTrans.position, moveDuration)
                .SetEase(moveEase)
                .OnComplete(() =>
                {
                    if (success)
                    {
                        leftTarget.OnCatch();
                        leftTarget.ObjTrans.SetParent(leftHand);
                    }
                    else
                    {
                        RetractLeftHand();
                    }
           
                });
    }

    /// <summary>
    /// 右手伸出去抓目标
    /// </summary>
    public void ExtendRightHand(FoodEntity target)
    {
        if (rightHand == null || target == null) return;
        PlayerPlugin.Instance.rightHandDoing = true;
        rightTarget = target;
        rightHand.DOMove(rightTarget.ObjTrans.position, moveDuration)
                 .SetEase(moveEase)
                 .OnComplete(() =>
                 {
                     rightTarget.OnCatch();
                     rightTarget.ObjTrans.SetParent(rightHand);

                 });
    }

    /// <summary>
    /// 左手收回
    /// </summary>
    /// 左手收回
    /// </summary>
    public void RetractLeftHand()
    {
        if (leftHand == null) return;
        leftHand.DOMove(leftHandOriginalPos, moveDuration).SetEase(moveEase);
        leftHand.DORotateQuaternion(leftHandOriginalRot, moveDuration).SetEase(moveEase)
            .OnComplete(() => {
                if (leftTarget != null)
                {
                    // 销毁物体
                    leftTarget.StopCatch();
                    var food =leftTarget.ObjTrans.GetComponent<FoodBase>();
                    StagePlugin.Instance.RemoveFood(food.foodId);
                }
                PlayerPlugin.Instance.leftHandDoing=false;
     
            });
    }

    /// <summary>
    /// 右手收回
    /// </summary>
    public void RetractRightHand(bool releaseTarget = false)
    {
        if (rightHand == null) return;

        rightHand.DOMove(rightHandOriginalPos, moveDuration).SetEase(moveEase);
        rightHand.DORotateQuaternion(rightHandOriginalRot, moveDuration).SetEase(moveEase).OnComplete(() => {
            if (rightTarget != null)
            {
                // 销毁物体
                rightTarget.StopCatch();
                var food =rightTarget.ObjTrans.GetComponent<FoodBase>();
                StagePlugin.Instance.RemoveFood(food.foodId);
            }
            PlayerPlugin.Instance.rightHandDoing=false;
     
        });
        
    }
}
