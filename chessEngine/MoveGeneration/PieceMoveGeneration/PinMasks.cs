namespace chessEngine.MoveGeneration;

public static class PinMasks
{
    // The 8 ray directions as (file shift, rank shift)
    // Rank shift is done as *8 in bit terms
    // File masks prevent wrap-around
    
    private static readonly (int fileDelta, int shift, ulong avoidWrap)[] Directions = new[]
    {
        // Rook-style (cardinal)
        ( 0,  8, 0UL),                      // North
        ( 0, -8, 0UL),                      // South
        (-1, -1, MoveGeneration.FILE_A),    // West
        ( 1,  1, MoveGeneration.FILE_H),    // East

        // Bishop-style (diagonal)
        (-1,  7, MoveGeneration.FILE_A),    // NorthWest
        ( 1,  9, MoveGeneration.FILE_H),    // NorthEast
        (-1, -9, MoveGeneration.FILE_A),    // SouthWest
        ( 1, -7, MoveGeneration.FILE_H),    // SouthEast
    };

    // Which enemy pieces can pin along cardinal vs diagonal rays
    // Cardinals: rooks and queens
    // Diagonals: bishops and queens
    private static ulong GetCardinalPinners(Board board, bool isWhite)
        => isWhite
            ? board.blackRooks | board.blackQueens
            : board.whiteRooks | board.whiteQueens;

    private static ulong GetDiagonalPinners(Board board, bool isWhite)
        => isWhite
            ? board.blackBishops | board.blackQueens
            : board.whiteBishops | board.whiteQueens;

    /// <summary>
    /// Returns an array of 64 pin masks, indexed by square index (bit position).
    /// For each friendly piece, its pin mask is the ray it must stay on.
    /// Unpinned pieces get ulong.MaxValue (no restriction).
    /// </summary>
    public static ulong[] GetPinMasks(Board board)
    {
        ulong[] pinMasks = new ulong[64];
        
        // Default: all pieces are unpinned (can move anywhere)
        for (int i = 0; i < 64; i++)
            pinMasks[i] = ulong.MaxValue;

        bool isWhite = board.whiteToMove;
        ulong king = isWhite ? board.whiteKing : board.blackKing;
        ulong friendly = isWhite ? board.whitePieces : board.blackPieces;
        ulong occupied = board.allPieces;

        ulong cardinalPinners = GetCardinalPinners(board, isWhite);
        ulong diagonalPinners = GetDiagonalPinners(board, isWhite);

        int kingSquare = BitScan(king);

        for (int dir = 0; dir < 8; dir++)
        {
            var (fileDelta, shift, avoidWrap) = Directions[dir];

            // Is this a diagonal or cardinal ray?
            ulong validPinners = (dir < 4) ? cardinalPinners : diagonalPinners;

            ulong ray = 0UL;
            ulong current = king;
            ulong pinnedCandidate = 0UL;
            bool foundFriendly = false;

            while (true)
            {
                // Check wrap-around before shifting
                if ((current & avoidWrap) != 0) break;

                // Shift in this direction
                current = shift > 0
                    ? current << shift
                    : current >> (-shift);

                if (current == 0) break;

                ray |= current;

                if ((current & friendly) != 0)
                {
                    if (foundFriendly)
                        break; // Second friendly piece — no pin possible

                    foundFriendly = true;
                    pinnedCandidate = current;
                }
                else if ((current & occupied) != 0)
                {
                    // Hit an enemy piece
                    if (foundFriendly && (current & validPinners) != 0)
                    {
                        // Confirmed pin — restrict that piece to this ray
                        int pinnedSquare = BitScan(pinnedCandidate);
                        pinMasks[pinnedSquare] = ray; // ray includes attacker square
                    }
                    break; // Either way, ray is blocked
                }
            }
        }

        return pinMasks;
    }

    // Returns index (0-63) of the lowest set bit
    private static int BitScan(ulong bb)
    {
        return System.Numerics.BitOperations.TrailingZeroCount(bb);
    }
}