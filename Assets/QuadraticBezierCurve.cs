using UnityEngine;
using System.Collections;

class QuadraticBezierCurve{

	public Vector3 point1;
	public Vector3 point2;
	public Vector3 point3;

	public QuadraticBezierCurve(Vector3 p1, Vector3 p2, Vector3 p3){
		point1 = p1;
		point2 = p2;
		point3 = p3;
	}

	public Vector3 GetPosition(float t){
		Vector3 p12 = point1 * (1.0f - t) + point2 * t;
		Vector3 p23 = point2 * (1.0f - t) + point3 * t;
		Vector3 p = p12 * (1.0f - t) + p23 * t;
		return p;
	}
}
