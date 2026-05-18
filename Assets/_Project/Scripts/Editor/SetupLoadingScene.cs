using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

public class SetupLoadingScene : EditorWindow
{
    [InitializeOnLoadMethod]
    private static void OnLoad()
    {
        if (SessionState.GetBool("SetupLoadingSceneDone", false)) return;
        SessionState.SetBool("SetupLoadingSceneDone", true);
        
        EditorApplication.delayCall += Setup;
    }

    [MenuItem("Tools/Setup Loading Scene")]
    public static void Setup()
    {
        var scene = EditorSceneManager.OpenScene("Assets/_Project/Scenes/Loading.unity");
        
        // Setup Canvas
        GameObject canvasGo = new GameObject("Canvas");
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920); // typical mobile portrait
        canvasGo.AddComponent<GraphicRaycaster>();
        
        // Background
        GameObject bgGo = new GameObject("Background");
        bgGo.transform.SetParent(canvasGo.transform, false);
        Image bgImg = bgGo.AddComponent<Image>();
        bgImg.color = new Color(0.1f, 0.1f, 0.15f, 1f); // dark blueish background
        RectTransform bgRect = bgGo.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        
        // LoadingView
        GameObject loadingViewGo = new GameObject("LoadingView");
        loadingViewGo.transform.SetParent(canvasGo.transform, false);
        RectTransform lvRect = loadingViewGo.AddComponent<RectTransform>();
        lvRect.anchorMin = Vector2.zero;
        lvRect.anchorMax = Vector2.one;
        lvRect.sizeDelta = Vector2.zero;
        
        CanvasGroup cg = loadingViewGo.AddComponent<CanvasGroup>();
        LoadingView loadingView = loadingViewGo.AddComponent<LoadingView>();
        
        // Slider Background
        GameObject sliderBgGo = new GameObject("SliderBackground");
        sliderBgGo.transform.SetParent(loadingViewGo.transform, false);
        Image sliderBgImg = sliderBgGo.AddComponent<Image>();
        sliderBgImg.color = new Color(0, 0, 0, 0.5f);
        RectTransform sliderBgRect = sliderBgGo.GetComponent<RectTransform>();
        sliderBgRect.anchorMin = new Vector2(0.1f, 0.2f);
        sliderBgRect.anchorMax = new Vector2(0.9f, 0.2f);
        sliderBgRect.anchoredPosition = new Vector2(0, 0);
        sliderBgRect.sizeDelta = new Vector2(0, 40);
        
        // Slider Fill Area
        GameObject fillAreaGo = new GameObject("Fill Area");
        fillAreaGo.transform.SetParent(sliderBgGo.transform, false);
        RectTransform fillAreaRect = fillAreaGo.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.sizeDelta = Vector2.zero;
        
        // Slider Fill
        GameObject fillGo = new GameObject("Fill");
        fillGo.transform.SetParent(fillAreaGo.transform, false);
        Image fillImg = fillGo.AddComponent<Image>();
        fillImg.color = Color.white;
        RectTransform fillRect = fillGo.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        
        // Slider Component
        Slider slider = sliderBgGo.AddComponent<Slider>();
        slider.fillRect = fillRect;
        slider.interactable = false;
        slider.transition = Selectable.Transition.None;
        
        // Boat Image (chạy theo thanh loading)
        GameObject boatGo = new GameObject("BoatImage");
        boatGo.transform.SetParent(loadingViewGo.transform, false);
        Image boatImg = boatGo.AddComponent<Image>();
        boatImg.color = Color.white; // Sprite sẽ gán thủ công sau
        RectTransform boatRect = boatGo.GetComponent<RectTransform>();
        boatRect.sizeDelta = new Vector2(120f, 80f); // kích thước mặc định, chỉnh lại cho phù hợp sprite
        // Đặt pivot về chính giữa đáy để thuyền "ngồi" trên thanh bar
        boatRect.pivot = new Vector2(0.5f, 0f);
        // Vị trí ban đầu: khớp với cạnh trái slider (sẽ bị override khi game chạy)
        boatRect.anchoredPosition = new Vector2(
            sliderBgRect.anchoredPosition.x - (sliderBgRect.sizeDelta.x * 0.4f),
            sliderBgRect.anchoredPosition.y + 20f
        );

        // Setup LoadingView references
        SerializedObject so = new SerializedObject(loadingView);
        so.FindProperty("_canvasGroup").objectReferenceValue = cg;
        so.FindProperty("_progressBar").objectReferenceValue = slider;
        so.FindProperty("_boatImage").objectReferenceValue = boatRect;
        so.ApplyModifiedProperties();
        
        // LoadingSceneController
        GameObject controllerGo = new GameObject("LoadingSceneController");
        LoadingSceneController controller = controllerGo.AddComponent<LoadingSceneController>();
        SerializedObject cso = new SerializedObject(controller);
        cso.FindProperty("_loadingView").objectReferenceValue = loadingView;
        cso.ApplyModifiedProperties();
        // Đã xóa phần tạo EventSystem vì Loading Scene không cần tương tác UI, tránh lỗi InputSystem
        
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Loading Scene Setup Complete!");
    }
}
