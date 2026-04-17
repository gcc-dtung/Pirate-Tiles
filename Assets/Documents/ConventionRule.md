# 📏 Convention Rules — Wonder Match

> Quy tắc đặt tên, coding convention, và quy ước Unity cho dự án Wonder Match.  
> Tất cả thành viên trong team **BẮT BUỘC** tuân thủ các quy tắc dưới đây.

---

## 1. Naming Convention — Quy Tắc Đặt Tên

### 1.1 C# Code

| Loại | Convention | Ví dụ |
|---|---|---|
| **Class / Struct** | PascalCase | `CardModel`, `StackController`, `GameConfigSO` |
| **Interface** | I + PascalCase | `IGameEvent`, `IPowerUp` |
| **Enum** | PascalCase | `CardType`, `GamePhase` |
| **Enum Value** | PascalCase | `CardType.CardA`, `GamePhase.Playing` |
| **Public Method** | PascalCase | `FindMatch()`, `UpdateSelectableStatus()` |
| **Private Method** | PascalCase | `HandleCardClick()`, `SyncViewWithModel()` |
| **Public Property** | PascalCase | `CurrentHearts`, `IsSelectable` |
| **Private Field** | _camelCase (underscore prefix) | `_cards`, `_stackModel`, `_isProcessing` |
| **[SerializeField] private** | _camelCase (underscore prefix) | `_spriteRenderer`, `_timerText` |
| **Public Field** (tránh dùng) | camelCase | `maxHearts`, `healTime` |
| **Local Variable** | camelCase | `cardCount`, `matchIndex`, `targetPos` |
| **Parameter** | camelCase | `cardType`, `duration`, `onComplete` |
| **Constant** | PascalCase hoặc UPPER_SNAKE | `MaxStackSize`, `DEFAULT_HEAL_TIME` |
| **Event** | On + PascalCase | `OnCardClicked`, `OnMatchFound` |
| **Boolean** | Is/Has/Can/Should prefix | `IsSelectable`, `HasHearts`, `CanInteract` |

### 1.2 Ví dụ áp dụng

```csharp
public class BoardController : MonoBehaviour {
    // === SerializeField ===
    [SerializeField] private BoardView _boardView;
    [SerializeField] private CardDatabaseSO _cardDatabase;

    // === Private fields ===
    private BoardModel _boardModel;
    private GameStateModel _gameState;
    private bool _isInitialized;

    // === Public properties ===
    public bool IsCleared => _boardModel.RemainingCards <= 0;
    public int CardCount => _boardModel.Cards.Count;

    // === Public methods ===
    public void Initialize(LevelModel level) {
        _boardModel = new BoardModel(level.Cards);
        _boardView.SpawnCards(_boardModel.Cards);
        _isInitialized = true;
    }

    // === Private methods ===
    private void HandleCardClicked(CardView cardView) {
        if (!_gameState.CanInteract) return;
        CardModel cardModel = GetModelForView(cardView);
        // ...
    }

    // === Events ===
    public event Action<CardModel> OnCardSelected;
}
```

### 1.3 Unity Assets & Files

| Loại | Convention | Ví dụ |
|---|---|---|
| **Scene** | PascalCase | `InGame.unity`, `GameMode.unity` |
| **Prefab** | PascalCase | `Card.prefab`, `WinPanel.prefab` |
| **ScriptableObject Asset** | PascalCase + Suffix | `BICH_CardData.asset`, `Level01_Config.asset` |
| **Sprite** | snake_case hoặc PascalCase | `card_heart_A.png`, `btn_play.png` |
| **Audio Clip** | snake_case | `sfx_match.wav`, `bgm_main.mp3` |
| **Material** | PascalCase + `_Mat` | `Card_Mat`, `Dissolve_Mat` |
| **Shader** | PascalCase | `CardDissolve.shader` |
| **Animation Clip** | PascalCase + Action | `CardFlip.anim`, `PanelSlideIn.anim` |
| **Folder** | PascalCase | `Scripts/`, `Models/`, `CardData/` |

### 1.4 MVC Suffix Convention

| Layer | Suffix bắt buộc | Ví dụ |
|---|---|---|
| **Model** | `Model` | `CardModel`, `BoardModel`, `StackModel` |
| **View** | `View` | `CardView`, `BoardView`, `TimerView` |
| **Controller** | `Controller` | `BoardController`, `GameController` |
| **Service** | `Service` | `AudioService`, `SaveService` |
| **ScriptableObject** | `SO` | `CardDatabaseSO`, `GameConfigSO` |
| **Event** | `Event` | `CardSelectedEvent`, `GameWonEvent` |

---

## 2. Coding Convention — Quy Tắc Viết Code

### 2.1 File Structure — Thứ tự trong class

```csharp
public class ExampleController : MonoBehaviour {
    // 1. CONSTANTS
    private const int MaxRetries = 3;

    // 2. SERIALIZED FIELDS (Inspector)
    [Header("References")]
    [SerializeField] private ExampleView _view;
    [SerializeField] private ExampleSO _config;

    // 3. PRIVATE FIELDS
    private ExampleModel _model;
    private bool _isInitialized;

    // 4. PUBLIC PROPERTIES
    public bool IsReady => _isInitialized;

    // 5. EVENTS
    public event Action<int> OnValueChanged;

    // 6. UNITY LIFECYCLE (theo thứ tự gọi)
    private void Awake() { }
    private void OnEnable() { }
    private void Start() { }
    private void Update() { }
    private void OnDisable() { }
    private void OnDestroy() { }

    // 7. PUBLIC METHODS
    public void Initialize() { }
    public void DoSomething() { }

    // 8. PRIVATE METHODS
    private void HandleEvent() { }
    private void UpdateView() { }

    // 9. COROUTINES (nếu có)
    private IEnumerator WaitAndDo() { }
}
```

### 2.2 Braces Style — K&R (mở ngoặc cùng dòng)

```csharp
// ✅ ĐÚNG — K&R style (theo codebase hiện tại)
public class CardModel {
    public void DoSomething() {
        if (condition) {
            // ...
        } else {
            // ...
        }
    }
}

// ❌ SAI — Allman style (KHÔNG dùng)
public class CardModel
{
    public void DoSomething()
    {
        if (condition)
        {
        }
    }
}
```

### 2.3 Quy tắc chung

```
1. ✅ Mỗi file chỉ chứa 1 class/interface/enum (trừ inner class nhỏ)
2. ✅ Mỗi class tối đa 300 dòng — nếu dài hơn → tách
3. ✅ Mỗi method tối đa 30 dòng — nếu dài hơn → extract method
4. ✅ Dùng `var` khi kiểu rõ ràng: var card = new CardModel();
5. ✅ Dùng explicit type khi không rõ: List<CardModel> cards = GetCards();
6. ✅ Nullable: dùng null check pattern (card?.DoSomething())
7. ✅ String interpolation: $"Level {levelIndex}"
8. ❌ KHÔNG dùng magic numbers — khai báo const/config
9. ❌ KHÔNG dùng Debug.Log trong production — dùng #if UNITY_EDITOR hoặc Logger
10. ❌ KHÔNG public fields (trừ struct data) — dùng property
```

### 2.4 Access Modifier

```
📌 QUY TẮC: Luôn khai báo access modifier rõ ràng

✅ private void HandleClick() { }
❌ void HandleClick() { }           // Implicit private — khó đọc

📌 THỨ TỰ ƯU TIÊN: private > protected > internal > public
   → Cho scope nhỏ nhất có thể
   → Chỉ public khi THỰC SỰ cần truy cập từ bên ngoài
```

### 2.5 Comment & Documentation

```csharp
// ✅ Dùng XML doc cho public API
/// <summary>
/// Tìm vị trí chèn bài vào stack, ưu tiên chèn cạnh bài cùng loại.
/// </summary>
/// <param name="cardType">Loại bài cần chèn</param>
/// <returns>Index vị trí chèn (0-based)</returns>
public int GetInsertIndex(CardType cardType) { ... }

// ✅ Dùng comment ngắn cho logic phức tạp
// Fisher-Yates shuffle — O(n), in-place
private void ShuffleList<T>(List<T> list) { ... }

// ❌ KHÔNG comment điều hiển nhiên
int count = 0; // Khai báo biến count ← THỪA
```

---

## 3. Unity Convention — Quy Tắc Unity

### 3.1 MonoBehaviour Guidelines

```
1. ✅ Dùng [SerializeField] private thay vì public cho Inspector
2. ✅ Dùng [Header("Section")] để nhóm fields trong Inspector
3. ✅ Subscribe events trong OnEnable, Unsubscribe trong OnDisable
4. ✅ Kill DOTween trong OnDestroy
5. ❌ KHÔNG dùng Awake cho logic phức tạp — dùng Initialize() method
6. ❌ KHÔNG dùng Update nếu có thể — dùng event-driven
7. ❌ KHÔNG dùng Find/FindWithTag trong runtime — inject dependencies
```

### 3.2 Singleton Pattern (chỉ cho Service)

```csharp
// ✅ Chuẩn Singleton cho Service
public class AudioService : MonoBehaviour {
    public static AudioService Instance { get; private set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
            return;
        }
        InitializeService();
    }

    private void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }
}
```

### 3.3 Prefab & Scene Hierarchy

```
📌 THỨ TỰ HIERARCHY TRONG SCENE:

Scene Root
├── --- MANAGERS ---         ← Separator (empty GO)
│   ├── GameController
│   ├── BoardController
│   ├── StackController
│   └── ...Controllers
├── --- ENVIRONMENT ---
│   ├── Camera
│   └── Background
├── --- GAMEPLAY ---
│   ├── BoardView
│   │   └── [Cards]
│   └── StackView
│       └── [Slots]
├── --- UI ---
│   └── Canvas
│       ├── HUD (hearts, coins, timer, power-ups)
│       ├── WinPanel
│       ├── LosePanel
│       └── SettingPanel
└── --- DEBUG ---             ← Chỉ trong Editor
    └── DebugCanvas

📌 Dùng "--- NAME ---" separator để dễ đọc hierarchy
```

### 3.4 Tag & Layer

| Tag | Dùng cho |
|---|---|
| `Card` | Tất cả CardView GameObjects |
| `Player` | Player root objects |
| `PlayerA` / `PlayerB` | Phân biệt player |

| Layer | Dùng cho |
|---|---|
| `Default` | Mặc định |
| `Card` | Cards (cho physics raycast nếu cần) |
| `UI` | Tất cả UI elements |
| `Ignore Raycast` | Background, decorations |

### 3.5 Prefab Rules

```
1. ✅ Prefab name = Class name (Card.prefab → CardView component)
2. ✅ Prefab root phải có component chính (CardView trên root GO)
3. ✅ Nested prefab cho UI panels phức tạp
4. ❌ KHÔNG sửa prefab instance trong scene — sửa prefab gốc
5. ❌ KHÔNG đặt prefab quá nhiều tầng lồng (max 3 levels)
```

---

## 4. Git Convention — Quy Tắc Git

### 4.1 Branch Naming

| Loại | Format | Ví dụ |
|---|---|---|
| **Feature** | `feature/tên-ngắn` | `feature/stack-system`, `feature/power-ups` |
| **Bugfix** | `fix/mô-tả-bug` | `fix/magic-leak`, `fix/timer-reset` |
| **Refactor** | `refactor/mô-tả` | `refactor/board-model`, `refactor/event-bus` |
| **Release** | `release/version` | `release/1.0.0` |

### 4.2 Commit Message

```
Format: <type>(<scope>): <description>

Types:
  feat     — Tính năng mới
  fix      — Sửa bug
  refactor — Tái cấu trúc không thay đổi behavior
  test     — Thêm/sửa test
  docs     — Tài liệu
  style    — Format code (không thay đổi logic)
  chore    — Build, config, dependencies

Ví dụ:
  feat(board): implement overlap detection in BoardModel
  fix(stack): fix smart insert index when no same type exists
  test(stack): add unit tests for StackModel.FindMatch
  refactor(events): migrate from static events to EventBus
  docs: update Architecture document with EventBus section
```

### 4.3 Pull Request Rules

```
1. ✅ Mỗi PR đi kèm:
   - Mô tả ngắn gọn thay đổi
   - Screenshots/recordings nếu thay đổi visual
   - Danh sách task đã hoàn thành (tham chiếu _03.TaskBreakdown)
   - Tests đã pass

2. ✅ PR size:
   - Ideal: < 300 dòng thay đổi
   - Max: 500 dòng
   - Nếu lớn hơn → tách thành nhiều PR

3. ✅ Review:
   - Dev khác phải review trước khi merge
   - Tối thiểu 1 approval
```

---

## 5. MVC-Specific Rules — Quy Tắc MVC

### 5.1 Model Rules

```
✅ DO:
  - Pure C# class (không MonoBehaviour)
  - Chứa data + business logic
  - Raise events khi state thay đổi (optional)
  - Unit testable

❌ DON'T:
  - Reference UnityEngine.UI, UnityEngine.SceneManagement
  - Reference bất kỳ View hoặc Controller nào
  - Gọi DOTween, animation, rendering
  - Dùng Find, GetComponent, Instantiate
```

### 5.2 View Rules

```
✅ DO:
  - MonoBehaviour, quản lý visual
  - Expose public methods cho Controller gọi
  - Raise UI events (button click, input)
  - Handle animation, VFX, shader

❌ DON'T:
  - Chứa business logic (match-3, overlap, win/lose)
  - Trực tiếp thay đổi Model
  - Reference chéo View khác (dùng Controller/EventBus)
  - Quyết định game flow (chỉ hiển thị)
```

### 5.3 Controller Rules

```
✅ DO:
  - MonoBehaviour, kết nối Model ↔ View
  - Xử lý input → cập nhật Model → cập nhật View
  - Subscribe/Publish EventBus
  - Quản lý lifecycle (init, cleanup)

❌ DON'T:
  - Chứa rendering, animation (delegate cho View)
  - Chứa dữ liệu state (delegate cho Model)
  - Trực tiếp gọi Controller khác (dùng EventBus)
  - Quá 300 dòng (tách nếu cần)
```

### 5.4 Kiểm tra quy tắc — Quick Checklist

Trước khi commit, tự hỏi:

```
□ Model có import UnityEngine.UI không? → ❌ Sai
□ View có chứa if/else game logic không? → ❌ Sai
□ Controller có hơn 300 dòng không? → ⚠️ Cần tách
□ Có dùng Find/FindWithTag trong runtime không? → ❌ Sai
□ Event có được Unsubscribe trong OnDisable không? → ✅ Phải có
□ DOTween có .SetAutoKill(true) không? → ✅ Phải có
□ Field có access modifier rõ ràng không? → ✅ Phải có
□ Magic number có được thay bằng const/config không? → ✅ Phải có
```

---

## 6. Folder & File Organization

### 6.1 Quy tắc đặt file

```
📌 File name = Class name (1 file = 1 class)
   CardModel.cs → chứa class CardModel
   BoardView.cs → chứa class BoardView

📌 File nằm đúng thư mục layer:
   Models/CardModel.cs       ✅
   Controllers/CardModel.cs  ❌ (Model không ở Controllers)

📌 Thư mục con khi > 5 files cùng domain:
   Views/
   ├── Card/
   │   ├── CardView.cs
   │   └── CardVFXView.cs
   ├── UI/
   │   ├── WinPanelView.cs
   │   └── LosePanelView.cs
   └── ...
```

### 6.2 Import Order

```csharp
// 1. System namespaces
using System;
using System.Collections.Generic;
using System.Linq;

// 2. Unity namespaces
using UnityEngine;
using UnityEngine.UI;

// 3. Third-party namespaces
using DG.Tweening;
using TMPro;

// 4. Project namespaces (nếu dùng)
using WonderMatch.Models;
using WonderMatch.Events;
```

---

## Tóm Tắt Nhanh — Cheat Sheet

```
╔══════════════════════════════════════════════════════╗
║  NAMING                                              ║
║  Class/Method/Property → PascalCase                  ║
║  Private field         → _camelCase                  ║
║  Local/Param           → camelCase                   ║
║  Constant              → PascalCase / UPPER_SNAKE    ║
║  Boolean               → Is/Has/Can prefix           ║
║  MVC Suffix            → Model/View/Controller       ║
╠══════════════════════════════════════════════════════╣
║  CODING                                              ║
║  Braces                → K&R (same line)             ║
║  Access modifier       → Always explicit             ║
║  Fields in Inspector   → [SerializeField] private    ║
║  Max class length      → 300 lines                   ║
║  Max method length     → 30 lines                    ║
╠══════════════════════════════════════════════════════╣
║  MVC                                                 ║
║  Model                 → Pure C#, no MonoBehaviour   ║
║  View                  → MonoBehaviour, visual only  ║
║  Controller            → MonoBehaviour, M↔V bridge   ║
║  Communication         → EventBus (typed, generic)   ║
╠══════════════════════════════════════════════════════╣
║  GIT                                                 ║
║  Branch                → feature/fix/refactor/...    ║
║  Commit                → type(scope): description    ║
║  PR                    → < 300 lines, 1 approval     ║
╚══════════════════════════════════════════════════════╝
```
