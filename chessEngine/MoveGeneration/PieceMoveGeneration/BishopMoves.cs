using chessEngine.MoveGeneration.MagicBitBoards;
using System.Collections.Generic;
using ChessBot.Core.Core;
using ChessEngine;

namespace chessEngine.MoveGeneration;

public static class BishopMoves 
{
    public static List<Move> GenerateBishopMoves(Board board, ulong checkMask, ulong[] pinMasks)
    {
        List<Move> legalMoves = new List<Move>();

        bool isWhite = board.whiteToMove;
        ulong bishops = isWhite ? board.whiteBishops : board.blackBishops;
        ulong friendly = board.friendlyPieces;

        while (bishops != 0)
        {
            ulong currentSquare = bishops & (~bishops + 1);
            int sq = MagicBitboards.BitIndex(currentSquare);

            ulong moveMask = checkMask & pinMasks[sq];
            ulong attacks = MagicBitboards.GetBishopAttacks(sq, board.allPieces) & ~friendly & moveMask;

            while (attacks != 0)
            {
                ulong to = attacks & (~attacks + 1);
                Piece? captured = (to & board.enemyPieces) != 0 
                    ? board.GetPieceOnSquare(to) 
                    : null;
                legalMoves.Add(new Move { from = currentSquare, to = to, capturedPiece = captured });
                attacks &= attacks - 1;
            }
            bishops &= bishops - 1;
        }
        return legalMoves;
    }
    
    public static ulong GenerateEnemyBishopAttacks(Board board)
    {
        bool isWhite = board.whiteToMove;
        ulong bishops = isWhite ? board.blackBishops : board.whiteBishops;
        ulong occupancy = board.allPieces ^ (isWhite ? board.whiteKing : board.blackKing);
        ulong attacks = 0;

        while (bishops != 0)
        {
            ulong currentSquare = bishops & (~bishops + 1);
            int sq = MagicBitboards.BitIndex(currentSquare);
            attacks |= MagicBitboards.GetBishopAttacks(sq, occupancy);
            bishops &= bishops - 1;
        }
        return attacks;
    }

}