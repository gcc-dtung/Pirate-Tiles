# 📏 Convention Rules — Pirate Tiles

> Quy tắc đặt tên, coding convention, và quy ước Unity cho dự án Pirate Tiles.  
> Tất cả thành viên trong team **BẮT BUỘC** tuân thủ các quy tắc dưới đây.

---

## 1. Naming Convention — Quy Tắc Đặt Tên

### 1.1 C# Code

| Loại | Convention | Ví dụ |
|---|---|---|
| **Class / Struct** | PascalCase | `TileModel`, `StackController`, `GameConfigSO` |
| **Interface** | I + PascalCase | `IState`, `IPowerUp` |
| **Enum** | PascalCase | `TileType`, `GamePhase` |
| **Enum Value** | PascalCase | `TileType.Sword`, `GamePhase.Playing` |
| **Public Method** | PascalCase | `FindMatch()`, `UpdateSelectableStatus()` |
| **Private Method** | PascalCase | `HandleTileClick()`, `SyncViewWithModel()` |
| **Public Property** | PascalCase | `CurrentHearts`, `IsSelectable` |
| **Private Field** | _camelCase (underscore prefix) | `_tiles`, `_stackModel`, `_isProcessing` |
| **[SerializeField] private** | _camelCase (underscore prefix) | `_spriteRenderer`, `_timerText` |
| **Public Field** (tránh dùng) | camelCase | `maxHearts`, `healTime` |
| **Local Variable** | camelCase | `tileCount`, `matchIndex`, `targetPos` |
| **Parameter** | camelCase | `tileType`, `duration`, `onComplete` |
| **Constant** | PascalCase hoặc UPPER_SNAKE | `MaxStackSize`, `DEFAULT_HEAL_TIME` |
| **Event Channel SO** | PascalCase + `Channel` | `TileSelectedChannel`, `GameWonChannel` |
| **Boolean** | Is/Has/Can/Should prefix | `IsSelectable`, `HasHearts`, `CanInteract` |

### 1.2 Unity Assets & Files

| Loại | Convention | Ví dụ |
|---|---|---|
| **Scene** | PascalCase | `InGame.unity`, `StartScreen.unity` |
| **Prefab** | PascalCase | `Tile.prefab`, `WinPanel.prefab` |
| **ScriptableObject Asset** | PascalCase + Suffix | `Pirate_TileData.asset`, `Level01_Config.asset` |
| **Event Channel Asset** | PascalCase + `Channel` | `TileSelectedChannel.asset`, `GameWonChannel.asset` |
| **Sprite** | snake_case hoặc PascalCase | `tile_sword.png`, `btn_play.png` |
| **Audio Clip** | snake_case | `sfx_match.wav`, `bgm_pirate.mp3` |
| **Material** | PascalCase + `_Mat` | `Tile_Mat`, `Dissolve_Mat` |
| **Folder** | PascalCase | `Scripts/`, `Models/`, `EventChannels/` |

### 1.3 MVC + Event Channel Suffix Convention

| Layer | Suffix bắt buộc | Ví dụ |
|---|---|---|
| **Model** | `Model` | `TileModel`, `BoardModel`, `StackModel` |
| **View** | `View` | `TileView`, `BoardView`, `TimerView` |
| **Controller** | `Controller` | `BoardController`, `GameController` |
| **Service** | `Service` | `AudioService`, `SaveService` |
| **ScriptableObject** | `SO` | `TileDatabaseSO`, `GameConfigSO` |
| **Event Channel SO** | `ChannelSO` | `VoidEventChannelSO`, `TileSelectedChannelSO` |
| **Event Data** | `EventData` | `TileSelectedEventData`, `PowerUpUsedEventData` |

---

## 2. Coding Convention

### 2.1 File Structure — Thứ tự trong class

```csharp
public class ExampleController : MonoBehaviour {
    // 1. CONSTANTS
    private const int MaxRetries = 3;

    // 2. SERIALIZED FIELDS (Inspector)
    [Header("References")]
    [SerializeField] private ExampleView _view;
    [SerializeField] private ExampleSO _config;

    [Header("Event Channels")]
    [SerializeField] private VoidEventChannelSO _someChannel;

    // 3. PRIVATE FIELDS
    private ExampleModel _model;
    private bool _isInitialized;

    // 4. PUBLIC PROPERTIES
    public bool IsReady => _isInitialized;

    // 5. EVENTS (C# events cho View→Controller)
    public event Action<int> OnValueChanged;

    // 6. UNITY LIFECYCLE
    private void Awake() { }
    private void OnEnable() { /* Subscribe Event Channels */ }
    private void Start() { }
    private void Update() { }
    private void OnDisable() { /* Unsubscribe Event Channels */ }
    private void OnDestroy() { /* Kill DOTween */ }

    // 7. PUBLIC METHODS
    public void Initialize() { }

    // 8. PRIVATE METHODS
    private void HandleEvent() { }

    // 9. COROUTINES (nếu có)
    private IEnumerator WaitAndDo() { }
}
```

### 2.2 Braces Style — K&R

```csharp
// ✅ ĐÚNG — K&R style
public class TileModel {
    public void DoSomething() {
        if (condition) {
            // ...
        } else {
            // ...
        }
    }
}
```

### 2.3 Quy tắc chung

```
1. ✅ Mỗi file chỉ chứa 1 class/interface/enum
2. ✅ Mỗi class tối đa 300 dòng
3. ✅ Mỗi method tối đa 30 dòng
4. ✅ Dùng var khi kiểu rõ ràng
5. ✅ String interpolation: $"Level {levelIndex}"
6. ❌ KHÔNG magic numbers — dùng const/config
7. ❌ KHÔNG Debug.Log trong production — dùng #if UNITY_EDITOR
8. ❌ KHÔNG public fields — dùng property
9. ✅ Luôn khai báo access modifier rõ ràng
```

### 2.4 Import Order

```csharp
// 1. System namespaces
using System;
using System.Collections.Generic;

// 2. Unity namespaces
using UnityEngine;
using UnityEngine.UI;

// 3. Third-party
using DG.Tweening;
using TMPro;

// 4. Project namespaces
using PirateTiles.Models;
using PirateTiles.Events;
```

---

## 3. Unity Convention

### 3.1 MonoBehaviour Guidelines

```
1. ✅ Dùng [SerializeField] private thay vì public
2. ✅ Dùng [Header("Section")] để nhóm fields
3. ✅ Subscribe Event Channels trong OnEnable, Unsubscribe trong OnDisable
4. ✅ Kill DOTween trong OnDestroy
5. ❌ KHÔNG dùng Find/FindWithTag trong runtime
6. ❌ KHÔNG dùng Update nếu có thể — dùng event-driven
```

### 3.2 Scene Hierarchy

```
Scene Root
├── --- MANAGERS ---
│   ├── GameController
│   ├── BoardController
│   └── ...Controllers
├── --- ENVIRONMENT ---
│   ├── Camera
│   └── Background
├── --- GAMEPLAY ---
│   ├── BoardView → [Tiles]
│   └── StackView → [Slots]
├── --- UI ---
│   └── Canvas
│       ├── HUD
│       ├── WinPanel
│       ├── LosePanel
│       └── SettingPanel
└── --- DEBUG ---
```

---

## 4. Git Convention

### 4.1 Branch Naming

| Loại | Format | Ví dụ |
|---|---|---|
| **Feature** | `feature/tên-ngắn` | `feature/stack-system`, `feature/event-channels` |
| **Bugfix** | `fix/mô-tả` | `fix/tile-overlap`, `fix/channel-null` |
| **Refactor** | `refactor/mô-tả` | `refactor/board-model` |

### 4.2 Commit Message

```
Format: <type>(<scope>): <description>

Ví dụ:
  feat(board): implement overlap detection in BoardModel
  feat(events): create TileSelectedChannelSO
  fix(stack): fix smart insert index
  test(stack): add unit tests for StackModel.FindMatch
```

---

## 5. MVC-Specific Rules

### 5.1 Model Rules

```
✅ Pure C# class (không MonoBehaviour)
✅ Chứa data + business logic
✅ Unit testable
❌ KHÔNG reference UnityEngine.UI
❌ KHÔNG reference View hoặc Controller
❌ KHÔNG gọi DOTween, animation
```

### 5.2 View Rules

```
✅ MonoBehaviour, quản lý visual
✅ Expose public methods cho Controller gọi
✅ Handle animation, VFX, shader
❌ KHÔNG chứa business logic
❌ KHÔNG trực tiếp thay đổi Model
```

### 5.3 Controller Rules

```
✅ MonoBehaviour, kết nối Model ↔ View
✅ Subscribe/Raise Event Channel SO
✅ Quản lý lifecycle
❌ KHÔNG chứa rendering, animation
❌ KHÔNG chứa dữ liệu state (delegate cho Model)
❌ KHÔNG gọi trực tiếp Controller khác → dùng Event Channel
```

### 5.4 Event Channel Rules

```
✅ ScriptableObject asset
✅ Wire qua [SerializeField] trong Inspector
✅ Subscribe trong OnEnable, Unsubscribe trong OnDisable
✅ Dùng named method (không anonymous lambda)
❌ KHÔNG tạo Event Channel bằng code tại runtime
❌ KHÔNG dùng static reference đến Event Channel
```

---

## Tóm Tắt — Cheat Sheet

```
╔══════════════════════════════════════════════════════╗
║  NAMING                                              ║
║  Class/Method/Property → PascalCase                  ║
║  Private field         → _camelCase                  ║
║  Local/Param           → camelCase                   ║
║  Boolean               → Is/Has/Can prefix           ║
║  MVC Suffix            → Model/View/Controller       ║
║  Event Channel         → ChannelSO suffix            ║
╠══════════════════════════════════════════════════════╣
║  CODING                                              ║
║  Braces                → K&R (same line)             ║
║  Access modifier       → Always explicit             ║
║  Fields in Inspector   → [SerializeField] private    ║
║  Max class length      → 300 lines                   ║
║  Max method length     → 30 lines                    ║
╠══════════════════════════════════════════════════════╣
║  MVC + EVENT CHANNEL                                 ║
║  Model                 → Pure C#, no MonoBehaviour   ║
║  View                  → MonoBehaviour, visual only  ║
║  Controller            → MonoBehaviour, M↔V bridge   ║
║  Communication         → Event Channel SO            ║
║  Subscribe             → OnEnable                    ║
║  Unsubscribe           → OnDisable                   ║
╚══════════════════════════════════════════════════════╝
```
