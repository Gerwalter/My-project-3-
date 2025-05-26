
public interface IAnimObservable
{
    void Subscribe(IAnimObserver x);
    void Unsubscribe(IAnimObserver x);
}
