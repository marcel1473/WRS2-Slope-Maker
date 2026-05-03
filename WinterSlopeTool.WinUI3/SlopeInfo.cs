namespace WinterSlopeTool.WinUI3;

internal sealed record SlopeInfo(
    string Slope,
    int InfoLayer,
    string Name,
    string From,
    string To,
    int Difficulty,
    int Length,
    int Capacity)
{
    public string? Validate()
    {
        if (string.IsNullOrWhiteSpace(Slope)) return "Slope number/id is required.";
        if (InfoLayer < 0) return "Info Layer must be 0 or higher.";
        if (string.IsNullOrWhiteSpace(Name)) return "Run name is required.";
        if (string.IsNullOrWhiteSpace(From)) return "From is required.";
        if (string.IsNullOrWhiteSpace(To)) return "To is required.";
        if (Difficulty is < 1 or > 3) return "Difficulty must be 1, 2, or 3.";
        if (Length < 1) return "Length must be at least 1 second.";
        if (Capacity < 0) return "Capacity must be 0 or higher.";
        return null;
    }
}
