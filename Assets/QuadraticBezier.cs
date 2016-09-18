using UnityEngine;

[ExecuteInEditMode]
public class QuadraticBezier : MonoBehaviour
{
	public Transform targetObj;
	public Transform p1;
	public Transform p2;
	public Transform p3;
	public float t;
	public int gizmoDivNum = 32;

	void Update () {
		targetObj.position = QuadaticBezierCurve(p1.position, p2.position, p3.position, t);
		// targetObj.forward = Vector3.Normalize(QuadaticBezierCurve(p1.position, p2.position, p3.position, t + 0.001f) - QuadaticBezierCurve(p1.position, p2.position, p3.position, t));
	}

	Vector3 QuadaticBezierCurve(Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		Vector3 p12 = p1 * (1.0f - t) + p2 * t;
		Vector3 p23 = p2 * (1.0f - t) + p3 * t;
		Vector3 p = p12 * (1.0f - t) + p23 * t;
		return p;
	}

	// ---- 以下描画処理 ----
	void OnDrawGizmos()
	{
		// Sceneビュー上の線を描く処理
		float dt = 1.0f / gizmoDivNum;
		for (int i = 0; i < gizmoDivNum; ++i)
		{
			Gizmos.DrawLine(
				QuadaticBezierCurve(p1.position, p2.position, p3.position, i * dt),
				QuadaticBezierCurve(p1.position, p2.position, p3.position, (i + 1) * dt)
			);
		}
	}

	void OnValidate()
	{
		gizmoDivNum = Mathf.Max(gizmoDivNum, 1);
	}


}