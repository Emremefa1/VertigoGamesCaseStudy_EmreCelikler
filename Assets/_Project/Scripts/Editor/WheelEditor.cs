#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using WheelGame.Core;

[CustomEditor(typeof(WheelBuilder))]
public class WheelBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var builder = (WheelBuilder)target;
        if (GUILayout.Button("Rebuild Slices"))
        {
            builder.SetupSlices(new System.Collections.Generic.List<WheelGame.Data.SliceDefinitionSO>());
        }

        if (builder.transform == null) return;

        // Basic UI checks
        if (builder.GetComponentInParent<RectTransform>() == null)
        {
            EditorGUILayout.HelpBox("WheelBuilder should be under a Canvas/RectTransform.", MessageType.Warning);
        }
    }
}
#endif
