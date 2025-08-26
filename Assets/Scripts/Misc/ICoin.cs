public interface ICoin
{
    int Value { get; }
    void Collect(GoldManager manager);
}
