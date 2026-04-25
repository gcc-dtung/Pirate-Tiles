using NUnit.Framework;

[TestFixture]
public class SupportModelsTests
{
    [Test]
    public void GameStateModel_CanInteract_ReturnsTrue_WhenPlayingAndNotProcessing()
    {
        var model = new GameStateModel();
        model.Phase = GamePhase.Playing;
        model.IsProcessingTile = false;
        model.IsUsingPower = false;
        model.IsAnimating = false;

        Assert.IsTrue(model.CanInteract);
    }

    [Test]
    public void GameStateModel_CanInteract_ReturnsFalse_WhenNotPlaying()
    {
        var model = new GameStateModel();
        model.Phase = GamePhase.Paused;
        model.IsProcessingTile = false;

        Assert.IsFalse(model.CanInteract);
    }

    [Test]
    public void TileHistoryModel_RecordAndPop_WorksCorrectly()
    {
        var model = new TileHistoryModel();
        model.RecordMove(10);
        model.RecordMove(20);

        Assert.AreEqual(2, model.Count);
        Assert.IsTrue(model.HasHistory);
        
        Assert.AreEqual(20, model.PopLastMove());
        Assert.AreEqual(10, model.PopLastMove());
        
        Assert.IsFalse(model.HasHistory);
        Assert.AreEqual(-1, model.PopLastMove()); // Hết lịch sử trả về -1
    }

    [Test]
    public void PowerUpModel_AddAndUsePowerUp_UpdatesCounts()
    {
        var model = new PowerUpModel();
        
        model.AddPowerUp(PowerType.Undo, 2);
        Assert.AreEqual(2, model.GetCount(PowerType.Undo));

        model.UsePowerUp(PowerType.Undo);
        Assert.AreEqual(1, model.GetCount(PowerType.Undo));

        model.UsePowerUp(PowerType.Undo);
        model.UsePowerUp(PowerType.Undo); // Cố gắng dùng khi hết thì không làm số lượng bị âm
        Assert.AreEqual(0, model.GetCount(PowerType.Undo));
    }

    [Test]
    public void HeartsModel_AddAndConsumeHeart_RespectsBounds()
    {
        var model = new HeartsModel { MaxHearts = 5, CurrentHearts = 4 };

        model.AddHeart();
        Assert.AreEqual(5, model.CurrentHearts);
        Assert.IsTrue(model.IsFull);

        model.AddHeart(); // Không được vượt quá max
        Assert.AreEqual(5, model.CurrentHearts);

        model.ConsumeHeart();
        model.ConsumeHeart();
        Assert.AreEqual(3, model.CurrentHearts);
    }

    [Test]
    public void CoinsModel_ConsumeCoins_ReturnsTrueIfEnough()
    {
        var model = new CoinsModel { CurrentCoins = 100 };

        model.AddCoins(50);
        Assert.AreEqual(150, model.CurrentCoins);

        bool success = model.ConsumeCoins(100);
        Assert.IsTrue(success);
        Assert.AreEqual(50, model.CurrentCoins);

        bool fail = model.ConsumeCoins(100);
        Assert.IsFalse(fail); // Không đủ tiền
        Assert.AreEqual(50, model.CurrentCoins); // Số dư không đổi
    }

    [Test]
    public void LevelModel_IsTimeUp_ReturnsCorrectly()
    {
        var model = new LevelModel();
        
        model.CurrentTimeRemaining = 10f;
        Assert.IsFalse(model.IsTimeUp);

        model.CurrentTimeRemaining = 0f;
        Assert.IsTrue(model.IsTimeUp);

        model.CurrentTimeRemaining = -1f;
        Assert.IsTrue(model.IsTimeUp);
    }
}
