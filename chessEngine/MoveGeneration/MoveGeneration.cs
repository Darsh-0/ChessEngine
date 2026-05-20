namespace chessEngine.MoveGeneration;

public struct Move
{
    public ulong from;
    public ulong to;
    public bool? isPawn;
    public bool? isEnPassant;
}

public static class MoveGeneration
{
    public static List<Move> GenerateMoves(Board board)
    {
        List<Move> legalMoves = new List<Move>();

        legalMoves.AddRange(PawnMoves.GeneratePawnMoves(board));

        return legalMoves;
    }
} 

