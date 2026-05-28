using chessEngine.MoveGeneration;
using chessEngine.MoveGeneration.MagicBitBoards;
using Xunit;
using Xunit.Abstractions;

namespace chessEngine.Test;

public class PerftTests
{
    private readonly ITestOutputHelper _output;

    public PerftTests(ITestOutputHelper output)
    {
        _output = output;
        MagicBitboards.Initialize();
    }

    // ── Starting position ──────────────────────────────────────
    [Fact] public void Start_Depth1() => RunPerft("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 1, 20);
    //[Fact] public void Start_Depth2() => RunPerft("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 2, 400);
    //[Fact] public void Start_Depth3() => RunPerft("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 3, 8902);
    //[Fact] public void Start_Depth4() => RunPerft("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 4, 197281);

    // ── Kiwipete ───────────────────────────────────────────────
    [Fact] public void Kiwipete_Depth1() => RunPerft("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1", 1, 48);
    //[Fact] public void Kiwipete_Depth2() => RunPerft("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1", 2, 2039);
    //[Fact] public void Kiwipete_Depth3() => RunPerft("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1", 3, 97862);
    //[Fact] public void Kiwipete_Depth4() => RunPerft("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1", 4, 4085603);

    // ── Position 3 ─────────────────────────────────────────────
    [Fact] public void Position3_Depth1() => RunPerft("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1", 1, 14);
    //[Fact] public void Position3_Depth2() => RunPerft("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1", 2, 191);
    //[Fact] public void Position3_Depth3() => RunPerft("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1", 3, 2812);
    //[Fact] public void Position3_Depth4() => RunPerft("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1", 4, 43238);

    // ── Position 4 ─────────────────────────────────────────────
    [Fact] public void Position4_Depth1() => RunPerft("r3k2r/Pppp1ppp/1b3nbB/nP6/BBp1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1", 1, 6);
    //[Fact] public void Position4_Depth2() => RunPerft("r3k2r/Pppp1ppp/1b3nbB/nP6/BBp1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1", 2, 264);
    //[Fact] public void Position4_Depth3() => RunPerft("r3k2r/Pppp1ppp/1b3nbB/nP6/BBp1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1", 3, 9467);
    //[Fact] public void Position4_Depth4() => RunPerft("r3k2r/Pppp1ppp/1b3nbB/nP6/BBp1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1", 4, 422333);

    // ── Divide helper (call manually to debug a failing test) ──
    public void Divide(string fen, int depth)
    {
        FenBitboardParser parser = new FenBitboardParser();
        Board board = parser.FenToBitboard(fen);

        List<Move> moves = MoveGeneration.MoveGeneration.GenerateMoves(board);
        ulong total = 0;

        foreach (Move move in moves)
        {
            Board newBoard = BoardHelper.ApplyMove(board, move);
            ulong nodes = Perft(newBoard, depth - 1);
            total += nodes;

            string from = BitboardUtils.BitToAlgebraic[move.from];
            string to   = BitboardUtils.BitToAlgebraic[move.to];
            _output.WriteLine($"{from}{to}: {nodes}");
        }

        _output.WriteLine($"\nTotal: {total}");
    }

    void RunPerft(string fen, int depth, ulong expected)
    {
        FenBitboardParser parser = new FenBitboardParser();
        Board board = parser.FenToBitboard(fen);

        long start  = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        ulong result = Perft(board, depth);
        long elapsed = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - start;

        _output.WriteLine($"depth {depth}: {result} nodes in {elapsed}ms");

        Assert.Equal(expected, result);
    }

    static ulong Perft(Board board, int depth)
    {
        if (depth == 0) return 1;

        List<Move> moves = MoveGeneration.MoveGeneration.GenerateMoves(board);
        ulong nodes = 0;

        foreach (Move move in moves)
        {
            Board newBoard = BoardHelper.ApplyMove(board, move);
            nodes += Perft(newBoard, depth - 1);
        }

        return nodes;
    }
}