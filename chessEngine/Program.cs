using System;
using System.Collections.Generic;
using chessEngine.MoveGeneration.MagicBitBoards;
using ChessEngine;
using chessEngine.MoveGeneration;

namespace chessEngine;

internal static class Program
{
    public static void Main()
    {
        #if !BROWSER
        MagicBitboards.Initialize();

        string fen = "2r1q2k/3P4/8/8/8/8/8/7K w - - 0 1";
        //string fen = "7k/P7/7K/8/8/8/8/8 w - - 0 1";
        //string fen = "7k/8/8/8/8/8/PP6/Kr6 w - - 0 1";
        //string fen = "k7/8/8/8/8/7p/5p2/7K w - - 0 1";
        // string fen = "k7/8/8/8/8/p3p2p/8/7K w - - 0 1";
        
        Board board = FenBitboardParser.FenToBitboard(fen);
        
        List<Move> legalMoves = MoveGeneration.MoveGeneration.GenerateMoves(board);
        if (legalMoves.Count == 0) return;

        Move bestMove = MoveSelection.MoveSelection.SelctMove(board, legalMoves);
        Console.WriteLine(BitboardUtils.MoveToUci(bestMove));
        #endif
    }
}