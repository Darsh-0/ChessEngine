using System.Diagnostics;
using chessEngine.MoveGeneration;
using chessEngine.MoveGeneration.MagicBitBoards;

namespace chessEngine;

internal static class Program
{
    public static void Main(String[] args)
    {  
        Stopwatch stopwatch = Stopwatch.StartNew();
        stopwatch.Start();
        
        //string startingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        // string startingFEN = "rnbqkbnr/pp1p1ppp/8/2p1p3/3P4/7P/PPP1PPP1/RNBQKBNR b KQkq c6 0 3";
        //string startingFEN = "rnbqkbnr/pppp2pp/5p2/3Pp3/8/8/PPP1PPPP/RNBQKBNR w KQkq e6 0 3";
        string startingFEN = "7k/8/3Q4/4p3/7K/8/8/8 w - - 0 1";
        
        MagicBitboards.Initialize();
        
        FenBitboardParser parser = new FenBitboardParser();
        Board board = parser.FenToBitboard(startingFEN);

        List<Move> legalMoves = MoveGeneration.MoveGeneration.GenerateMoves(board);
        foreach (Move legalMove in legalMoves)
        {
            Console.WriteLine(BitboardUtils.BitToAlgebraic[legalMove.from] + " - " + BitboardUtils.BitToAlgebraic[legalMove.to]);
        }
        Console.WriteLine(legalMoves.Count);
        stopwatch.Stop();
        Console.WriteLine(stopwatch.Elapsed);
    }
}
