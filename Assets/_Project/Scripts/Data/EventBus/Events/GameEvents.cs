public struct GamePhaseChangedEvent : IEvent {
    public GamePhase PreviousPhase;

    public GamePhase NewPhase;
}

public struct SceneLoadRequestedEvent : IEvent {
    public string SceneName;

    public bool UseLoadingScreen;
}

public struct SaveDataChangedEvent : IEvent {
    public string Key;
}
