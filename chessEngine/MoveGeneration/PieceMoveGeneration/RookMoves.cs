using chessEngine.MoveGeneration.MagicBitBoards;

namespace chessEngine.MoveGeneration;

public static class RookMoves
{
    public static List<Move> GenerateRookMoves(Board board)
    {
        List<Move> legalMoves = new List<Move>();

        bool isWhite = board.whiteToMove;
        ulong rooks = isWhite ? board.whiteRooks : board.blackRooks;
        ulong friendly = board.friendlyPieces;
        ulong enemy = board.enemyPieces;

        while (rooks != 0)
        {
            ulong currentSquare = rooks & (~rooks + 1);
            int sq = MagicBitboards.BitIndex(currentSquare);

            ulong attacks = MagicBitboards.GetRookAttacks(sq, board.allPieces) & ~friendly;

            while (attacks != 0)
            {
                ulong to = attacks & (~attacks + 1);
                legalMoves.Add(new Move { from = currentSquare, to = to });
                attacks &= attacks - 1;
            }
            rooks &= rooks - 1;
        }
        return legalMoves;
    }
}