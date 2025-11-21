# Wheel of Fortune - Complete Setup Guide

## Files Created

### Data Layer (`Assets/_Project/Scripts/Data/`)
1. **SliceDefinitionSO.cs** - Individual reward slice configuration
2. **WheelPresetSO.cs** - Wheel preset with list of slices (Normal/Safe/Super)
3. **RewardItemDataSO.cs** - Reward item definition with icon, type, and rarity

### Core Game Logic (`Assets/_Project/Scripts/Core/`)
1. **EventBus.cs** - Static event dispatcher for game-wide events
2. **GameManager.cs** - Central game orchestrator (Singleton)
3. **ZoneController.cs** - Zone tracking and preset selection
4. **RewardManager.cs** - Reward tracking (temporary/banked)
5. **SaveSystem.cs** - Simple PlayerPrefs-based persistence
6. **WheelController.cs** - Wheel spin mechanics with DOTween
7. **WheelBuilder.cs** - Slice instantiation and configuration
8. **WheelSliceView.cs** - Individual slice visual representation (ENHANCED)
9. **WheelPointer.cs** - Pointer animation on wheel
10. **WheelSliceAnimationController.cs** - Manages all slice animations

### UI Layer (`Assets/_Project/Scripts/UI/`)
1. **UIController.cs** - Main UI wiring and event handling (UPDATED)
2. **RewardItemView.cs** - Single reward item display (icon + quantity)
3. **RewardsDisplayPanel.cs** - Rewards collection panel with EXIT button
4. **WheelSliceAnimator.cs** - Individual slice animation effects
5. **UISpinButton.cs** - Spin button handler
6. **UIWalkAwayButton.cs** - Walk away button handler

### Utilities (`Assets/_Project/Scripts/Utils/`)
1. **Extensions.cs** - Helper methods (angle calculations)
2. **TweenUtility.cs** - DOTween animation helpers

### Editor & Tests
1. **WheelEditor.cs** - Custom editor for WheelBuilder
2. **WheelLogicTests.cs** - NUnit test template

---

## Scene Setup Instructions

### Step 1: Create Canvas
1. Right-click Hierarchy → UI → Canvas - TextMeshPro
2. Select Canvas, change:
   - **Render Mode**: Screen Space - Expand
   - **Canvas Scaler → UI Scale Mode**: Scale With Screen Size
   - **Reference Resolution**: 1920 x 1080

### Step 2: Create Wheel
1. Right-click under Canvas → Create Empty → Rename to `Wheel`
2. Add **RectTransform** component, set size to `400 x 400`, centered

### Step 3: Create Slice Container
1. Create Empty child under Wheel → `SlicesContainer`
2. Add **RectTransform** component

### Step 4: Create Slice Prefab
1. Create folder `Assets/_Project/Prefabs/`
2. Right-click → Create Empty → Name `Slice_Prefab`
3. Add components:
   - **Image** (background, use your golden/colored material)
   - **RectTransform** (adjust size for 8-part wheel)
4. Add child objects with naming convention:
   ```
   Slice_Prefab (Image with WheelSliceView)
   ├── ui_image_slice_icon (Image component)
   ├── ui_text_slice_label (TextMeshPro)
   └── ui_text_slice_value_value (TextMeshPro)
   ```
5. Add **WheelSliceView** script to root Slice_Prefab
6. Assign references in inspector:
   - ui_image_slice_icon → the icon Image
   - ui_text_slice_value_value → the value text
   - ui_image_slice_bg → the background Image
   - ui_text_slice_label → the label text
7. Drag into Prefabs folder to make a prefab, delete original from scene

### Step 5: Add WheelBuilder
1. Select **Wheel** in hierarchy
2. Add **WheelBuilder** script component
3. Assign:
   - **slicesContainer**: Drag SlicesContainer from hierarchy
   - **slicePrefab**: Drag Slice_Prefab from Prefabs folder
   - **desiredSliceCount**: 8

### Step 6: Create Reward Item Prefab
1. Create Empty → `RewardItem_Prefab`
2. Add **Image** component for background
3. Add children:
   ```
   RewardItem_Prefab (Image)
   ├── ui_image_reward_icon (Image)
   ├── ui_text_reward_quantity_value (TextMeshPro)
   └── ui_text_reward_name (TextMeshPro)
   ```
4. Add **RewardItemView** script
5. Assign references, make it a prefab

### Step 7: Create Rewards Display Panel
1. Right-click under Canvas → Panel - Image → `RewardsDisplayPanel`
2. Add **RewardsDisplayPanel** script
3. Add children:
   ```
   RewardsDisplayPanel (Image)
   ├── Button_Exit (Button with Text)
   ├── Container_Rewards (GridLayoutGroup)
   └── Text_TotalValue (TextMeshPro)
   ```
4. Add **GridLayoutGroup** to Container_Rewards with:
   - **Child Force Expand**: Width & Height enabled
   - **Child Preferred Size**: Enabled
   - **Spacing**: 10, 10
5. Assign script references:
   - ui_button_exit → Button_Exit
   - rewardsContainer → Container_Rewards
   - ui_text_reward_total_value → Text_TotalValue
   - rewardItemPrefab → RewardItem_Prefab

### Step 8: Create UI Panels
1. Under Canvas, create Panels:
   - **Panel_Top** (displays Zone, Temporary Reward, Banked Reward)
     - Add TextMeshPro children:
       - ZoneLabel_value
       - RewardLabel_value
       - BankedLabel_value
   - **Buttons** (holds Spin and Walk Away buttons)
     - Button_Spin (Button with TextMeshPro text "SPIN")
     - Button_WalkAway (Button with TextMeshPro text "WALK AWAY")

### Step 9: Create GameManager
1. Create Empty in scene → `GameManager`
2. Add **GameManager** script
3. Create child GameObjects:
   - `ZoneController` → Add **ZoneController** script
   - `RewardManager` → Add **RewardManager** script
   - `WheelController` → Add **WheelController** script
   - `UIController` → Add **UIController** script
4. Assign references in GameManager inspector:
   - **zoneController**: Drag ZoneController
   - **rewardManager**: Drag RewardManager
   - **wheelController**: Drag WheelController
   - **uiController**: Drag UIController

### Step 10: Setup WheelController
1. Select **WheelController** in hierarchy
2. Assign:
   - **wheelRoot**: Drag Wheel RectTransform
   - **builder**: Drag WheelBuilder (from Wheel)
   - **zoneController**: Drag ZoneController
   - **spinDuration**: 3.0 (adjust as needed)
   - **minFullRotations**: 3

### Step 11: Setup ZoneController
1. Select **ZoneController** in hierarchy
2. Create wheel presets (see below)
3. Assign:
   - **normalPreset**: WheelPreset_Normal
   - **safePreset**: WheelPreset_Safe
   - **superPreset**: WheelPreset_Super

### Step 12: Setup UIController
1. Select **UIController** in hierarchy
2. Auto-find will work, or manually assign:
   - **spinButton**: Button_Spin
   - **walkButton**: Button_WalkAway
   - **zoneText_value**: ZoneLabel_value
   - **tempReward_value**: RewardLabel_value
   - **bankedReward_value**: BankedLabel_value

---

## ScriptableObject Setup

### Create Slice Rewards
1. Create folder: `Assets/_Project/Data/Slices/`
2. Right-click → Create → WheelGame/SliceDefinition
3. Create 8 slices for a Normal wheel:
   - **Slice_Gold_100** → RewardType: Gold, amount: 100
   - **Slice_Money_500** → RewardType: Money, amount: 500
   - **Slice_Money_1000** → RewardType: Money, amount: 1000
   - **Slice_Character** → RewardType: Character, amount: 1
   - **Slice_Weapon** → RewardType: Weapon, amount: 1
   - **Slice_Special** → RewardType: Special, amount: 1
   - **Slice_Bonus_2x** → RewardType: Special, amount: 200
   - **Slice_Bomb** → RewardType: Bomb, amount: 0

### Create Wheel Presets
1. Create folder: `Assets/_Project/Data/Presets/`

**WheelPreset_Normal:**
- Right-click → Create → WheelGame/WheelPreset
- WheelType: Normal
- Add 8 slices (include Bomb)
- Theme Color: White

**WheelPreset_Safe:**
- WheelType: Safe
- Add 8 slices (7 rewards + 1 extra bonus, NO BOMB)
- Theme Color: Silver (0.8, 0.8, 0.8)

**WheelPreset_Super:**
- WheelType: Super
- Add 8 slices (high-value rewards only)
- Theme Color: Gold (1, 0.84, 0)

---

## Testing

### Play Mode
1. Press Play in Unity
2. Click SPIN button
3. Watch wheel rotate
4. Check console for event logs
5. Verify rewards display
6. Test WALK AWAY (only available in safe/super zones)

### Unit Tests
1. Window → Testing → Test Runner
2. Go to EditMode tab
3. Run WheelLogicTests

### Debugging
- Check Console for validation errors
- Verify all script references are assigned
- Ensure TextMeshPro is imported (should be with Canvas)
- DOTween should be in your project (add via Package Manager if needed)

---

## Key Features Implemented

✅ **Wheel Mechanics**
- 8-slice wheel with DOTween rotation
- Deterministic slice landing
- Full rotation animation

✅ **Zone System**
- Zone tracking (1+)
- Safe zones every 5th (no bomb)
- Super zones every 30th (special rewards)
- Automatic preset switching

✅ **Reward Management**
- Temporary reward tracking
- Banked reward persistence (PlayerPrefs)
- Walk away to bank rewards
- Bomb resets temporary rewards

✅ **UI Integration**
- Auto-finding UI elements by naming convention
- EventBus-driven UI updates
- Rewards display panel with stacking
- Zone and reward displays

✅ **Animations**
- Slice highlight animations
- Landing feedback
- Bomb effect
- DOTween-based smooth rotations

---

## Next Steps

1. **Add your UI assets** (buttons, panels, backgrounds)
2. **Create reward item icons** (gold, money, weapons, characters)
3. **Tune animation timings** (spinDuration, easing curves)
4. **Add sound effects** (spin, landing, bomb)
5. **Add visual effects** (particle systems, screen shake)
6. **Build APK** for Android deployment

---

## Troubleshooting

**"Script component not found"**
- Ensure all files are in correct folders
- Wait for Unity to recompile (check bottom right)

**"References are null in inspector"**
- Click "Rebuild Slices" on WheelBuilder
- Manually drag references if auto-find doesn't work

**"Wheel not rotating"**
- Check wheelRoot is assigned
- Verify DOTween is installed
- Check spinDuration > 0

**"Rewards not showing"**
- Verify RewardsDisplayPanel script is added
- Check rewardItemPrefab is assigned
- Ensure EventBus events are firing (check Console)

---

**Status**: ✅ Core system ready for scene integration
