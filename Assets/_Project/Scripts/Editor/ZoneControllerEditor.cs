using UnityEngine;
using UnityEditor;
using WheelGame.Core;
using WheelGame.Data;

namespace WheelGame.Editor
{
    [CustomEditor(typeof(ZoneController))]
    public class ZoneControllerEditor : UnityEditor.Editor
    {
        private SerializedProperty zoneWheelsProperty;

        private void OnEnable()
        {
            zoneWheelsProperty = serializedObject.FindProperty("zoneWheels");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Zone Wheel Assignments", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Assign WheelPresetSO assets for each zone.\n" +
                "Zones 5, 10, 15... (multiples of 5) must use SafePreset.\n" +
                "Zones 30, 60, 90... (multiples of 30) must use SuperPreset.\n\n" +
                "Flexible: works with any number of zones (5, 25, 27, 60, etc.)", MessageType.Info);

            // Draw array size
            int newSize = EditorGUILayout.IntField("Number of Zones", zoneWheelsProperty.arraySize);
            if (newSize > 0 && newSize != zoneWheelsProperty.arraySize)
            {
                zoneWheelsProperty.arraySize = newSize;
            }

            EditorGUILayout.Space();

            if (zoneWheelsProperty.arraySize == 0)
            {
                EditorGUILayout.HelpBox("Set 'Number of Zones' to start adding wheels", MessageType.Warning);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            // Draw each zone with validation
            for (int zone = 1; zone <= zoneWheelsProperty.arraySize; zone++)
            {
                SerializedProperty wheelProperty = zoneWheelsProperty.GetArrayElementAtIndex(zone - 1);
                WheelPresetSO wheel = wheelProperty.objectReferenceValue as WheelPresetSO;

                // Determine required type for this zone
                bool isMultipleOf30 = zone % 30 == 0;
                bool isMultipleOf5 = zone % 5 == 0;
                WheelType? requiredType = null;
                string requirement = "";

                if (isMultipleOf30)
                {
                    requiredType = WheelType.Super;
                    requirement = " [Super Zone]";
                }
                else if (isMultipleOf5)
                {
                    requiredType = WheelType.Safe;
                    requirement = " [Safe Zone]";
                }

                // Highlight validation errors
                GUI.backgroundColor = wheel == null ? Color.yellow :
                                      requiredType.HasValue && wheel.wheelType != requiredType ? Color.red :
                                      Color.white;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Zone {zone}{requirement}", GUILayout.Width(150));
                EditorGUILayout.PropertyField(wheelProperty, GUIContent.none);
                EditorGUILayout.EndHorizontal();

                GUI.backgroundColor = Color.white;

                // Show validation error message
                if (wheel != null && requiredType.HasValue && wheel.wheelType != requiredType)
                {
                    EditorGUILayout.HelpBox(
                        $"Zone {zone} requires {requiredType} preset, but assigned {wheel.wheelType}",
                        MessageType.Error);
                }
                else if (wheel == null && isMultipleOf5)
                {
                    EditorGUILayout.HelpBox($"Zone {zone} is unassigned (requires {requiredType})", MessageType.Warning);
                }
                else if (wheel == null)
                {
                    EditorGUILayout.HelpBox($"Zone {zone} is unassigned", MessageType.Warning);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
