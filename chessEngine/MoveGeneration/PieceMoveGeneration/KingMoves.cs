using System;
using static chessEngine.MoveGeneration.MoveGeneration;
using System.Collections.Generic;
using ChessBot.Core.Core;
using ChessEngine;

namespace chessEngine.MoveGeneration;

public static class KingMoves
{
    public static List<Move> GenerateKingMoves(Board board, ulong enemyAttacks)
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
        
        attackingSquares &= ~friendly & ~enemyAttacks;
        
        //casteling

        if (canCastleKingSide)
        {
            ulong emptySquares = isWhite ? ((1UL << 5) | (1UL << 6)) : ((1UL << 61) | (1UL << 62));
            ulong passThroughSquare = isWhite ? (1UL << 5) : (1UL << 61);
            ulong targetSquare = isWhite ? (1UL << 6) : (1UL << 62);

            bool squaresEmpty = (board.allPieces & emptySquares) == 0;
            bool kingNotInCheck = (king & enemyAttacks) == 0;
            bool pathSafe = (passThroughSquare & enemyAttacks) == 0 && (targetSquare & enemyAttacks) == 0;

            if (squaresEmpty && kingNotInCheck && pathSafe)
            {
                legalMoves.Add(new Move { from = king, to = targetSquare, isCastle = true });
            }
        }

        if (canCastleQueenSide)
        {
            ulong emptySquares = isWhite ? ((1UL << 1) | (1UL << 2) | (1UL << 3)) : ((1UL << 57) | (1UL << 58) | (1UL << 59));
            ulong passThroughSquare = isWhite ? (1UL << 3) : (1UL << 59);
            ulong targetSquare = isWhite ? (1UL << 2) : (1UL << 58);

            bool squaresEmpty = (board.allPieces & emptySquares) == 0;
            bool kingNotInCheck = (king & enemyAttacks) == 0;
            bool pathSafe = (passThroughSquare & enemyAttacks) == 0 && (targetSquare & enemyAttacks) == 0;

            if (squaresEmpty && kingNotInCheck && pathSafe)
            {
                legalMoves.Add(new Move { from = king, to = targetSquare, isCastle = true });
            }
        }
        
        while (attackingSquares != 0)
        {
            ulong to = attackingSquares & (~attackingSquares + 1);
            Piece? captured = (to & board.enemyPieces) != 0 
                ? board.GetPieceOnSquare(to) 
                : null;
            legalMoves.Add(new Move
            {
                from = currentSquare,
                to = to,
                capturedPiece = captured
            });
            attackingSquares &= attackingSquares - 1;
        }
        return legalMoves;
    }
    
    public static ulong GenerateEnemyKingAttacks(Board board)
    {
        bool isWhite = board.whiteToMove;
        ulong king = isWhite ? board.blackKing : board.whiteKing;
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