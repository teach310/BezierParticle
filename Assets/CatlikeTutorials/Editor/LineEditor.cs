using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Line))]
public class LineEditor :  Editor{

    void OnSceneGUI()
    {
        Line line = target as Line; // Lineクラス取得
        Transform handleTransform = line.transform; //Transform取得
        //handleのRotationをpivotかlocalかで切り替え
        Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ? 
            handleTransform.rotation : Quaternion.identity;
        //p0とp1をhandleTransformをもとにワールド座標に変換
        Vector3 p0 = handleTransform.TransformPoint(line.p0);
        Vector3 p1 = handleTransform.TransformPoint(line.p1);

        Handles.color = Color.white; //lineの色
        Handles.DrawLine(p0, p1); //lineの描画

        EditorGUI.BeginChangeCheck();
        p0 = Handles.DoPositionHandle(p0, handleRotation); //Handleの表示
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(line, "Move Point");
            EditorUtility.SetDirty(line);
            // Handleの位置をローカル座標に変換
            line.p0 = handleTransform.InverseTransformPoint(p0); 
        }
        EditorGUI.BeginChangeCheck();
        p1 = Handles.DoPositionHandle(p1, handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(line, "Move Point");
            EditorUtility.SetDirty(line);
            line.p1 = handleTransform.InverseTransformPoint(p1);
        }
    }
}
