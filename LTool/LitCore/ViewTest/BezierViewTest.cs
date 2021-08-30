using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 贝塞尔曲线测试类，用于演示
/// </summary>
public class BezierViewTest : MonoBehaviour
{
    public Transform[] controlPoints;
    public LineRenderer lineRenderer;

    public int layerOrder = 0;
    private int _segmentNum = 10;


    void Start()
    {
        if ( !lineRenderer )
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        lineRenderer.sortingLayerID = layerOrder;

        //DrawCurve();
    }

    void Update()
    {

        DrawCurve();

    }

    void DrawCurve()
    {
        for ( int i = 1; i <= _segmentNum; i++ )
        {
            float t = i / ( float )_segmentNum;
            int nodeIndex = 0;
            Vector3 pixel = LMath.BezierCurve( controlPoints[ nodeIndex ].position,
                controlPoints[ nodeIndex + 1 ].position, controlPoints[ nodeIndex + 2 ].position, t );
            lineRenderer.positionCount = i;
            lineRenderer.SetPosition( i - 1, pixel );
        }

    }
}
