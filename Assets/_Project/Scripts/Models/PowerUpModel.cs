using System.Collections.Generic;

public class PowerUpModel
{
    private Dictionary<PowerType, int> _counts;

    public PowerUpModel()
    {
        _counts = new Dictionary<PowerType, int>();
    }

    public void SetCount(PowerType type, int count)
    {
        _counts[type] = count;
    }

    public int GetCount(PowerType type)
    {
        return _counts.TryGetValue(type, out int count) ? count : 0;
    }

    public void UsePowerUp(PowerType type)
    {
        if (GetCount(type) > 0)
        {
            _counts[type]--;
        }
    }

    public void AddPowerUp(PowerType type, int amount = 1)
    {
        _counts[type] = GetCount(type) + amount;
    }
}
