using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(YOTOButton))]
public class YOTOButtonEditor : ButtonEditor
{
    SerializedProperty hoverScale;
    SerializedProperty clickScale;
    SerializedProperty duration;
    SerializedProperty easeType;

    protected override void OnEnable()
    {
        base.OnEnable();
        hoverScale = serializedObject.FindProperty("hoverScale");
        clickScale = serializedObject.FindProperty("clickScale");
        duration = serializedObject.FindProperty("duration");
        easeType = serializedObject.FindProperty("easeType");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // 先绘制 Button 自带的 Inspector

        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("YOTO Button Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(hoverScale);
        EditorGUILayout.PropertyField(clickScale);
        EditorGUILayout.PropertyField(duration);
        EditorGUILayout.PropertyField(easeType);

        serializedObject.ApplyModifiedProperties();
    }
}