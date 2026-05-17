using System;

public class HeartsModel
{
    public int CurrentHearts { get; set; }
    public int MaxHearts { get; set; }
    public float HealTime { get; set; } = 10f; // 10 giây / tim
    public DateTime LastHealTime { get; set; }
    public float TimeUntilNextHeal { get; private set; }

    public bool IsFull => CurrentHearts >= MaxHearts;

    public void AddHeart()
    {
        if (!IsFull) CurrentHearts++;
    }

    public void ConsumeHeart()
    {
        if (CurrentHearts > 0)
        {
            if (IsFull) LastHealTime = DateTime.Now; // Bắt đầu tính giờ từ lúc bị mất tim đầu tiên
            CurrentHearts--;
        }
    }

    public bool CalculateRegeneration(DateTime currentTime)
    {
        if (IsFull)
        {
            TimeUntilNextHeal = 0;
            return false;
        }

        TimeSpan timePassed = currentTime - LastHealTime;
        int heartsToRecover = (int)(timePassed.TotalSeconds / HealTime);

        if (heartsToRecover > 0)
        {
            CurrentHearts += heartsToRecover;
            if (CurrentHearts >= MaxHearts)
            {
                CurrentHearts = MaxHearts;
                TimeUntilNextHeal = 0;
                LastHealTime = currentTime;
            }
            else
            {
                LastHealTime = LastHealTime.AddSeconds(heartsToRecover * HealTime);
                TimeUntilNextHeal = HealTime - (float)(currentTime - LastHealTime).TotalSeconds;
            }
            return true; // Trả về true nếu có mạng được hồi
        }
        else
        {
            TimeUntilNextHeal = HealTime - (float)timePassed.TotalSeconds;
            return false;
        }
    }
}
