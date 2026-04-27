public interface IInteractable
{
    bool CanInteract { get; }
    bool RequiresHold { get; }
    float HoldDuration { get; }

    string GetPromptText();

    void OnInteract();
    void OnHoldInteract();
}