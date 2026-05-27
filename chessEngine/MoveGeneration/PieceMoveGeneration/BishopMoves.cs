using chessEngine.MoveGeneration.MagicBitBoards;

namespace chessEngine.MoveGeneration;

public static class BishopMoves 
{
    public static List<Move> GenerateBishopMoves(Board board)
    {
        List<Move> legalMoves = new List<Move>();

        bool isWhite = board.whiteToMove;
        ulong bishops = isWhite ? board.whiteBishops : board.blackBishops;
        ulong friendly = board.friendlyPieces;

        while (bishops != 0)
        {
            ulong currentSquare = bishops & (~bishops + 1);
            int sq = MagicBitboards.BitIndex(currentSquare);

            ulong attacks = MagicBitboards.GetBishopAttacks(sq, board.allPieces) & ~friendly;

            while (attacks != 0)
            {
                ulong to = attacks & (~attacks + 1);
                legalMoves.Add(new Move { from = currentSquare, to = to });
                attacks &= attacks - 1;
            }
            bishops &= bishops - 1;
        }
        return legalMoves;
    }
}