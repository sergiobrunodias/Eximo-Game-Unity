using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;  

[Serializable]  
public class MoveSequence {
    public List<Move> moves;

    public MoveSequence() {
        moves = new List<Move>();
    }

    /// <summary>
    /// Adds a move to the sequence.
    /// </summary>
    /// <param name="move">Move to add</param>
    public void AddMove(Move move) {
        moves.Add(move);
    }

    public MoveSequence Clone() {
        MoveSequence cloned = new MoveSequence();
        foreach(Move move in moves) {
            cloned.moves.Add(move.Clone());
        }
        return cloned;
    }

    public override string ToString() {
        string str = "(";
        foreach(Move move in moves) {
            str += move.ToString() + " ";
        }
        return str + ")";
    }
}