using com.yah.LineRendererDemo;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SmoothBezierTest : MonoBehaviour
{
    public Transform startPoint;      // 起点
    public Transform endPoint;        // 终点// 控制点

    public int numberOfPoints = 25;
    public float controlFollowSpeed = 5f;
    public float curveHitOffset = 1f; // 控制点偏移距离

    private LineRenderer lineRenderer;
    private Bezier bezier;
    private Vector3 mid;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        bezier = new Bezier(numberOfPoints);
        lineRenderer.positionCount = numberOfPoints;

         mid = (startPoint.position + endPoint.position) / 2;

        // 初始曲线
        Vector3[] points = bezier.GetQuadraticCurvePoints(
            startPoint.position,mid, endPoint.position
        );
        lineRenderer.SetPositions(points);
    }

    void Update()
    {
        
       mid = (startPoint.position + endPoint.position) / 2;

        // 绘制二次贝塞尔曲线
        Vector3[] points = bezier.GetQuadraticCurvePoints(
            startPoint.position,
            mid+Vector3.left*0.2f,
            endPoint.position
        );
        lineRenderer.SetPositions(points);
    }
}