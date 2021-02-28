using static utils.Constants;
using System;

namespace models.Position
{
    [Serializable]
    public class Position : IEquatable<Position> {
        public int x;
        public int y;

        public Position(int x, int y) {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Checks if the position belongs to the board domain: x ∈ [0,BoardLength[ ∩ y ∈ [0, BoardLength].
        /// </summary>
        /// <returns>True if the self position is within the board bounderies, false otherwise.</returns>
        public bool IsWithinBounderies() {
            return x >= 0 && x < BoardLength && y >= 0 && y < BoardLength;
        }

        /// <summary>
        /// Checks if the position belongs to one end of a board.
        /// </summary>
        /// <returns>True if the self position belongs to one end of a board, false otherwise.</returns>
        public bool EndOfBoard() {
            return y == 0 || y == BoardLength - 1;
        }

        public override string ToString() {
            return "(" + x + ", " + y + ")\n";
        }

        public bool Equals(Position position) {
            return x == position.x && y == position.y;
        }

        public override bool Equals(object obj) {
            if (obj == null || obj.GetType() != this.GetType()) {
                return false;
            }
            return ReferenceEquals(obj, this) || Equals(obj);
        }

        public override int GetHashCode() {
            return x * y + x + y;
        }
    }
}
