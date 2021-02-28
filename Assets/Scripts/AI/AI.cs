using static utils.Constants;
using static utils.Utils;
using static StateEvaluator;
using static GameLogic;
using System;
using System.Collections.Generic;
using UnityEngine;
using models.Position;

[Serializable]
public class AI {
    private Match match;

    public AI(Match match) {
        this.match = match;
    }

    /// <summary>
    /// Returns the AI move for the current game state.
    /// </summary>
    /// <returns>The move sequence to execute</returns>
    public MoveSequence Play() {
        match.forcedJumps.Clear();
        List<MoveSequence> allSequences = GenerateAllSequences(match);
        return FindBestSequence(allSequences);
    }

    /// <summary>
    /// Finds the best position(s) to place new pieces at.
    /// </summary>
    /// <returns>An array with the best positions to place the new pieces at</returns>
    /// <remarks>Depending on how many unplaced pieces the player has, the returned array may contain one or two elements</remarks>
    public Position[] SpawnPieces() {
        List<Position[]> bestPositionSets = new List<Position[]>();
        int bestScore = int.MinValue;
        if(match.unplacedPieces == 2) {
            foreach(Position freeSlot1 in match.freeSlots) 
                foreach(Position freeSlot2 in match.freeSlots) {
                    if(freeSlot1.Equals(freeSlot2)) 
                        continue;
                    Match dummyMatch = match.Clone(); // Creates a copy of the match where each move can be simulated
                    dummyMatch.SpawnPiece(freeSlot1);
                    dummyMatch.SpawnPiece(freeSlot2);
                    int newScore = Minimax(dummyMatch, false, int.MinValue, int.MaxValue, 2) + BonusEvaluation(dummyMatch, 2);
                    if(newScore < bestScore)
                        continue;
                    Position[] positionSet = new Position[2];
                    positionSet[0] = freeSlot1;
                    positionSet[1] = freeSlot2;
                    if(newScore > bestScore) {
                        bestScore = newScore;
                        bestPositionSets.Clear();
                    }
                    bestPositionSets.Add(positionSet);
                } 
        } else {
            foreach(Position freeSlot in match.freeSlots) {
                Match dummyMatch = match.Clone(); // Creates a copy of the match where each move can be simulated
                dummyMatch.SpawnPiece(freeSlot);
                int newScore = Minimax(dummyMatch, false, int.MinValue, int.MaxValue, 2) + BonusEvaluation(dummyMatch, 2);
                if(newScore < bestScore)
                    continue;
                Position[] positionSet = new Position[1];
                positionSet[0] = freeSlot;
                if(newScore > bestScore) {
                    bestScore = newScore;
                    bestPositionSets.Clear();
                }
                bestPositionSets.Add(positionSet);
            } 
        }
        return bestPositionSets[new System.Random().Next(bestPositionSets.Count)]; 
    }

    /// <summary>
    /// Finds the best among the specified move sequences according to the selected heuristic function.
    /// </summary>
    /// <param name="availableSequences">List of the possible move sequences</param>
    /// <returns>The move sequence considered the best</returns>
    private MoveSequence FindBestSequence(List<MoveSequence> availableSequences) {
		int bestScore = int.MinValue;
		List<MoveSequence> bestSequences = new List<MoveSequence>();
        bestSequences.Add(availableSequences[0]); 
		int depth = 2; // Search depth
		foreach(MoveSequence moveSequence in availableSequences) {
			Match dummyMatch = match.Clone(); // Creates a new board to simulate each of the possible moves
            dummyMatch.board.ApplyMoveSequence(moveSequence);
            dummyMatch.ChangeTurn();
			int newScore = Minimax(dummyMatch, false, int.MinValue, int.MaxValue, depth) + BonusEvaluation(dummyMatch, 2    );
            if(newScore == bestScore) {
				bestSequences.Add(moveSequence);
			}
			if(newScore > bestScore) {
				bestScore = newScore;
                bestSequences.Clear();
				bestSequences.Add(moveSequence);
			}
		}
		return bestSequences[new System.Random().Next(bestSequences.Count)]; // Returns a random move among the best
	}

    /// <summary>
    /// Implementation of the Minimax algorithm.
    /// </summary>
    /// <param name="match">Match being evaluated</param>
    /// <param name="maximizing">True if the algorithm is being applied to the maximizing player, false otherwise</param>
    /// <param name="alpha">Best value that the maximizer can guarantee</param>
    /// <param name="beta">Best value that the maximizer can guarantee</param>
    /// <param name="depth">Search depth</param>
    /// <returns>The expected evaluation for the initially input <paramref name="match"/> based on the Minimax algorithm</returns>
    private int Minimax(Match match, bool maximizing, int alpha, int beta, int depth) {
        if(match.isGameOver()) {
            return maximizing? int.MaxValue : int.MinValue;
        }
		if(depth == 1) { // Reach of depth limit
            return EvaluateState(match, 2);
		}
		List<MoveSequence> nextSequences = GenerateAllSequences(match); // Generates the possible moves from the received game state
        
        int bestScore = maximizing? int.MinValue : int.MaxValue;
		foreach(MoveSequence moveSequence in nextSequences) {
            Match dummyMatch = match.Clone(); // Creates a copy of the match where each move can be simulated
            dummyMatch.board.ApplyMoveSequence(moveSequence); // Simulates the move or sequence of moves
            dummyMatch.ChangeTurn();
			int newScore = Minimax(dummyMatch, !maximizing, alpha, beta, depth - 1); // Calls minimax with the new game state
			if (maximizing) {
				bestScore = Math.Max(bestScore, newScore);
				alpha = Math.Max(alpha, newScore);
			} else {
				bestScore = Math.Min(bestScore, newScore);
				beta = Math.Min(beta, newScore);	
            }
            if(beta <= alpha) // Alpha-beta pruning
				break;
		} 
		return bestScore;
	}
}