#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using WheelGame.Data;
using System.Collections.Generic;

[CustomEditor(typeof(WheelPresetSO))]
public class WheelPresetSOEditor : Editor
{
    private const float SLICE_PREVIEW_SIZE = 100f;
    private const float ICON_SIZE = 60f;
    private const int COLUMNS = 4;

    private SerializedProperty wheelTypeProperty;
    private SerializedProperty slicesProperty;
    private SerializedProperty themeColorProperty;
    private SerializedProperty wheelImageProperty;
    private SerializedProperty indicatorImageProperty;

    private void OnEnable()
    {
        wheelTypeProperty = serializedObject.FindProperty("wheelType");
        slicesProperty = serializedObject.FindProperty("slices");
        themeColorProperty = serializedObject.FindProperty("themeColor");
        wheelImageProperty = serializedObject.FindProperty("wheelImage");
        indicatorImageProperty = serializedObject.FindProperty("indicatorImage");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var preset = (WheelPresetSO)target;

        // Header with preset info
        DrawPresetHeader(preset);

        EditorGUILayout.Space(15);

        // Wheel type and theme color
        EditorGUILayout.LabelField("Preset Configuration", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(wheelTypeProperty, new GUIContent("Wheel Type"));
        EditorGUILayout.PropertyField(themeColorProperty, new GUIContent("Theme Color"));

        EditorGUILayout.Space(10);

        // Visual assets
        EditorGUILayout.LabelField("Visual Assets", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(wheelImageProperty, new GUIContent("Wheel Image"));
        EditorGUILayout.PropertyField(indicatorImageProperty, new GUIContent("Indicator Image"));

        EditorGUILayout.Space(15);

        // Slices count info
        EditorGUILayout.LabelField("Wheel Slices", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            $"Total Slices: {slicesProperty.arraySize}\n" +
            $"Type: {preset.wheelType}\n" +
            $"Bombs: {CountBombs(preset)}\n" +
            $"Rewards: {slicesProperty.arraySize - CountBombs(preset)}",
            MessageType.Info);

        EditorGUILayout.Space(10);

        // Visual grid preview
        DrawSliceGrid(preset);

        EditorGUILayout.Space(15);

        // Slices list editor
        EditorGUILayout.LabelField("Edit Slices", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(slicesProperty, new GUIContent("Slices"), true);

        serializedObject.ApplyModifiedProperties();

        // Validation
        ValidatePreset(preset);
    }

    private void DrawPresetHeader(WheelPresetSO preset)
    {
        // Header box with theme color
        var headerRect = EditorGUILayout.GetControlRect(GUILayout.Height(50));
        
        // Background with theme color
        EditorGUI.DrawRect(headerRect, preset.themeColor * 0.7f);
        
        // Border
        EditorGUI.DrawRect(new Rect(headerRect.x, headerRect.y, headerRect.width, 2), preset.themeColor);
        
        // Text
        GUI.Label(new Rect(headerRect.x + 10, headerRect.y + 5, headerRect.width - 20, 40),
            $"{preset.wheelType} Wheel Preset\n{AssetDatabase.GetAssetPath(preset).Split('/')[^1]}",
            new GUIStyle(EditorStyles.boldLabel) { fontSize = 14, alignment = TextAnchor.MiddleLeft });
    }

    private void DrawSliceGrid(WheelPresetSO preset)
    {
        EditorGUILayout.LabelField("Slice Preview Grid", EditorStyles.boldLabel);

        if (slicesProperty.arraySize == 0)
        {
            EditorGUILayout.HelpBox("No slices configured. Add slices to see preview.", MessageType.Warning);
            return;
        }

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        int sliceIndex = 0;
        int slicesInRow = 0;

        EditorGUILayout.BeginHorizontal();

        for (int i = 0; i < slicesProperty.arraySize; i++)
        {
            var sliceProperty = slicesProperty.GetArrayElementAtIndex(i);
            var sliceSO = sliceProperty.objectReferenceValue as SliceDefinitionSO;

            // Draw slice preview
            DrawSlicePreview(sliceSO, i);

            slicesInRow++;

            if (slicesInRow >= COLUMNS)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                slicesInRow = 0;
            }

            sliceIndex++;
        }

        // Fill remaining space if needed
        for (int i = slicesInRow; i < COLUMNS && slicesInRow > 0; i++)
        {
            GUILayout.Space(SLICE_PREVIEW_SIZE + 10);
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void DrawSlicePreview(SliceDefinitionSO slice, int index)
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(SLICE_PREVIEW_SIZE + 10), GUILayout.Height(SLICE_PREVIEW_SIZE + 50));

        // Slice card
        var cardRect = EditorGUILayout.GetControlRect(GUILayout.Width(SLICE_PREVIEW_SIZE), GUILayout.Height(SLICE_PREVIEW_SIZE + 30));

        if (slice != null)
        {
            // Card background with color
            Color sliceColor = GetSliceColor(slice);
            EditorGUI.DrawRect(cardRect, sliceColor);

            // Border
            EditorGUI.DrawRect(new Rect(cardRect.x, cardRect.y, cardRect.width, 2), Color.black);

            // Index badge
            GUI.Label(new Rect(cardRect.x + 2, cardRect.y + 2, 20, 15), $"#{index}", EditorStyles.miniLabel);

            // Icon
            if (slice.icon != null)
            {
                var iconRect = new Rect(
                    cardRect.x + (cardRect.width - ICON_SIZE) / 2,
                    cardRect.y + 15,
                    ICON_SIZE,
                    ICON_SIZE);
                GUI.DrawTexture(iconRect, slice.icon.texture);
            }

            // Amount at bottom
            if (slice.rewardType != WheelGame.Core.RewardType.Bomb)
            {
                GUI.Label(new Rect(cardRect.xMax - 25, cardRect.yMax - 12, 23, 10),
                    $"x{slice.amount}",
                    new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleRight });
            }
            else
            {
                GUI.Label(new Rect(cardRect.xMax - 25, cardRect.yMax - 12, 23, 10),
                    "💣",
                    new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleRight });
            }
        }
        else
        {
            // Empty slot
            EditorGUI.DrawRect(cardRect, new Color(0.2f, 0.2f, 0.2f, 0.3f));
            GUI.Label(cardRect, "Empty", new GUIStyle(EditorStyles.centeredGreyMiniLabel));
        }

        EditorGUILayout.EndVertical();
    }

    private Color GetSliceColor(SliceDefinitionSO slice)
    {
        // Rarity colors take precedence
        if (slice.rarity > 0)
        {
            return slice.rarity switch
            {
                1 => new Color(0.2f, 0.8f, 0.2f, 0.6f),  // Green - Uncommon
                2 => new Color(0.2f, 0.5f, 1f, 0.6f),    // Blue - Rare
                3 => new Color(1f, 0.84f, 0f, 0.6f),     // Gold - Legendary
                _ => new Color(0.5f, 0.5f, 0.5f, 0.6f)
            };
        }

        // Type colors
        return slice.rewardType switch
        {
            WheelGame.Core.RewardType.Bomb => new Color(1f, 0.2f, 0.2f, 0.6f),           // Red
            WheelGame.Core.RewardType.Gold => new Color(1f, 0.84f, 0f, 0.6f),            // Gold
            WheelGame.Core.RewardType.Money => new Color(0.2f, 0.8f, 0.2f, 0.6f),        // Green
            WheelGame.Core.RewardType.Chests => new Color(0.8f, 0.6f, 0.2f, 0.6f),       // Brown/Chest
            WheelGame.Core.RewardType.Item => new Color(0.8f, 0.2f, 0.8f, 0.6f),         // Purple
            _ => new Color(0.5f, 0.5f, 0.5f, 0.6f)  // Gray
        };
    }

    private int CountBombs(WheelPresetSO preset)
    {
        int bombCount = 0;
        for (int i = 0; i < preset.slices.Count; i++)
        {
            if (preset.slices[i] != null && preset.slices[i].rewardType == WheelGame.Core.RewardType.Bomb)
            {
                bombCount++;
            }
        }
        return bombCount;
    }

    private void ValidatePreset(WheelPresetSO preset)
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Validation", EditorStyles.boldLabel);

        // Check for empty slices
        bool hasEmptySlices = false;
        for (int i = 0; i < preset.slices.Count; i++)
        {
            if (preset.slices[i] == null)
            {
                hasEmptySlices = true;
                break;
            }
        }

        if (hasEmptySlices)
        {
            EditorGUILayout.HelpBox("⚠️ Some slices are empty. Fill all slots.", MessageType.Warning);
        }

        // Safe zone check (should have no bombs)
        if (preset.wheelType == WheelType.Safe)
        {
            if (CountBombs(preset) > 0)
            {
                EditorGUILayout.HelpBox("⚠️ Safe zone shouldn't have bombs!", MessageType.Error);
            }
            else
            {
                EditorGUILayout.HelpBox("✓ Safe zone correctly configured (no bombs)", MessageType.Info);
            }
        }

        // Normal zone check (should have 1 bomb)
        if (preset.wheelType == WheelType.Normal)
        {
            int bombCount = CountBombs(preset);
            if (bombCount != 1)
            {
                EditorGUILayout.HelpBox($"⚠️ Normal zone should have exactly 1 bomb (currently: {bombCount})", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox("✓ Normal zone correctly configured (1 bomb)", MessageType.Info);
            }
        }

        // Super zone check (should have no bombs, all rewards)
        if (preset.wheelType == WheelType.Super)
        {
            if (CountBombs(preset) > 0)
            {
                EditorGUILayout.HelpBox("⚠️ Super zone shouldn't have bombs!", MessageType.Error);
            }
            else
            {
                EditorGUILayout.HelpBox("✓ Super zone correctly configured (all rewards)", MessageType.Info);
            }
        }
    }
}
#endif
