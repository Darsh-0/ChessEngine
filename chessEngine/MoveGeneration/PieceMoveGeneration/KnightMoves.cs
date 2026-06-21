using static chessEngine.MoveGeneration.MoveGeneration;
using System.Collections.Generic;
using ChessEngine;

namespace chessEngine.MoveGeneration;

public static class KnightMoves
{
    public static List<Move> GenerateKnightMoves(Board board, ulong checkMask, ulong[] pinMasks)
    {
        List<Move> legalMoves = new List<Move>();
        
        bool isWhite = board.whiteToMove;

        ulong knights = isWhite ? board.whiteKnights : board.blackKnights;
        ulong friendly = board.friendlyPieces;

        while (knights != 0)
        {
            ulong currentSquare = knights & (~knights + 1);
            int fromSquare = System.Numerics.BitOperations.TrailingZeroCount(currentSquare);
            ulong moveMask = checkMask & pinMasks[fromSquare];

            ulong attackingSquares = 0;
            if ((currentSquare & (FILE_A | FILE_B)) == 0)
            {
                attackingSquares |= currentSquare << 6 | currentSquare >> 10;
            }

            if ((currentSquare & (FILE_G | FILE_H)) == 0)
            {
                attackingSquares |= currentSquare >> 6 | currentSquare << 10;
            }

            if ((currentSquare & FILE_A) == 0)
            {
                attackingSquares |= currentSquare << 15 | currentSquare >> 17;
            }

            if ((currentSquare & FILE_H) == 0)
            {
                attackingSquares |= currentSquare >> 15 | currentSquare << 17;
            }
            
            attackingSquares &= ~friendly & moveMask;

            while (attackingSquares != 0)
            {
                ulong to = attackingSquares & (~attackingSquares + 1);
                if ((to & friendly) != 0)
                {
                    attackingSquares &= attackingSquares - 1;
                    continue;
                }
                legalMoves.Add(new Move
                {
                    from = currentSquare,
                    to = to
                });
                attackingSquares &= attackingSquares - 1;
            }

            knights &= knights - 1;
        }

        return legalMoves;
    }

    public static ulong GenerateEnemyKnightAttacks(Board board)
    {
        bool isWhite = board.whiteToMove;
        ulong knights = isWhite ? board.blackKnights : board.whiteKnights;
        ulong attacks = 0;

        while (knights != 0)
        {
            ulong currentSquare = knights & (~knights + 1);
            

            if ((currentSquare & (FILE_A | FILE_B)) == 0)
            {
                attacks |= currentSquare << 6 | currentSquare >> 10;
            }
            if ((currentSquare & (FILE_G | FILE_H)) == 0)
            {
                attacks |= currentSquare >> 6 | currentSquare << 10;
            }
            if ((currentSquare & FILE_A) == 0)
            {
                attacks |= currentSquare << 15 | currentSquare >> 17;
            }
            if ((currentSquare & FILE_H) == 0)
            {
                attacks |= currentSquare >> 15 | currentSquare << 17;
            }

            knights &= knights - 1;
        }
        return attacks;
    }
}