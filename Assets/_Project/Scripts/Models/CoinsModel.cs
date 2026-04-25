public class CoinsModel
{
    public int CurrentCoins { get; set; }
    public int PowerCost { get; set; }

    public void AddCoins(int amount)
    {
        CurrentCoins += amount;
    }

    public bool ConsumeCoins(int amount)
    {
        if (CurrentCoins >= amount)
        {
            CurrentCoins -= amount;
            return true;
        }
        return false;
    }
}
