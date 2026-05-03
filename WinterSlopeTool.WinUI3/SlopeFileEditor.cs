using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace WinterSlopeTool.WinUI3;

internal static class SlopeFileEditor
{
    public static string InsertSlope(string saveText, SlopeInfo slope)
    {
        if (ContainsExistingSlope(saveText, slope))
        {
            throw new InvalidOperationException("A slope with that slope id or Info Layer already appears to exist.");
        }

        var openBrace = FindSkiSlopesOpenBrace(saveText);
        if (openBrace < 0)
        {
            throw new InvalidOperationException("Could not find a skiSlopes table. Look for text like: skiSlopes = {");
        }

        var lineEnding = saveText.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";
        var indent = GuessEntryIndent(saveText, openBrace);
        var block = BuildSlopeBlock(slope, indent, lineEnding);
        var insertAt = openBrace + 1;

        return saveText.Insert(insertAt, lineEnding + block);
    }

    private static bool ContainsExistingSlope(string saveText, SlopeInfo slope)
    {
        var escapedSlope = Regex.Escape(EscapeLuaString(slope.Slope));
        var slopePattern = @"slope\s*=\s*""" + escapedSlope + @"""";
        var infoPattern = @"infoLayer\s*=\s*" + slope.InfoLayer.ToString(CultureInfo.InvariantCulture) + @"\b";
        return Regex.IsMatch(saveText, slopePattern) || Regex.IsMatch(saveText, infoPattern);
    }

    private static int FindSkiSlopesOpenBrace(string text)
    {
        var match = Regex.Match(text, @"\bskiSlopes\b\s*=\s*\{", RegexOptions.CultureInvariant);
        if (match.Success)
        {
            return match.Index + match.Value.LastIndexOf('{');
        }

        match = Regex.Match(text, @"\bskiSlopes\b", RegexOptions.CultureInvariant);
        if (!match.Success) return -1;

        for (var i = match.Index + match.Length; i < text.Length; i++)
        {
            if (text[i] == '{') return i;
            if (text[i] == '\n' && i - match.Index > 200) return -1;
        }

        return -1;
    }

    private static string GuessEntryIndent(string text, int openBrace)
    {
        var lineStart = text.LastIndexOf('\n', openBrace);
        lineStart = lineStart < 0 ? 0 : lineStart + 1;

        var existingIndent = new StringBuilder();
        for (var i = lineStart; i < openBrace && char.IsWhiteSpace(text[i]) && text[i] != '\r' && text[i] != '\n'; i++)
        {
            existingIndent.Append(text[i]);
        }

        return existingIndent + "    ";
    }

    private static string BuildSlopeBlock(SlopeInfo slope, string indent, string nl)
    {
        var inner = indent + "    ";
        return string.Join(nl, new[]
        {
            indent + "{",
            inner + "slope = \"" + EscapeLuaString(slope.Slope) + "\",",
            inner + "infoLayer = " + slope.InfoLayer.ToString(CultureInfo.InvariantCulture) + ",",
            inner + "name = \"" + EscapeLuaString(slope.Name) + "\",",
            inner + "from = \"" + EscapeLuaString(slope.From) + "\",",
            inner + "to = \"" + EscapeLuaString(slope.To) + "\",",
            inner + "diff = " + slope.Difficulty.ToString(CultureInfo.InvariantCulture) + ",",
            inner + "length = " + slope.Length.ToString(CultureInfo.InvariantCulture) + ",",
            inner + "currentGuests = 0,",
            inner + "maxSkiersCapacity = " + slope.Capacity.ToString(CultureInfo.InvariantCulture) + ",",
            indent + "},"
        });
    }

    private static string EscapeLuaString(string value)
    {
        return value.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal);
    }
}
