namespace console_spec.Models;

public static class Stats
{
    private static int _winStreak { get; set; }
    private static int _lossStreak { get; set; }
    private static int _turns { get; set; }

    public static void ResetWinStreak() => _winStreak = 0;
    public static void ResetLossStreak() => _lossStreak = 0;
    public static void ResetTurns() => _turns = 0;
    public static int GetWinStreak() => _winStreak;
    public static int UpdateWinStreak() => _winStreak++;
    public static int GetLossStreak() => _lossStreak;
    public static int UpdateLossStreak() => _lossStreak++;
    public static int GetTurns() => _turns;
    public static int UpdateTurnCounter() => _turns++;

    public static void ResetAllStats()
    {
        ResetWinStreak();
        ResetLossStreak();
        ResetTurns();
    }

}