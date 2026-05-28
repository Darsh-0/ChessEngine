using chessEngine;
using chessEngine.MoveGeneration;
using Xunit;

namespace chessEngineTests;

public class ApplyMoveTests
{
    [Fact]
    public void ApplyMove_WhitePawnMove_UpdatesBoardCorrectly()
    {
        // a2 -> a3
        ulong from = 1UL << 8;
        ulong to = 1UL << 16;

        Board board = new()
        {
            whitePawns = from,
            whiteToMove = true
        };

        board.whitePieces = board.whitePawns;
        board.allPieces = board.whitePieces;

        Move move = new()
        {
            from = from,
            to = to
        };

        // Act
        Board result = BoardHelper.ApplyMove(board, move);

        // Assert
        Assert.Equal(0UL, result.whitePawns & from);
        Assert.NotEqual(0UL, result.whitePawns & to);

        Assert.False(result.whiteToMove);
    }

    [Fact]
    public void ApplyMove_Capture_RemovesCapturedPiece()
    {
        // White rook captures black knight
        ulong rookSquare = 1UL << 0;     // a1
        ulong knightSquare = 1UL << 56; // a8

        Board board = new()
        {
            whiteRooks = rookSquare,
            blackKnights = knightSquare,
            whiteToMove = true
        };

        board.whitePieces = board.whiteRooks;
        board.blackPieces = board.blackKnights;
        board.allPieces = board.whitePieces | board.blackPieces;

        Move move = new()
        {
            from = rookSquare,
            to = knightSquare
        };

        // Act
        Board result = BoardHelper.ApplyMove(board, move);

        // Assert
        Assert.Equal(0UL, result.blackKnights);
        Assert.NotEqual(0UL, result.whiteRooks & knightSquare);
    }

    [Fact]
    public void ApplyMove_EnPassant_RemovesCapturedPawn()
    {
        /*
            White pawn on e5 captures black pawn on d5 en passant

            White pawn: e5 = bit 36
            Black pawn: d5 = bit 35
            Move target: d6 = bit 43
        */

        ulong whitePawn = 1UL << 36;
        ulong blackPawn = 1UL << 35;
        ulong destination = 1UL << 43;

        Board board = new()
        {
            whitePawns = whitePawn,
            blackPawns = blackPawn,
            whiteToMove = true
        };

        board.whitePieces = board.whitePawns;
        board.blackPieces = board.blackPawns;
        board.allPieces = board.whitePieces | board.blackPieces;

        Move move = new()
        {
            from = whitePawn,
            to = destination,
            isEnPassant = true
        };

        // Act
        Board result = BoardHelper.ApplyMove(board, move);

        // Assert
        Assert.Equal(0UL, result.blackPawns);
        Assert.NotEqual(0UL, result.whitePawns & destination);
    }

    [Fact]
    public void ApplyMove_RecalculatesAggregateBitboards()
    {
        ulong queenFrom = 1UL << 3;  // d1
        ulong queenTo = 1UL << 27;   // d4

        Board board = new()
        {
            whiteQueens = queenFrom,
            whiteToMove = true
        };

        board.whitePieces = board.whiteQueens;
        board.allPieces = board.whitePieces;

        Move move = new()
        {
            from = queenFrom,
            to = queenTo
        };

        // Act
        Board result = BoardHelper.ApplyMove(board, move);

        // Assert
        ulong expectedWhitePieces =
            result.whitePawns |
            result.whiteKnights |
            result.whiteBishops |
            result.whiteRooks |
            result.whiteQueens |
            result.whiteKing;

        ulong expectedBlackPieces =
            result.blackPawns |
            result.blackKnights |
            result.blackBishops |
            result.blackRooks |
            result.blackQueens |
            result.blackKing;

        Assert.Equal(expectedWhitePieces, result.whitePieces);
        Assert.Equal(expectedBlackPieces, result.blackPieces);
        Assert.Equal(expectedWhitePieces | expectedBlackPieces, result.allPieces);
    }

    [Fact]
    public void ApplyMove_UpdatesFriendlyAndEnemyPieces()
    {
        ulong knightFrom = 1UL << 1;  // b1
        ulong knightTo = 1UL << 18;   // c3

        Board board = new()
        {
            whiteKnights = knightFrom,
            whiteToMove = true
        };

        board.whitePieces = board.whiteKnights;
        board.allPieces = board.whitePieces;

        Move move = new()
        {
            from = knightFrom,
            to = knightTo
        };

        // Act
        Board result = BoardHelper.ApplyMove(board, move);

        // Assert
        Assert.False(result.whiteToMove);

        // Black to move now, so friendly = black pieces
        Assert.Equal(result.blackPieces, result.friendlyPieces);
        Assert.Equal(result.whitePieces, result.enemyPieces);
    }

    [Fact]
    public void ApplyMove_DoesNotModifyOriginalBoard()
    {
        ulong knightFrom = 1UL << 1; // b1
        ulong knightTo = 1UL << 18;  // c3

        Board board = new()
        {
            whiteKnights = knightFrom,
            whiteToMove = true
        };

        Move move = new()
        {
            from = knightFrom,
            to = knightTo
        };

        // Act
        Board result = BoardHelper.ApplyMove(board, move);

        // Assert
        Assert.NotEqual(board.whiteKnights, result.whiteKnights);

        // Original board unchanged
        Assert.NotEqual(0UL, board.whiteKnights & knightFrom);
        Assert.Equal(0UL, board.whiteKnights & knightTo);
    }
}