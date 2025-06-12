namespace console_spec.Interfaces;

public interface IBalance
{
    public void Add(long credits);
    public void Remove(long credits);
}