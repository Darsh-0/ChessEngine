using chessEngine.MoveGeneration.MagicBitBoards;
using System.Collections.Generic;
using ChessEngine;

namespace chessEngine.MoveGeneration;

public static class QueenMoves
{
    public static List<Move> GenerateQueenMoves(Board board, ulong checkMask, ulong[] pinMasks)
    {
        List<Move> legalMoves = new List<Move>();

        bool isWhite = board.whiteToMove;
        ulong queens = isWhite ? board.whiteQueens : board.blackQueens;
        ulong friendly = board.friendlyPieces;

        while (queens != 0)
        {
            ulong currentSquare = queens & (~queens + 1);
            int sq = MagicBitboards.BitIndex(currentSquare);

            ulong moveMask = checkMask & pinMasks[sq];
            ulong attacks = MagicBitboards.GetQueenAttacks(sq, board.allPieces) & ~friendly & moveMask;

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
    
    public static ulong GenerateEnemyQueenAttacks(Board board)
    {
        bool isWhite = board.whiteToMove;
        ulong queens = isWhite ? board.blackQueens : board.whiteQueens;
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



