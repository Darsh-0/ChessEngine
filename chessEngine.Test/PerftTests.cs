using Xunit;
using chessEngine.MoveGeneration;
using ChessEngine;

namespace chessEngine.Tests;

public class PerftTests
{
    // -----------------------------------------------------------------------
    // State save / restore
    // -----------------------------------------------------------------------

    private record BoardState(
        bool WhiteCastleKingSide,
        bool WhiteCastleQueenSide,
        bool BlackCastleKingSide,
        bool BlackCastleQueenSide,
        int  EnPassantFile,
        int  HalfmoveClock);

    private static BoardState SaveState(Board board) => new(
        board.whiteCastleKingSide,
        board.whiteCastleQueenSide,
        board.blackCastleKingSide,
        board.blackCastleQueenSide,
        board.enPassantFile,
        board.halfmoveClock);

    private static void RestoreState(Board board, Move move, BoardState s) =>
        board.UndoMove(move,
            s.WhiteCastleKingSide,
            s.WhiteCastleQueenSide,
            s.BlackCastleKingSide,
            s.BlackCastleQueenSide,
            s.EnPassantFile,
            s.HalfmoveClock);

    // -----------------------------------------------------------------------
    // Perft core
    // -----------------------------------------------------------------------

    private static long Perft(Board board, int depth)
    {
        if (depth == 0) return 1;

        var moves = MoveGeneration.MoveGeneration.GenerateMoves(board);
        if (depth == 1) return moves.Count;

        long nodes = 0;
        foreach (var move in moves)
        {
            var state = SaveState(board);
            board.MakeMove(move);
            nodes += Perft(board, depth - 1);
            RestoreState(board, move, state);
        }
        return nodes;
    }

    // -----------------------------------------------------------------------
    // Perft divide – useful for isolating bugs move-by-move
    // -----------------------------------------------------------------------

    public static Dictionary<string, long> PerftDivide(string fen, int depth)
    {
        var board  = FenBitboardParser.FenToBitboard(fen);
        var result = new Dictionary<string, long>();
        var moves  = MoveGeneration.MoveGeneration.GenerateMoves(board);

        foreach (var move in moves)
        {
            var state = SaveState(board);
            board.MakeMove(move);
            long nodes = Perft(board, depth - 1);
            RestoreState(board, move, state);
            result[MoveLabel(move)] = nodes;
        }

        return result;
    }

    private static string MoveLabel(Move move)
    {
        string promo = move.promotionPiece.HasValue
            ? move.promotionPiece.Value.ToString().ToLower()[0].ToString()
            : "";
        return $"{SquareLabel(move.from)}{SquareLabel(move.to)}{promo}";
    }

    private static string SquareLabel(ulong square)
    {
        int idx = System.Numerics.BitOperations.TrailingZeroCount(square);
        return $"{(char)('a' + idx % 8)}{(char)('1' + idx / 8)}";
    }

    // -----------------------------------------------------------------------
    // Known positions + expected node counts
    // -----------------------------------------------------------------------

    private const string StartPos = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    private const string Kiwipete = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1";
    private const string Pos3     = "8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1";
    private const string Pos4     = "r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1";
    private const string Pos5     = "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8";
    private const string Pos6     = "r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10";

    [Theory]
    [InlineData(1,        20)]
    [InlineData(2,       400)]
    [InlineData(3,      8902)]
    [InlineData(4,    197281)]
    [InlineData(5,   4865609)]
    [InlineData(6, 119060324)]
    public void StartingPosition_Perft(int depth, long expected)
        => Assert.Equal(expected, Perft(FenBitboardParser.FenToBitboard(StartPos), depth));

    [Theory]
    [InlineData(1,        48)]
    [InlineData(2,      2039)]
    [InlineData(3,     97862)]
    [InlineData(4,   4085603)]
    [InlineData(5, 193690690)]
    public void Kiwipete_Perft(int depth, long expected)
        => Assert.Equal(expected, Perft(FenBitboardParser.FenToBitboard(Kiwipete), depth));

    [Theory]
    [InlineData(1,       14)]
    [InlineData(2,      191)]
    [InlineData(3,     2812)]
    [InlineData(4,    43238)]
    [InlineData(5,   674624)]
    [InlineData(6, 11030083)]
    public void Position3_Perft(int depth, long expected)
        => Assert.Equal(expected, Perft(FenBitboardParser.FenToBitboard(Pos3), depth));

    [Theory]
    [InlineData(1,        6)]
    [InlineData(2,      264)]
    [InlineData(3,     9467)]
    [InlineData(4,   422333)]
    [InlineData(5, 15833292)]
    public void Position4_Perft(int depth, long expected)
        => Assert.Equal(expected, Perft(FenBitboardParser.FenToBitboard(Pos4), depth));

    [Theory]
    [InlineData(1,       44)]
    [InlineData(2,     1486)]
    [InlineData(3,    62379)]
    [InlineData(4,  2103487)]
    [InlineData(5, 89941194)]
    public void Position5_Perft(int depth, long expected)
        => Assert.Equal(expected, Perft(FenBitboardParser.FenToBitboard(Pos5), depth));

    [Theory]
    [InlineData(1,        46)]
    [InlineData(2,      2079)]
    [InlineData(3,     89890)]
    [InlineData(4,   3894594)]
    [InlineData(5, 164075551)]
    public void Position6_Perft(int depth, long expected)
        => Assert.Equal(expected, Perft(FenBitboardParser.FenToBitboard(Pos6), depth));
}