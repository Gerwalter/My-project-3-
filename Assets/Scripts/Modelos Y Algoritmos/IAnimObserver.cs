

public interface IAnimObserver
{
    void OnAttackTriggered(string triggerName); // ahora recibe el nodo
    void OnShootStateChanged(bool isShooting);
}
