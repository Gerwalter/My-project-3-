

public interface IAnimObserver
{
    void OnAttackTriggered(ComboNode node); // ahora recibe el nodo
    void OnShootStateChanged(bool isShooting);
}
