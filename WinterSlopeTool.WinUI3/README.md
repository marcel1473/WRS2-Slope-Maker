# Winter Slope Tool

Winter Slope Tool is a small Windows app for adding ski slopes to Winter Resort Simulator 2 savegame `.lua` files without manually editing the save in a text editor.

## What It Does

- Lets you pick a savegame `.lua` file.
- Asks for slope id, Info Layer, run name, start, end, difficulty, length, and capacity.
- Inserts the slope into the save file's `skiSlopes` table.
- Creates a timestamped backup before changing the save.

## Before You Use It

Close Winter Resort Simulator 2 before editing a save file.

The usual save folder is:

```text
Documents\My Games\WinterResortSimulator\savegames
```

## Difficulty Values

```text
1 = Blue / Easy
2 = Red / Medium
3 = Black / Hard
```

## Portable Version

Extract the ZIP and run:

```text
WinterSlopeTool.exe
```

Keep the files in the folder together. The app needs the included Windows App SDK files beside it.

## Safety

The app creates a backup next to the save before editing it. The backup filename looks like:

```text
your-save.lua.backup-20260503-153000
```

If something goes wrong, rename the backup back to `.lua`.

## Support

If the tool says it cannot find `skiSlopes`, the selected file may not be the right save file or the save format may be different.
