using System;

namespace WinterSlopeTool
{
    internal static class TestInsert
    {
        private static int Main()
        {
            var slope = new SlopeInfo();
            slope.Slope = "1";
            slope.InfoLayer = 12;
            slope.Name = "Valley Run";
            slope.From = "Mountain";
            slope.To = "Valley";
            slope.Difficulty = 1;
            slope.Length = 180;
            slope.Capacity = 6000;

            var sample = "return {\r\n    money = 5000,\r\n    skiSlopes = {\r\n    },\r\n}\r\n";
            var updated = SlopeFileEditor.InsertSlope(sample, slope);

            if (!updated.Contains("name = \"Valley Run\"")) return Fail("Missing slope name.");
            if (!updated.Contains("infoLayer = 12")) return Fail("Missing infoLayer.");
            if (updated.IndexOf("skiSlopes = {", StringComparison.Ordinal) > updated.IndexOf("name = \"Valley Run\"", StringComparison.Ordinal))
            {
                return Fail("Inserted slope before skiSlopes table.");
            }

            try
            {
                SlopeFileEditor.InsertSlope(updated, slope);
                return Fail("Duplicate slope was not rejected.");
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Insertion test passed.");
                return 0;
            }
        }

        private static int Fail(string message)
        {
            Console.Error.WriteLine(message);
            return 1;
        }
    }
}
