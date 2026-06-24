using System;
using ChessEngine;

namespace chessEngine.MoveGeneration;

using ChessBot.Core.Core;
public class Board
{
    public ulong whitePawns { get; set; }
    public ulong whiteKnights { get; set; }
    public ulong whiteBishops { get; set; }
    public ulong whiteRooks { get; set; }
    public ulong whiteQueens { get; set; }
    public ulong whiteKing { get; set; }

    public ulong blackPawns { get; set; }
    public ulong blackKnights { get; set; }
    public ulong blackBishops { get; set; }
    public ulong blackRooks { get; set; }
    public ulong blackQueens { get; set; }
    public ulong blackKing { get; set; }

    public ulong whitePieces { get; set; }
    public ulong blackPieces { get; set; }
    public ulong allPieces { get; set; }

    public bool whiteToMove { get; set; }

    public ulong friendlyPieces { get; set; }
    public ulong enemyPieces { get; set; }

    public bool whiteCastleKingSide { get; set; }
    public bool whiteCastleQueenSide { get; set; }
    public bool blackCastleKingSide { get; set; }
    public bool blackCastleQueenSide { get; set; }

    public int enPassantFile { get; set; }

    public int halfmoveClock { get; set; }
    public int fullmoveNumber { get; set; }

    // -----------------------------------------------------------------------
    // Make / Undo
    // -----------------------------------------------------------------------

    public void MakeMove(Move move)
    {
        Piece movingPiece = move.promotionPiece ?? GetPieceOnSquare(move.from);

        RemovePiece(move.from, whiteToMove);

        if (move.isEnPassant)
        {
            ulong capturedPawnSquare = whiteToMove
                ? move.to >> 8
                : move.to << 8;
            RemovePiece(capturedPawnSquare, !whiteToMove);
        }
        else if (move.capturedPiece.HasValue)
        {
            RemovePiece(move.to, !whiteToMove);
        }

        Piece placedPiece = move.promotionPiece ?? movingPiece;
        PlacePiece(move.to, placedPiece, whiteToMove);

        if (move.isCastle)
            MoveCastlingRook(move.to);

        UpdateAggregateBitboards();
        UpdateCastlingRights(move, movingPiece);

        enPassantFile = GetNewEnPassantFile(move, movingPiece);

        bool isPawnMove = movingPiece == Piece.Pawn;
        bool isCapture  = move.capturedPiece.HasValue || move.isEnPassant;
        halfmoveClock   = (isPawnMove || isCapture) ? 0 : halfmoveClock + 1;
        if (!whiteToMove) fullmoveNumber++;

        whiteToMove = !whiteToMove;
        UpdateFriendlyEnemy();
    }

    public void UndoMove(Move move,
                         bool prevWhiteCastleKingSide,
                         bool prevWhiteCastleQueenSide,
                         bool prevBlackCastleKingSide,
                         bool prevBlackCastleQueenSide,
                         int  prevEnPassantFile,
                         int  prevHalfmoveClock)
    {
        whiteToMove = !whiteToMove;
        if (!whiteToMove) fullmoveNumber--;

        Piece movedPiece = move.promotionPiece ?? GetPieceOnSquare(move.to);
        RemovePiece(move.to, whiteToMove);

        // If it was a promotion, restore a pawn; otherwise restore the piece that moved
        Piece originalPiece = move.promotionPiece.HasValue ? Piece.Pawn : movedPiece;
        PlacePiece(move.from, originalPiece, whiteToMove);

        if (!move.isEnPassant && move.capturedPiece.HasValue)
            PlacePiece(move.to, move.capturedPiece.Value, !whiteToMove);

        if (move.isEnPassant)
        {
            ulong capturedPawnSquare = whiteToMove
                ? move.to >> 8
                : move.to << 8;
            PlacePiece(capturedPawnSquare, Piece.Pawn, !whiteToMove);
        }

        if (move.isCastle)
            UndoCastlingRook(move.to);

        whiteCastleKingSide  = prevWhiteCastleKingSide;
        whiteCastleQueenSide = prevWhiteCastleQueenSide;
        blackCastleKingSide  = prevBlackCastleKingSide;
        blackCastleQueenSide = prevBlackCastleQueenSide;
        enPassantFile        = prevEnPassantFile;
        halfmoveClock        = prevHalfmoveClock;

        UpdateAggregateBitboards();
        UpdateFriendlyEnemy();
    }

    // -----------------------------------------------------------------------
    // Private helpers
    // -----------------------------------------------------------------------

    private void RemovePiece(ulong square, bool isWhite)
    {
        if (isWhite)
        {
            if ((whitePawns   & square) != 0) { whitePawns   &= ~square; return; }
            if ((whiteKnights & square) != 0) { whiteKnights &= ~square; return; }
            if ((whiteBishops & square) != 0) { whiteBishops &= ~square; return; }
            if ((whiteRooks   & square) != 0) { whiteRooks   &= ~square; return; }
            if ((whiteQueens  & square) != 0) { whiteQueens  &= ~square; return; }
            if ((whiteKing    & square) != 0) { whiteKing    &= ~square; return; }
        }
        else
        {
            if ((blackPawns   & square) != 0) { blackPawns   &= ~square; return; }
            if ((blackKnights & square) != 0) { blackKnights &= ~square; return; }
            if ((blackBishops & square) != 0) { blackBishops &= ~square; return; }
            if ((blackRooks   & square) != 0) { blackRooks   &= ~square; return; }
            if ((blackQueens  & square) != 0) { blackQueens  &= ~square; return; }
            if ((blackKing    & square) != 0) { blackKing    &= ~square; return; }
        }
    }

    private void PlacePiece(ulong square, Piece piece, bool isWhite)
    {
        if (isWhite)
        {
            switch (piece)
            {
                case Piece.Pawn:   whitePawns   |= square; break;
                case Piece.Knight: whiteKnights |= square; break;
                case Piece.Bishop: whiteBishops |= square; break;
                case Piece.Rook:   whiteRooks   |= square; break;
                case Piece.Queen:  whiteQueens  |= square; break;
                case Piece.King:   whiteKing    |= square; break;
            }
        }
        else
        {
            switch (piece)
            {
                case Piece.Pawn:   blackPawns   |= square; break;
                case Piece.Knight: blackKnights |= square; break;
                case Piece.Bishop: blackBishops |= square; break;
                case Piece.Rook:   blackRooks   |= square; break;
                case Piece.Queen:  blackQueens  |= square; break;
                case Piece.King:   blackKing    |= square; break;
            }
        }
    }

    public Piece GetPieceOnSquare(ulong square)
    {
        if (((whitePawns   | blackPawns)   & square) != 0) return Piece.Pawn;
        if (((whitePawns   | blackPawns)   & square) != 0) return Piece.Pawn;
        if (((whiteKnights | blackKnights) & square) != 0) return Piece.Knight;
        if (((whiteBishops | blackBishops) & square) != 0) return Piece.Bishop;
        if (((whiteRooks   | blackRooks)   & square) != 0) return Piece.Rook;
        if (((whiteQueens  | blackQueens)  & square) != 0) return Piece.Queen;
        if (((whiteKing    | blackKing)    & square) != 0) return Piece.King;
        throw new InvalidOperationException($"No piece found on square {square}");
    }

    private void UpdateAggregateBitboards()
    {
        whitePieces = whitePawns | whiteKnights | whiteBishops | whiteRooks | whiteQueens | whiteKing;
        blackPieces = blackPawns | blackKnights | blackBishops | blackRooks | blackQueens | blackKing;
        allPieces   = whitePieces | blackPieces;
    }

    private void UpdateFriendlyEnemy()
    {
        friendlyPieces = whiteToMove ? whitePieces : blackPieces;
        enemyPieces    = whiteToMove ? blackPieces : whitePieces;
    }

    private void UpdateCastlingRights(Move move, Piece movingPiece)
    {
        if (movingPiece == Piece.King)
        {
            if (whiteToMove) { whiteCastleKingSide = false; whiteCastleQueenSide = false; }
            else             { blackCastleKingSide = false; blackCastleQueenSide = false; }
        }

        const ulong whiteKSRook = 1UL << 7;
        const ulong whiteQSRook = 1UL;
        const ulong blackKSRook = 1UL << 63;
        const ulong blackQSRook = 1UL << 56;

        if (move.from == whiteKSRook || move.to == whiteKSRook) whiteCastleKingSide  = false;
        if (move.from == whiteQSRook || move.to == whiteQSRook) whiteCastleQueenSide = false;
        if (move.from == blackKSRook || move.to == blackKSRook) blackCastleKingSide  = false;
        if (move.from == blackQSRook || move.to == blackQSRook) blackCastleQueenSide = false;
    }

    private int GetNewEnPassantFile(Move move, Piece movingPiece)
    {
        if (movingPiece != Piece.Pawn) return -1;

        bool isDoublePush = whiteToMove
            ? move.to == (move.from << 16)
            : move.to == (move.from >> 16);

        return isDoublePush
            ? System.Numerics.BitOperations.TrailingZeroCount(move.from) % 8
            : -1;
    }

    private void MoveCastlingRook(ulong kingTo)
    {
        switch (kingTo)
        {
            case 1UL << 6:  whiteRooks &= ~(1UL << 7); whiteRooks |= (1UL << 5); break; // g1 → f1
            case 1UL << 2:  whiteRooks &= ~1UL;         whiteRooks |= (1UL << 3); break; // c1 → d1
            case 1UL << 62: blackRooks &= ~(1UL << 63); blackRooks |= (1UL << 61); break; // g8 → f8
            case 1UL << 58: blackRooks &= ~(1UL << 56); blackRooks |= (1UL << 59); break; // c8 → d8
        }
    }

    private void UndoCastlingRook(ulong kingTo)
    {
        switch (kingTo)
        {
            case 1UL << 6:  whiteRooks &= ~(1UL << 5); whiteRooks |= (1UL << 7); break;
            case 1UL << 2:  whiteRooks &= ~(1UL << 3); whiteRooks |= 1UL;         break;
            case 1UL << 62: blackRooks &= ~(1UL << 61); blackRooks |= (1UL << 63); break;
            case 1UL << 58: blackRooks &= ~(1UL << 59); blackRooks |= (1UL << 56); break;
        }
    }
}