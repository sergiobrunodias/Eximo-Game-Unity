using static utils.Constants;
using static utils.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using models.Position;

[Serializable]
public class Board {
    private byte[,] cells;

    public Board() {
        Initialize();
    }

    /// <summary>
    /// Initializes the board to match the initial game state.
    /// </summary>
    private void Initialize() {
        cells = new byte[8, 8];
        // Drop zone pieces
        for (int i = 1; i <= 6; i++) {
            if(!PlayerPrefs.HasKey("numPiecesPlayer1") || PlayerPrefs.GetInt("numPiecesPlayer1") > 10)
                cells[i, 0] = WhiteCell;
            cells[i, 1] = WhiteCell;
            cells[i, 6] = BlackCell;
            if(!PlayerPrefs.HasKey("numPiecesPlayer2") || PlayerPrefs.GetInt("numPiecesPlayer2") > 10)
                cells[i, 7] = BlackCell;
        }
        // Third row pieces
        if(!PlayerPrefs.HasKey("numPiecesPlayer1") || PlayerPrefs.GetInt("numPiecesPlayer1") == 10 || PlayerPrefs.GetInt("numPiecesPlayer1") == 16) {
            cells[1, 2] = WhiteCell;
            cells[2, 2] = WhiteCell;
            cells[5, 2] = WhiteCell; 
            cells[6, 2] = WhiteCell;
        }
        if(!PlayerPrefs.HasKey("numPiecesPlayer2") || PlayerPrefs.GetInt("numPiecesPlayer2") == 10 || PlayerPrefs.GetInt("numPiecesPlayer2") == 16) {
            cells[1, 5] = BlackCell; 
            cells[2, 5] = BlackCell;
            cells[5, 5] = BlackCell; 
            cells[6, 5] = BlackCell;
        }
    }

    /// <summary>
    /// Retrieves the value of the cell at the specified position.
    /// </summary>
    /// <param name="position">Position of the cell</param>
    /// <returns>The value in the cell if the input position is valid, -1 otherwise.</returns>
    public byte GetCellValue(Position position) {
        try {
            return cells[position.x, position.y];
        } catch (System.IndexOutOfRangeException) {
            UnityEngine.Debug.Log("Error while accessing cell value of board!\nAttempted to access " + position);
            return 0;
        }   
    }

    /// <summary>
    /// Sets the value of the cell at the specified position.
    /// </summary>
    /// <param name="position">Position of the cell</param>
    /// <param name="value">Value of the cell</param>
    /// <remarks>
    /// The <param name="value"> must range between 0 and 2.
    /// EmptyCell, WhiteCell and BlackCell constants are defined in <see cref="Constants"/>.
    /// </remarks>
    public void SetCellValue(Position position, byte value) {
        try {
            cells[position.x, position.y] = value;
        } catch (System.IndexOutOfRangeException) {
            UnityEngine.Debug.Log("Error while accessing cell value of board!\nAttempted to access " + position);
        }
    }

    /// <summary>
    /// Checks if a cell is empty, i.e. if there is no piece on it.
    /// </summary>
    /// <param name="position">Position of the cell</param>
    /// <returns>True if the cell is empty, false otherwise.</returns>
    public bool IsCellEmpty(Position position) {
        try {
            return cells[position.x, position.y] == EmptyCell;
        } catch (System.IndexOutOfRangeException) {
            UnityEngine.Debug.Log("Error while accessing cell value of board!\nAttempted to access " + position);
            return false;
        }
    }

    /// <summary>
    /// Retrieves the positions where the pieces of a given type are placed.
    /// <param name="pieceType">Type of the pieces</param>
    /// </summary>
    /// <returns>A list with the positions where the pieces are located in the board.</returns>
    /// <remarks>
    /// The <param name="pieceType"> must range between 1 and 2.
    /// hiteCell and BlackCell constants defined in <see cref="Constants"/>.
    /// </remarks>
    public List<Position> GetPiecesPositions(int pieceType) {
        List<Position> positions = new List<Position>();
        for(int x = 0; x < BoardLength; x++) 
            for(int y = 0; y < BoardLength; y++) {
                Position position = new Position(x, y);
                if(GetCellValue(position) == pieceType) {
                    positions.Add(position);
                }
            }
        return positions;
    }

    /// <summary>
    /// Retrieves the positions of the free spots in the drop zone. 
    /// <param name="player">Player to check against, either 1 or 2</param>
    /// </summary>
    /// <returns>A list with the positions of the free spots.</returns>
    /// <remarks>
    /// Player is identified by an integer with a value of either 1 or 2.
    /// <remarks>
    public List<Position> GetFreeSpawnSpots(int player) {
        List<Position> freeSpots = new List<Position>();
        List<int> yValues = player == 1? new List<int>{0, 1} : new List<int>{6, 7};
        for (int x = 1; x <= 6; x++) 
            foreach(int y in yValues) {
                Position spot = new Position(x, y);
                if(IsCellEmpty(spot)) {
                    freeSpots.Add(spot);
                }
            }
        return freeSpots;
    }

    /// <summary>
    /// Counts the pieces of a given type that are placed in the specified column.
    /// </summary>
    /// <param name="pieceType">Type of the pieces</param>
    /// <param name="column">Column index</param>
    /// <returns>The number of pieces placed in the column</returns>
    /// <remarks>
    /// The <param name="pieceType"> must range between 1 and 2.
    /// The <param name="column"> must range between 0 and BoardLength - 1.
    /// WhiteCell, BlackCell and BoardLength constants defined in <see cref="Constants"/>.
    /// </remarks>
    public int CountPiecesInColumn(int pieceType, int column) {
        int count = 0;
        for(int y = 0; y < BoardLength; y++) {
            if(cells[column, y] == pieceType) 
                count++;
        }
        return count;
    } 

    // Blindly applies a move, throw exception se fora de bounderies ou move errada
    // como já é de se esperar que a move passada seja valida, entao justifica-se mandar exception

    /// <summary>
    /// Plays a move on the board.
    /// </summary>
    /// <param name="move">Move to play</param>
    /// <returns>The type of the played move</returns>
    /// <remarks>
    /// This method does not verify if the <paramref name="move"> is valid, it just blindly applies it.
    /// The types of moves are definied in MoveType enumeration in <see cref="Constants"/>.
    /// </remarks>
    public MoveType ApplyMove(Move move) {
        MoveType moveType;
        byte destinyValue = GetCellValue(move.startPosition);
        SetCellValue(move.startPosition, EmptyCell);
        SetCellValue(move.endPosition, destinyValue);
        Position midpoint = move.Midpoint();
        // If midpoint exists, this is a special move
        if(midpoint != null) { 
            if(GetCellValue(midpoint) != destinyValue) { // Capture
                moveType = MoveType.Capture;
                SetCellValue(midpoint, EmptyCell); 
            } else { // Jump
                moveType = MoveType.Jump;
            }
        } else {
            moveType = MoveType.Normal;
        }
        return moveType;
    }

    /// <summary>
    /// Plays a sequence of moves on the board.
    /// </summary>
    /// <param name="moveSequence">MoveSequence object containing all the moves to play</param>
    /// <remarks>
    /// This method does not verify if the moves in the sequence are valid, it just blindly applies them.
    /// </remarks>
    public void ApplyMoveSequence(MoveSequence moveSequence) {
        foreach(Move move in moveSequence.moves) {
            byte destinyValue = GetCellValue(move.startPosition);
            SetCellValue(move.startPosition, EmptyCell);
            SetCellValue(move.endPosition, destinyValue);
            Position midpoint = move.Midpoint();
            // If midpoint exists, this is a special move
            if(midpoint != null && GetCellValue(midpoint) != destinyValue) { // Capture
                SetCellValue(midpoint, EmptyCell); 
            }
        }
    }

    /// <summary>
    /// Extracts a part of the board into a two-dimentional array.
    /// </summary>
    /// <param name="controlPoint1">Position of the upper left corner of the extracted board</param>
    /// <param name="controlPoint2">Position of the lower right corner of the extracted board</param>
    /// <returns>A two-dimentional array representing the extracted board</returns>
    /// <remarks>
    /// The extracted sub-board is of rectangular form. 
    /// </remarks>
    public int[,] ExtractSubBoard(Position controlPoint1, Position controlPoint2) {
        int numColumns = Math.Abs(controlPoint2.x - controlPoint1.x) + 1;
        int numRows = Math.Abs(controlPoint2.y - controlPoint1.y) + 1;
        int[,] subBoard = new int[numColumns, numRows];
        for(int x = 0; x < numColumns; x++) {  
            for(int y = 0; y < numRows; y++) { 
                subBoard[y, x] = cells[controlPoint1.x + x, controlPoint1.y + y];
            }
        }
        return subBoard;
    }

    /// <summary>
    /// Clones the current Board.
    /// </summary>
    /// <returns>A copy of the current Board.</returns>
    public Board Clone() {
        Board cloned = (Board) this.MemberwiseClone();
        cloned.cells = (byte[,]) cells.Clone();
        return cloned;
    }
}