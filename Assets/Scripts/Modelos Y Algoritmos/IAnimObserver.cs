

public interface IAnimObserver
{
    void OnAttackTriggered(string triggerName);
    void OnShootStateChanged(bool isShooting);
}
