using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class PowerUpBarView : MonoBehaviour
{
    [System.Serializable]
    public struct PowerUpButton
    {
        public PowerType Type;
        public Button Button;
        public TextMeshProUGUI CountText;
        public Image Icon;
    }

    [SerializeField] private List<PowerUpButton> _powerUpButtons;

    public event Action<PowerType> OnPowerUpClicked;

    private void Awake()
    {
        foreach (var p in _powerUpButtons)
        {
            var type = p.Type;
            p.Button.onClick.AddListener(() => OnPowerUpClicked?.Invoke(type));
        }
    }

    public void UpdatePowerUpCount(PowerType type, int count)
    {
        var p = _powerUpButtons.Find(x => x.Type == type);
        if (p.Button != null)
        {
            p.CountText.text = count.ToString();
            
            if (p.Icon != null)
            {
                p.Icon.color = count > 0 ? Color.white : new Color(1f, 1f, 1f, 0.5f);
            }
        }
    }
    public void SetButtonInteractable(PowerType type, bool interactable)
    {
        var p = _powerUpButtons.Find(x => x.Type == type);
        if (p.Button != null)
        {
            p.Button.interactable = interactable;
            if (p.Icon != null)
            {
                if (!interactable)
                {
                    p.Icon.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                }
                else
                {
                    int count = 0;
                    if (p.CountText != null) int.TryParse(p.CountText.text, out count);
                    p.Icon.color = count > 0 ? Color.white : new Color(1f, 1f, 1f, 0.5f);
                }
            }
        }
    }
}
