using console_spec.Interfaces;

namespace console_spec.Models;

public class CurrentBalance : IBalance
{
    public long Balance { get; set; } = 0;
    
    public void Add(long credits)
    {
        Balance += credits;
    }

    public void Remove(long credits)
    {
        Balance -= credits;
        if (Balance <= 0)
        {
            Balance = 0;
        }
    }
}