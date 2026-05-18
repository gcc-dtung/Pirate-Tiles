#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LevelGeneratorWindow : EditorWindow
{
    [MenuItem("Pirate Tiles/Level Generator")]
    public static void ShowWindow()
    {
        GetWindow<LevelGeneratorWindow>("Level Generator");
    }

    private int levelIndex = 2;
    private float timeLimit = 300f;
    private int maxStackSize = 7;
    
    public enum LayoutType { Pyramid, Rectangle, Diamond }
    private LayoutType layoutType = LayoutType.Pyramid;
    
    private int baseWidth = 6;
    private int baseHeight = 6;
    private int maxLayers = 4;
    private float spacingX = 1.0f; // Khoảng cách X giữa các tile cùng layer
    private float spacingY = 1.0f; // Khoảng cách Y giữa các tile cùng layer
    private float layerOffsetX = 0.5f; // Độ lệch X của layer trên so với layer dưới
    private float layerOffsetY = 0.5f; // Độ lệch Y của layer trên so với layer dưới (để 0.5 thì tháp sẽ căn giữa)
    private bool randomShuffle = true;

    private bool useBounds = false;
    private float minBoundX = -3.5f;
    private float maxBoundX = 3.5f;
    private float minBoundY = -4.5f;
    private float maxBoundY = 3.5f;

    private void OnGUI()
    {
        GUILayout.Label("Level Settings", EditorStyles.boldLabel);
        levelIndex = EditorGUILayout.IntField("Level Index", levelIndex);
        timeLimit = EditorGUILayout.FloatField("Time Limit", timeLimit);
        maxStackSize = EditorGUILayout.IntField("Max Stack Size", maxStackSize);
        
        GUILayout.Space(10);
        GUILayout.Label("Layout Generator", EditorStyles.boldLabel);
        layoutType = (LayoutType)EditorGUILayout.EnumPopup("Layout Type", layoutType);
        baseWidth = EditorGUILayout.IntField("Base Width", baseWidth);
        baseHeight = EditorGUILayout.IntField("Base Height", baseHeight);
        maxLayers = EditorGUILayout.IntField("Max Layers", maxLayers);
        
        spacingX = EditorGUILayout.FloatField("Grid Spacing X", spacingX);
        spacingY = EditorGUILayout.FloatField("Grid Spacing Y", spacingY);
        layerOffsetX = EditorGUILayout.FloatField("Layer Offset X", layerOffsetX);
        layerOffsetY = EditorGUILayout.FloatField("Layer Offset Y", layerOffsetY);
        randomShuffle = EditorGUILayout.Toggle("Random Shuffle Types", randomShuffle);
        
        GUILayout.Space(10);
        GUILayout.Label("Placement Bounds", EditorStyles.boldLabel);
        useBounds = EditorGUILayout.Toggle("Use Bounds Limit", useBounds);
        if (useBounds)
        {
            minBoundX = EditorGUILayout.FloatField("Min X", minBoundX);
            maxBoundX = EditorGUILayout.FloatField("Max X", maxBoundX);
            minBoundY = EditorGUILayout.FloatField("Min Y", minBoundY);
            maxBoundY = EditorGUILayout.FloatField("Max Y", maxBoundY);
        }

        GUILayout.Space(20);
        if (GUILayout.Button("Generate and Save LevelConfigSO", GUILayout.Height(40)))
        {
            GenerateLevel();
        }
    }

    private void GenerateLevel()
    {
        LevelConfigSO newLevel = ScriptableObject.CreateInstance<LevelConfigSO>();
        
        List<LevelTileData> generatedTiles = GenerateLayout();
        
        // Đảm bảo số lượng tile chia hết cho 3
        int remainder = generatedTiles.Count % 3;
        if (remainder != 0)
        {
            // Bỏ bớt vài tile ở layer cao nhất (đang ở cuối danh sách)
            generatedTiles.RemoveRange(generatedTiles.Count - remainder, remainder);
        }
        
        // Gán CardType ngẫu nhiên nhưng theo bộ 3
        if (randomShuffle)
        {
            AssignRandomCardTypes(generatedTiles);
        }
        else
        {
            AssignSequentialCardTypes(generatedTiles);
        }

        // Lưu tài nguyên
        newLevel.SetupForEditor(levelIndex, timeLimit, maxStackSize, generatedTiles);

        string folderPath = "Assets/_Project/Resources/SO/LevelConfig";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
            AssetDatabase.Refresh();
        }

        string path = $"{folderPath}/LevelConfig_{levelIndex:00}.asset";
        
        // Nếu đã tồn tại file thì ghi đè một phần tử mới vào asset
        LevelConfigSO existing = AssetDatabase.LoadAssetAtPath<LevelConfigSO>(path);
        if (existing != null)
        {
            existing.SetupForEditor(levelIndex, timeLimit, maxStackSize, generatedTiles);
            EditorUtility.SetDirty(existing);
            newLevel = existing;
        }
        else
        {
            AssetDatabase.CreateAsset(newLevel, path);
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newLevel;
        
        Debug.Log($"Generated level {levelIndex} with {generatedTiles.Count} tiles at {path}");
    }

    private List<LevelTileData> GenerateLayout()
    {
        List<LevelTileData> tiles = new List<LevelTileData>();

        for (int layer = 0; layer < maxLayers; layer++)
        {
            int currentWidth = baseWidth;
            int currentHeight = baseHeight;

            if (layoutType == LayoutType.Pyramid)
            {
                currentWidth = baseWidth - layer;
                currentHeight = baseHeight - layer;
            }
            else if (layoutType == LayoutType.Diamond)
            {
                currentWidth = baseWidth - layer * 2;
                currentHeight = baseHeight - layer * 2;
            }

            if (currentWidth <= 0 || currentHeight <= 0) break;

            // Lấy baseWidth và baseHeight làm mốc gốc cho tất cả các layer để tránh việc tự động dịch tâm gây sai lệch.
            // Nhờ đó lưới tọa độ của layer 0, 1, 2... luôn đồng bộ với nhau.
            // Sau đó cộng thêm offset do user tự định nghĩa.
            float startX = -(baseWidth - 1) * spacingX / 2f + (layer * layerOffsetX);
            float startY = -(baseHeight - 1) * spacingY / 2f + (layer * layerOffsetY);

            for (int x = 0; x < currentWidth; x++)
            {
                for (int y = 0; y < currentHeight; y++)
                {
                    Vector2 pos = new Vector2(startX + x * spacingX, startY + y * spacingY);
                    
                    if (useBounds)
                    {
                        if (pos.x < minBoundX || pos.x > maxBoundX || pos.y < minBoundY || pos.y > maxBoundY)
                        {
                            continue;
                        }
                    }

                    LevelTileData tileData = new LevelTileData
                    {
                        GridPosition = pos,
                        Size = Vector2.one,
                        LayerIndex = layer,
                        Type = CardType.None 
                    };
                    
                    tiles.Add(tileData);
                }
            }
        }

        return tiles;
    }

    private List<CardType> GetAvailableCardTypesWithSprites()
    {
        List<CardType> validTypes = new List<CardType>();
        string[] guids = AssetDatabase.FindAssets("t:TileDatabaseSO");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            TileDatabaseSO db = AssetDatabase.LoadAssetAtPath<TileDatabaseSO>(path);
            if (db != null && db.TileVisuals != null)
            {
                foreach (var visual in db.TileVisuals)
                {
                    if (visual.Sprite != null && (int)visual.Type > 0 && (int)visual.Type < 100)
                    {
                        if (!validTypes.Contains(visual.Type))
                        {
                            validTypes.Add(visual.Type);
                        }
                    }
                }
            }
        }

        if (validTypes.Count == 0)
        {
            Debug.LogWarning("Level Generator: Không tìm thấy TileDatabaseSO hoặc không có sprite nào được gán cho normal tiles. Dùng mặc định từ 1 đến 13.");
            for (int i = 1; i <= 13; i++) 
            {
                validTypes.Add((CardType)i);
            }
        }
        
        return validTypes;
    }

    private void AssignRandomCardTypes(List<LevelTileData> tiles)
    {
        List<CardType> availableTypes = GetAvailableCardTypesWithSprites();

        List<CardType> assignedDeck = new List<CardType>();
        int setsOfThree = tiles.Count / 3;

        for (int i = 0; i < setsOfThree; i++)
        {
            CardType randomType = availableTypes[Random.Range(0, availableTypes.Count)];
            assignedDeck.Add(randomType);
            assignedDeck.Add(randomType);
            assignedDeck.Add(randomType);
        }

        // Shuffle deck (Fisher-Yates)
        System.Random rng = new System.Random();
        int n = assignedDeck.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            CardType value = assignedDeck[k];
            assignedDeck[k] = assignedDeck[n];
            assignedDeck[n] = value;
        }

        // Gán vào tiles
        for (int i = 0; i < tiles.Count; i++)
        {
            var data = tiles[i];
            data.Type = assignedDeck[i];
            tiles[i] = data; 
        }
    }
    
    private void AssignSequentialCardTypes(List<LevelTileData> tiles)
    {
        List<CardType> availableTypes = GetAvailableCardTypesWithSprites();

        int typeIndex = 0;
        for (int i = 0; i < tiles.Count; i += 3)
        {
            CardType currentType = availableTypes[typeIndex % availableTypes.Count];
            typeIndex++;
            
            for (int j = 0; j < 3; j++)
            {
                if (i + j < tiles.Count)
                {
                    var data = tiles[i + j];
                    data.Type = currentType;
                    tiles[i + j] = data;
                }
            }
        }
    }
}
#endif