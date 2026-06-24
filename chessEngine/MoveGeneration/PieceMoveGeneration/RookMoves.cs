using chessEngine.MoveGeneration.MagicBitBoards;
using System.Collections.Generic;
using ChessBot.Core.Core;
using ChessEngine;

namespace chessEngine.MoveGeneration;

public static class RookMoves
{
    public static List<Move> GenerateRookMoves(Board board, ulong checkMask, ulong[] pinMasks)
    {
        List<Move> legalMoves = new List<Move>();

        bool isWhite = board.whiteToMove;
        ulong rooks = isWhite ? board.whiteRooks : board.blackRooks;
        ulong friendly = board.friendlyPieces;

        while (rooks != 0)
        {
            ulong currentSquare = rooks & (~rooks + 1);
            int sq = MagicBitboards.BitIndex(currentSquare);

            ulong moveMask = checkMask & pinMasks[sq];
            ulong attacks = MagicBitboards.GetRookAttacks(sq, board.allPieces) & ~friendly & moveMask;

            while (attacks != 0)
            {
                ulong to = attacks & (~attacks + 1);
                Piece? captured = (to & board.enemyPieces) != 0 
                    ? board.GetPieceOnSquare(to) 
                    : null;
                legalMoves.Add(new Move { from = currentSquare, to = to, capturedPiece = captured });
                attacks &= attacks - 1;
            }
            rooks &= rooks - 1;
        }

        return legalMoves;
    }
    
    public static ulong GenerateEnemyRookAttacks(Board board)
    {
        bool isWhite = board.whiteToMove;
        ulong rooks = isWhite ? board.blackRooks : board.whiteRooks;
        ulong occupancy = board.allPieces ^ (isWhite ? board.whiteKing : board.blackKing);
        ulong attacks = 0;

        while (rooks != 0)
        {
            ulong currentSquare = rooks & (~rooks + 1);
            int sq = MagicBitboards.BitIndex(currentSquare);
            attacks |= MagicBitboards.GetRookAttacks(sq, occupancy);
            rooks &= rooks - 1;
        }
        return attacks;
    }
}