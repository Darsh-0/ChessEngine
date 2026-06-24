using chessEngine.MoveGeneration.MagicBitBoards;
using System.Collections.Generic;
using ChessBot.Core.Core;
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
                Piece? captured = (to & board.enemyPieces) != 0 
                    ? board.GetPieceOnSquare(to) 
                    : null;
                legalMoves.Add(new Move { from = currentSquare, to = to, capturedPiece = captured });
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
        ulong occupancy = board.allPieces ^ (isWhite ? board.whiteKing : board.blackKing);
        ulong attacks = 0;

        while (queens != 0)
        {
            ulong currentSquare = queens & (~queens + 1);
            int sq = MagicBitboards.BitIndex(currentSquare);
            attacks |= MagicBitboards.GetRookAttacks(sq, occupancy)
                       | MagicBitboards.GetBishopAttacks(sq, occupancy);
            queens &= queens - 1;
        }
        return attacks;
    }
}



