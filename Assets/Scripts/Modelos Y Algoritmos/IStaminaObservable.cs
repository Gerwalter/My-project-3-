
public interface IStaminaObservable
{
    void Subscribe(IStaminaObserver x);
    void Unsubscribe(IStaminaObserver x);
}
