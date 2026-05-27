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
    public const ulong FILE_A = 0x0101010101010101;
    public const ulong FILE_B = 0x0202020202020202;
    public const ulong FILE_G = 0x4040404040404040; 
    public const ulong FILE_H = 0x8080808080808080;
    public const ulong RANK_2 = 0x000000000000FF00;
    public const ulong RANK_7 = 0x00FF000000000000;
    public static List<Move> GenerateMoves(Board board)
    {
        List<Move> legalMoves = new List<Move>();

        legalMoves.AddRange(PawnMoves.GeneratePawnMoves(board));
        legalMoves.AddRange(KnightMoves.GenerateKnightMoves(board));
        legalMoves.AddRange(RookMoves.GenerateRookMoves(board));
        legalMoves.AddRange(BishopMoves.GenerateBishopMoves(board));
        legalMoves.AddRange(QueenMoves.GenerateQueenMoves(board));
        
        return legalMoves;
    }
} 

