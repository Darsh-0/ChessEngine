using System.Collections.Generic;
using ChessBot.Core.Core;
using ChessEngine;
using chessEngine.MoveGeneration;

public static class BitboardUtils
{
    public static readonly Dictionary<ulong, string> BitToAlgebraic = new Dictionary<ulong, string>
    {
        { 1UL << 0, "a1" }, { 1UL << 1, "b1" }, { 1UL << 2, "c1" }, { 1UL << 3, "d1" },
        { 1UL << 4, "e1" }, { 1UL << 5, "f1" }, { 1UL << 6, "g1" }, { 1UL << 7, "h1" },

        { 1UL << 8, "a2" }, { 1UL << 9, "b2" }, { 1UL << 10, "c2" }, { 1UL << 11, "d2" },
        { 1UL << 12, "e2" }, { 1UL << 13, "f2" }, { 1UL << 14, "g2" }, { 1UL << 15, "h2" },

        { 1UL << 16, "a3" }, { 1UL << 17, "b3" }, { 1UL << 18, "c3" }, { 1UL << 19, "d3" },
        { 1UL << 20, "e3" }, { 1UL << 21, "f3" }, { 1UL << 22, "g3" }, { 1UL << 23, "h3" },

        { 1UL << 24, "a4" }, { 1UL << 25, "b4" }, { 1UL << 26, "c4" }, { 1UL << 27, "d4" },
        { 1UL << 28, "e4" }, { 1UL << 29, "f4" }, { 1UL << 30, "g4" }, { 1UL << 31, "h4" },

        { 1UL << 32, "a5" }, { 1UL << 33, "b5" }, { 1UL << 34, "c5" }, { 1UL << 35, "d5" },
        { 1UL << 36, "e5" }, { 1UL << 37, "f5" }, { 1UL << 38, "g5" }, { 1UL << 39, "h5" },

        { 1UL << 40, "a6" }, { 1UL << 41, "b6" }, { 1UL << 42, "c6" }, { 1UL << 43, "d6" },
        { 1UL << 44, "e6" }, { 1UL << 45, "f6" }, { 1UL << 46, "g6" }, { 1UL << 47, "h6" },

        { 1UL << 48, "a7" }, { 1UL << 49, "b7" }, { 1UL << 50, "c7" }, { 1UL << 51, "d7" },
        { 1UL << 52, "e7" }, { 1UL << 53, "f7" }, { 1UL << 54, "g7" }, { 1UL << 55, "h7" },

        { 1UL << 56, "a8" }, { 1UL << 57, "b8" }, { 1UL << 58, "c8" }, { 1UL << 59, "d8" },
        { 1UL << 60, "e8" }, { 1UL << 61, "f8" }, { 1UL << 62, "g8" }, { 1UL << 63, "h8" }
    };

    public static string MoveToUci(Move move)
    {
        string promotionString = "";
        if (move.promotionPiece == Piece.Queen) promotionString = "q";
        else if (move.promotionPiece == Piece.Rook) promotionString = "r";
        else if (move.promotionPiece == Piece.Knight) promotionString = "n";
        else if (move.promotionPiece == Piece.Bishop) promotionString = "b";
        
        return BitToAlgebraic[move.from] + BitToAlgebraic[move.to] + promotionString;
    }
    
    public static string UlongToBoard(ulong bitboard)
    {
        var sb = new System.Text.StringBuilder();
        for (int rank = 7; rank >= 0; rank--)
        {
            for (int file = 0; file < 8; file++)
            {
                ulong square = 1UL << (rank * 8 + file);
                sb.Append((bitboard & square) != 0 ? '1' : '0');
                if (file < 7) sb.Append(' ');
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}