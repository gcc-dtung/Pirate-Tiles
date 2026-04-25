using System.Collections.Generic;

public class TileHistoryModel
{
    private Stack<int> _history;

    public int Count => _history.Count;
    public bool HasHistory => _history.Count > 0;

    public TileHistoryModel()
    {
        _history = new Stack<int>();
    }

    public void RecordMove(int tileId)
    {
        _history.Push(tileId);
    }

    public int PopLastMove()
    {
        if (_history.Count > 0)
        {
            return _history.Pop();
        }
        return -1;
    }

    public void Clear()
    {
        _history.Clear();
    }
}
