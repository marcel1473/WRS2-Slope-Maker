using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WinterSlopeTool
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    internal sealed class MainForm : Form
    {
        private readonly TextBox fileBox = new TextBox();
        private readonly TextBox slopeBox = new TextBox();
        private readonly NumericUpDown infoLayerBox = new NumericUpDown();
        private readonly TextBox nameBox = new TextBox();
        private readonly TextBox fromBox = new TextBox();
        private readonly TextBox toBox = new TextBox();
        private readonly ComboBox diffBox = new ComboBox();
        private readonly NumericUpDown lengthBox = new NumericUpDown();
        private readonly NumericUpDown capacityBox = new NumericUpDown();
        private readonly Label statusLabel = new Label();

        public MainForm()
        {
            Text = "Winter Resort Simulator 2 - Slope Adder";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(620, 500);
            Size = new Size(720, 560);
            Font = new Font("Segoe UI", 10F);

            slopeBox.Text = "1";
            nameBox.Text = "Valley Run";
            fromBox.Text = "Mountain";
            toBox.Text = "Valley";

            infoLayerBox.Minimum = 0;
            infoLayerBox.Maximum = 999999;
            lengthBox.Minimum = 1;
            lengthBox.Maximum = 999999;
            lengthBox.Value = 180;
            capacityBox.Minimum = 0;
            capacityBox.Maximum = 999999;
            capacityBox.Value = 6000;

            diffBox.DropDownStyle = ComboBoxStyle.DropDownList;
            diffBox.Items.AddRange(new object[]
            {
                "1 - Blue / Easy",
                "2 - Red / Medium",
                "3 - Black / Hard"
            });
            diffBox.SelectedIndex = 0;

            var root = new TableLayoutPanel();
            root.Dock = DockStyle.Fill;
            root.Padding = new Padding(14);
            root.ColumnCount = 3;
            root.RowCount = 11;
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));

            for (var i = 0; i < 10; i++)
            {
                root.RowStyles.Add(new RowStyle(SizeType.Absolute, i == 0 ? 38 : 42));
            }
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var browseButton = new Button();
            browseButton.Text = "Browse...";
            browseButton.Click += delegate { BrowseFile(); };

            fileBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            root.Controls.Add(MakeLabel("Save .lua file"), 0, 0);
            root.Controls.Add(fileBox, 1, 0);
            root.Controls.Add(browseButton, 2, 0);

            AddRow(root, 1, "Slope number/id", slopeBox);
            AddRow(root, 2, "Info Layer", infoLayerBox);
            AddRow(root, 3, "Run name", nameBox);
            AddRow(root, 4, "From", fromBox);
            AddRow(root, 5, "To", toBox);
            AddRow(root, 6, "Difficulty", diffBox);
            AddRow(root, 7, "Length seconds", lengthBox);
            AddRow(root, 8, "Max skier capacity", capacityBox);

            var addButton = new Button();
            addButton.Text = "Add Slope To File";
            addButton.Height = 38;
            addButton.Anchor = AnchorStyles.Right;
            addButton.Width = 170;
            addButton.Click += delegate { AddSlope(); };
            root.Controls.Add(addButton, 1, 9);

            statusLabel.Text = "Pick your latest savegame .lua file, fill the slope info, then click Add.";
            statusLabel.ForeColor = Color.FromArgb(50, 82, 120);
            statusLabel.AutoSize = false;
            statusLabel.Height = 48;
            statusLabel.Dock = DockStyle.Fill;
            root.Controls.Add(statusLabel, 0, 10);
            root.SetColumnSpan(statusLabel, 3);

            Controls.Add(root);
        }

        private static Label MakeLabel(string text)
        {
            var label = new Label();
            label.Text = text;
            label.AutoSize = true;
            label.Anchor = AnchorStyles.Left;
            label.TextAlign = ContentAlignment.MiddleLeft;
            return label;
        }

        private static void AddRow(TableLayoutPanel root, int row, string label, Control input)
        {
            input.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            root.Controls.Add(MakeLabel(label), 0, row);
            root.Controls.Add(input, 1, row);
            root.SetColumnSpan(input, 2);
        }

        private void BrowseFile()
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Select Winter Resort Simulator savegame Lua file";
                dialog.Filter = "Lua save files (*.lua)|*.lua|All files (*.*)|*.*";
                dialog.InitialDirectory = GetDefaultSaveDirectory();

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    fileBox.Text = dialog.FileName;
                    statusLabel.Text = "Ready.";
                }
            }
        }

        private static string GetDefaultSaveDirectory()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var candidate = Path.Combine(documents, "My Games", "WinterResortSimulator", "savegames");
            return Directory.Exists(candidate) ? candidate : documents;
        }

        private void AddSlope()
        {
            try
            {
                var filePath = fileBox.Text.Trim();
                if (!File.Exists(filePath))
                {
                    Fail("Choose an existing .lua save file first.");
                    return;
                }

                var slope = new SlopeInfo();
                slope.Slope = slopeBox.Text.Trim();
                slope.InfoLayer = (int)infoLayerBox.Value;
                slope.Name = nameBox.Text.Trim();
                slope.From = fromBox.Text.Trim();
                slope.To = toBox.Text.Trim();
                slope.Difficulty = diffBox.SelectedIndex + 1;
                slope.Length = (int)lengthBox.Value;
                slope.Capacity = (int)capacityBox.Value;

                var validationError = slope.Validate();
                if (validationError != null)
                {
                    Fail(validationError);
                    return;
                }

                var original = File.ReadAllText(filePath, Encoding.UTF8);
                var updated = SlopeFileEditor.InsertSlope(original, slope);
                if (updated == original)
                {
                    Fail("No change was made. The slope may already exist.");
                    return;
                }

                var backupPath = filePath + ".backup-" + DateTime.Now.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture);
                File.Copy(filePath, backupPath, false);
                File.WriteAllText(filePath, updated, new UTF8Encoding(false));

                statusLabel.ForeColor = Color.FromArgb(30, 120, 68);
                statusLabel.Text = "Added slope and created backup: " + Path.GetFileName(backupPath);
                MessageBox.Show(this, "Slope added successfully.\n\nBackup created:\n" + backupPath,
                    "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Fail(ex.Message);
            }
        }

        private void Fail(string message)
        {
            statusLabel.ForeColor = Color.FromArgb(164, 44, 44);
            statusLabel.Text = message;
            MessageBox.Show(this, message, "Could not add slope", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    internal sealed class SlopeInfo
    {
        public string Slope = "";
        public int InfoLayer;
        public string Name = "";
        public string From = "";
        public string To = "";
        public int Difficulty;
        public int Length;
        public int Capacity;

        public string Validate()
        {
            if (string.IsNullOrWhiteSpace(Slope)) return "Slope number/id is required.";
            if (InfoLayer < 0) return "Info Layer must be 0 or higher.";
            if (string.IsNullOrWhiteSpace(Name)) return "Run name is required.";
            if (string.IsNullOrWhiteSpace(From)) return "From is required.";
            if (string.IsNullOrWhiteSpace(To)) return "To is required.";
            if (Difficulty < 1 || Difficulty > 3) return "Difficulty must be 1, 2, or 3.";
            if (Length < 1) return "Length must be at least 1 second.";
            if (Capacity < 0) return "Capacity must be 0 or higher.";
            return null;
        }
    }

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

            var lineEnding = saveText.Contains("\r\n") ? "\r\n" : "\n";
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
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
