using chessEngine.MoveGeneration.MagicBitBoards;

namespace chessEngine.MoveGeneration;

public static class QueenMoves
{
    public static List<Move> GenerateQueenMoves(Board board)
    {
        List<Move> legalMoves = new List<Move>();

        bool isWhite = board.whiteToMove;
        ulong queens = isWhite ? board.whiteQueens : board.blackQueens;
        ulong friendly = board.friendlyPieces;

        while (queens != 0)
        {
            ulong currentSquare = queens & (~queens + 1);
            int sq = MagicBitboards.BitIndex(currentSquare);

            ulong attacks = MagicBitboards.GetQueenAttacks(sq, board.allPieces) & ~friendly;

            while (attacks != 0)
            {
                ulong to = attacks & (~attacks + 1);
                legalMoves.Add(new Move { from = currentSquare, to = to });
                attacks &= attacks - 1;
            }
            queens &= queens - 1;
        }
        return legalMoves;
    }
    
    public static ulong GenerateQueenAttacks(Board board)
    {
        bool isWhite = board.whiteToMove;
        ulong queens = isWhite ? board.whiteQueens : board.blackQueens;
        ulong attacks = 0;

        while (queens != 0)
        {
            ulong currentSquare = queens & (~queens + 1);
            int sq = MagicBitboards.BitIndex(currentSquare);
            attacks |= MagicBitboards.GetRookAttacks(sq, board.allPieces)
                       | MagicBitboards.GetBishopAttacks(sq, board.allPieces);
            queens &= queens - 1;
        }
        return attacks;
    }
}



