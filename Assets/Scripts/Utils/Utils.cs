using static utils.Constants;
using System.Collections.Generic;
using UnityEngine;
using models.Position;

namespace utils {
    public static class Utils {

        /// <summary>
        /// Maps a difficulty id to a string.
        /// </summary>
        /// <param name="difficultyId">Id of the difficulty</param>
        /// <returns>A string representing the difficulty</returns>
        public static string DifficultyName(int difficultyId) {
            switch(difficultyId) {
                case Easy: return "Easy";
                case Medium: return "Medium";
                case MediumPlus: return "Medium+";
                case Hard: return "Hard";
                default: return "Unassigned";
            }
        } 

        /// <summary>
        /// Plays an input audio if the UI audio is switched on.
        /// </summary>
        /// <param name="audio">AudioSource to play</param>
        public static void PlayUIAudioIfEnabled(AudioSource audio) {
            if(PlayerPrefs.GetFloat("menuSound", DefaultMenuSound) != 0) {
                audio.Play();
            }
        }

        /// <summary>
        /// Plays an input audio if the game audio is switched on.
        /// </summary>
        /// <param name="audio">AudioSource to play</param>
        public static void PlayGameAudioIfEnabled(AudioSource audio) {
            if(PlayerPrefs.GetFloat("gameSound", DefaultGameSound) != 0) {
                audio.Play();
            }
        }

        /// <summary>
        /// Maps an option value to a number of pieces.
        /// </summary>
        /// <param name="value">Option value</param>
        public static int MapOptValueToNumPieces(int value) {
            switch(value) {
                case 0: return 16;
                case 1: return 12;
                case 2: return 10;
                case 3: return 6;
                default: return 16;
            }
        }

        /// <summary>
        /// Returns the option value for a given number of pieces.
        /// </summary>
        /// <param name="numPieces">Number of pieces</param>
        public static int MapNumPiecesToOptValue(int numPieces) {
            switch(numPieces) {
                case 16: return 0;
                case 12: return 1;
                case 10: return 2;
                case 6: return 3;
                default: return 0;
            }
        }

        /// <summary>
        /// Converts a Position object into the corresponding spot id of a board.
        /// </summary>
        /// <param name="position">Position to be converted</param>
        /// <returns>An integer representing the spot id of the input Position</returns>
        public static int PositionToId(Position position) {
            return position.y * BoardLength + position.x;
        }

        /// <summary>
        /// Converts a spot id of a board into a Position object.
        /// </summary>
        /// <param name="id">Id of the spot</param>
        /// <returns>The Position object that resulted from the conversion</returns>
        public static Position IdToPosition(int id) {
            int x = id % BoardLength;
            int y = id / BoardLength; 
            return new Position(x, y);
        }

        /// <summary>
        /// Returns the id of the next player.
        /// </summary>
        /// <param name="currentPlayer">Id of the current player</param>
        /// <returns>An integer representing the id of the next player</returns>
        public static int NextPlayer(int currentPlayer) {
            return currentPlayer % 2 + 1;
        }

        /// <summary>
        /// Builds a list of moves that all start from the same position but end in different positions.
        /// </summary>
        /// <param name="startPosition">Position where the moves start</param>
        /// <param name="endPositions">List of destinies</param>
        /// <param name="moveType">Type of the moves</param>
        /// <returns>The list of available moves.</returns>
        public static List<Move> ToMovesList(Position startPosition, List<Position> endPositions, MoveType moveType) {
            List<Move> movesList = new List<Move>();
            foreach(Position endPosition in endPositions) {
                Move move = new Move(startPosition, endPosition);
                move.type = moveType;
                movesList.Add(move);
            }
            return movesList;
        }

        public enum MoveType {
            Normal,
            Jump,
            Capture,
            Invalid 
        }
    }

    
}
