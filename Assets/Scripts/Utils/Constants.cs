using System.Collections.Generic;

namespace utils {
    public static class Constants {
        // Game modes
        public static int SinglePlayer = 0;
        public static int MultiPlayer = 1;
        public static Dictionary<int, string> gameModes = new Dictionary<int, string>() {
            { SinglePlayer, "Singleplayer" },
            { MultiPlayer, "Multiplayer" }
        };
        // Board cells
        public static byte EmptyCell = 0;
        public static byte WhiteCell = 1;
        public static byte BlackCell = 2;
        // Size of a board line
        public static int BoardLength = 8;
        // Board sprites
        public static int BlackCellEmpty = 0;
        public static int BlackCellBlackPiece = 1;
        public static int BlackCellWhitePiece = 2;
        public static int WhiteCellEmpty = 3;
        public static int WhiteCellBlackPiece = 4;
        public static int WhiteCellWhitePiece = 5; 
        public static int BlackCellHighlighted = 6;
        public static int WhiteCellHighlighted = 7;
        // Load game
        public static int MaxSavedMatches = 4;
        // Difficulties
        public const int Easy = 0;
        public const int Medium = 1;
        public const int MediumPlus = 2;
        public const int Hard = 3;
        // Defaults
        public static int DefaultDifficulty = Easy;
        public static int DefaultMoveTime = 1;
        public static int DefaultNumPieces = 16;
        public static int DefaultHighlightStyle = SquareBorder;
        public static int DefaultBoardStyle = ClassicBoard;
        public static int DefaultMenuSound = 1;
        public static int DefaultGameSound = 1;
        // Scenes
        public static int MenuScene = 1;
        public static int MatchScene = 2;
        public static int AboutScene = 4;
        public static int SettingsScene = 5;
        public static int LoadGameScene = 6;
        // Highlight styles
        public static int SquareBorder = 0;
        public static int Transparency = 1;
        // Board styles
        public static int ClassicBoard = 0;
        public static int GrayishBoard = 1;
    }
}