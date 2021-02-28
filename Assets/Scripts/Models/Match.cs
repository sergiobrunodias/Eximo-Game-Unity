using static utils.Constants;
using static utils.Utils;
using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using models.Position;

public static class MoveFeedback {
    public const string
        KeepCapturing = "You must keep capturing while possible!",
        KeepJumping = "You must keep jumping over while possible!",
        ChooseSpots = "Choose the spots where you want to place the new pieces!",
        NoPiecesGiven = "Since you have no free slots in your drop zone, you were not given any extra pieces";
}

[Serializable]
public class Match {
    // Game data is acessible for UI controller to read but is not modifiable
    public int gameMode {get; private set;}
    public int difficulty {get; private set;}
    public Board board {get; private set;}
    public int playerTurn {get; private set;}
    /// <summary>
    /// List of the possible moves updated when a player selects a starting position.
    /// It is used to highlight the destiny spots and to validate a move after the user inputs the destination.
    /// </summary>
    public List<Position> lastValidDestinies {get; private set;} // hashset--------------------------------------------
    /// <summary>
    /// Number of pieces that a player has to place.
    /// This field will be set to 0 unless a player receives new pieces to place on the drop zone.
    /// As they place the pieces, this variable is gradually decremented and eventually set to 0 again.
    /// </summary>
    public int unplacedPieces {get; private set;}
    /// <summary>
    /// List containing the position of the free slots in a player's dropzone.
    /// This variable will be updated when a player is asked to place new pieces on the board.
    /// </summary>
    public List<Position> freeSlots {get; private set;}
    /// <summary>
    /// List of the possible subsequential jumps after a player executes a jump over move.
    /// This variable will be updated when the obligation to jump over is noticed.
    /// </summary>
    public List<Position> forcedJumps {get; private set;}
    /// <summary>
    /// Position resulting from the last jump over move.
    /// This variable will be updated when the obligation to jump over is noticed.
    /// </summary>
    public Position lastJumpPosition {get; private set;}
    public bool isOver {get; private set;}
    /// <summary>
    /// Convinience array storing the number of each player's pieces.
    /// numPieces[0] -> number of pieces of the player 1
    /// numPieces[1] -> number of pieces of the player 2
    /// </summary>
    public int[] numPieces {get; private set;}
    public string currentFeedback {get; private set;}
    public Clock time;

    public Match(int gameMode) {
        this.gameMode = gameMode;
        playerTurn = 1;
        unplacedPieces = 0;
        isOver = false;
        numPieces = new int[]{
            PlayerPrefs.HasKey("numPiecesPlayer1")? PlayerPrefs.GetInt("numPiecesPlayer1") : 16, 
            PlayerPrefs.HasKey("numPiecesPlayer2")? PlayerPrefs.GetInt("numPiecesPlayer2") : 16
        };
        forcedJumps = new List<Position>();
        freeSlots = new List<Position>();
        board = new Board();
        lastValidDestinies = new List<Position>();
        time = new Clock();
    }

    /// <summary>
    /// Updates the list of valid moves based on the last selected starting position.
    /// </summary>
    /// <param name="startPosition">Start position of the moves</param>
    public void UpdateValidDestinies(Position startPosition) {
        lastValidDestinies = GameLogic.GetValidDestinies(this, startPosition);
    }

    /// <summary>
    /// Executes a move by updating the board cells and the next player's turn.
    /// </summary>
    /// <param name="startPosition">Initial position of the target piece</param>
    /// <param name="endPosition">Destiny position of the target piece</param>
    /// <returns>A tip for the next move if applicable.</returns>
    public void ExecuteMove(Move move) {
        currentFeedback = "";
        forcedJumps.Clear();
        // If the destiny is not contained in the previously computed possible destinations, then it is an invalid move
        if(!lastValidDestinies.Contains(move.endPosition) && !(playerTurn == 2 && gameMode == SinglePlayer)) {
            return; 
        }
        switch(board.ApplyMove(move)) {
            case MoveType.Jump:
                EnforceSequentialJump(move.endPosition);
                break;
            case MoveType.Capture:
                HandleCapture(move.endPosition);
                break;
            case MoveType.Normal:
                ChangeTurn();
                break;
            default:
                Debug.Log("Invalid move type!");
                break;
        }
        if(!isOver && move.endPosition.EndOfBoard()) { // Checks if the piece has reached the end of board
            HandleEndOfBoardArrival(move.endPosition); 
        }
    }

    /// <summary>
    /// Handles and enforces the consequences of a jump move.
    /// Switches player turn unless another jump move from the <c>lastJumpPosition</c> is possible.
    /// If it is possible, updates the <c>lastJumpPosition</c> variable so as to valid the next jump, which must start from that position.
    /// </summary>
    /// <param name="lastCapturePosition">Destiny position of the jump move</param>
    public void EnforceSequentialJump(Position lastJumpPosition) {
        forcedJumps = GameLogic.GetJumpMoveDestinies(this, lastJumpPosition);
        if(forcedJumps.Count > 0) {
            this.lastJumpPosition = lastJumpPosition; // Stores the landing position to ensure the next jump starts there
            currentFeedback = MoveFeedback.KeepJumping; // Updates the feedback
        } else {
            ChangeTurn();
        }
    }

    /// <summary>
    /// Handles and enforces the consequences of a capture move.
    /// Decrements the number of pieces of the player whose piece was captured. If that number reaches 0, sets the game as over.
    /// Switches player turn unless another capture from the <c>lastCapturePosition</c> is possible.
    /// </summary>
    /// <param name="lastCapturePosition">Destiny position of the capture move</param>
    public void HandleCapture(Position lastCapturePosition) {
        int opponentPlayer = NextPlayer(playerTurn);
        numPieces[opponentPlayer - 1]--;

        // If all opponent's pieces were captured, they lost - it's the end of match
        if(isGameOver()) { 
            isOver = true;
            currentFeedback = (playerTurn == 1? "White" : "Black") + " wins the game!";
            return;
        }
        if(GameLogic.GetCaptureMoveDestinies(this, lastCapturePosition).Count > 0) {
            // Note: player turn is not changed, capture is mandatory while possible
            currentFeedback = MoveFeedback.KeepCapturing; // Updates the feedback                  
        } else {
            ChangeTurn();
        }
    }

    /// <summary>
    /// Places a new piece on the board at a specified position, provided it is a free cell located in the drop zone.
    /// </summary>
    /// <param name="spawnPosition">Position where the piece should be placed</param>
    public bool isGameOver() {
        return numPieces[0] == 0 || numPieces[1] == 0;
    }

    /// <summary>
    /// Handles the particular event that occurs when a piece reaches the other side of the board.
    /// Updates the <c>unplacedPieces</c> with the number of awarded pieces.
    /// Updates the <c>freeSlots</c> variable with the positions where the player may place the awarded pieces.
    /// </summary>
    /// <param name="endPosition">Position where the end of board arrival occured</param>
    public void HandleEndOfBoardArrival(Position endPosition) {
        playerTurn = board.GetCellValue(endPosition);
        board.SetCellValue(endPosition, EmptyCell); // Piece is removed
        numPieces[playerTurn - 1]--;
        freeSlots = board.GetFreeSpawnSpots(playerTurn);
        if((unplacedPieces = Math.Min(freeSlots.Count, 2)) == 0) {
            currentFeedback = MoveFeedback.NoPiecesGiven;
            // Player turn is changed since no piece can be placed
            ChangeTurn();
        } else {
            // Player turn is not changed, the current player needs to choose the position for the awarded pieces
            currentFeedback = MoveFeedback.ChooseSpots;
        }
    }

    /// <summary>
    /// Places a new piece on the board at a specified position, provided it is a free cell located in the drop zone.
    /// </summary>
    /// <param name="spawnPosition">Position where the piece should be placed</param>
    public void SpawnPiece(Position spawnPosition) {
        if(freeSlots.Contains(spawnPosition)) {
            board.SetCellValue(spawnPosition, (byte) playerTurn);
            numPieces[playerTurn - 1]++; // Increase number of pieces
            unplacedPieces--; 
            freeSlots.Remove(spawnPosition); // Ensures that if there is another piece to place, it is placed elsewhere
            if(unplacedPieces == 0) { // No more pieces to spawn
                currentFeedback = "";
                playerTurn = NextPlayer(playerTurn); // Player turn changes
            }
        }
    }

    /// <summary>
    /// Changes the player turn, i.e. the next player to make a move.
    /// </summary>
    public void ChangeTurn() {
        playerTurn = NextPlayer(playerTurn);
    }

    /// <summary>
    /// Sets the AI difficulty.
    /// </summary>
    public void SetDifficulty(int difficulty) {
        this.difficulty = difficulty;
    }

    /// <summary>
    /// Clones the current Match.
    /// </summary>
    /// <returns>A copy of the current Match.</returns>
    public Match Clone() {
        Match cloned = (Match) this.MemberwiseClone();
        cloned.board = board.Clone();
        cloned.freeSlots = new List<Position>(freeSlots);
        cloned.forcedJumps = new List<Position>(forcedJumps);
        cloned.numPieces = (int[]) numPieces.Clone();
        return cloned;
    }
}