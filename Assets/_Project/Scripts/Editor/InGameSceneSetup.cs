using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InGameSceneSetup : EditorWindow
{
    [MenuItem("Pirate Tiles/Setup InGame Scene")]
    public static void SetupScene()
    {
        EnsureEventSystem();

        // 1. GameManager
        var gmObj = new GameObject("GameManager");
        gmObj.AddComponent<GameManager>();
        gmObj.AddComponent<SaveService>();
        
        var sceneService = gmObj.AddComponent<SceneService>();
        var audioService = gmObj.AddComponent<AudioService>();
        
        var bgmSource = gmObj.AddComponent<AudioSource>();
        bgmSource.loop = true;
        var sfxSource = gmObj.AddComponent<AudioSource>();
        sfxSource.loop = false;
        
        var soAudio = new SerializedObject(audioService);
        soAudio.FindProperty("_bgmSource").objectReferenceValue = bgmSource;
        soAudio.FindProperty("_sfxSource").objectReferenceValue = sfxSource;
        soAudio.ApplyModifiedProperties();

        var loadingCanvas = new GameObject("LoadingScreen").AddComponent<Canvas>();
        loadingCanvas.transform.SetParent(gmObj.transform);
        loadingCanvas.gameObject.SetActive(false);
        var loadingView = loadingCanvas.gameObject.AddComponent<LoadingView>();
        
        var soScene = new SerializedObject(sceneService);
        soScene.FindProperty("_loadingScreen").objectReferenceValue = loadingCanvas.gameObject;
        soScene.ApplyModifiedProperties();

        // 2. Camera
        var camObj = GameObject.Find("Main Camera");
        if (camObj == null)
        {
            camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
        }
        var cam = camObj.GetComponent<Camera>() ?? camObj.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 6f;

        // 3. Controllers
        var levelCtrlObj = new GameObject("LevelController");
        var levelCtrl = levelCtrlObj.AddComponent<LevelController>();

        var gameConfig = AssetDatabase.LoadAssetAtPath<GameConfigSO>("Assets/_Project/Scripts/Data/SO/GameConfig.asset");
        if (gameConfig != null)
        {
            var so = new SerializedObject(levelCtrl);
            so.FindProperty("_gameConfig").objectReferenceValue = gameConfig;
            so.ApplyModifiedProperties();
        }

        var gameCtrlObj = new GameObject("GameController");
        var gameCtrl = gameCtrlObj.AddComponent<GameController>();

        var boardObj = new GameObject("Board");
        var boardCtrl = boardObj.AddComponent<BoardController>();
        var boardView = boardObj.AddComponent<BoardView>();

        var stackObj = new GameObject("Stack");
        var stackCtrl = stackObj.AddComponent<StackController>();
        var stackView = stackObj.AddComponent<StackView>();
        
        var stackBg = new GameObject("StackBackground").AddComponent<SpriteRenderer>();
        stackBg.transform.SetParent(stackObj.transform);
        var slotsObj = new GameObject("SlotPositions");
        slotsObj.transform.SetParent(stackObj.transform);
        
        var soStackView = new SerializedObject(stackView);
        soStackView.FindProperty("_stackContainer").objectReferenceValue = stackBg.transform;
        
        SerializedProperty slotPositionsProp = soStackView.FindProperty("_slots");
        slotPositionsProp.arraySize = 8;
        for(int i=0; i<8; i++)
        {
            var slot = new GameObject($"Slot_{i}");
            slot.transform.SetParent(slotsObj.transform);
            slot.transform.localPosition = new Vector3(-3f + i*1f, 0, 0);
            
            var bgRenderer = slot.AddComponent<SpriteRenderer>();
            
            var lockObj = new GameObject("LockIcon");
            lockObj.transform.SetParent(slot.transform);
            lockObj.transform.localPosition = Vector3.zero;
            var lockRenderer = lockObj.AddComponent<SpriteRenderer>();

            var slotProp = slotPositionsProp.GetArrayElementAtIndex(i);
            slotProp.FindPropertyRelative("SlotTransform").objectReferenceValue = slot.transform;
            slotProp.FindPropertyRelative("Background").objectReferenceValue = bgRenderer;
            slotProp.FindPropertyRelative("LockIcon").objectReferenceValue = lockRenderer;
        }
        soStackView.ApplyModifiedProperties();

        var timerCtrlObj = new GameObject("TimerController");
        var timerCtrl = timerCtrlObj.AddComponent<TimerController>();

        var powerUpCtrlObj = new GameObject("PowerUpController");
        var powerUpCtrl = powerUpCtrlObj.AddComponent<PowerUpController>();

        var heartsCtrlObj = new GameObject("HeartsController");
        var heartsCtrl = heartsCtrlObj.AddComponent<HeartsController>();

        var coinsCtrlObj = new GameObject("CoinsController");
        var coinsCtrl = coinsCtrlObj.AddComponent<CoinsController>();

        var audioCtrlObj = new GameObject("AudioController");
        var audioCtrl = audioCtrlObj.AddComponent<AudioController>();

        var tutorialCtrlObj = new GameObject("TutorialController");
        var tutorialCtrl = tutorialCtrlObj.AddComponent<TutorialController>();

        // Load SOs
        var levelConfig = AssetDatabase.LoadAssetAtPath<LevelConfigSO>("Assets/_Project/Resources/SO/LevelConfig_01.asset");
        var tileDatabase = AssetDatabase.LoadAssetAtPath<TileDatabaseSO>("Assets/_Project/Resources/SO/TileDatabase.asset");
        
        var soLevel = new SerializedObject(levelCtrl);
        soLevel.FindProperty("_currentLevelConfig").objectReferenceValue = levelConfig;
        soLevel.FindProperty("_tileDatabase").objectReferenceValue = tileDatabase;
        soLevel.FindProperty("_boardController").objectReferenceValue = boardCtrl;
        soLevel.FindProperty("_stackController").objectReferenceValue = stackCtrl;
        soLevel.FindProperty("_timerController").objectReferenceValue = timerCtrl;
        soLevel.ApplyModifiedProperties();

        // Load Channels
        var tileSelectedChannel = AssetDatabase.LoadAssetAtPath<TileSelectedChannelSO>("Assets/_Project/Resources/EventChannels/TileSelectedChannel.asset");
        var boardClearedChannel = AssetDatabase.LoadAssetAtPath<VoidEventChannelSO>("Assets/_Project/Resources/EventChannels/BoardClearedChannel.asset");
        var undoRequestChannel = AssetDatabase.LoadAssetAtPath<VoidEventChannelSO>("Assets/_Project/Resources/EventChannels/UndoRequestChannel.asset");
        var tileMatchedChannel = AssetDatabase.LoadAssetAtPath<VoidEventChannelSO>("Assets/_Project/Resources/EventChannels/TileMatchedChannel.asset");
        var stackFullChannel = AssetDatabase.LoadAssetAtPath<VoidEventChannelSO>("Assets/_Project/Resources/EventChannels/StackFullChannel.asset");
        var timerExpiredChannel = AssetDatabase.LoadAssetAtPath<VoidEventChannelSO>("Assets/_Project/Resources/EventChannels/TimerExpiredChannel.asset");
        var gameWonChannel = AssetDatabase.LoadAssetAtPath<VoidEventChannelSO>("Assets/_Project/Resources/EventChannels/GameWonChannel.asset");
        var gameLostChannel = AssetDatabase.LoadAssetAtPath<VoidEventChannelSO>("Assets/_Project/Resources/EventChannels/GameLostChannel.asset");
        var powerUpUsedChannel = AssetDatabase.LoadAssetAtPath<PowerTypeEventChannelSO>("Assets/_Project/Resources/EventChannels/PowerUpUsedChannel.asset");
        var outOfHeartsChannel = AssetDatabase.LoadAssetAtPath<VoidEventChannelSO>("Assets/_Project/Resources/EventChannels/OutOfHeartsChannel.asset");
        var shuffleCompletedChannel = AssetDatabase.LoadAssetAtPath<VoidEventChannelSO>("Assets/_Project/Resources/EventChannels/ShuffleCompletedChannel.asset");
        var magicRequestChannel = AssetDatabase.LoadAssetAtPath<VoidEventChannelSO>("Assets/_Project/Resources/SO_MagicRequest.asset");
        var addOneCellRequestChannel = AssetDatabase.LoadAssetAtPath<VoidEventChannelSO>("Assets/_Project/Resources/SO_AddOneCellRequest.asset");

        var soBoardCtrl = new SerializedObject(boardCtrl);
        soBoardCtrl.FindProperty("_boardView").objectReferenceValue = boardView;
        soBoardCtrl.FindProperty("_tileSelectedChannel").objectReferenceValue = tileSelectedChannel;
        soBoardCtrl.FindProperty("_boardClearedChannel").objectReferenceValue = boardClearedChannel;
        soBoardCtrl.FindProperty("_undoRequestChannel").objectReferenceValue = undoRequestChannel;
        soBoardCtrl.FindProperty("_shuffleRequestChannel").objectReferenceValue = shuffleCompletedChannel;
        soBoardCtrl.FindProperty("_magicRequestChannel").objectReferenceValue = magicRequestChannel;
        soBoardCtrl.FindProperty("_tilesMatchedChannel").objectReferenceValue = tileMatchedChannel;
        soBoardCtrl.ApplyModifiedProperties();

        var soStackCtrl = new SerializedObject(stackCtrl);
        soStackCtrl.FindProperty("_stackView").objectReferenceValue = stackView;
        soStackCtrl.FindProperty("_tileSelectedChannel").objectReferenceValue = tileSelectedChannel;
        soStackCtrl.FindProperty("_tilesMatchedChannel").objectReferenceValue = tileMatchedChannel;
        soStackCtrl.FindProperty("_stackFullChannel").objectReferenceValue = stackFullChannel;
        soStackCtrl.FindProperty("_undoRequestChannel").objectReferenceValue = undoRequestChannel;
        soStackCtrl.FindProperty("_addOneCellRequestChannel").objectReferenceValue = addOneCellRequestChannel;
        soStackCtrl.ApplyModifiedProperties();
        
        var soAudioCtrl = new SerializedObject(audioCtrl);
        soAudioCtrl.FindProperty("_tilesMatchedChannel").objectReferenceValue = tileMatchedChannel;
        soAudioCtrl.FindProperty("_gameWonChannel").objectReferenceValue = gameWonChannel;
        soAudioCtrl.FindProperty("_gameLostChannel").objectReferenceValue = gameLostChannel;
        soAudioCtrl.ApplyModifiedProperties();

        // 4. UI Canvas
        var canvasObj = new GameObject("UI_Canvas");
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1080, 1920);
        canvasObj.AddComponent<GraphicRaycaster>();

        var topBarObj = new GameObject("TopBar", typeof(RectTransform));
        topBarObj.transform.SetParent(canvasObj.transform, false);
        var heartsView = new GameObject("HeartsView").AddComponent<HeartsView>();
        heartsView.transform.SetParent(topBarObj.transform, false);
        var coinsView = new GameObject("CoinsView").AddComponent<CoinsView>();
        coinsView.transform.SetParent(topBarObj.transform, false);
        var timerView = new GameObject("TimerView").AddComponent<TimerView>();
        timerView.transform.SetParent(topBarObj.transform, false);

        var soTimerCtrl = new SerializedObject(timerCtrl);
        soTimerCtrl.FindProperty("_timerView").objectReferenceValue = timerView;
        soTimerCtrl.FindProperty("_timerExpiredChannel").objectReferenceValue = timerExpiredChannel;
        soTimerCtrl.ApplyModifiedProperties();

        var powerUpBarObj = new GameObject("PowerUpBar", typeof(RectTransform));
        powerUpBarObj.transform.SetParent(canvasObj.transform, false);
        var powerUpBarView = powerUpBarObj.AddComponent<PowerUpBarView>();

        var winPanelObj = new GameObject("WinPanel", typeof(RectTransform));
        winPanelObj.transform.SetParent(canvasObj.transform, false);
        var winPanelView = winPanelObj.AddComponent<WinPanelView>();
        winPanelObj.SetActive(false);

        var losePanelObj = new GameObject("LosePanel", typeof(RectTransform));
        losePanelObj.transform.SetParent(canvasObj.transform, false);
        var losePanelView = losePanelObj.AddComponent<LosePanelView>();
        losePanelObj.SetActive(false);

        var spendCoinsPanelObj = new GameObject("SpendCoinsPanel", typeof(RectTransform));
        spendCoinsPanelObj.transform.SetParent(canvasObj.transform, false);
        var spendCoinsPanelView = spendCoinsPanelObj.AddComponent<SpendCoinsPanelView>();
        spendCoinsPanelObj.SetActive(false);

        var outOfHeartPanelObj = new GameObject("OutOfHeartPanel", typeof(RectTransform));
        outOfHeartPanelObj.transform.SetParent(canvasObj.transform, false);
        var outOfHeartPanelView = outOfHeartPanelObj.AddComponent<OutOfHeartPanelView>();
        outOfHeartPanelObj.SetActive(false);

        var settingPanelObj = new GameObject("SettingPanel", typeof(RectTransform));
        settingPanelObj.transform.SetParent(canvasObj.transform, false);
        var settingPanelView = settingPanelObj.AddComponent<SettingPanelView>();
        settingPanelObj.SetActive(false);

        var tutorialPanelObj = new GameObject("TutorialPanel", typeof(RectTransform));
        tutorialPanelObj.transform.SetParent(canvasObj.transform, false);
        var tutorialView = tutorialPanelObj.AddComponent<TutorialView>();
        tutorialPanelObj.SetActive(false);

        var soGameCtrl = new SerializedObject(gameCtrl);
        soGameCtrl.FindProperty("_boardController").objectReferenceValue = boardCtrl;
        soGameCtrl.FindProperty("_stackController").objectReferenceValue = stackCtrl;
        soGameCtrl.FindProperty("_powerUpController").objectReferenceValue = powerUpCtrl;
        soGameCtrl.FindProperty("_timerController").objectReferenceValue = timerCtrl;
        soGameCtrl.FindProperty("_tilesMatchedChannel").objectReferenceValue = tileMatchedChannel;
        soGameCtrl.FindProperty("_stackFullChannel").objectReferenceValue = stackFullChannel;
        soGameCtrl.FindProperty("_timerExpiredChannel").objectReferenceValue = timerExpiredChannel;
        soGameCtrl.FindProperty("_gameWonChannel").objectReferenceValue = gameWonChannel;
        soGameCtrl.FindProperty("_gameLostChannel").objectReferenceValue = gameLostChannel;
        soGameCtrl.FindProperty("_boardClearedChannel").objectReferenceValue = boardClearedChannel;
        soGameCtrl.FindProperty("_winPanelView").objectReferenceValue = winPanelView;
        soGameCtrl.FindProperty("_losePanelView").objectReferenceValue = losePanelView;
        soGameCtrl.ApplyModifiedProperties();

        var soPowerUpCtrl = new SerializedObject(powerUpCtrl);
        soPowerUpCtrl.FindProperty("_powerUpBarView").objectReferenceValue = powerUpBarView;
        soPowerUpCtrl.FindProperty("_spendCoinsPanelView").objectReferenceValue = spendCoinsPanelView;
        soPowerUpCtrl.FindProperty("_spendCoinsRequestChannel").objectReferenceValue = powerUpUsedChannel;
        soPowerUpCtrl.FindProperty("_undoRequestChannel").objectReferenceValue = undoRequestChannel;
        soPowerUpCtrl.FindProperty("_shuffleRequestChannel").objectReferenceValue = shuffleCompletedChannel;
        soPowerUpCtrl.FindProperty("_magicRequestChannel").objectReferenceValue = magicRequestChannel;
        soPowerUpCtrl.FindProperty("_addOneCellRequestChannel").objectReferenceValue = addOneCellRequestChannel;
        soPowerUpCtrl.ApplyModifiedProperties();

        var soHeartsCtrl = new SerializedObject(heartsCtrl);
        soHeartsCtrl.FindProperty("_heartsView").objectReferenceValue = heartsView;
        soHeartsCtrl.FindProperty("_outOfHeartPanelView").objectReferenceValue = outOfHeartPanelView;
        soHeartsCtrl.FindProperty("_outOfHeartsChannel").objectReferenceValue = outOfHeartsChannel;
        soHeartsCtrl.ApplyModifiedProperties();

        var soCoinsCtrl = new SerializedObject(coinsCtrl);
        soCoinsCtrl.FindProperty("_coinsView").objectReferenceValue = coinsView;
        soCoinsCtrl.FindProperty("_powerUpController").objectReferenceValue = powerUpCtrl;
        soCoinsCtrl.FindProperty("_spendCoinsRequestChannel").objectReferenceValue = powerUpUsedChannel;
        soCoinsCtrl.ApplyModifiedProperties();

        var soTutorialCtrl = new SerializedObject(tutorialCtrl);
        soTutorialCtrl.FindProperty("_tutorialView").objectReferenceValue = tutorialView;
        soTutorialCtrl.ApplyModifiedProperties();

        Debug.Log("InGame Scene Setup Complete!");
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
}
