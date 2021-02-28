using static utils.Constants;
using static utils.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using models.Position;

enum InputState {
    PieceSelection,
    DestinySelection,
    SpawnSelection
}

public class GameController : MonoBehaviour {

    public Match match;
    public AI aI;

    // Unity Elements
    public Button[] boardButtons;
    public Sprite[] boardIcons;
    public TextMeshProUGUI tip, timeString, playerTurnIndicator, player1Pieces, player2Pieces, player2Name;
    public GameObject saveWindow;
    public AudioSource pieceMovementSound, winSound;

    // State control
    private InputState inputState;
    private int lastSelection;

    public void Start() {
        bool loadingMatch = PlayerPrefs.HasKey("matchToLoad"); 

        if(loadingMatch) { // Loading a previously saved game
            match = SaveSystem.LoadMatch(PlayerPrefs.GetString("matchToLoad"));
            PlayerPrefs.DeleteKey("matchToLoad");
        } else { // Starting a game from scratch
            match = new Match(PlayerPrefs.GetInt("gameMode", SinglePlayer));
        }

        if(match.gameMode == SinglePlayer) { // Human vs AI match
            if(!loadingMatch) { // If a new match is started, the difficulty set in configurations is applied
                match.SetDifficulty(PlayerPrefs.GetInt("difficulty"));
            }
            aI = new AI(match);
            player2Name.text = "Bot (" + DifficultyName(match.difficulty) + ")";
        }
        
        inputState = InputState.PieceSelection;
        timeString.text = "";  
        DrawCurrentState();
    }

    public void Update() {
        UpdateTimer();
    }

    /// <summary>
    /// Updates the match scene with the current game state.
    /// </summary>
    public void DrawCurrentState() {
        DrawBoard();
        UpdateNumPieces();
        UpdatePlayerTurnIndicator();
    }

    /// <summary>
    /// Displays the board according to the current game state.
    /// </summary>
    public void DrawBoard() {
        // White and black background spots in the first two rows for an 8x8 board (this pattern is repeated 4x)
        List<int> whiteSpots = new List<int>{0, 2, 4, 6, 9, 11, 13, 15};
        List<int> blackSpots = new List<int>{1, 3, 5, 7, 8, 10, 12, 14}; 

        foreach(int spot in whiteSpots) {
            // Pattern is repeated 4 times to cover the 8 rows
            for(int slice = 0; slice < 4; slice++) {
                int cellId = spot + slice * 16;
                Position position = IdToPosition(cellId);
                int value = match.board.GetCellValue(position);
                boardButtons[cellId].GetComponent<Image>().sprite = boardIcons[value + 3];
            }
        }
        foreach(int spot in blackSpots) {
            for(int slice = 0; slice < 4; slice++) {
                int cellId = spot + slice * 16;
                Position position = IdToPosition(cellId);
                int value = match.board.GetCellValue(position);
                boardButtons[cellId].GetComponent<Image>().sprite = boardIcons[value];
            }
        } 

        if(PlayerPrefs.GetInt("highlightStyle", 0) == 1) {
            ResetHighlighting();
        }
        
    }

    /// <summary>
    /// Handles the event of clicking a board button.
    /// </summary>
    /// <param name="cellId">Id of the cell that was clicked</param>
    /// <remarks>
    /// Handler for the board buttons (set up via inspector).
    /// </ramarks>
    public void OnBoardClick(int cellId) { 
        if(match.gameMode == SinglePlayer && match.playerTurn == 2) // Inputs are ignored it is AI's turn
            return;

        switch(inputState) {
            case InputState.PieceSelection:
                HandlePieceSelection(cellId);
                break;
            case InputState.DestinySelection:
                HandleDestinySelection(cellId);
                break;
            case InputState.SpawnSelection:
                HandleSpawnSelection(cellId);
                break;
        }
    }

    /// <summary>
    /// Handles the selection of the starting position for a move and saves it for later reference.
    /// Highlights the possible end positions for the potentially selected piece.
    /// </summary>
    /// <param name="cellId">Id of the cell that was clicked</param>
    public void HandlePieceSelection(int cellId) {
        match.UpdateValidDestinies(IdToPosition(cellId));

        if(match.lastValidDestinies.Count == 0) {
            highlightStartPositions();
            return; // State is not changed
        }

        HighlightCells(match.lastValidDestinies);
        lastSelection = cellId;
        inputState = InputState.DestinySelection;
    }

    /// <summary>
    /// Handles the selection of the end position, i.e. the event that occurs when a move is fully input.
    /// Requests the match model to execute the input move and updates UI according to the resulting game state.
    /// If the <paramref name="cellId"/> matches a piece of player's own, it is treated as a starting position selection.
    /// </summary>
    /// <param name="cellId">Id of the cell that was clicked</param>
    public void HandleDestinySelection(int cellId) {
        // If a player selects a piece of their own as the destiny, it is assumed to be the new target piece
        if(match.board.GetCellValue(IdToPosition(cellId)) == match.playerTurn) {
            DrawCurrentState(); // Removes the highlight
            HandlePieceSelection(cellId);
            inputState = InputState.DestinySelection; // We remain in the same state
        } else { // Attempts to execute the move
            Move move = new Move(IdToPosition(lastSelection), IdToPosition(cellId));
            match.ExecuteMove(move);
            PlayGameAudioIfEnabled(pieceMovementSound);
            DrawCurrentState(); // Reflects the new move into the view
            HandleMoveEnd();
            if(match.isOver) {
                PlayGameAudioIfEnabled(winSound);
                DisableBoardButtons();
            }
        }
    }

    /// <summary>
    /// Handles the selection of a cell for a new piece to be placed on.
    /// </summary>
    /// <param name="cellId">Id of the cell where the piece should be placed on</param>
    public void HandleSpawnSelection(int cellId) {
        match.SpawnPiece(IdToPosition(cellId));
        PlayGameAudioIfEnabled(pieceMovementSound);
        DrawCurrentState();
        HandleMoveEnd();
    }

    /// <summary>
    /// Updates the controller state after a move is executed.
    /// Updates the displayed feedback and player turn.
    /// </summary>
    public void HandleMoveEnd() {
        tip.text = match.currentFeedback;

        if(match.unplacedPieces > 0) {
            // If there are pieces to place, next input is about their position
            inputState = InputState.SpawnSelection;
            HighlightCells(match.freeSlots);
        } else {
            inputState = InputState.PieceSelection;
        }

        UpdatePlayerTurnIndicator();

        // If the second player is human, they can play by using the buttons.
        // If the second player is computer, AI must be called after the human player finishes their turn.
        if(match.gameMode == SinglePlayer && match.playerTurn == 2) {
            tip.text = "Bot is thinking...";
            StartCoroutine(TriggerAi());
        }
    }

    /// <summary>
    /// Gets the AI move sequence for the current state and plays it in a separate thread.
    /// </summary>
    public IEnumerator TriggerAi() {
        yield return null;
        MoveSequence aIMoveSequence = aI?.Play();
        tip.text = "";
        StartCoroutine(PlayMoveSequence(aIMoveSequence));
    }

    /// <summary>
    /// Sequentially plays an AI move.
    /// </summary>
    /// <param name="moveSequence">The move sequence to be played</param>
    /// <remarks>
    /// The delay between each frame is the same as defined in PlayerPrefs('moveTime').
    /// If unset, the default move time is applied.
    /// DefaultMoveTime is definied in <see cref="Constants"/>.
    /// </remarks>
    public IEnumerator PlayMoveSequence(MoveSequence moveSequence) {
        foreach(Move move in moveSequence.moves) {
            HighlightCells(new List<Position>{move.endPosition});
            yield return new WaitForSeconds(PlayerPrefs.GetFloat("moveTime", 1));
            match.ExecuteMove(move);
            PlayGameAudioIfEnabled(pieceMovementSound);
            DrawCurrentState();
            if(match.isOver) {
                PlayGameAudioIfEnabled(winSound);
                tip.text = match.currentFeedback;
                DisableBoardButtons();
                yield break; // Exists the coroutine; since the game is over, no more moves should be drawn.
            }
        }
        if(match.unplacedPieces > 0) {
            Position[] aISpawnPositions = aI?.SpawnPieces();
            StartCoroutine(PlayPieceSpawning(aISpawnPositions));
        }
    }

    /// <summary>
    /// Sequentially spawns AI pieces in the dropzone. 
    /// </summary>
    /// <param name="spawnPositions">An array containing the positions where the pieces will spawn</param>
    /// <remarks>
    /// The delay between each frame is the same as defined in PlayerPrefs('moveTime').
    /// If unset, the default move time is applied.
    /// DefaultMoveTime is definied in <see cref="Constants"/>.
    /// </remarks>
    public IEnumerator PlayPieceSpawning(Position[] spawnPositions) {
        foreach(Position position in spawnPositions) {
            yield return new WaitForSeconds(PlayerPrefs.GetFloat("moveTime", 1));
            match.SpawnPiece(position);
            PlayGameAudioIfEnabled(pieceMovementSound);
            DrawCurrentState();
        }
    }

    /// <summary>
    /// Disables all the board buttons by making them uninteractable.
    /// </summary>
    public void DisableBoardButtons() {
        foreach(Button boardButton in boardButtons) {
            boardButton.GetComponent<Button>().interactable = false;
        }
    }

    /// <summary>
    /// Highlights a set of specified cells using the selected style.
    /// </summary>
    /// <param name="cells">List containing the position of the cells being highlighted</param>
    public void HighlightCells(List<Position> cells) {
        switch(PlayerPrefs.GetInt("highlightStyle", 0)) {
            case 0:
                HighlightCells_1(cells);
                break;
            case 1:
                HighlightCells_2(cells);
                break;
        }
    }

    /// <summary>
    /// Highlights a set of specified cells by drawing a yellow square around them.
    /// </summary>
    /// <param name="cells">List containing the position of the cells being highlighted</param>
    public void HighlightCells_1(List<Position> cells) {
        foreach(Position position in cells) {
            // White background spots in the first two rows for an 8x8 board
            HashSet<int> whiteSpots = new HashSet<int>{0, 2, 4, 6, 9, 11, 13, 15};
            int cellId = PositionToId(position);
            Sprite highlighted;
            
            if(whiteSpots.Contains(cellId % 16)) {
                highlighted = boardIcons[WhiteCellHighlighted];
            } else {
                highlighted = boardIcons[BlackCellHighlighted];
            }

            boardButtons[cellId].GetComponent<Image>().sprite = highlighted;
        }
    }

    /// <summary>
    /// Highlights a set of specified cells by adjusting color parameters.
    /// </summary>
    /// <param name="cells">List containing the position of the cells being highlighted</param>
    public void HighlightCells_2(List<Position> cells) {
        for(int cellId = 0; cellId < boardButtons.Length; cellId++) {
            Position position = IdToPosition(cellId);
            Image cellIcon = boardButtons[cellId].GetComponent<Image>();
            var cellColor = cellIcon.color;
            if(cells.Contains(position)) {
                cellColor.a = 1f;
                cellColor.b = 0.7f;
                cellIcon.color = cellColor;
            } else {
                cellColor.a = 0.5f;
                cellColor.b = 1f;
                cellIcon.color = cellColor;
            }
        }
    }

    /// <summary>
    /// Removes the transparency highlighting from every piece in the board.
    /// </summary>
    public void ResetHighlighting() {
        for(int cellId = 0; cellId < boardButtons.Length; cellId++) {
            Position position = IdToPosition(cellId);
            Image cellIcon = boardButtons[cellId].GetComponent<Image>();
            var cellColor = cellIcon.color;
            cellColor.a = 1f;
            cellColor.b = 1f;
            cellIcon.color = cellColor;
        }
    }

    /// <summary>
    /// Saves the current match if it is in a consistent state.
    /// </summary>
    public void SaveGame() {
        // If it is a singleplayer match, the game cannot be saved while the bot is executing a move.
        if(match.gameMode == SinglePlayer && match.playerTurn == 2) {
            return;
        }
        // If there is a sequential jump move going on, the game cannot be saved.
        if(match.forcedJumps.Count > 0) {
            return;
        }
        // Finished matches cannot be saved.
        if(match.isOver) {
            return;
        }
        SaveSystem.SaveMatch(match);
        saveWindow.SetActive(true);
        StartCoroutine(CloseSaveWindow());
    }

    /// <summary>
    /// Wipes off the saving confirmation message after one second.
    /// </summary>
    public IEnumerator CloseSaveWindow() {
        yield return new WaitForSeconds(1);
        saveWindow.SetActive(false);
    }

    /// <summary>
    /// Handles the updating of the game clock.
    /// </summary>
    public void UpdateTimer() {
        if(match != null && !match.isOver) {
            match.time.Update();
            timeString.text = match.time.ToString();   
        }
    }

    /// <summary>
    /// Handles the updating of the player turn indicator.
    /// </summary>
    public void UpdatePlayerTurnIndicator() {
        int playerTurn = match.playerTurn;
        playerTurnIndicator.text = playerTurnIndicator.text.Remove(0, 5).Insert(0, playerTurn == 1? "White" : "Black");
    } 

    /// <summary>
    /// Updates the displayed number of pieces for each player according to the game state.
    /// </summary>
    public void UpdateNumPieces() {
        player1Pieces.text = match.numPieces[0] + "";
        player2Pieces.text = match.numPieces[1] + "";
    }

    /// <summary>
    /// Highlights the positions where the current valid moves start from.
    /// If there is one or more valid moves starting at each position, that position is highlighted for 2 seconds.
    /// </summary>
    public void highlightStartPositions() {
        StartCoroutine(highlightStartPositionsAux());
    }

    /// <summary>
    /// Auxiliary coroutine to execute highlightStartPositions().
    /// </summary>
    public IEnumerator highlightStartPositionsAux() {
        List<Position> startPositions = new List<Position>(); 
        if(match.forcedJumps.Count > 0) {
            startPositions.Add(match.lastJumpPosition);
        } else {
            List<Move> availableMoves = GameLogic.GetValidMoves(match);
            foreach(Move move in availableMoves) {
                startPositions.Add(move.startPosition);
            }
        }
        HighlightCells_2(startPositions);
        yield return new WaitForSeconds(1);
        ResetHighlighting();
    }
}



