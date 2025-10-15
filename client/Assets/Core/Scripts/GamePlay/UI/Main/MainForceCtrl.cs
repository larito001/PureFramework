using UnityEngine;
using DG.Tweening;

public class MainForceCtrl : MonoBehaviour
{
    [Header("四个角的 RectTransform")]
    public RectTransform leftTop;
    public RectTransform rightTop;
    public RectTransform leftBottom;
    public RectTransform rightBottom;

    [Header("动画参数")]
    public float moveDuration = 0.8f;
    public float delayBetween = 0.1f;

    [Header("聚焦偏移（相对于 this.transform）")]
    public Vector2 offset = new Vector2(100f, 100f);

    private Vector3 targetPos;

    void Start()
    {
        targetPos = transform.position;
    }

    public void StartForce()
    {
        Vector2 screen = new Vector2(Screen.width, Screen.height);

        // 屏幕外起始位置（保持交叉方向 + 推出屏幕外）
        Vector3 leftTopStart     = new Vector3(screen.x * 1.2f,  screen.y * 1.2f, 0);   // 从右上飞入
        Vector3 rightTopStart    = new Vector3(-screen.x * 0.2f, screen.y * 1.2f, 0);   // 从左上飞入
        Vector3 leftBottomStart  = new Vector3(screen.x * 1.2f, -screen.y * 0.2f, 0);   // 从右下飞入
        Vector3 rightBottomStart = new Vector3(-screen.x * 0.2f, -screen.y * 0.2f, 0);  // 从左下飞入


        // 设置起始位置
        leftTop.position = leftTopStart;
        rightTop.position = rightTopStart;
        leftBottom.position = leftBottomStart;
        rightBottom.position = rightBottomStart;

        // 计算目标位置（带偏移）
        Vector3 ltTarget = targetPos + new Vector3(offset.x, offset.y, 0);
        Vector3 rtTarget = targetPos + new Vector3(-offset.x, offset.y, 0);
        Vector3 lbTarget = targetPos + new Vector3(offset.x, -offset.y, 0);
        Vector3 rbTarget = targetPos + new Vector3(-offset.x, -offset.y, 0);

        // 使用 DOTween 动画飞入
        leftTop.DOMove(ltTarget, moveDuration).SetEase(Ease.Linear);
        rightTop.DOMove(rtTarget, moveDuration).SetEase(Ease.Linear);
        leftBottom.DOMove(lbTarget, moveDuration).SetEase(Ease.Linear);
        rightBottom.DOMove(rbTarget, moveDuration).SetEase(Ease.Linear);
    }
}