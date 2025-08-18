using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using YOTO;
using Random = UnityEngine.Random;


public class FlyTextCtrl : ObjectBase, PoolItem<Transform>
{
    public static DataObjPool<FlyTextCtrl, Transform> pool = new DataObjPool<FlyTextCtrl, Transform>("FlyTextCtrl", 40);
    float speed;
    float time = 0;
    public TextMeshProUGUI tmp;
    private bool isInit = false;
    private Transform root;
    private FlyTextData data;
    private RectTransform rct;
    private bool isFly = false;
    private Vector3 currentPos;

    public void Fly(FlyTextData data)
    {
        currentPos = data.pos;
        if (!isInit)
        {
            this.data = data;
            isFly = true;
            return;
        }

        tmp = objTrans.GetComponent<TextMeshProUGUI>();
        tmp.text = data.text;


        if (data.flyTextType == FlyTextType.Normal)
        {
            tmp.color = Color.white;

            StartAnim(0.5f, 1.2f, 1.0f, 0.4f, 0.1f, 1, 1, Ease.OutQuad, Ease.InOutQuad);
        }
        else if (data.flyTextType == FlyTextType.Quick)
        {
            tmp.color = Color.red;
            StartAnim(0.3f, 1.5f, 1.5f, 0.3f, 0.1f, 0.6f, 0.2f, Ease.OutElastic, Ease.OutBack);
        }

        if (data.flyTextType == FlyTextType.PlayerHurt)
        {
            tmp.color = Color.yellow;

            StartAnim(0.5f, 1.2f, 1.0f, 0.4f, 0.1f, 1, 1, Ease.OutQuad, Ease.InOutQuad);
        }

        if (data.flyTextType == FlyTextType.WoodRes)
        {
            tmp.color = new Color(101/255.0f, 67/255.0f, 33/255.0f,1);
            StartAnim(0.5f, 1.2f, 1.0f, 0.4f, 0.1f, 1, 1, Ease.OutQuad, Ease.InOutQuad);  
        }
        if (data.flyTextType == FlyTextType.IronRes)
        {
            tmp.color =new Color(192/255.0f, 192/255.0f, 192/255.0f,1);
            StartAnim(0.5f, 1.2f, 1.0f, 0.4f, 0.1f, 1, 1, Ease.OutQuad, Ease.InOutQuad);
        }
    }

    protected override void YOTOOnload()
    {
    }

    public override void YOTOStart()
    {
    }


    public void StartAnim(float startScale, float MaxScale, float lastScale,
        float toMaxDuration, float toLastDuration,
        float upTime, float fadeTime,
        Ease upAnim, Ease downAnim)
    {
        rct.transform.position = currentPos;
        rct.localScale = Vector3.one * startScale;

        CanvasGroup cg = rct.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = rct.gameObject.AddComponent<CanvasGroup>();
        }

        cg.alpha = 1f;

        DOTween.Kill(rct);
        DOTween.Kill(cg);

        Sequence seq = DOTween.Sequence();

        // 🔥 生成随机爆发方向偏移
        Vector2 burstDir = Random.insideUnitCircle.normalized;
        burstDir.y = Mathf.Abs(burstDir.y); // 保证向上
        Vector3 burstOffset = new Vector3(burstDir.x, burstDir.y, 0) * Random.Range(40f, 80f);
        Vector3 burstTarget = currentPos + burstOffset;

        // 🔥 最终位置（在爆发基础上再往上漂）
        Vector3 finalTarget = burstTarget + new Vector3(0, Random.Range(40f, 80f), 0);

        float burstTime = toMaxDuration; // 第段时间同步于缩放最大阶段
        float floatTime = upTime - burstTime; // 后续慢漂浮时间

        // ✅ 同时做：初始爆发缩放 + 位移
        seq.Join(rct.DOScale(MaxScale, burstTime).SetEase(upAnim)); // 放大
        seq.Join(rct.DOMove(burstTarget, burstTime).SetEase(Ease.OutCubic)); // 爆发

        // ✅ 同时做：缩回 + 向上漂浮
        seq.Append(rct.DOScale(lastScale, toLastDuration).SetEase(downAnim)); // 回弹缩放
        seq.Join(rct.DOMove(finalTarget, floatTime).SetEase(Ease.OutSine)); // 上漂

        // ✅ 同步淡出
        seq.Join(cg.DOFade(0f, fadeTime).SetEase(Ease.InQuad));
        seq.OnComplete(AnimComplete);
    }

    private void AnimComplete()
    {
        RecoverObject();
        pool.RecoverItem(this);
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
        speed = 10;
        isInit = true;
        rct = objTrans.GetComponent<RectTransform>();
        rct.SetParent(root, false);
        rct.transform.position = new Vector3(9999, 9999, 9999);
        tmp = objTrans.GetComponent<TextMeshProUGUI>();
        tmp.text = data.text;
        if (isFly)
        {
            Fly(this.data);
            isFly = false;
        }
    }

    public void AfterIntoObjectPool()
    {
        isInit = false;
    }

    public void SetData(Transform serverData)
    {
        SetInVision(true);
        SetPrefabBundlePath("Assets/HotUpdate/prefabs/FlyTextPrefab.prefab");
        root = serverData;
    }
}