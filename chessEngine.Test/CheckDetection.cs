using chessEngine.MoveGeneration;
using chessEngine.MoveGeneration.MagicBitBoards;
using Xunit;

namespace chessEngineTests;

public class CheckDetectionTests
{
    [Fact]
    public void IsInCheck_WhiteKingAttackedByRook_ReturnsTrue()
    {
        //MagicBitboards.Initialize();
        /*
            White king on e1
            Black rook on e8

            The rook attacks the king vertically.
        */

        Board board = new()
        {
            whiteKing = 1UL << 4,     // e1
            blackRooks = 1UL << 60,   // e8
            whiteToMove = true
        };

        board.whitePieces = board.whiteKing;
        board.blackPieces = board.blackRooks;
        board.allPieces = board.whitePieces | board.blackPieces;

        // Act
        bool result = CheckDetection.IsInCheck(board, true);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInCheck_WhiteKingNotAttacked_ReturnsFalse()
    {
        /*
            White king on e1
            Black rook on a8

            No attack on the king.
        */

        Board board = new()
        {
            whiteKing = 1UL << 4,     // e1
            blackRooks = 1UL << 56,   // a8
            whiteToMove = true
        };

        board.whitePieces = board.whiteKing;
        board.blackPieces = board.blackRooks;
        board.allPieces = board.whitePieces | board.blackPieces;

        // Act
        bool result = CheckDetection.IsInCheck(board, true);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInCheck_BlackKingAttackedByBishop_ReturnsTrue()
    {
        /*
            Black king on h8
            White bishop on a1

            Bishop attacks diagonally.
        */

        Board board = new()
        {
            blackKing = 1UL << 63,      // h8
            whiteBishops = 1UL << 0,    // a1
            whiteToMove = false
        };

        board.whitePieces = board.whiteBishops;
        board.blackPieces = board.blackKing;
        board.allPieces = board.whitePieces | board.blackPieces;

        // Act
        bool result = CheckDetection.IsInCheck(board, false);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInCheck_BlackKingAttackedByKnight_ReturnsTrue()
    {
        /*
            Black king on e5
            White knight on d3

            Knight attacks e6.
        */

        Board board = new()
        {
            blackKing = 1UL << 36,      // e5
            whiteKnights = 1UL << 19,   // d3
            whiteToMove = false
        };

        board.whitePieces = board.whiteKnights;
        board.blackPieces = board.blackKing;
        board.allPieces = board.whitePieces | board.blackPieces;

        // Act
        bool result = CheckDetection.IsInCheck(board, false);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInCheck_BlockedRookAttack_ReturnsFalse()
    {
        /*
            White king on e1
            Black rook on e8
            Black pawn on e4 blocking attack
        */

        Board board = new()
        {
            whiteKing = 1UL << 4,      // e1
            blackRooks = 1UL << 60,    // e8
            blackPawns = 1UL << 28,    // e4
            whiteToMove = true
        };

        board.whitePieces = board.whiteKing;
        board.blackPieces = board.blackRooks | board.blackPawns;
        board.allPieces = board.whitePieces | board.blackPieces;

        // Act
        bool result = CheckDetection.IsInCheck(board, true);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInCheck_DoesNotRequireSideToMoveToMatchKingColor()
    {
        /*
            Tests that the method correctly flips whiteToMove internally.
        */

        Board board = new()
        {
            whiteKing = 1UL << 4,     // e1
            blackQueens = 1UL << 32,  // a5
            whiteToMove = false
        };

        board.whitePieces = board.whiteKing;
        board.blackPieces = board.blackQueens;
        board.allPieces = board.whitePieces | board.blackPieces;

        // Act
        bool result = CheckDetection.IsInCheck(board, true);

        // Assert
        Assert.True(result);
    }
}