public interface IInteractable
{
    void Interact();
    string GetInteractPrompt();
}

public interface IDamageable
{
    void TakeDamage(int amount);
    bool IsAlive();
}

public interface ICollectible
{
    void Collect();
    bool IsCollected { get; }
}

public interface IUsable
{
    void Use();
    string GetDescription();
}