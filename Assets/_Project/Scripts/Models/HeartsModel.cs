public class HeartsModel
{
    public int CurrentHearts { get; set; }
    public int MaxHearts { get; set; }
    public float HealTime { get; set; } // Thời gian hồi 1 tim (giây)
    public long LastHealTimestamp { get; set; } // Lưu dưới dạng UNIX timestamp hoặc binary ticks

    public bool IsFull => CurrentHearts >= MaxHearts;

    public void AddHeart()
    {
        if (!IsFull) CurrentHearts++;
    }

    public void ConsumeHeart()
    {
        if (CurrentHearts > 0) CurrentHearts--;
    }
}
