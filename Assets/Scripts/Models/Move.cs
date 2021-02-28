using static utils.Utils;
using System;
using models.Position;

[Serializable]
public class Move {
    public Position startPosition;
    public Position endPosition;
    public MoveType type;

    public Move(Position startPosition, Position endPosition) {
        this.startPosition = startPosition;
        this.endPosition = endPosition;
    }

    /// <summary>
    /// Calculates the mid point of two points in a board. 
    /// The coordinates of a board position must be integers.
    /// </summary>
    /// <param name="firstPoint">First point</param>
    /// <param name="secondPoint">Second point</param>
    /// <returns>The mid point if it is a valid board position, null otherwise.</returns>
    public Position Midpoint() {
        int offsetX = startPosition.x - endPosition.x;
        int offsetY = startPosition.y - endPosition.y;
        if(offsetX % 2 != 0 || offsetY % 2 != 0) {
            // There is no mid point since we are working in a domain of integers
            return null;
        }
        return new Position(startPosition.x - offsetX/2, startPosition.y - offsetY/2);
    }

    public override string ToString() {
        return startPosition + "->" + endPosition;
    }

    public Move Clone() {
        Move cloned = new Move(startPosition, endPosition);
        cloned.type = type;
        return cloned;
    }
}