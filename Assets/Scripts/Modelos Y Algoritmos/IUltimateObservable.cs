
public interface IUltimateObservable
{
    void Subscribe(IUltimateObserver x);
    void Unsubscribe(IUltimateObserver x);
}
