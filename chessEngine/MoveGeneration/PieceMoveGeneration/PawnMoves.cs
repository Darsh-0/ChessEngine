using System;
using static chessEngine.MoveGeneration.MoveGeneration;
using System.Collections.Generic;
using ChessBot.Core.Core;
using ChessEngine;

namespace chessEngine.MoveGeneration;

public static class PawnMoves
{


    public static List<Move> GeneratePawnMoves(Board board, ulong checkMask, ulong[] pinMasks)
    {
        List<Move> legalMoves = new List<Move>();

        bool isWhite = board.whiteToMove;
        ulong pawns = isWhite ? board.whitePawns : board.blackPawns;
        ulong enemy = board.enemyPieces;
        ulong all = board.allPieces;
        ulong promotionRank = isWhite ? RANK_7 : RANK_2;

        while (pawns != 0)
        {
            ulong currentSquare = pawns & (~pawns + 1);
            int fromSquare = System.Numerics.BitOperations.TrailingZeroCount(currentSquare);
            ulong moveMask = checkMask & pinMasks[fromSquare];

            ulong inFrontSquare = isWhite ? currentSquare << 8 : currentSquare >> 8;

            // move once
            if ((all & inFrontSquare) == 0)
            {
                if ((inFrontSquare & moveMask) != 0)
                {
                    if ((currentSquare & promotionRank) != 0)
                    {
                        legalMoves.Add(new Move { from = currentSquare, to = inFrontSquare, promotionPiece = Piece.Queen });
                        legalMoves.Add(new Move { from = currentSquare, to = inFrontSquare, promotionPiece = Piece.Rook });
                        legalMoves.Add(new Move { from = currentSquare, to = inFrontSquare, promotionPiece = Piece.Knight });
                        legalMoves.Add(new Move { from = currentSquare, to = inFrontSquare, promotionPiece = Piece.Bishop });
                    }
                    else
                    {
                        legalMoves.Add(new Move { from = currentSquare, to = inFrontSquare });
                    }
                }

                // double push — checked independently, not nested inside single push
                ulong doubleInFrontSquare = isWhite ? currentSquare << 16 : currentSquare >> 16;
                bool onStartRank = isWhite ? (currentSquare & RANK_2) != 0 : (currentSquare & RANK_7) != 0;

                if (onStartRank && (all & doubleInFrontSquare) == 0 && (doubleInFrontSquare & moveMask) != 0)
                {
                    legalMoves.Add(new Move { from = currentSquare, to = doubleInFrontSquare });
                }
            }

            ulong captureLeft = isWhite ? currentSquare << 7 : currentSquare >> 9;
            ulong captureRight = isWhite ? currentSquare << 9 : currentSquare >> 7;

            if ((currentSquare & FILE_A) == 0 && (captureLeft & enemy) != 0 && (captureLeft & moveMask) != 0)
            {
                Piece captured = board.GetPieceOnSquare(captureLeft);
                if ((currentSquare & promotionRank) != 0)
                {
                    legalMoves.Add(new Move { from = currentSquare, to = captureLeft, promotionPiece = Piece.Queen, capturedPiece = captured });
                    legalMoves.Add(new Move { from = currentSquare, to = captureLeft, promotionPiece = Piece.Rook, capturedPiece = captured });
                    legalMoves.Add(new Move { from = currentSquare, to = captureLeft, promotionPiece = Piece.Knight, capturedPiece = captured });
                    legalMoves.Add(new Move { from = currentSquare, to = captureLeft, promotionPiece = Piece.Bishop, capturedPiece = captured });
                }
                else
                {
                    legalMoves.Add(new Move
                    {
                        from = currentSquare,
                        to = captureLeft,
                        capturedPiece = captured 
                    }); 
                }

            }

            if ((currentSquare & FILE_H) == 0 && (captureRight & enemy) != 0 && (captureRight & moveMask) != 0)
            {
                Piece captured = board.GetPieceOnSquare(captureRight);
                if ((currentSquare & promotionRank) != 0)
                {
                    legalMoves.Add(new Move { from = currentSquare, to = captureRight, promotionPiece = Piece.Queen, capturedPiece = captured});
                    legalMoves.Add(new Move { from = currentSquare, to = captureRight, promotionPiece = Piece.Rook, capturedPiece = captured });
                    legalMoves.Add(new Move { from = currentSquare, to = captureRight, promotionPiece = Piece.Knight, capturedPiece = captured });
                    legalMoves.Add(new Move { from = currentSquare, to = captureRight, promotionPiece = Piece.Bishop, capturedPiece = captured }); 
                }
                else
                {
                    legalMoves.Add(new Move
                    {
                        from = currentSquare,
                        to = captureRight,
                        capturedPiece = captured 
                    }); 
                }

            }

            if (board.enPassantFile != -1)
            {
                int epRank = isWhite ? 5 : 2;
                ulong epTarget = 1UL << (epRank * 8 + board.enPassantFile);
                ulong capturedPawn = isWhite ? epTarget >> 8 : epTarget << 8;

                bool resolvesCheck = (epTarget & checkMask) != 0 || (capturedPawn & checkMask) != 0;
                bool staysOnPin = (epTarget & pinMasks[fromSquare]) != 0;

                if (resolvesCheck && staysOnPin)
                {
                    // Horizontal discovery check — simulate the capture and verify the king is safe
                    bool epIsLegal = !EnPassantLeavesKingInCheck(board, currentSquare, capturedPawn, isWhite);

                    if ((currentSquare & FILE_A) == 0 && captureLeft == epTarget && epIsLegal)
                        legalMoves.Add(new Move { from = currentSquare, to = captureLeft, capturedPiece = Piece.Pawn, isEnPassant = true });

                    if ((currentSquare & FILE_H) == 0 && captureRight == epTarget && epIsLegal)
                        legalMoves.Add(new Move { from = currentSquare, to = captureRight, capturedPiece = Piece.Pawn, isEnPassant = true });

                }
            }
            
            pawns &= pawns - 1;
        }

        return legalMoves;
    }
    
    private static bool EnPassantLeavesKingInCheck(Board board, ulong capturingPawn, ulong capturedPawn, bool isWhite)
    {
        ulong king        = isWhite ? board.whiteKing   : board.blackKing;
        ulong enemyRooks  = isWhite ? board.blackRooks  : board.whiteRooks;
        ulong enemyQueens = isWhite ? board.blackQueens : board.whiteQueens;

        // Simulate both pawns being removed from the board
        ulong occupied = (board.allPieces ^ capturingPawn) ^ capturedPawn;

        // Check for rook/queen attacks on the king along the rank
        ulong rankAttackers = enemyRooks | enemyQueens;
        return (HyperbolicQuintessence(king, occupied, isRank: true) & rankAttackers) != 0;
    }

// Sliding attack along a rank using the o^(o-2r) trick
    private static ulong HyperbolicQuintessence(ulong slider, ulong occupied, bool isRank)
    {
        ulong mask = isRank ? RankMask(slider) : FileMask(slider);
        ulong o = occupied & mask;
        return ((o - 2 * slider) ^ Reverse(Reverse(o) - 2 * Reverse(slider))) & mask;
    }

    private static ulong RankMask(ulong square)
    {
        int rank = System.Numerics.BitOperations.TrailingZeroCount(square) / 8;
        return 0xFFUL << (rank * 8);
    }
    
    private static ulong FileMask(ulong square)
    {
        int file = System.Numerics.BitOperations.TrailingZeroCount(square) % 8;
        return FILE_A << file;
    }

    private static ulong Reverse(ulong v)
    {
        // Reverses bit order of a ulong
        v = ((v >> 1)  & 0x5555555555555555UL) | ((v & 0x5555555555555555UL) << 1);
        v = ((v >> 2)  & 0x3333333333333333UL) | ((v & 0x3333333333333333UL) << 2);
        v = ((v >> 4)  & 0x0F0F0F0F0F0F0F0FUL) | ((v & 0x0F0F0F0F0F0F0F0FUL) << 4);
        return System.Buffers.Binary.BinaryPrimitives.ReverseEndianness(v);
    }
    
    public static ulong GenerateEnemyPawnAttacks(Board board)
    {
        bool isWhite = board.whiteToMove;
        ulong pawns = isWhite ? board.blackPawns : board.whitePawns;

        ulong attacks = 0;

        while (pawns != 0)
        {
            ulong pawn = pawns & (~pawns + 1);

            if (isWhite)
            {
                if ((pawn & FILE_H) == 0)
                    attacks |= pawn >> 7;

                if ((pawn & FILE_A) == 0)
                    attacks |= pawn >> 9;
            }
            else // enemy is white
            {
                if ((pawn & FILE_A) == 0)
                    attacks |= pawn << 7;

                if ((pawn & FILE_H) == 0)
                    attacks |= pawn << 9;
            }

            pawns &= pawns - 1;
        }

        return attacks;
    }
}