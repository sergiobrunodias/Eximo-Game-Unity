using static utils.Constants;
using static utils.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using models.Position;

public static class GameLogic {

    /// <summary>
    /// Checks if any of the current player's pieces is able to capture.
    /// </summary>
    /// <param name="match">Match to check against</param>
    /// <returns>True if some capture is available, false otherwise.</returns>
    public static bool IsCaptureAvailable(Match match) {
        for(int x = 0; x < BoardLength; x++)
            for(int y = 0; y < BoardLength; y++) {
                Position position = new Position(x, y);
                if(GetCaptureMoveDestinies(match, position).Count > 0) {
                    return true;
                }
            }
        return false;
    }

    /// <summary>
    /// Returns a list with the possible destinies for ordinary moves starting in a given position.
    /// </summary>
    /// <param name="match">Match to check against</param>
    /// <param name="startPosition">Start position of the moves</param>
    /// <returns>A list of positions referring to the possible ordinary moves' destiny spots.</returns>
    public static List<Position> GetOrdinaryMoveDestinies(Match match, Position startPosition) {
        List<Position> destinies = new List<Position>();
        // Y pos always changes, no matter if it is an orthogonal or diagonal move
        int offsetY = 1 + match.playerTurn * (1 - match.playerTurn); // "1" if player 1, "-1" if player 2
        int newY = startPosition.y + offsetY;
        destinies.Add(new Position(startPosition.x, newY)); // North move
        destinies.Add(new Position(startPosition.x + 1, newY)); // Diagonal move (Northeast / Northwest)
        destinies.Add(new Position(startPosition.x - 1, newY)); // Diagonal move (Northeast / Northwest)
        // Invalid moves removed from the returned list
        destinies.RemoveAll(destiny => !IsMoveValid(match, new Move(startPosition, destiny)));
        return destinies;
    } 

    /// <summary>
    /// Returns a list with the possible destinies for jump moves starting in a given position.
    /// </summary>
    /// <param name="match">Match to check against</param>
    /// <param name="startPosition">Start position of the moves</param>
    /// <returns>A list of positions referring to the possible jump moves' destiny spots.</returns>
    public static List<Position> GetJumpMoveDestinies(Match match, Position startPosition) {
        List<Position> destinies = new List<Position>();
        // Y pos always changes, no matter if it is an orthogonal or diagonal jump
        int offsetY = (1 + match.playerTurn * (1 - match.playerTurn)) * 2; // "2" if player 1, "-2" if player 2
        int newY = startPosition.y + offsetY;
        destinies.Add(new Position(startPosition.x, newY)); // North jump move
        destinies.Add(new Position(startPosition.x + 2, newY)); // Diagonal jump move (Northeast / Northwest)
        destinies.Add(new Position(startPosition.x - 2, newY)); // Diagonal jump move (Northeast / Northwest)
        // Invalid jump moves removed from the returned list
        destinies.RemoveAll(destiny => !IsMoveValid(match, new Move(startPosition, destiny)) || 
            !IsJumpValid(match, new Move(startPosition, destiny)));
        return destinies;
    }

    /// <summary>
    /// Returns a list with the possible destinies for capture moves starting in a given position.
    /// </summary>
    /// <param name="match">Match to check against</param>
    /// <param name="startPosition">Start position of the moves</param>
    /// <returns>A list of positions referring to the possible capture moves' destiny spots.</returns>
    public static List<Position> GetCaptureMoveDestinies(Match match, Position startPosition) {
        List<Position> destinies = new List<Position>();
        // Y pos changes if it is an orthogonal or diagonal capture
        int offsetY = (1 + match.playerTurn * (1 - match.playerTurn)) * 2; // "2" if player 1, "-2" if player 2
        int newY = startPosition.y + offsetY;
        destinies.Add(new Position(startPosition.x, newY)); // North capture move
        destinies.Add(new Position(startPosition.x + 2, newY)); // Diagonal capture move (Northeast / Northwest)
        destinies.Add(new Position(startPosition.x - 2, newY)); // Diagonal capture move (Northeast / Northwest)
        destinies.Add(new Position(startPosition.x + 2, startPosition.y)); // Side capture move (East / West)
        destinies.Add(new Position(startPosition.x - 2, startPosition.y)); // Side capture move (East / West)
        // Invalid capture moves are removed from the returned list
        destinies.RemoveAll(destiny => !IsMoveValid(match, new Move(startPosition, destiny)) || 
            !IsCaptureValid(match, new Move(startPosition, destiny)));
        return destinies;
    }

    /// <summary>
    /// Analyzes whether a move is valid according to the general moves' preconditions.
    /// Does not include additional verifications for special moves, such as jumps or captures.
    /// </summary>
    /// <param name="match">Match to check against</param>
    /// <param name="move">Move to be tested</param>
    /// <returns>True if the mentioned conditions are met, false otherwise.</returns>
    public static bool IsMoveValid(Match match, Move move) {
        if(!move.endPosition.IsWithinBounderies()) {
            return false;
        }
        // Checks if the piece at the starting position belongs to the player
        if(match.board.GetCellValue(move.startPosition) != match.playerTurn) { 
            return false;
        }
        // Checks if the target cell is free 
        if(!match.board.IsCellEmpty(move.endPosition)) {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if the jump precondition is met, i.e. if there is a piece at the move's midpoint that belongs to the player who executes the move.!--
    /// This method does not check the preconditions of a general move, i.e. it does not extend IsMoveValid()
    /// </summary>
    /// <param name="match">Match to check against</param>
    /// <param name="jumpMove">Jump move to be tested</param>
    /// <returns>True if the jump precondition is met, false otherwise.</returns>
    public static bool IsJumpValid(Match match, Move jumpMove) {
        Position midPoint = jumpMove.Midpoint();
        return match.board.GetCellValue(midPoint) == match.playerTurn;
    }

    /// <summary>
    /// Checks if the capture precondition is met, i.e. if there is an opponent's piece at the move's midpoint.
    /// This method does not check the preconditions of a general move, i.e. it does not extend IsMoveValid()
    /// </summary>
    /// <param name="match">Match to check against</param>
    /// <param name="move">Capture move to be tested</param>
    /// <returns>True if the capture precondition is met, false otherwise.</returns>
    public static bool IsCaptureValid(Match match, Move captureMove) {
        Position midPoint = captureMove.Midpoint();
        return match.board.GetCellValue(midPoint) == NextPlayer(match.playerTurn);
    }

    /// <summary>
    /// Searches for all the possible destinies of moves starting in a specified position.
    /// </summary>
    /// <param name="match">Match to check against</param>
    /// <param name="startPosition">Start position of the moves</param>
    /// <returns>A list containing the destiny positions of the available moves.</returns>
    public static List<Position> GetValidDestinies(Match match, Position startPosition) {
        List<Position> validDestinies = new List<Position>();
        if(match.forcedJumps.Count != 0) { 
            if(startPosition.Equals(match.lastJumpPosition))
                validDestinies.AddRange(match.forcedJumps);
        } else {
            validDestinies.AddRange(GetCaptureMoveDestinies(match, startPosition));
            if(!IsCaptureAvailable(match)) {
                // Other moves are only available if capturing is not possible 
                validDestinies.AddRange(GetJumpMoveDestinies(match, startPosition));
                validDestinies.AddRange(GetOrdinaryMoveDestinies(match, startPosition));
            }
        }
        return validDestinies;
    }

    /// <summary>
    /// Expands a move to form every possible capture or jump sequence from it.
    /// </summary>
    /// <param name="expandingSequence">Move sequence being expanded</param>
    /// <param name="move">Move to expand from</param>
    /// <param name="match">Match to check against</param>
    /// <param name="moveSeqList">List of the expanded move sequences</param>
    /// <remarks>
    /// The <paramref name="moveSeqList"/> should be initialized as a new object before the initial function call.
    /// </remarks>
    private static void ExpandMoveSequences(MoveSequence expandingSequence, Move move, Match match, List<MoveSequence> moveSeqList) {
        expandingSequence.AddMove(move);
        match.ExecuteMove(move);
        List<Position> nextDestinies = move.type == MoveType.Capture? GameLogic.GetCaptureMoveDestinies(match, move.endPosition) : GameLogic.GetJumpMoveDestinies(match, move.endPosition);
        if (nextDestinies.Count == 0) {
            moveSeqList.Add(expandingSequence);
            return;
        }  
        List<Move> nextMoves = ToMovesList(move.endPosition, nextDestinies, move.type == MoveType.Capture? MoveType.Capture : MoveType.Jump);
        foreach(Move nextMove in nextMoves) {
            Match dummyMatch = match.Clone();
            dummyMatch.ExecuteMove(nextMove);
        	MoveSequence moveSeqTemp = expandingSequence.Clone();
        	ExpandMoveSequences(moveSeqTemp, nextMove, match.Clone(), moveSeqList);
        }
    }

    /// <summary>
    /// Finds all the possible move sequences for the current player in a given match.
    /// </summary>
    /// <param name="match">Match to check against</param>
    /// <returns>A list of the possible move sequences</returns>
    public static List<MoveSequence> GenerateAllSequences(Match match) {
        List<Move> validMoves = GameLogic.GetValidMoves(match);
        List<MoveSequence> allSequences = new List<MoveSequence>();
        
        foreach(Move possibleMove in validMoves) {
            Match dummyMatch = match.Clone();
            switch(dummyMatch.board.ApplyMove(possibleMove)) {
                case MoveType.Jump:
                case MoveType.Capture:
                    List<MoveSequence> possibleSequences = new List<MoveSequence>();
                    ExpandMoveSequences(new MoveSequence(), possibleMove, match.Clone(), possibleSequences);
                    allSequences.AddRange(possibleSequences);
                    break;
                case MoveType.Normal:
                    MoveSequence basic = new MoveSequence();
                    basic.AddMove(possibleMove);
                    allSequences.Add(basic);
                    break;
            }
        }
        return allSequences;
    }

    /// <summary>
    /// Searches for all the available moves for the current game state.
    /// </summary>
    /// <param name="match">Match to check against</param>
    /// <returns>The list of available moves.</returns>
    public static List<Move> GetValidMoves(Match match) {
        List<Move> validMoves = new List<Move>();
        for(int x = 0; x < BoardLength; x++)
            for(int y = 0; y < BoardLength; y++) {
                Position position = new Position(x, y);
                validMoves.AddRange(ToMovesList(position, GetCaptureMoveDestinies(match, position), MoveType.Capture));
                if(!IsCaptureAvailable(match)) {
                    // Other moves are only available if capturing is not possible 
                    validMoves.AddRange(ToMovesList(position, GetJumpMoveDestinies(match, position), MoveType.Jump));
                    validMoves.AddRange(ToMovesList(position, GetOrdinaryMoveDestinies(match, position), MoveType.Normal));
                }
            }
        return validMoves;
    }
}