using UnityEngine;
using System.Collections;
using System;

public enum BezierControlPointMode
{
    Free, // 向き、長さは自由
    Aligned, // 向きは反対、長さは自由
    Mirrored //向きは反対、長さは一緒
}

public class BezierSpline : MonoBehaviour {

    [SerializeField]
    private Vector3[] points;



    [SerializeField]
    private BezierControlPointMode[] modes;

    public int ControlPointCount
    {
        get {
            return points.Length;
        }
    }

    public Vector3 GetControlPoint(int index)
    {
        return points[index];
    }

    public void SetControlPoint(int index, Vector3 point)
    {
		//model : 0, 3, 6...
		// modelを移動させたらその前後も一緒い動くようにする．
		if (index % 3 == 0) {
			//delta : 移動距離
			Vector3 delta = point - points [index];
			// ループ時
			if (loop) {
				if (index == 0) {// 先頭
					points [1] += delta;
					points [points.Length - 2] += delta;
					points [points.Length - 1] = point;
				} else if (index == points.Length - 1) { //末尾
					points [0] = point;
					points [1] += delta;
					points [points.Length - 2] += delta;
				} else {
					points [index - 1] += delta;
					points [index + 1] += delta;
				}
			} else {

				// move previous point
				if (index > 0) {
					points [index - 1] += delta;
				}
				// move next point
				if (index + 1 < points.Length) {
					points [index + 1] += delta;
				}
			}
		}
        points[index] = point;

        EnforceMode(index); // 線の方向の補正
    }

    public int CurveCount {
        get
        {
            return (points.Length - 1) / 3;
        }
    }

    [SerializeField]
    private bool loop;
    public bool Loop {
        get
        {
            return loop;
        }set
        {
            loop = value;
            if (value == true)
            {
                modes[modes.Length - 1] = modes[0];
                SetControlPoint(0, points[0]);
            }
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

        modes = new BezierControlPointMode[]
        {
            BezierControlPointMode.Free,
            BezierControlPointMode.Free
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

        // 配列の長さを更新
        Array.Resize(ref modes, modes.Length + 1);
        //一番後ろの値をコピー
        modes[modes.Length - 1] = modes[modes.Length - 2];
        // 元一番後ろの点の向きを補正
        EnforceMode(points.Length - 4);

		// loopなら先頭と同期
		if (loop) {
			points [points.Length - 1] = points [0];
			modes [modes.Length - 1] = modes [0];
			EnforceMode (0);
		}
    }


    // point index sequence 0,1,2,3,4,5...　
    // mode index sequence 0,0,1,1,1,2,2... 点と方向のグループ

    public BezierControlPointMode GetControlPointMode(int index)
    {
        return modes[(index + 1) / 3];
    }

    public void SetControlPointMode(int index, BezierControlPointMode mode)
    {
		// 真ん中の点 = model
		int modelIndex = (index + 1) / 3;
		modes[modelIndex] = mode;
		//ループなら，端っこのmodeを同期
		if (loop) {
			if (modelIndex == 0) {
				modes [modes.Length - 1] = mode;
			}else if(modelIndex == modes.Length - 1){
				modes [0] = mode;
			}
		}
        EnforceMode(index);
    }

    //Modeによる補正をかける
    void EnforceMode(int index)
    {
        int modeIndex = (index + 1) / 3;
        BezierControlPointMode mode = modes[modeIndex];
        //はじっこ or Freeならば
		if(mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Length - 1))
        {
            return;
        }
        // 点
        int middleIndex = modeIndex * 3;
        int fixedIndex, enforcedIndex;
        // index : クリック、移動した点の番号
        if (index <= middleIndex) // いじった点がleft or middleならば
        {
            fixedIndex = middleIndex - 1; // leftを元に
			if (fixedIndex < 0) {
				fixedIndex = points.Length - 2;
			}
            enforcedIndex = middleIndex + 1; // rightを補正する
			if (enforcedIndex >= points.Length) {
				enforcedIndex = 1;
			}
        }
        else
        {
            fixedIndex = middleIndex + 1;
			if (fixedIndex >= points.Length) {
				fixedIndex = 1;
			}
            enforcedIndex = middleIndex - 1;
			if (enforcedIndex < 0) {
				enforcedIndex = points.Length - 2;
			}
        }

        Vector3 middle = points[middleIndex];
        // fixedからmiddleへのベクトル
        Vector3 enforcedTangent = middle - points[fixedIndex];

        if (mode == BezierControlPointMode.Aligned)
        {
            enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
        }
        // fixedと反対方向にenforceを配置
        points[enforcedIndex] = middle + enforcedTangent;


    }
}
