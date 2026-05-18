#if UNITY_EDITOR
using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapSceneSetup : EditorWindow
{
    private static T EnsureComponent<T>(GameObject go) where T : Component
    {
        var comp = go.GetComponent<T>();
        if (comp == null) comp = go.AddComponent<T>();
        return comp;
    }

    [MenuItem("Pirate Tiles/Setup Map Scene")]
    public static void SetupScene()
    {
        // Clear selection to avoid SerializedObjectNotCreatableException when modifying objects in the Inspector
        Selection.activeGameObject = null;

        EnsureMainCamera();
        EnsureEventSystem();

        var gameManager = EnsureGameManager();
        var sceneService = EnsureComponent<SceneService>(gameManager);
        var loadingScreen = EnsureLoadingScreen(gameManager.transform);
        AssignPrivateField(sceneService, "_loadingScreen", loadingScreen.gameObject);

        var canvas = EnsureCanvas("MapCanvas");
        
        // 1. Controller
        var controllerGo = GameObject.Find("MapController");
        if (controllerGo == null) controllerGo = new GameObject("MapController");
        var controller = EnsureComponent<MapController>(controllerGo);

        // 2. TopBar
        var topBar = EnsureRectTransform("TopBar", canvas.transform);
        SetAnchors(topBar, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, 0), new Vector2(0, 150));

        var accountPlaceholder = EnsureRectTransform("AccountPlaceholder", topBar);
        SetAnchors(accountPlaceholder, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(50, 0), new Vector2(120, 120));
        var accImg = EnsureComponent<Image>(accountPlaceholder.gameObject);
        accImg.color = Color.grey;

        var heartsViewGo = EnsureRectTransform("HeartsDisplay", topBar);
        SetAnchors(heartsViewGo, new Vector2(0.3f, 0.5f), new Vector2(0.3f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(250, 100));
        var heartsBg = EnsureComponent<Image>(heartsViewGo.gameObject);
        heartsBg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        var heartsView = EnsureComponent<HeartsView>(heartsViewGo.gameObject);
        var heartsText = EnsureText("HeartsText", heartsViewGo, "5/5", 36, TextAlignmentOptions.Center);
        SetAnchors(heartsText.rectTransform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        AssignPrivateField(heartsView, "_heartsCountText", heartsText);
        
        var coinsViewGo = EnsureRectTransform("CoinsDisplay", topBar);
        SetAnchors(coinsViewGo, new Vector2(0.7f, 0.5f), new Vector2(0.7f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(250, 100));
        var coinsBg = EnsureComponent<Image>(coinsViewGo.gameObject);
        coinsBg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        var coinsView = EnsureComponent<CoinsView>(coinsViewGo.gameObject);
        var coinsText = EnsureText("CoinsText", coinsViewGo, "100", 36, TextAlignmentOptions.Center);
        SetAnchors(coinsText.rectTransform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        AssignPrivateField(coinsView, "_coinsText", coinsText);

        var settingsBtn = EnsureButton("SettingsButton", topBar, "SET", new Vector2(-50, 0), new Vector2(120, 120));
        SetAnchors(settingsBtn.GetComponent<RectTransform>(), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-50, 0), new Vector2(120, 120));

        // 3. MapPanel
        var mapPanel = EnsureRectTransform("MapPanel", canvas.transform);
        SetAnchors(mapPanel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -50), new Vector2(900, 1200));
        
        var mapView = EnsureComponent<MapView>(mapPanel.gameObject);
        var mapBg = EnsureComponent<Image>(mapPanel.gameObject);
        mapBg.color = new Color(0.9f, 0.85f, 0.75f, 1f); 

        var chapterTitle = EnsureText("ChapterTitleText", mapPanel, "Chapter 1", 64, TextAlignmentOptions.Center);
        chapterTitle.color = Color.black;
        SetAnchors(chapterTitle.rectTransform, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -80), new Vector2(600, 100));

        var levelPath = EnsureRectTransform("LevelPathContainer", mapPanel);
        SetAnchors(levelPath, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        var leftArrow = EnsureButton("LeftArrowButton", mapPanel, "<", new Vector2(-500, 0), new Vector2(100, 150));
        var rightArrow = EnsureButton("RightArrowButton", mapPanel, ">", new Vector2(500, 0), new Vector2(100, 150));

        AssignPrivateField(mapView, "_chapterTitleText", chapterTitle);
        AssignPrivateField(mapView, "_mapBackgroundImage", mapBg);
        AssignPrivateField(mapView, "_levelContainer", levelPath);
        AssignPrivateField(mapView, "_leftArrowButton", leftArrow);
        AssignPrivateField(mapView, "_rightArrowButton", rightArrow);

        // 4. SideButtons (Bottom Right)
        var sideButtons = EnsureRectTransform("SideButtons", canvas.transform);
        SetAnchors(sideButtons, new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-50, 50), new Vector2(150, 350));
        
        var shopBtn = EnsureButton("ShopButton", sideButtons, "SHOP", new Vector2(0, 100), new Vector2(120, 120));
        var collectionBtn = EnsureButton("CollectionButton", sideButtons, "PACK", new Vector2(0, 250), new Vector2(120, 120));

        // 5. Panels
        var settingPanelView = EnsureSettingPanel(canvas.transform);
        var shopPanelView = EnsureShopPanel(canvas.transform);
        var collectionPanelView = EnsureCollectionPanel(canvas.transform);

        // 6. Prefabs & Configs
        var existingBtnObj = canvas.transform.Find("LevelButtonPrefab");
        var levelButtonPrefabObj = existingBtnObj != null ? existingBtnObj.gameObject : new GameObject("LevelButtonPrefab", typeof(RectTransform));
        
        var lvlBtnRect = levelButtonPrefabObj.GetComponent<RectTransform>();
        lvlBtnRect.sizeDelta = new Vector2(100, 100);
        var lvlBtnImg = EnsureComponent<Image>(levelButtonPrefabObj);
        var lvlBtnBtn = EnsureComponent<Button>(levelButtonPrefabObj);
        var lvlBtnView = EnsureComponent<LevelButtonView>(levelButtonPrefabObj);
        var lvlBtnText = EnsureText("Text", lvlBtnRect, "1", 36, TextAlignmentOptions.Center);
        SetAnchors(lvlBtnText.rectTransform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        
        AssignPrivateField(lvlBtnView, "_button", lvlBtnBtn);
        AssignPrivateField(lvlBtnView, "_levelText", lvlBtnText);
        AssignPrivateField(lvlBtnView, "_iconImage", lvlBtnImg);

        levelButtonPrefabObj.SetActive(false);
        levelButtonPrefabObj.transform.SetParent(canvas.transform);
        AssignPrivateField(mapView, "_levelButtonPrefab", lvlBtnView);

        var gameConfig = AssetDatabase.LoadAssetAtPath<GameConfigSO>("Assets/_Project/Scripts/Data/SO/GameConfig.asset");

        // 7. Wire MapController
        AssignPrivateField(controller, "_mapView", mapView);
        AssignPrivateField(controller, "_heartsView", heartsView);
        AssignPrivateField(controller, "_coinsView", coinsView);
        AssignPrivateField(controller, "_settingPanelView", settingPanelView);
        AssignPrivateField(controller, "_shopPanelView", shopPanelView);
        AssignPrivateField(controller, "_collectionPanelView", collectionPanelView);
        AssignPrivateField(controller, "_settingsButton", settingsBtn);
        AssignPrivateField(controller, "_shopButton", shopBtn);
        AssignPrivateField(controller, "_collectionButton", collectionBtn);
        AssignPrivateField(controller, "_gameConfig", gameConfig);

        EditorUtility.SetDirty(mapView);
        EditorUtility.SetDirty(controller);
        EditorUtility.SetDirty(sceneService);
        Debug.Log("Map Scene setup complete.");
    }

    private static GameObject EnsureGameManager()
    {
        var gm = GameObject.Find("GameManager");
        if (gm == null) gm = new GameObject("GameManager");

        EnsureComponent<GameManager>(gm);
        EnsureComponent<SaveService>(gm);
        EnsureComponent<SceneService>(gm);

        var audioService = EnsureComponent<AudioService>(gm);
        var bgmSource = EnsureAudioSource(gm, "BGMSource", true);
        var sfxSource = EnsureAudioSource(gm, "SFXSource", false);

        AssignPrivateField(audioService, "_bgmSource", bgmSource);
        AssignPrivateField(audioService, "_sfxSource", sfxSource);
        return gm;
    }

    private static AudioSource EnsureAudioSource(GameObject parent, string name, bool loop)
    {
        var child = parent.transform.Find(name);
        var go = child != null ? child.gameObject : new GameObject(name);
        go.transform.SetParent(parent.transform);
        var source = EnsureComponent<AudioSource>(go);
        source.loop = loop;
        return source;
    }

    private static Canvas EnsureLoadingScreen(Transform parent)
    {
        var existing = parent.Find("LoadingScreen");
        if (existing != null) return existing.GetComponent<Canvas>();

        var loadingGo = new GameObject("LoadingScreen", typeof(RectTransform));
        loadingGo.transform.SetParent(parent);
        var canvas = EnsureComponent<Canvas>(loadingGo);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = EnsureComponent<CanvasScaler>(loadingGo);
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        EnsureComponent<GraphicRaycaster>(loadingGo);
        loadingGo.SetActive(false);

        var group = EnsureComponent<CanvasGroup>(loadingGo);
        var view = EnsureComponent<LoadingView>(loadingGo);
        AssignPrivateField(view, "_canvasGroup", group);

        return canvas;
    }

    private static Canvas EnsureCanvas(string name)
    {
        var canvasGo = GameObject.Find(name);
        if (canvasGo == null) canvasGo = new GameObject(name, typeof(RectTransform));

        var canvas = EnsureComponent<Canvas>(canvasGo);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        var scaler = EnsureComponent<CanvasScaler>(canvasGo);
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);

        EnsureComponent<GraphicRaycaster>(canvasGo);
        return canvas;
    }

    private static void EnsureMainCamera()
    {
        var camObj = GameObject.Find("Main Camera");
        if (camObj == null)
        {
            camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
        }

        var cam = EnsureComponent<Camera>(camObj);
        cam.orthographic = true;
        cam.orthographicSize = 6f;

        EnsureComponent<AudioListener>(camObj);
    }

    private static void EnsureEventSystem()
    {
        var eventSystem = UnityEngine.Object.FindAnyObjectByType<EventSystem>();
        GameObject eventSystemGo;

        if (eventSystem == null)
        {
            eventSystemGo = new GameObject("EventSystem");
            EnsureComponent<EventSystem>(eventSystemGo);
        }
        else
        {
            eventSystemGo = eventSystem.gameObject;
        }

        EnsureComponent<StandaloneInputModule>(eventSystemGo);
    }

    private static RectTransform EnsureRectTransform(string name, Transform parent)
    {
        var child = parent.Find(name);
        if (child != null) 
        {
            var r = child.GetComponent<RectTransform>();
            if (r != null) return r;
            // Chuyển đổi Transform thành RectTransform nếu nó chưa phải là RectTransform
            return EnsureComponent<RectTransform>(child.gameObject);
        }

        var go = new GameObject(name, typeof(RectTransform));
        var rect = EnsureComponent<RectTransform>(go);
        go.transform.SetParent(parent, false);
        return rect;
    }

    private static Button EnsureButton(string name, Transform parent, string text, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        var buttonRect = EnsureRectTransform(name, parent);
        SetAnchors(buttonRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), anchoredPosition, sizeDelta);

        var image = EnsureComponent<Image>(buttonRect.gameObject);
        image.color = new Color(0.27f, 0.2f, 0.13f, 1f);
        var button = EnsureComponent<Button>(buttonRect.gameObject);

        var label = EnsureText("Label", buttonRect, text, 34, TextAlignmentOptions.Center);
        SetAnchors(label.rectTransform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        return button;
    }

    private static TextMeshProUGUI EnsureText(string name, Transform parent, string value, int size, TextAlignmentOptions alignment)
    {
        var textRect = EnsureRectTransform(name, parent);
        var text = EnsureComponent<TextMeshProUGUI>(textRect.gameObject);
        text.text = value;
        text.fontSize = size;
        text.alignment = alignment;
        text.color = Color.white;
        return text;
    }

    private static SettingPanelView EnsureSettingPanel(Transform parent)
    {
        var panel = EnsureRectTransform("SettingPanel", parent);
        var group = EnsureComponent<CanvasGroup>(panel.gameObject);
        SetAnchors(panel, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        var bgImage = EnsureComponent<Image>(panel.gameObject);
        bgImage.color = new Color(0f, 0f, 0f, 0.75f);

        var panelRoot = EnsureRectTransform("PanelRoot", panel);
        SetAnchors(panelRoot, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(640f, 760f));
        var panelRootImg = EnsureComponent<Image>(panelRoot.gameObject);
        panelRootImg.color = new Color(0.16f, 0.13f, 0.1f, 1f);

        var title = EnsureText("TitleText", panelRoot, "SETTINGS", 48, TextAlignmentOptions.Center);
        SetAnchors(title.rectTransform, new Vector2(0.5f, 0.9f), new Vector2(0.5f, 0.9f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(420f, 120f));

        var musicToggle = EnsureToggle("MusicToggle", panelRoot, "Music", new Vector2(0f, 170f));
        var sfxToggle = EnsureToggle("SfxToggle", panelRoot, "SFX", new Vector2(0f, 90f));

        var continueButton = EnsureButton("ContinueButton", panelRoot, "CONTINUE", new Vector2(0f, -40f), new Vector2(400, 90));
        var replayButton = EnsureButton("ReplayButton", panelRoot, "REPLAY", new Vector2(0f, -150f), new Vector2(400, 90));
        var mapButton = EnsureButton("MapButton", panelRoot, "MAP", new Vector2(0f, -260f), new Vector2(400, 90));

        var view = EnsureComponent<SettingPanelView>(panel.gameObject);
        AssignPrivateField(view, "_canvasGroup", group);
        AssignPrivateField(view, "_panelRoot", panelRoot);
        AssignPrivateField(view, "_musicToggle", musicToggle);
        AssignPrivateField(view, "_sfxToggle", sfxToggle);
        AssignPrivateField(view, "_continueButton", continueButton);
        AssignPrivateField(view, "_replayButton", replayButton);
        AssignPrivateField(view, "_mapButton", mapButton);

        panel.gameObject.SetActive(false);
        return view;
    }
    
    private static ShopPanelView EnsureShopPanel(Transform parent)
    {
        var panel = EnsureRectTransform("ShopPanel", parent);
        var group = EnsureComponent<CanvasGroup>(panel.gameObject);
        SetAnchors(panel, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        var bgImage = EnsureComponent<Image>(panel.gameObject);
        bgImage.color = new Color(0f, 0f, 0f, 0.75f);
        var bgButton = EnsureComponent<Button>(panel.gameObject);

        var panelRoot = EnsureRectTransform("PanelRoot", panel);
        SetAnchors(panelRoot, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(720f, 800f));
        var panelRootImg = EnsureComponent<Image>(panelRoot.gameObject);
        panelRootImg.color = new Color(0.16f, 0.13f, 0.1f, 1f);

        var title = EnsureText("TitleText", panelRoot, "SHOP", 48, TextAlignmentOptions.Center);
        SetAnchors(title.rectTransform, new Vector2(0.5f, 0.9f), new Vector2(0.5f, 0.9f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(420f, 120f));

        var closeButton = EnsureButton("CloseButton", panelRoot, "CLOSE", new Vector2(0, -320), new Vector2(400, 90));

        var undoBtn = EnsureButton("BuyUndoBtn", panelRoot, "Buy Undo", new Vector2(-150, 100), new Vector2(250, 100));
        var magicBtn = EnsureButton("BuyMagicBtn", panelRoot, "Buy Magic", new Vector2(150, 100), new Vector2(250, 100));
        var shuffleBtn = EnsureButton("BuyShuffleBtn", panelRoot, "Buy Shuffle", new Vector2(-150, -100), new Vector2(250, 100));
        var addCellBtn = EnsureButton("BuyAddCellBtn", panelRoot, "Buy +Cell", new Vector2(150, -100), new Vector2(250, 100));

        var undoCost = EnsureText("UndoCost", undoBtn.transform, "50", 24, TextAlignmentOptions.Center);
        SetAnchors(undoCost.rectTransform, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 1), new Vector2(0, -10), new Vector2(100, 40));
        
        var magicCost = EnsureText("MagicCost", magicBtn.transform, "50", 24, TextAlignmentOptions.Center);
        SetAnchors(magicCost.rectTransform, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 1), new Vector2(0, -10), new Vector2(100, 40));
        
        var shuffleCost = EnsureText("ShuffleCost", shuffleBtn.transform, "50", 24, TextAlignmentOptions.Center);
        SetAnchors(shuffleCost.rectTransform, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 1), new Vector2(0, -10), new Vector2(100, 40));
        
        var addCellCost = EnsureText("AddCellCost", addCellBtn.transform, "50", 24, TextAlignmentOptions.Center);
        SetAnchors(addCellCost.rectTransform, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 1), new Vector2(0, -10), new Vector2(100, 40));

        var view = EnsureComponent<ShopPanelView>(panel.gameObject);
        AssignPrivateField(view, "_canvasGroup", group);
        AssignPrivateField(view, "_panelRoot", panelRoot);
        AssignPrivateField(view, "_closeButton", closeButton);
        AssignPrivateField(view, "_backgroundButton", bgButton);
        AssignPrivateField(view, "_buyUndoButton", undoBtn);
        AssignPrivateField(view, "_buyMagicButton", magicBtn);
        AssignPrivateField(view, "_buyShuffleButton", shuffleBtn);
        AssignPrivateField(view, "_buyAddCellButton", addCellBtn);
        AssignPrivateField(view, "_undoCostText", undoCost);
        AssignPrivateField(view, "_magicCostText", magicCost);
        AssignPrivateField(view, "_shuffleCostText", shuffleCost);
        AssignPrivateField(view, "_addCellCostText", addCellCost);

        panel.gameObject.SetActive(false);
        return view;
    }

    private static CollectionPanelView EnsureCollectionPanel(Transform parent)
    {
        var panel = EnsureRectTransform("CollectionPanel", parent);
        var group = EnsureComponent<CanvasGroup>(panel.gameObject);
        SetAnchors(panel, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        var bgImage = EnsureComponent<Image>(panel.gameObject);
        bgImage.color = new Color(0f, 0f, 0f, 0.75f);
        var bgButton = EnsureComponent<Button>(panel.gameObject);

        var panelRoot = EnsureRectTransform("PanelRoot", panel);
        SetAnchors(panelRoot, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(720f, 800f));
        var panelRootImg = EnsureComponent<Image>(panelRoot.gameObject);
        panelRootImg.color = new Color(0.16f, 0.13f, 0.1f, 1f);

        var title = EnsureText("TitleText", panelRoot, "COLLECTION", 48, TextAlignmentOptions.Center);
        SetAnchors(title.rectTransform, new Vector2(0.5f, 0.9f), new Vector2(0.5f, 0.9f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(420f, 120f));

        var placeholder = EnsureText("PlaceholderText", panelRoot, "Coming Soon...", 36, TextAlignmentOptions.Center);
        SetAnchors(placeholder.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(400, 100));

        var closeButton = EnsureButton("CloseButton", panelRoot, "CLOSE", new Vector2(0, -320), new Vector2(400, 90));

        var view = EnsureComponent<CollectionPanelView>(panel.gameObject);
        AssignPrivateField(view, "_canvasGroup", group);
        AssignPrivateField(view, "_panelRoot", panelRoot);
        AssignPrivateField(view, "_closeButton", closeButton);
        AssignPrivateField(view, "_backgroundButton", bgButton);

        panel.gameObject.SetActive(false);
        return view;
    }

    private static UnityEngine.UI.Toggle EnsureToggle(string name, Transform parent, string labelText, Vector2 anchoredPosition)
    {
        var toggleRect = EnsureRectTransform(name, parent);
        SetAnchors(toggleRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), anchoredPosition, new Vector2(440f, 60f));
        var toggle = EnsureComponent<UnityEngine.UI.Toggle>(toggleRect.gameObject);

        var bg = EnsureRectTransform("Background", toggleRect);
        SetAnchors(bg, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(24f, 0f), new Vector2(36f, 36f));
        var bgImage = EnsureComponent<Image>(bg.gameObject);
        bgImage.color = Color.white;

        var checkmark = EnsureRectTransform("Checkmark", bg);
        SetAnchors(checkmark, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        var checkImage = EnsureComponent<Image>(checkmark.gameObject);
        checkImage.color = Color.black;

        var label = EnsureText("Label", toggleRect, labelText, 32, TextAlignmentOptions.Left);
        SetAnchors(label.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0f, 0.5f), new Vector2(70f, 0f), new Vector2(-70f, 0f));

        toggle.targetGraphic = bgImage;
        toggle.graphic = checkImage;
        return toggle;
    }

    private static void SetAnchors(RectTransform rect, Vector2 min, Vector2 max, Vector2 pivot, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = sizeDelta;
    }

    private static void AssignPrivateField(object target, string fieldName, UnityEngine.Object value)
    {
        if (target == null) return;

        var type = target.GetType();
        var field = type.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        while (field == null && type.BaseType != null)
        {
            type = type.BaseType;
            field = type.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        }

        if (field != null)
        {
            field.SetValue(target, value);
            if (target is UnityEngine.Object unityObj)
            {
                EditorUtility.SetDirty(unityObj);
            }
        }
    }
}
#endif