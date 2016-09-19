using UnityEngine;
using System.Collections;
using System;

public class BezierSpline : MonoBehaviour {

    public Vector3[] points;

    public int CurveCount {
        get
        {
            return (points.Length - 1) / 3;
        }
    }

    // Reset Func  called when the component is created or reset
    public void Reset()
    {
        points = new Vector3[] {
            new Vector3(1f, 0, 0),
            new Vector3(2f, 0, 0),
            new Vector3(3f, 0, 0),
            new Vector3(4f, 0, 0)
        };
    }
    

    //------  4点---
    // 位置を取得
    public Vector3 GetPoint(float t)
    {
        int i;
        if (t >= 1.0f) // スプラインの先端
        {
            t = 1f;
            i = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount; // 1: 0~1,  2: 1 ~ 2
            i = (int)t;
            t -= i; // tの少数点部分を取得
            i *= 3; // 1: 0 , 2: 3, ...
        }
        return transform.TransformPoint(Bezier.GetPoint(points[i], points[i+1], points[i+2], points[i+3], t));
    }

    // 位置ベクトルを微分 -> 速度ベクトル
    public Vector3 GetVelocity(float t)
    {
        int i;
        if (t >= 1.0f)
        {
            t = 1f;
            i = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        //一度TransformPointをかけることで、TransformのRotationとScaleの影響を受ける
        //そして、transform.positionを引くことで原点からの速度ベクトルに戻す
        return transform.TransformPoint(Bezier.GetFirstDerivative(points[i], points[i+1], points[i+2], points[i+3], t)) - transform.position;
    }

    public Vector3 GetDirection(float t)
    {
        return GetVelocity(t).normalized;
    }

    public void AddCurve()
    {
        Vector3 point = points[points.Length - 1];
        Array.Resize(ref points, points.Length + 3);
        point.x += 1f;
        points[points.Length - 3] = point;
        point.x += 1f;
        points[points.Length - 2] = point;
        point.x += 1f;
        points[points.Length - 1] = point;
        
    }
}
