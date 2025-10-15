using System;
using UnityEngine;
using DG.Tweening;
using YOTO;

public class MainForceCtrl : MonoBehaviour
{
    [Header("四个角的 RectTransform")]
    public RectTransform leftTop;
    public RectTransform rightTop;
    public RectTransform leftBottom;
    public RectTransform rightBottom;

    [Header("动画参数")]
    public float moveDuration = 1f;
    public float delayBetween = 0.1f;

    [Header("聚焦偏移（相对于 this.transform）")]
    public Vector2 offset = new Vector2(100f, 100f);

    private Vector2 screenPadding = new Vector2(30f, 30f);
    private Vector3 targetPos;
    private RectTransform rectTransform;
    private FoodEntity _focusFood;
    private Camera cam;
    private Vector3 outScreen = Vector3.one * 99999;
    
    // 新增：存储四个角的目标位置
    private Vector2 ltTarget;
    private Vector2 rtTarget;
    private Vector2 lbTarget;
    private Vector2 rbTarget;
    
    // 新增：动画引用
    // private Tweener ltTweener;
    // private Tweener rtTweener;
    // private Tweener lbTweener;
    // private Tweener rbTweener;

    void Start()
    {
        if (cam == null)
        {
            cam = YOTOFramework.cameraMgr.getMainCamera();
        }

        targetPos = transform.position;
        rectTransform = GetComponent<RectTransform>();
    }

    public void StartForce()
    {
        Vector2 screen = new Vector2(Screen.width, Screen.height);

        // 屏幕外起始位置
        Vector2 leftTopStart = new Vector2(screen.x * 1.2f, screen.y * 1.2f);
        Vector2 rightTopStart = new Vector2(-screen.x * 0.2f, screen.y * 1.2f);
        Vector2 leftBottomStart = new Vector2(screen.x * 1.2f, -screen.y * 0.2f);
        Vector2 rightBottomStart = new Vector2(-screen.x * 0.2f, -screen.y * 0.2f);

        // 设置起始位置
        leftTop.position = leftTopStart;    
        rightTop.position = rightTopStart;
        leftBottom.position = leftBottomStart;
        rightBottom.position = rightBottomStart;

        // 计算目标位置（基于当前主物体位置）
        UpdateCornerTargets();
        
    }

    // 新增：更新四个角的目标位置
    private void UpdateCornerTargets()
    {
        ltTarget = rectTransform.position + new Vector3(offset.x, offset.y,0);
        rtTarget = rectTransform.position + new Vector3(-offset.x, offset.y,0);
        lbTarget = rectTransform.position + new Vector3(offset.x, -offset.y,0);
        rbTarget = rectTransform.position + new Vector3(-offset.x, -offset.y,0);
    }

    // 新增：实时更新四个角的位置
    private void UpdateCornersPosition()
    {
        leftTop.position =Vector3.Lerp(leftTop.position, ltTarget, Time.fixedDeltaTime*10);
        rightTop.position = Vector3.Lerp(rightTop.position, rtTarget, Time.fixedDeltaTime*10);
        leftBottom.position = Vector3.Lerp(leftBottom.position, lbTarget, Time.fixedDeltaTime*10);
        rightBottom.position = Vector3.Lerp(rightBottom.position, rbTarget, Time.fixedDeltaTime*10);
    }

    private void FixedUpdate()
    {
       
        if (_focusFood != null && _focusFood.ObjTrans != null)
        {
         
            Vector3 screenPos = cam.WorldToScreenPoint(_focusFood.ObjTrans.position);
        
            if (screenPos.z > 0)
            {
                Vector2 uiSize = rectTransform.rect.size * rectTransform.lossyScale;
                float halfWidth = uiSize.x / 2f;
                float halfHeight = uiSize.y / 2f;

                float minX = halfWidth + screenPadding.x;
                float maxX = Screen.width - halfWidth - screenPadding.x;
                float minY = halfHeight + screenPadding.y;
                float maxY = Screen.height - halfHeight - screenPadding.y;

                screenPos.x = Mathf.Clamp(screenPos.x, minX, maxX);
                screenPos.y = Mathf.Clamp(screenPos.y, minY, maxY);

                rectTransform.position = screenPos;
                UpdateCornerTargets();
                // 新增：每次主物体位置更新后，更新四个角的目标位置
                UpdateCornersPosition();
            }
            else
            {
                rectTransform.position = outScreen;
                UpdateCornersPosition(); // 即使目标在相机后方也要更新
            }
        }
        else
        {
            rectTransform.position = outScreen;
            UpdateCornerTargets();
            UpdateCornersPosition(); 
      
        }
    }

    public void AddForceFood(int foodId)
    {
        var food = StagePlugin.Instance.GetFoodEntityById(foodId);
        
        if (food.ObjTrans != null)
        {
            _focusFood = food;
            StartForce();
        }
    }

    public void RemoveForce()
    {
        _focusFood = null;
        
    }
}