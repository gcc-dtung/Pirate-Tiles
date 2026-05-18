#if UNITY_EDITOR
using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartScreenSceneSetup : EditorWindow
{
    [MenuItem("Pirate Tiles/Setup StartScreen Scene")]
    public static void SetupScene()
    {
        EnsureMainCamera();
        EnsureEventSystem();

        var gameManager = EnsureGameManager();
        var sceneService = gameManager.GetComponent<SceneService>();
        var loadingScreen = EnsureLoadingScreen(gameManager.transform);
        AssignPrivateField(sceneService, "_loadingScreen", loadingScreen.gameObject);

        var canvas = EnsureCanvas("StartScreenCanvas");
        var root = EnsureRectTransform("StartScreenRoot", canvas.transform);

        var view = root.GetComponent<StartScreenView>() ?? root.gameObject.AddComponent<StartScreenView>();
        var controller = root.GetComponent<StartScreenController>() ?? root.gameObject.AddComponent<StartScreenController>();

        var logo = EnsureText("LogoText", root, "PIRATE TILES", 96, TextAlignmentOptions.Center);
        SetAnchors(logo.rectTransform, new Vector2(0.5f, 0.82f), new Vector2(0.5f, 0.82f), new Vector2(0.5f, 0.5f), new Vector2(0f, 0f), new Vector2(900f, 180f));

        var buttonsRoot = EnsureRectTransform("ButtonsGroup", root);
        var buttonsGroup = buttonsRoot.GetComponent<CanvasGroup>() ?? buttonsRoot.gameObject.AddComponent<CanvasGroup>();
        SetAnchors(buttonsRoot, new Vector2(0.5f, 0.45f), new Vector2(0.5f, 0.45f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(500f, 420f));

        var playButton = EnsureButton("PlayButton", buttonsRoot, "PLAY", new Vector2(0f, 130f));
        var settingButton = EnsureButton("SettingsButton", buttonsRoot, "SETTINGS", new Vector2(0f, 0f));
        var quitButton = EnsureButton("QuitButton", buttonsRoot, "QUIT", new Vector2(0f, -130f));

        var heartsText = EnsureText("HeartsText", root, "5/5", 36, TextAlignmentOptions.Center);
        SetAnchors(heartsText.rectTransform, new Vector2(0.2f, 0.95f), new Vector2(0.2f, 0.95f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(260f, 70f));

        var coinsText = EnsureText("CoinsText", root, "100", 36, TextAlignmentOptions.Center);
        SetAnchors(coinsText.rectTransform, new Vector2(0.8f, 0.95f), new Vector2(0.8f, 0.95f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(260f, 70f));

        var settingPanelView = EnsureSettingPanel(root);

        AssignPrivateField(view, "_logoRoot", logo.rectTransform);
        AssignPrivateField(view, "_buttonsGroup", buttonsGroup);
        AssignPrivateField(view, "_playButton", playButton);
        AssignPrivateField(view, "_settingsButton", settingButton);
        AssignPrivateField(view, "_quitButton", quitButton);
        AssignPrivateField(view, "_heartsText", heartsText);
        AssignPrivateField(view, "_coinsText", coinsText);

        var gameConfig = AssetDatabase.LoadAssetAtPath<GameConfigSO>("Assets/_Project/Scripts/Data/SO/GameConfig.asset");
        AssignPrivateField(controller, "_startScreenView", view);
        AssignPrivateField(controller, "_settingPanelView", settingPanelView);
        AssignPrivateField(controller, "_gameConfig", gameConfig);

        EditorUtility.SetDirty(view);
        EditorUtility.SetDirty(controller);
        EditorUtility.SetDirty(sceneService);
        Debug.Log("StartScreen Scene setup complete.");
    }

    private static GameObject EnsureGameManager()
    {
        var gm = GameObject.Find("GameManager");
        if (gm == null) gm = new GameObject("GameManager");

        if (gm.GetComponent<GameManager>() == null) gm.AddComponent<GameManager>();
        if (gm.GetComponent<SaveService>() == null) gm.AddComponent<SaveService>();
        if (gm.GetComponent<SceneService>() == null) gm.AddComponent<SceneService>();

        var audioService = gm.GetComponent<AudioService>() ?? gm.AddComponent<AudioService>();
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
        var source = go.GetComponent<AudioSource>() ?? go.AddComponent<AudioSource>();
        source.loop = loop;
        return source;
    }

    private static Canvas EnsureLoadingScreen(Transform parent)
    {
        var existing = parent.Find("LoadingScreen");
        if (existing != null) return existing.GetComponent<Canvas>();

        var loadingGo = new GameObject("LoadingScreen");
        loadingGo.transform.SetParent(parent);
        var canvas = loadingGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = loadingGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        loadingGo.AddComponent<GraphicRaycaster>();
        loadingGo.SetActive(false);

        var group = loadingGo.AddComponent<CanvasGroup>();
        var view = loadingGo.AddComponent<LoadingView>();
        AssignPrivateField(view, "_canvasGroup", group);

        return canvas;
    }

    private static Canvas EnsureCanvas(string name)
    {
        var canvasGo = GameObject.Find(name);
        if (canvasGo == null) canvasGo = new GameObject(name);

        var canvas = canvasGo.GetComponent<Canvas>() ?? canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        var scaler = canvasGo.GetComponent<CanvasScaler>() ?? canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);

        if (canvasGo.GetComponent<GraphicRaycaster>() == null) canvasGo.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private static SettingPanelView EnsureSettingPanel(Transform parent)
    {
        var panel = EnsureRectTransform("SettingPanel", parent);
        var group = panel.GetComponent<CanvasGroup>() ?? panel.gameObject.AddComponent<CanvasGroup>();
        SetAnchors(panel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(720f, 900f));

        var bgImage = panel.GetComponent<Image>() ?? panel.gameObject.AddComponent<Image>();
        bgImage.color = new Color(0f, 0f, 0f, 0.75f);

        var panelRoot = EnsureRectTransform("PanelRoot", panel);
        SetAnchors(panelRoot, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(640f, 760f));
        var panelRootImg = panelRoot.GetComponent<Image>() ?? panelRoot.gameObject.AddComponent<Image>();
        panelRootImg.color = new Color(0.16f, 0.13f, 0.1f, 1f);

        var title = EnsureText("TitleText", panelRoot, "SETTINGS", 48, TextAlignmentOptions.Center);
        SetAnchors(title.rectTransform, new Vector2(0.5f, 0.9f), new Vector2(0.5f, 0.9f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(420f, 120f));

        var musicToggle = EnsureToggle("MusicToggle", panelRoot, "Music", new Vector2(0f, 170f));
        var sfxToggle = EnsureToggle("SfxToggle", panelRoot, "SFX", new Vector2(0f, 90f));

        var continueButton = EnsureButton("ContinueButton", panelRoot, "CONTINUE", new Vector2(0f, -40f));
        var replayButton = EnsureButton("ReplayButton", panelRoot, "REPLAY", new Vector2(0f, -150f));
        var mapButton = EnsureButton("MapButton", panelRoot, "MAP", new Vector2(0f, -260f));

        var view = panel.GetComponent<SettingPanelView>() ?? panel.gameObject.AddComponent<SettingPanelView>();
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

    private static void EnsureMainCamera()
    {
        var camObj = GameObject.Find("Main Camera");
        if (camObj == null)
        {
            camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
        }

        var cam = camObj.GetComponent<Camera>() ?? camObj.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 6f;

        if (camObj.GetComponent<AudioListener>() == null)
        {
            camObj.AddComponent<AudioListener>();
        }
    }

    private static void EnsureEventSystem()
    {
        var eventSystem = UnityEngine.Object.FindAnyObjectByType<EventSystem>();
        GameObject eventSystemGo;

        if (eventSystem == null)
        {
            eventSystemGo = new GameObject("EventSystem");
            eventSystem = eventSystemGo.AddComponent<EventSystem>();
        }
        else
        {
            eventSystemGo = eventSystem.gameObject;
        }

        EnsureCompatibleInputModule(eventSystemGo);
    }

    private static void EnsureCompatibleInputModule(GameObject eventSystemGo)
    {
        var standalone = eventSystemGo.GetComponent<StandaloneInputModule>();
        if (standalone != null)
        {
            UnityEngine.Object.DestroyImmediate(standalone);
        }

        var inputSystemUiType = Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
        if (inputSystemUiType != null)
        {
            if (eventSystemGo.GetComponent(inputSystemUiType) == null)
            {
                eventSystemGo.AddComponent(inputSystemUiType);
            }
            return;
        }

        if (eventSystemGo.GetComponent<StandaloneInputModule>() == null)
        {
            eventSystemGo.AddComponent<StandaloneInputModule>();
        }
    }

    private static RectTransform EnsureRectTransform(string name, Transform parent)
    {
        var child = parent.Find(name);
        if (child != null) return child as RectTransform;

        var go = new GameObject(name, typeof(RectTransform));
        var rect = go.GetComponent<RectTransform>();
        go.transform.SetParent(parent, false);
        return rect;
    }

    private static Button EnsureButton(string name, Transform parent, string text, Vector2 anchoredPosition)
    {
        var buttonRect = EnsureRectTransform(name, parent);
        SetAnchors(buttonRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), anchoredPosition, new Vector2(400f, 90f));

        var image = buttonRect.GetComponent<Image>() ?? buttonRect.gameObject.AddComponent<Image>();
        image.color = new Color(0.27f, 0.2f, 0.13f, 1f);
        var button = buttonRect.GetComponent<Button>() ?? buttonRect.gameObject.AddComponent<Button>();

        var label = EnsureText("Label", buttonRect, text, 34, TextAlignmentOptions.Center);
        SetAnchors(label.rectTransform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        return button;
    }

    private static Toggle EnsureToggle(string name, Transform parent, string labelText, Vector2 anchoredPosition)
    {
        var toggleRect = EnsureRectTransform(name, parent);
        SetAnchors(toggleRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), anchoredPosition, new Vector2(440f, 60f));
        var toggle = toggleRect.GetComponent<Toggle>() ?? toggleRect.gameObject.AddComponent<Toggle>();

        var bg = EnsureRectTransform("Background", toggleRect);
        SetAnchors(bg, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(24f, 0f), new Vector2(36f, 36f));
        var bgImage = bg.GetComponent<Image>() ?? bg.gameObject.AddComponent<Image>();
        bgImage.color = Color.white;

        var checkmark = EnsureRectTransform("Checkmark", bg);
        SetAnchors(checkmark, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        var checkImage = checkmark.GetComponent<Image>() ?? checkmark.gameObject.AddComponent<Image>();
        checkImage.color = Color.black;

        var label = EnsureText("Label", toggleRect, labelText, 32, TextAlignmentOptions.Left);
        SetAnchors(label.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0f, 0.5f), new Vector2(70f, 0f), new Vector2(-70f, 0f));

        toggle.targetGraphic = bgImage;
        toggle.graphic = checkImage;
        return toggle;
    }

    private static TextMeshProUGUI EnsureText(string name, Transform parent, string value, int size, TextAlignmentOptions alignment)
    {
        var textRect = EnsureRectTransform(name, parent);
        var text = textRect.GetComponent<TextMeshProUGUI>() ?? textRect.gameObject.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = size;
        text.alignment = alignment;
        text.color = Color.white;
        return text;
    }

    private static void SetAnchors(RectTransform rect, Vector2 min, Vector2 max, Vector2 pivot, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = sizeDelta;
    }

    private static void AssignPrivateField(UnityEngine.Object target, string fieldName, UnityEngine.Object value)
    {
        if (target == null) return;

        var serialized = new SerializedObject(target);
        var prop = serialized.FindProperty(fieldName);
        if (prop != null)
        {
            prop.objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
#endif