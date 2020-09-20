namespace Messiah.Editor {
  using UnityEditor;
  using UnityEngine;

  [CustomEditor(typeof(HandsUICtrl))]
  public class HandsUICtrlEditor : Editor {
    static GUIContent curvatureLabel = new GUIContent("曲率");

    new HandsUICtrl target;
    SerializedProperty curvature;

    void OnEnable() {
      target = base.target as HandsUICtrl;
      curvature = serializedObject.FindProperty(nameof(target.curvature));
      UpdateTransData();
      UpdateArcData();
    }

    public override void OnInspectorGUI() {
      EditorGUI.BeginChangeCheck();
      EditorGUILayout.Slider(curvature, 0, 1 / transData.halfWidth, curvatureLabel);
      var curvatureChanged = EditorGUI.EndChangeCheck();
      if (serializedObject.hasModifiedProperties) {
        serializedObject.ApplyModifiedProperties();
        if (curvatureChanged) UpdateArcData();
      }
    }

    void OnSceneGUI() {
      var trans = target.transform as RectTransform;
      if (trans != transData.trans || trans.hasChanged)
        UpdateTransData();

      Handles.color = Color.cyan;
      if (curvature.floatValue == 0)
        Handles.DrawLine(
          transData.rect[0],
          transData.rect[3]);
      else
        Handles.DrawWireArc(
          arcData.center,
          -trans.forward,
          arcData.from,
          arcData.degree,
          arcData.radius);
    }

    (Vector3 center, Vector3 from, float degree, float radius) arcData =
    (Vector3.zero, Vector3.zero, 0, 0);
    void UpdateArcData() {
      var curvature = this.curvature.floatValue;
      if (curvature == 0) return;
      var trans = transData.trans;
      var rect = transData.rect;
      var radius = 1 / curvature;
      var centerPos = trans.position - trans.up * radius;
      var halfRadian = Mathf.Asin(transData.halfWidth / radius);
      var startPos = (rect[0] - trans.up * (radius - Mathf.Sqrt(radius * radius - transData.halfWidth * transData.halfWidth))) - centerPos;
      var degree = Mathf.Rad2Deg * halfRadian * 2;

      arcData.center = centerPos;
      arcData.from = startPos;
      arcData.degree = degree;
      arcData.radius = radius;
    }

    (RectTransform trans, Vector3[] rect, float halfWidth) transData =
    (null, new Vector3[4], 0);
    void UpdateTransData() {
      transData.trans = target.transform as RectTransform;
      transData.trans.GetWorldCorners(transData.rect);
      transData.halfWidth = (transData.rect[0] - transData.rect[3]).magnitude / 2;
      transData.trans.hasChanged = false;
    }
  }
}