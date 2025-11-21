#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using WheelGame.Data;
using WheelGame.Core;

[CustomEditor(typeof(SliceDefinitionSO))]
public class SliceDefinitionSOEditor : Editor
{
    private const float PREVIEW_SIZE = 120f;
    private const float ICON_SIZE = 100f;
    private SerializedProperty rewardTypeProperty;
    private SerializedProperty amountProperty;
    private SerializedProperty iconProperty;
    private SerializedProperty rarityProperty;
    private SerializedProperty itemGoldConversionValueProperty;

    private void OnEnable()
    {
        rewardTypeProperty = serializedObject.FindProperty("rewardType");
        amountProperty = serializedObject.FindProperty("amount");
        iconProperty = serializedObject.FindProperty("icon");
        rarityProperty = serializedObject.FindProperty("rarity");
        itemGoldConversionValueProperty = serializedObject.FindProperty("itemGoldConversionValue");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var slice = (SliceDefinitionSO)target;

        // Draw preview card
        DrawSlicePreview(slice);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Slice Properties", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        // Draw properties
        EditorGUILayout.PropertyField(rewardTypeProperty, new GUIContent("Reward Type"));
        EditorGUILayout.PropertyField(iconProperty, new GUIContent("Icon"));
        EditorGUILayout.PropertyField(rarityProperty, new GUIContent("Rarity (0=Common, 1=Uncommon, 2=Rare, 3=Legendary)"));

        RewardType currentType = (RewardType)rewardTypeProperty.enumValueIndex;

        // Only show amount for stackable types
        if (currentType != RewardType.Item && currentType != RewardType.Bomb)
        {
            EditorGUILayout.PropertyField(amountProperty, new GUIContent("Amount"));
        }

        // Only show gold conversion value for Item type
        if (currentType == RewardType.Item)
        {
            EditorGUILayout.PropertyField(itemGoldConversionValueProperty, new GUIContent("Gold Conversion Value"));
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSlicePreview(SliceDefinitionSO slice)
    {
        EditorGUILayout.LabelField("Slice Preview", EditorStyles.boldLabel);
        
        var cardRect = EditorGUILayout.GetControlRect(GUILayout.Height(PREVIEW_SIZE + 40));
        
        // Card background
        GUI.Box(cardRect, GUIContent.none, EditorStyles.helpBox);

        // Color based on reward type or rarity
        Color cardColor = GetRewardColor(slice.rewardType, slice.rarity);
        EditorGUI.DrawRect(new Rect(cardRect.x + 2, cardRect.y + 2, cardRect.width - 4, 30), cardColor);

        // Icon preview
        if (slice.icon != null)
        {
            var iconRect = new Rect(cardRect.x + (cardRect.width - ICON_SIZE) / 2, 
                cardRect.y + 35, ICON_SIZE, ICON_SIZE);
            GUI.DrawTexture(iconRect, slice.icon.texture);
        }
        else
        {
            GUI.Label(new Rect(cardRect.x + 10, cardRect.y + 50, cardRect.width - 20, 70), 
                "No Icon", EditorStyles.centeredGreyMiniLabel);
        }

        // Amount at bottom right
        if (slice.rewardType != RewardType.Bomb && slice.amount > 0)
        {
            GUI.Label(new Rect(cardRect.xMax - 50, cardRect.yMax - 25, 45, 20), 
                $"x{slice.amount}", EditorStyles.boldLabel);
        }
    }

    private Color GetRewardColor(RewardType type, int rarity)
    {
        // Rarity colors take precedence
        return rarity switch
        {
            1 => new Color(0.2f, 0.8f, 0.2f, 0.3f),  // Green - Uncommon
            2 => new Color(0.2f, 0.5f, 1f, 0.3f),    // Blue - Rare
            3 => new Color(1f, 0.84f, 0f, 0.3f),     // Gold - Legendary
            _ => type switch
            {
                RewardType.Bomb => new Color(1f, 0.2f, 0.2f, 0.3f),      // Red
                RewardType.Gold => new Color(1f, 0.84f, 0f, 0.3f),       // Gold
                RewardType.Money => new Color(0.2f, 0.8f, 0.2f, 0.3f),   // Green
                RewardType.Chests => new Color(0.8f, 0.6f, 0.2f, 0.3f),  // Brown/Chest
                RewardType.Item => new Color(0.8f, 0.2f, 0.8f, 0.3f),    // Purple
                _ => new Color(0.5f, 0.5f, 0.5f, 0.3f)  // Gray
            }
        };
    }
}
#endif
