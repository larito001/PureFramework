using com.yah.LineRendererDemo;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SmoothBezierTest : MonoBehaviour
{
    public Transform startPoint;      // 起点
    public Transform endPoint;        // 终点
    public Transform controlPoint;    // 控制点

    public int numberOfPoints = 25;
    public float controlFollowSpeed = 5f;
    public float curveHitOffset = 1f; // 控制点偏移距离

    private LineRenderer lineRenderer;
    private Bezier bezier;
    private Vector3 curvePointPosition;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        bezier = new Bezier(numberOfPoints);
        lineRenderer.positionCount = numberOfPoints;

        // 初始化控制点在起点和终点中间
        if (controlPoint != null && startPoint != null && endPoint != null)
            controlPoint.position = (startPoint.position + endPoint.position) / 2;

        // 初始曲线
        Vector3[] points = bezier.GetQuadraticCurvePoints(
            startPoint.position, controlPoint.position, endPoint.position
        );
        lineRenderer.SetPositions(points);
    }

    void Update()
    {
        if (startPoint == null || controlPoint == null || endPoint == null) return;

        // 计算“控制点目标位置”，不是终点，而是偏移点
        curvePointPosition = endPoint.position + Vector3.up * curveHitOffset;

        // 控制点平滑移动
        controlPoint.position = Vector3.Lerp(
            controlPoint.position,
            curvePointPosition,
            controlFollowSpeed * Time.deltaTime
        );

        // 绘制二次贝塞尔曲线
        Vector3[] points = bezier.GetQuadraticCurvePoints(
            startPoint.position,
            controlPoint.position,
            endPoint.position
        );
        lineRenderer.SetPositions(points);
    }
}