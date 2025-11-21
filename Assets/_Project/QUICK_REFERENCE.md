# Quick Reference Card - Wheel of Fortune Game

## 🎯 Game Rules at a Glance

| Zone | Type | Content | Theme | Features |
|------|------|---------|-------|----------|
| 1-4, 6-9, 11-14, etc. | Normal | 7 rewards + 1 bomb | White | Risk/reward |
| 5, 10, 15, 20, 25, etc. | Safe | 8 rewards (no bomb) | Silver | Can walk away |
| 30, 60, 90, etc. | Super | 8 high rewards | Gold | Can walk away |

## 📊 Reward System

```
Temporary Rewards (Current Session)
    ↓ Spin wheel
    ├─ Hit reward slice → +amount
    ├─ Hit bomb → LOSE ALL, reset to zone 1
    └─ Click Walk Away → Bank all (Safe/Super zones)
        ↓
Banked Rewards (Persistent)
    ├─ Saved to PlayerPrefs
    └─ Survives game restart
```

## 🎮 Player Flow

```
1. Start at Zone 1
2. Spin wheel
3. Land on reward slice
   a. Temporary reward increases
   b. Zone advances by 1
4. Choose:
   a. Spin again (risk losing all)
   b. Walk away (only on Safe/Super zones)
5. Hit bomb
   a. Lose all temporary rewards
   b. Reset to Zone 1
6. Game ends when player walks away or hits bomb
```

## 📁 Key Files to Know

### Must Have Assigned
```
GameManager
├─ ZoneController
├─ RewardManager
├─ WheelController
│  ├─ wheelRoot (RectTransform)
│  ├─ builder (WheelBuilder)
│  └─ zoneController (reference)
└─ UIController
   ├─ spinButton
   ├─ walkButton
   ├─ rewardsDisplayPanel
   └─ text displays

WheelBuilder
├─ slicesContainer
├─ slicePrefab
└─ desiredSliceCount (8)

RewardsDisplayPanel
├─ ui_button_exit
├─ rewardsContainer
└─ rewardItemPrefab
```

## 🎨 UI Naming Examples

```
✅ Correct Names:
ui_button_spin
ui_button_walkaway
ui_text_zone_value
ui_text_reward_temporary_value
ui_image_slice_icon
ui_panel_rewards
ui_grid_rewards_container
ui_text_reward_quantity_value

❌ Wrong Names:
Spin_Button
RewardText
sliceImage
panel
```

## 📝 Script Responsibilities

| Class | Does What |
|-------|-----------|
| **GameManager** | Starts game, initializes systems |
| **ZoneController** | Tracks zone, picks preset |
| **RewardManager** | Tracks money, bank/temp |
| **WheelController** | Spins wheel, lands on slice |
| **WheelBuilder** | Creates slice GameObjects |
| **UIController** | Updates UI elements |
| **RewardsDisplayPanel** | Shows collected items |
| **EventBus** | Sends game events |

## 🔄 Important Events

```csharp
EventBus.OnSpinStarted          // Wheel starts spinning
EventBus.OnSpinCompleted        // Wheel stopped, result ready
EventBus.OnZoneChanged          // Zone number updated
EventBus.OnRewardChanged        // Temp or banked reward changed
EventBus.OnBombTriggered        // Bomb hit, lose rewards
EventBus.OnWalkAway             // Player chose to walk away
```

## 📊 Zone Calculation

```csharp
zone % 30 == 0 → SUPER zone (30, 60, 90...)
zone % 5 == 0 && zone % 30 != 0 → SAFE zone (5, 10, 15, 20, 25...)
else → NORMAL zone (all others)
```

## 🎲 Random Landing

```csharp
// Inside WheelController
seed = DateTime.Now.Millisecond ^ CurrentZone
random = new System.Random(seed)
targetSlice = random.Next(0, 8)  // 0-7
```

## ⏱️ Animation Timeline

```
0.0s  → Spin starts, wheel begins rotating
0.1s  → Slice animators highlight
2.9s  → Wheel slowing down
3.0s  → Wheel stops on target
0.2s  → Landing feedback (pulse animation)
1.5s  → Bomb effect (if needed)
```

## 📱 Canvas Setup

```
Canvas (Render Mode: Expand)
├─ Panel_Top
│  ├─ ZoneLabel_value (TextMeshPro)
│  ├─ RewardLabel_value (TextMeshPro)
│  └─ BankedLabel_value (TextMeshPro)
├─ Buttons
│  ├─ Button_Spin
│  └─ Button_WalkAway
├─ Wheel (RectTransform 400x400)
│  └─ SlicesContainer
│     └─ [8x Slice instances]
└─ RewardsDisplayPanel
   ├─ Button_Exit
   ├─ Container_Rewards (GridLayout)
   └─ Text_TotalValue (TextMeshPro)
```

## 🔧 Common Customizations

### Change Spin Speed
```
WheelController.spinDuration = 2.0f  // Faster
WheelController.spinDuration = 5.0f  // Slower
```

### Change Rotation Amount
```
WheelController.minFullRotations = 5  // More spins
WheelController.minFullRotations = 1  // Less spins
```

### Change Easing
```
WheelController.spinEase = Ease.OutBounce  // Bouncy
WheelController.spinEase = Ease.Linear      // Linear
WheelController.spinEase = Ease.InOutQuad   // Smooth
```

### Change Reward Display
```
RewardsDisplayPanel.maxDisplayItems = 20  // Show more items
RewardsDisplayPanel.stackIdenticalRewards = false  // Don't stack
```

## ✅ Pre-Launch Checklist

- [ ] All GameManager references assigned
- [ ] All UI elements properly named
- [ ] ScriptableObject presets created (Normal/Safe/Super)
- [ ] Slice prefab configured
- [ ] Reward item prefab configured
- [ ] RewardsDisplayPanel assigned rewardItemPrefab
- [ ] Canvas set to Expand mode
- [ ] TextMeshPro materials imported
- [ ] DOTween imported
- [ ] Play and test spin
- [ ] Test walk away on zone 5
- [ ] Test bomb effect
- [ ] Test zone advancement

## 📈 Progression Examples

```
Zone 1 (Normal)
  ├─ Spin: Land on 500 money
  └─ Temp: 500, Zone: 2

Zone 2 (Normal)
  ├─ Spin: Land on bomb
  └─ Temp: 0, Zone: 1, Banked: 500

Zone 1 (Restart)
  ├─ Spin: Land on 1000 money
  └─ Temp: 1000, Zone: 2

... (continue)

Zone 5 (SAFE - No bomb!)
  ├─ Spin: Land on 500 money
  ├─ Temp: 2000, Zone: 6
  └─ Option: Walk Away → Bank 2000
             or Spin Again → Continue risk
```

## 🎯 Score Calculation

```
Total Score = Banked Rewards + Temporary Rewards
Example:
  Banked: 5000
  Temporary: 2000
  Total: 7000
```

---

**Bookmark this for quick reference during development!**
