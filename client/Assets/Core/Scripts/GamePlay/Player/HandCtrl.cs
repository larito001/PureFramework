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

    private Transform leftTarget;
    private Transform rightTarget;

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
    public void ExtendLeftHand(Transform target,bool success)
    {
        if (leftHand == null || target == null) return;

        leftTarget = target;

        leftHand.DOMove(target.position, moveDuration)
                .SetEase(moveEase)
                .OnComplete(() =>
                {
                    if (success)
                    {
                        target.SetParent(leftHand);
                        target.localPosition = Vector3.zero;
                        target.localRotation = Quaternion.identity;
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
    public void ExtendRightHand(Transform target)
    {
        if (rightHand == null || target == null) return;

        rightTarget = target;

        rightHand.DOMove(target.position, moveDuration)
                 .SetEase(moveEase)
                 .OnComplete(() =>
                 {
                     target.SetParent(rightHand);
                     target.localPosition = Vector3.zero;
                     target.localRotation = Quaternion.identity;
                 });
    }

    /// <summary>
    /// 左手收回
    /// </summary>
    public void RetractLeftHand(bool releaseTarget = false)
    {
        if (leftHand == null) return;

        leftHand.DOMove(leftHandOriginalPos, moveDuration).SetEase(moveEase);
        leftHand.DORotateQuaternion(leftHandOriginalRot, moveDuration).SetEase(moveEase);

        if (releaseTarget && leftTarget != null)
        {
            leftTarget.SetParent(null);
            leftTarget = null;
        }
    }

    /// <summary>
    /// 右手收回
    /// </summary>
    public void RetractRightHand(bool releaseTarget = false)
    {
        if (rightHand == null) return;

        rightHand.DOMove(rightHandOriginalPos, moveDuration).SetEase(moveEase);
        rightHand.DORotateQuaternion(rightHandOriginalRot, moveDuration).SetEase(moveEase);

        if (releaseTarget && rightTarget != null)
        {
            rightTarget.SetParent(null);
            rightTarget = null;
        }
    }
}
