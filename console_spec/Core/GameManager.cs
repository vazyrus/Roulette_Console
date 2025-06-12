using console_spec.Utilities;
using Spectre.Console;

namespace console_spec.Models;

public class GameManager
{
    public Player CurrentPlayer { get; set; }
    private Style _promptYellow = new Style(Colour.Colours[RouletteColour.Yellow]);
    private Style _promptBlue = new Style(Colour.Colours[RouletteColour.Blue]);

    public GameManager()
    {
        CurrentPlayer = new Player();
    }
    
    public void Run()
    {
        AnsiConsole.Clear();
        RunFiglet();
        
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .MoreChoicesText("[grey](Move up and down to select)[/]")
                .HighlightStyle(_promptYellow)
                .AddChoices("New Session", "Quit"));
        
        switch (selection)
        {
            case "New Session":
                StartGame();
                break;
            case "Quit":
                AnsiConsole.Clear();
                Environment.Exit(0);
                break;
        }
    }

    private static void RunFiglet()
    {
        var font = FigletFont.Load(@"..\Resources\ANSI Regular.flf");
        
        AnsiConsole.Write(
            new FigletText(font, "ROULETTE")
                .Justify(Justify.Center)
                .Color(Colour.Colours[RouletteColour.Cyan]));
    }

    public void StartGame()
    {
        InitializePlayer();
        PressEnterPrompt("Start");
        
        while (Console.ReadKey(true).Key != ConsoleKey.Q)
        {
            AnsiConsole.Clear();
            RunFiglet();
            PlayRoulette();
        }
    }

    private void InitializePlayer()
    {
        AnsiConsole.Markup($"[#fff78a] What is your name?[/]");
        CurrentPlayer.Name = AnsiConsole.Ask<string>("");
        AnsiConsole.Write(new Panel(new Text($"Greetings, {CurrentPlayer.Name}!", new Style(Colour.Colours[RouletteColour.Blue])).Centered()).Border(BoxBorder.Rounded));
        Thread.Sleep(1500);
        AnsiConsole.Write(new Panel(new Text($"Here's 100000 Credits to start your roulette journey. Have fun!", new Style(Colour.Colours[RouletteColour.Yellow])).Centered()).Border(BoxBorder.None));
        CurrentPlayer.CurrentBalance.Add(1000000);
        AnsiConsole.Markup($"[#fff78a] Current Balance:[/] [bold #70b7f5]{CurrentPlayer.CurrentBalance.Balance}[/]");
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Panel(new Text($"Press Q at any time to Quit.", new Style(Colour.Colours[RouletteColour.LightPurple])).Centered()).Border(BoxBorder.None));
    }

    public void PressEnterPrompt(string message)
    {
        AnsiConsole.Write(new Panel(new Rule($"Press [#fff78a]ENTER[/] to {message}!").RuleStyle(Style.Parse("silver")).Centered()).Border(BoxBorder.Rounded));
    }

    public void PlayRoulette()
    {
        string[] choices = ["Red", "Black", "1 - 18", "19 - 36", "Even", "Odd", "1-12", "13-24", "25-36"];
        for (int i = 0; i < choices.Length; i++)
        {
            AnsiConsole.Write(new Panel(new Markup($"[bold #fff78a]{i + 1}:[/] [#70b7f5]{choices[i]}[/]").Centered()).Expand().RoundedBorder());
        }
        AnsiConsole.Write(new Text($"Enter your choice: ", new Style(Colour.Colours[RouletteColour.Yellow])));

        int playerChoice = AnsiConsole.Prompt(new TextPrompt<int>("")
            .PromptStyle(_promptBlue)
            .AddChoices([1, 2, 3, 4, 5, 6, 7, 8, 9])
            .DefaultValue(1));

        AnsiConsole.Write(new Text($"Enter your bet amount:", new Style(Colour.Colours[RouletteColour.Yellow])));
        long playerBetAmount = AnsiConsole.Prompt(new TextPrompt<long>($"")
            .PromptStyle(_promptBlue)
            .Validate(input =>
            {
                if (input > CurrentPlayer.CurrentBalance.Balance)
                {
                    return ValidationResult.Error($"[red]That's more than what you have at the moment: {CurrentPlayer.CurrentBalance.Balance}[/]");
                }
                return ValidationResult.Success();
            }));

        long balanceBeforeBet = CurrentPlayer.CurrentBalance.Balance;
        CurrentPlayer.CurrentBalance.Balance -= playerBetAmount;
        Stats.UpdateTurnCounter();
        
        AnsiConsole.Markup($"[#fff78a]You have bet[/] [bold #70b7f5]{playerBetAmount}[/] [#fff78a]on[/] [italic #70b7f5]({choices[playerChoice-1]})[/]");
        AnsiConsole.WriteLine();
        
        AnsiConsole.Status()
            .AutoRefresh(true)
            .Spinner(new SharkSpinnerExtended())
            .Start("Running your bet!", ctx =>
            {
                Thread.Sleep(5000);
            });
        
        (int rouletteNumber, bool red, bool hasPlayerWon) = SpinRouletteWheel(playerChoice, playerBetAmount);
        
        string rouletteNumberWheelColour = CheckIfRed(red);
        AnsiConsole.Markup($"[#fff78a]The ball landed on[/] [bold #70b7f5]{rouletteNumber}[/] [italic #70b7f5]({rouletteNumberWheelColour})[/]");
        AnsiConsole.WriteLine();
        UpdatePlayerStats(balanceBeforeBet, hasPlayerWon);
        AnsiConsole.WriteLine();
        PressEnterPrompt("Continue");
    }

    private void UpdatePlayerStats(long balance, bool hasPlayerWon)
    {
        string currentStreak = "";
        int streak = 0;
        if (balance > CurrentPlayer.CurrentBalance.Balance )
        {
            Stats.ResetWinStreak();
            Stats.UpdateLossStreak();
            currentStreak = "Loss Streak";
            streak = Stats.GetLossStreak();
        }
        else if (balance < CurrentPlayer.CurrentBalance.Balance)
        {
            Stats.ResetLossStreak();
            Stats.UpdateWinStreak();
            currentStreak = "Win Streak";
            streak = Stats.GetWinStreak();
        }
        
        AnsiConsole.Markup($"[#fff78a]You currently have[/] [bold #70b7f5]{CurrentPlayer.CurrentBalance.Balance}[/], [#ac87c5]Current {currentStreak}:[/] [#ef4040] {streak}[/], [#fff78a]Turn: {Stats.GetTurns()}[/]");
        
        if (CurrentPlayer.CurrentBalance.Balance <= 0)
        {
            Stats.ResetAllStats();
            CurrentPlayer.CurrentBalance.Balance = 1000000;
            AnsiConsole.WriteLine();
            AnsiConsole.Markup($"[#ac87c5 bold]Press any key to reset game.[/]");
            if (Console.KeyAvailable)
            {
                AnsiConsole.Clear();
                RunFiglet();
                PlayRoulette();
            }
        }
    }

    public (int rouletteNumber, bool red, bool odd) SpinRouletteWheel(int playerChoice, long betAmount)
    {
        Random random = new Random();
        int rouletteNumber = random.Next(0, 37);
        bool red = CheckIfRedOrOdd(rouletteNumber);
        bool hasPlayerWon = false;
        
        switch (choice: playerChoice, red)
        {
            case (1, true):
                CalculateBetResult(betAmount, 2);
                hasPlayerWon = true;
                break;
            case (2, false):
                CalculateBetResult(betAmount, 2);
                hasPlayerWon = true;
                break;
            case (3, _):
                if (rouletteNumber is > 0 and < 19)
                {
                    CalculateBetResult(betAmount, 2);
                    hasPlayerWon = true;
                }
                break;
            case (4, _):
                if (rouletteNumber is > 18 )
                {
                    CalculateBetResult(betAmount, 2);
                    hasPlayerWon = true;
                }
                break;
            case (5, false):
                hasPlayerWon = true;
                break;
            case (6, true):
                hasPlayerWon = true;
                break;
            case (7, _):
                if (rouletteNumber is > 0 and < 13 )
                {
                    CalculateBetResult(betAmount, 3);
                    hasPlayerWon = true;
                }
                break;
            case (8, _):
                if (rouletteNumber is > 12 and < 25 )
                {
                    CalculateBetResult(betAmount, 3);
                    hasPlayerWon = true;
                }
                break;
            case (9, _):
                if (rouletteNumber is > 24 and < 37 )
                {
                    CalculateBetResult(betAmount, 3);
                    hasPlayerWon = true;
                }
                break;
            default:
                hasPlayerWon = false;
                break;
        }
        
        return (rouletteNumber, red, hasPlayerWon);
    }

    private void CalculateBetResult(long betAmount, int multiplier)
    {
        CurrentPlayer.CurrentBalance.Balance += (betAmount * multiplier);
    }
    
    private bool CheckIfRedOrOdd(int number) => number % 2 == 1;
    
    private string CheckIfRed(bool red) => red ? "RED" : "BLACK";
}
