using System.Diagnostics;
using chessEngine.MoveGeneration;

namespace chessEngine;

internal static class Program
{
    public static void Main(String[] args)
    {  
        Stopwatch stopwatch = Stopwatch.StartNew();
        stopwatch.Start();
        
        string startingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        // string startingFEN = "rnbqkbnr/pp1p1ppp/8/2p1p3/3P4/7P/PPP1PPP1/RNBQKBNR b KQkq c6 0 3";
        //string startingFEN = "rnbqkbnr/pppp2pp/5p2/3Pp3/8/8/PPP1PPPP/RNBQKBNR w KQkq e6 0 3";
        
        FenBitboardParser parser = new FenBitboardParser();
        Board board = parser.FenToBitboard(startingFEN);
        Console.WriteLine(Convert.ToString((long)board.whitePawns, 2));
        

        List<Move> legalMoves = MoveGeneration.MoveGeneration.GenerateMoves(board);
        foreach (Move legalMove in legalMoves)
        {
            Console.WriteLine(BitboardUtils.BitToAlgebraic[legalMove.from] + " - " + BitboardUtils.BitToAlgebraic[legalMove.to]);
        }
        stopwatch.Stop();
        Console.WriteLine(stopwatch.Elapsed);
    }
}
