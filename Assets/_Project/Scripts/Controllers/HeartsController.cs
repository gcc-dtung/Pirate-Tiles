using UnityEngine;

public class HeartsController : MonoBehaviour
{
    [SerializeField] private HeartsView _heartsView;
    [SerializeField] private OutOfHeartPanelView _outOfHeartPanelView;
    
    [SerializeField] private VoidEventChannelSO _outOfHeartsChannel;

    private HeartsModel _heartsModel;
    
    public void Initialize()
    {
        _heartsModel = new HeartsModel();
        _heartsModel.MaxHearts = 5;
        _heartsModel.CurrentHearts = 5;
        
        UpdateView();
    }



    public void ConsumeHeart()
    {
        if (_heartsModel.CurrentHearts > 0)
        {
            _heartsModel.ConsumeHeart();
            UpdateView();
        }
        else
        {
            if (_outOfHeartsChannel != null)
            {
                _outOfHeartsChannel.EventRaise();
            }
        }
    }

    private void OnOutOfHearts()
    {
        if (_outOfHeartPanelView != null)
        {
            _outOfHeartPanelView.Show();
        }
    }

    private void Update()
    {
        if (_heartsModel != null && !_heartsModel.IsFull)
        {
            if (_heartsView != null) _heartsView.UpdateCountdown("05:00");
        }
        else if (_heartsModel != null)
        {
            if (_heartsView != null) _heartsView.UpdateCountdown("");
        }
    }

    private void UpdateView()
    {
        if (_heartsView != null && _heartsModel != null)
        {
            _heartsView.UpdateHearts(_heartsModel.CurrentHearts, _heartsModel.MaxHearts);
        }
    }
}
