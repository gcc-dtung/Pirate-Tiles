public class GameStateModel
{
    public GamePhase Phase { get; set; }
    public bool IsProcessingTile { get; set; }
    public bool IsUsingPower { get; set; }
    public bool IsAnimating { get; set; }

    public bool CanInteract =>
        Phase == GamePhase.Playing
        && !IsProcessingTile
        && !IsUsingPower
        && !IsAnimating;
}
