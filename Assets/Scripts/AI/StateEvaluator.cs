using static utils.Constants;
using static utils.Utils;
using System;
using System.Collections.Generic;
using models.Position;

public static class StateEvaluator {

    /// <summary>
    /// Evaluates a game state from the standpoint of a given player.
    /// </summary>
    /// <param name="match">The match (game state) to check against</param>
    /// <param name="player">Player to check against</param>
    /// <returns>The move sequence considered the best</returns>
    /// <remarks>
    /// Player is identified by an integer with a value of either 1 or 2.
    /// <remarks>
    public static int EvaluateState(Match match, int player) {
        switch(match.difficulty) {
            case Medium: 
                return MediumHeuristic(match, player);
            case MediumPlus: 
                return MediumHeuristic(match, player);
            default: 
                return EasyHeuristic(match, player);
        }
    }

    /// <summary>
    /// Evaluates a game state using the heuristic for medium difficulty.
    /// </summary>
    /// <param name="match">The match (game state) to check against</param>
    /// <param name="player">Player to check against</param>
    /// <returns>The score for the input state</returns>
    /// <remarks>
    /// Player is identified by an integer with a value of either 1 or 2.
    /// <remarks>
    public static int MediumHeuristic(Match match, int player) {
        List<Position> playerPieces = match.board.GetPiecesPositions(2);
        List<Position> opponentPieces = match.board.GetPiecesPositions(1);
        int evaluation = 0;
        foreach(Position pos in playerPieces) {
            int pieceVal = 50;
            int piecePos = player == 1? pos.y : 7 - pos.y;
            int piecePosSide = pos.x == 0 || pos.x == 7? 10 : 0;
            evaluation += pieceVal + piecePos + piecePosSide;
        }
        foreach(Position pos in opponentPieces) {
            int pieceVal = 50; 
            int piecePos = player == 2? pos.y : 7 - pos.y;
            int piecePosSide = pos.x == 0 || pos.x == 7? 10 : 0;
            evaluation -= pieceVal + piecePos + piecePosSide;
        }
        return evaluation; 
    }

    /// <summary>
    /// Evaluates a game state using the heuristic for easy difficulty.
    /// </summary>
    /// <param name="match">The match (game state) to check against</param>
    /// <param name="player">Player to check against</param>
    /// <returns>The score for the input state</returns>
    /// <remarks>
    /// Player is identified by an integer with a value of either 1 or 2.
    /// <remarks>
    public static int EasyHeuristic(Match match, int player) {
        List<Position> playerPieces = match.board.GetPiecesPositions(2);
        List<Position> opponentPieces = match.board.GetPiecesPositions(1);
        return (playerPieces.Count - opponentPieces.Count) * (30 + new System.Random().Next(10));
    }

    /// <summary>
    /// Determines the bonus evaluation that is applied to a game state, according to a ruled strategy.
    /// </summary>
    /// <param name="match">The match (game state) to check against</param>
    /// <param name="player">Player to check against</param>
    /// <returns>The bonus score</returns>
    public static int BonusEvaluation(Match match, int player) {
        // The bonus evaluation is only applied for the enhanced medium difficulty.
        if(match.difficulty != MediumPlus) {
            return 0;
        }
        int opponent = NextPlayer(player);

        // Relative board positions
        int firstLine = player == 1? 0 : BoardLength - 1;
        int lastLine = player == 1? BoardLength - 1 : 0;

        int secondLine = player == 1? 1 : BoardLength - 2;
        int thirdLine = player == 1? 2 : BoardLength - 3;
        int penultimateLine = player == 1? BoardLength - 2 : 1;

        int firstColumn = 0;
        int lastColumn = BoardLength - 1;

        int bonus = 0;

        // Since the boundary columns are critical, having more pieces in each of those than the opponent is considered an advantage
        if(match.board.CountPiecesInColumn(player, firstColumn) >= match.board.CountPiecesInColumn(opponent, firstColumn)) {
            bonus += 60;
        }
        if(match.board.CountPiecesInColumn(player, lastColumn) >= match.board.CountPiecesInColumn(opponent, lastColumn)) {
            bonus += 60;
        }
        
        bool morePiecesOnLeftThanRight = match.board.CountPiecesInColumn(player, firstColumn) - match.board.CountPiecesInColumn(player, lastColumn) > 0;
        
        // If this condition is met, left side attacks have higher chances of being prevented
        if(match.board.GetCellValue(new Position(1, secondLine)) == player && match.board.GetCellValue(new Position(2, firstLine)) == player) {
            bonus += 50 + (morePiecesOnLeftThanRight? 1 : 0);
        }

        // If this condition is met, right side attacks have higher chances of being prevented
        if(match.board.GetCellValue(new Position(6, secondLine)) == player && match.board.GetCellValue(new Position(5, firstLine)) == player) {
            bonus += 50 + (morePiecesOnLeftThanRight? 0 : 1);
        }

        // Incentive to reach the end of the board
        for(int x = 0; x < BoardLength; x++) {
            if(match.board.GetCellValue(new Position(x, lastLine)) == player) {
                bonus += 50;
            }  
        }

        // Defense strategy
        for(int x = 1; x < BoardLength - 1; x++) {
            // If this condition is met, an opponent's piece is just 2 cells far from the end of board
            if(match.board.GetCellValue(new Position(x, thirdLine)) == opponent) {
                // Blocks opponent's movement
                if(match.board.GetCellValue(new Position(x, firstLine)) == player) {
                    bonus += 1;
                    // Enables capturing opponent's piece
                    if(match.board.GetCellValue(new Position(x, secondLine)) == player) {
                        bonus += 1;
                    }
                }
                // Blocks opponent's movement
                if(match.board.GetCellValue(new Position(x - 1, firstLine)) == player) {
                    bonus += 1;
                }
                // Blocks opponent's movement
                if(match.board.GetCellValue(new Position(x + 1, firstLine)) == player) {
                    bonus += 1;
                }
            }
        }
        return bonus;
    } 
}