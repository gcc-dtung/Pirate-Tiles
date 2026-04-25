using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapView : MonoBehaviour
{
    [SerializeField] private Button _levelButtonPrefab;
    [SerializeField] private Transform _contentContainer;

    private List<Button> _spawnedButtons = new List<Button>();

    public void SetupLevels(int totalLevels, int unlockedLevel, System.Action<int> onLevelSelected)
    {
        foreach(var b in _spawnedButtons) Destroy(b.gameObject);
        _spawnedButtons.Clear();

        for (int i = 1; i <= totalLevels; i++)
        {
            var btn = Instantiate(_levelButtonPrefab, _contentContainer);
            int levelIndex = i;
            
            bool isUnlocked = i <= unlockedLevel;
            btn.interactable = isUnlocked;
            
            btn.onClick.AddListener(() => onLevelSelected?.Invoke(levelIndex));
            
            _spawnedButtons.Add(btn);
        }
    }
}
