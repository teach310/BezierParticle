using UnityEngine;
using System.Collections;

public class BezierCurve : MonoBehaviour {

    public Vector3[] points;

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
    /*
    // 位置を取得
    public Vector3 GetPoint(float t)
    {
        return transform.TransformPoint(Bezier.GetPoint(points[0], points[1], points[2], t));
    }

    // 位置ベクトルを微分 -> 速度ベクトル
    public Vector3 GetVelocity(float t)
    {
        
        //一度TransformPointをかけることで、TransformのRotationとScaleの影響を受ける
        //そして、transform.positionを引くことで原点からの速度ベクトルに戻す
        return transform.TransformPoint(Bezier.GetFirstDerivative(points[0], points[1], points[2], t)) - transform.position;
    }

    public Vector3 GetDirection(float t)
    {
        return GetVelocity(t).normalized;
    }

    */

    //------  4点---
    // 位置を取得
    public Vector3 GetPoint(float t)
    {
        return transform.TransformPoint(Bezier.GetPoint(points[0], points[1], points[2],points[3], t));
    }

    // 位置ベクトルを微分 -> 速度ベクトル
    public Vector3 GetVelocity(float t)
    {

        //一度TransformPointをかけることで、TransformのRotationとScaleの影響を受ける
        //そして、transform.positionを引くことで原点からの速度ベクトルに戻す
        return transform.TransformPoint(Bezier.GetFirstDerivative(points[0], points[1], points[2],points[3], t)) - transform.position;
    }

    public Vector3 GetDirection(float t)
    {
        return GetVelocity(t).normalized;
    }
}
