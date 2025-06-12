namespace console_spec.Models;

public class Player
{
    public string? Name { get; set; }
    public CurrentBalance CurrentBalance { get; set; }

    public Player()
    {
        CurrentBalance = new CurrentBalance();
    }
}