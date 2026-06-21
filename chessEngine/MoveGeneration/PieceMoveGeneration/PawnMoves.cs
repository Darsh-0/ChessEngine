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
            if ((all & inFrontSquare) == 0 && (inFrontSquare & moveMask) != 0)
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
                    legalMoves.Add(new Move
                    {
                        from = currentSquare,
                        to = inFrontSquare
                    });
                }

                // move double on first move (unchanged logic style)
                ulong doubleInFrontSquare = isWhite ? currentSquare << 16 : currentSquare >> 16;

                bool onStartRank = isWhite ? (currentSquare & RANK_2) != 0 : (currentSquare & RANK_7) != 0;

                if (onStartRank && (all & inFrontSquare) == 0 
                                && (all & doubleInFrontSquare) == 0 
                                && (doubleInFrontSquare & moveMask) != 0)
                {
                    legalMoves.Add(new Move
                    {
                        from = currentSquare,
                        to = doubleInFrontSquare
                    });
                }
            }

            ulong captureLeft = isWhite ? currentSquare << 7 : currentSquare >> 9;
            ulong captureRight = isWhite ? currentSquare << 9 : currentSquare >> 7;

            if ((currentSquare & FILE_A) == 0 && (captureLeft & enemy) != 0 && (captureLeft & moveMask) != 0)
            {
                if ((currentSquare & promotionRank) != 0)
                {
                    legalMoves.Add(new Move { from = currentSquare, to = captureLeft, promotionPiece = Piece.Queen });
                    legalMoves.Add(new Move { from = currentSquare, to = captureLeft, promotionPiece = Piece.Rook });
                    legalMoves.Add(new Move { from = currentSquare, to = captureLeft, promotionPiece = Piece.Knight });
                    legalMoves.Add(new Move { from = currentSquare, to = captureLeft, promotionPiece = Piece.Bishop });
                }
                else
                {
                    legalMoves.Add(new Move
                    {
                        from = currentSquare,
                        to = captureLeft
                    }); 
                }

            }

            if ((currentSquare & FILE_H) == 0 && (captureRight & enemy) != 0 && (captureRight & moveMask) != 0)
            {
                if ((currentSquare & promotionRank) != 0)
                {
                    legalMoves.Add(new Move { from = currentSquare, to = captureRight, promotionPiece = Piece.Queen });
                    legalMoves.Add(new Move { from = currentSquare, to = captureRight, promotionPiece = Piece.Rook });
                    legalMoves.Add(new Move { from = currentSquare, to = captureRight, promotionPiece = Piece.Knight });
                    legalMoves.Add(new Move { from = currentSquare, to = captureRight, promotionPiece = Piece.Bishop }); 
                }
                else
                {
                    legalMoves.Add(new Move
                    {
                        from = currentSquare,
                        to = captureRight
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

                if ((currentSquare & FILE_A) == 0 && captureLeft == epTarget && resolvesCheck && staysOnPin)
                    legalMoves.Add(new Move { from = currentSquare, to = captureLeft, isEnPassant = true });

                if ((currentSquare & FILE_H) == 0 && captureRight == epTarget && resolvesCheck && staysOnPin)
                    legalMoves.Add(new Move { from = currentSquare, to = captureRight, isEnPassant = true });
            }
            
            pawns &= pawns - 1;
        }

        return legalMoves;
    }
    
    public static ulong GenerateEnemyPawnAttacks(Board board)
    {
        bool isWhite = board.whiteToMove;
        ulong pawns = isWhite ? board.blackPawns: board.whitePawns;
        ulong attacks = 0;

        if (isWhite)
        {
            if ((pawns & FILE_A) == 0)
            {
                attacks |= pawns << 7;
            }
            if ((pawns & FILE_H) == 0)
            {
                attacks |= pawns << 9;
            }
        }
        else
        {
            if ((pawns & FILE_H) == 0)
            {
                attacks |= pawns >> 7;
            }
            if ((pawns & FILE_A) == 0)
            {
                attacks |= pawns >> 9;
            }
        }
        return attacks;
    }
}