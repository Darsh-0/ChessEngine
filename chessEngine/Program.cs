using System;
using System.Collections.Generic;
using chessEngine.MoveGeneration.MagicBitBoards;
using chessEngine;
using chessEngine.MoveGeneration;

namespace chessEngine;

internal static class Program
{
    public static void Main()
    {
        #if !BROWSER
        MagicBitboards.Initialize();

        //string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        string fen = "7k/P7/7K/8/8/8/8/8 w - - 0 1";
        //string fen = "7k/8/8/8/8/8/PP6/Kr6 w - - 0 1";
        //string fen = "7k/8/8/8/2K5/8/8/8 w - - 0 1";

        FenBitboardParser parser = new FenBitboardParser();
        Board board = parser.FenToBitboard(fen);
        
        List<Move> legalMoves = MoveGeneration.MoveGeneration.GenerateMoves(board);
        Console.WriteLine(legalMoves.Count);
        Random rand = new Random();
        Move randomMove = legalMoves[rand.Next(0, legalMoves.Count)];
        string randomMoveUci = BitboardUtils.MoveToUci(randomMove);
        Console.WriteLine(randomMoveUci);
        // foreach (Move move in legalMoves)
        // {
        //     Console.WriteLine(BitboardUtils.BitToAlgebraic[move.from] + BitboardUtils.BitToAlgebraic[move.to]);
        // }
        // Console.WriteLine(randomMoveUCI);
        #endif
    }
}