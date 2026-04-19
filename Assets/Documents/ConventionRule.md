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
| **Enum** | PascalCase | `CardType`, `GamePhase` |
| **Enum Value** | PascalCase | `CardType.Sword`, `GamePhase.Playing` |
| **Public Method** | PascalCase | `FindMatch()`, `UpdateSelectableStatus()` |
| **Private Method** | PascalCase | `HandleTileClick()`, `SyncViewWithModel()` |
| **Public Property** | PascalCase | `CurrentHearts`, `IsSelectable` |
| **Private Field** | _camelCase | `_tiles`, `_stackModel`, `_isProcessing` |
| **[SerializeField] private** | _camelCase | `_spriteRenderer`, `_timerText` |
| **Local Variable** | camelCase | `tileCount`, `matchIndex`, `targetPos` |
| **Parameter** | camelCase | `tileType`, `duration`, `onComplete` |
| **Constant** | PascalCase hoặc UPPER_SNAKE | `MaxStackSize`, `DEFAULT_HEAL_TIME` |
| **Boolean** | Is/Has/Can/Should prefix | `IsSelectable`, `HasHearts`, `CanInteract` |

### 1.2 Event System Naming

| Loại | Convention | Ví dụ |
|---|---|---|
| **EventBus Event Struct** | PascalCase + `Event` | `GamePhaseChangedEvent`, `TileStateChangedEvent` |
| **Event Channel SO Class** | PascalCase + `ChannelSO` | `VoidEventChannelSO`, `TileSelectedChannelSO` |
| **Event Channel SO Asset** | PascalCase + `Channel` | `TileSelectedChannel.asset`, `GameWonChannel.asset` |
| **Event Data Struct** | PascalCase + `EventData` | `TileSelectedEventData`, `PowerUpUsedEventData` |
| **EventBinding Field** | _camelCase + `Binding` | `_phaseBinding`, `_tileStateBinding` |

### 1.3 Unity Assets & Files

| Loại | Convention | Ví dụ |
|---|---|---|
| **Scene** | PascalCase | `InGame.unity`, `StartScreen.unity` |
| **Prefab** | PascalCase | `Tile.prefab`, `WinPanel.prefab` |
| **ScriptableObject Asset** | PascalCase + Suffix | `Pirate_TileData.asset`, `Level01_Config.asset` |
| **Sprite** | snake_case hoặc PascalCase | `tile_sword.png`, `btn_play.png` |
| **Audio Clip** | snake_case | `sfx_match.wav`, `bgm_pirate.mp3` |
| **Material** | PascalCase + `_Mat` | `Tile_Mat`, `Dissolve_Mat` |
| **Folder** | PascalCase | `Scripts/`, `Models/`, `EventChannels/` |

### 1.4 MVC + Event Suffix Convention

| Layer | Suffix bắt buộc | Ví dụ |
|---|---|---|
| **Model** | `Model` | `TileModel`, `BoardModel` |
| **View** | `View` | `TileView`, `BoardView` |
| **Controller** | `Controller` | `BoardController`, `GameController` |
| **Service** | `Service` | `AudioService`, `SaveService` |
| **ScriptableObject** | `SO` | `TileDatabaseSO`, `GameConfigSO` |
| **Event Channel SO** | `ChannelSO` | `VoidEventChannelSO`, `TileSelectedChannelSO` |
| **EventBus Event** | `Event` | `GamePhaseChangedEvent`, `TileStateChangedEvent` |
| **Event Data** | `EventData` | `TileSelectedEventData` |

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

    [Header("Event Channels")]
    [SerializeField] private VoidEventChannelSO _someChannel;

    // 3. PRIVATE FIELDS
    private ExampleModel _model;
    private EventBinding<GamePhaseChangedEvent> _phaseBinding;

    // 4. PUBLIC PROPERTIES
    public bool IsReady => _isInitialized;

    // 5. UNITY LIFECYCLE
    private void Awake() { }
    private void OnEnable() {
        // Subscribe Event Channels
        // Register EventBus bindings
    }
    private void Start() { }
    private void OnDisable() {
        // Unsubscribe Event Channels
        // Deregister EventBus bindings
    }
    private void OnDestroy() { /* Kill DOTween */ }

    // 6. PUBLIC METHODS
    public void Initialize() { }

    // 7. PRIVATE METHODS
    private void HandleEvent() { }

    // 8. EVENT HANDLERS
    private void OnPhaseChanged(GamePhaseChangedEvent e) { }
    private void OnGameWon() { }
}
```

### 2.2 Braces Style — K&R

```csharp
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
1. ✅ Mỗi file chỉ chứa 1 class/interface/enum (trừ EventListener đi kèm ChannelSO)
2. ✅ Mỗi class tối đa 300 dòng
3. ✅ Mỗi method tối đa 30 dòng
4. ✅ Dùng var khi kiểu rõ ràng
5. ✅ String interpolation: $"Level {levelIndex}"
6. ❌ KHÔNG magic numbers — dùng const/config
7. ❌ KHÔNG Debug.Log trong production — dùng #if UNITY_EDITOR
8. ❌ KHÔNG public fields — dùng property
9. ✅ Luôn khai báo access modifier rõ ràng
10. ✅ Event payload luôn dùng struct
```

---

## 3. Unity Convention

### 3.1 MonoBehaviour Guidelines

```
1. ✅ Dùng [SerializeField] private thay vì public
2. ✅ Dùng [Header("Section")] để nhóm fields
3. ✅ Subscribe Event Channels trong OnEnable, Unsubscribe trong OnDisable
4. ✅ Register EventBus bindings trong OnEnable, Deregister trong OnDisable
5. ✅ Lưu EventBinding vào field để deregister
6. ✅ Kill DOTween trong OnDestroy
7. ❌ KHÔNG dùng Find/FindWithTag trong runtime
8. ❌ KHÔNG dùng Update nếu có thể — dùng event-driven
```

---

## 4. Event System Rules

### 4.1 Event Channel SO Rules

```
✅ ScriptableObject asset
✅ Wire qua [SerializeField] trong Inspector
✅ Subscribe trong OnEnable, Unsubscribe trong OnDisable
✅ Dùng named method (không anonymous lambda)
✅ Payload dùng struct (EventData suffix)
❌ KHÔNG tạo Event Channel bằng code tại runtime
❌ KHÔNG dùng static reference đến Event Channel
```

### 4.2 EventBus Rules

```
✅ Event struct implement IEvent
✅ Payload luôn là struct (tránh GC allocation)
✅ Lưu EventBinding vào private field
✅ Register trong OnEnable, Deregister trong OnDisable
✅ Dùng cho sự kiện nội bộ, system-level, pure C# context
❌ KHÔNG dùng EventBus cho sự kiện cross-layer cần Inspector debug
❌ KHÔNG quên Deregister → gây memory leak
❌ KHÔNG tạo binding inline mà không lưu reference
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
║  EventBus Event        → Event suffix                ║
║  Event Data            → EventData suffix            ║
╠══════════════════════════════════════════════════════╣
║  CODING                                              ║
║  Braces                → K&R (same line)             ║
║  Access modifier       → Always explicit             ║
║  Fields in Inspector   → [SerializeField] private    ║
║  Max class length      → 300 lines                   ║
║  Max method length     → 30 lines                    ║
║  Event payload         → Always struct               ║
╠══════════════════════════════════════════════════════╣
║  HYBRID EVENT SYSTEM                                 ║
║  EventBus              → Internal, system-level      ║
║  Event Channel SO      → Cross-layer, Inspector      ║
║  Model                 → Pure C#, can use EventBus   ║
║  View                  → MonoBehaviour, visual only  ║
║  Controller            → M↔V bridge, uses BOTH       ║
║  Subscribe/Register    → OnEnable                    ║
║  Unsubscribe/Deregister → OnDisable                  ║
╚══════════════════════════════════════════════════════╝
```
