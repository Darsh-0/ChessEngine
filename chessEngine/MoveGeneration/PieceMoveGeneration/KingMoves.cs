using static chessEngine.MoveGeneration.MoveGeneration;
using System.Collections.Generic;

namespace chessEngine.MoveGeneration;

public static class KingMoves
{
    public static List<Move> GenerateKingMoves(Board board)
    {
        List<Move> legalMoves = new List<Move>();
        
        bool isWhite = board.whiteToMove;
        bool canCastleKingSide = isWhite ? board.whiteCastleKingSide : board.blackCastleKingSide;
        bool canCastleQueenSide = isWhite ? board.whiteCastleQueenSide : board.blackCastleQueenSide;

        ulong king = isWhite ? board.whiteKing : board.blackKing;
        ulong friendly = board.friendlyPieces;

        ulong currentSquare = king;

        ulong attackingSquares = 0;
        
        if ((currentSquare & FILE_A) == 0)
        {
            attackingSquares |= (currentSquare << 7) | (currentSquare >> 1) | (currentSquare >> 9);
        }
        
        if ((currentSquare & FILE_H) == 0)
        {
            attackingSquares |= (currentSquare << 9) | (currentSquare << 1) | currentSquare >> (7);
        }

        attackingSquares |= (currentSquare >> 8) | (currentSquare << 8);
        
        attackingSquares &= ~friendly;
        
        //casteling

        if (canCastleKingSide)
        {
            ulong emptySquares = isWhite ? ((1UL << 5) | (1UL << 6)) : ((1UL << 61) | (1UL << 62));

            ulong targetSquare = isWhite ? (1UL << 6) : (1UL << 62);

            if ((board.allPieces & emptySquares) == 0)
            {
                legalMoves.Add(new Move
                {
                    from = king,
                    to = targetSquare,
                    castle = true
                });
            }
        }

        if (canCastleQueenSide)
        {
            ulong emptySquares = isWhite ? ((1UL << 1) | (1UL << 2) | (1UL << 3)) : ((1UL << 57) | (1UL << 58) | (1UL << 59));

            ulong targetSquare = isWhite ? (1UL << 2) : (1UL << 58);

            if ((board.allPieces & emptySquares) == 0)
            {
                legalMoves.Add(new Move
                {
                    from = king,
                    to = targetSquare,
                    castle = true
                });
            }
        }
        
        while (attackingSquares != 0)
        {
            ulong to = attackingSquares & (~attackingSquares + 1);
            legalMoves.Add(new Move
            {
                from = currentSquare,
                to = to
            });
            attackingSquares &= attackingSquares - 1;
        }
        return legalMoves;
    }
    
    public static ulong GenerateKingAttacks(Board board)
    {
        bool isWhite = board.whiteToMove;
        ulong king = isWhite ? board.whiteKing : board.blackKing;
        ulong attacks = 0;

        if ((king & FILE_H) == 0)
        {
            attacks |= king << 9 | king >> 7 | king << 1;
        }
        if ((king & FILE_A) == 0)
        {
            attacks |= king << 7 | king >> 9 | king >> 1;
        }
        attacks |= king << 8 | king >> 8;

        return attacks;
    }
}