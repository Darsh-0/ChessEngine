using static chessEngine.MoveGeneration.MoveGeneration;

namespace chessEngine.MoveGeneration;

public static class AttackMaps
{
    public static ulong GetAllAttackedSquares(Board board)
    {
        return PawnMoves.GeneratePawnAttacks(board)
               | KnightMoves.GenerateKnightAttacks(board)
               | BishopMoves.GenerateBishopAttacks(board)
               | RookMoves.GenerateRookAttacks(board)
               | QueenMoves.GenerateQueenAttacks(board)
               | KingMoves.GenerateKingAttacks(board);
    }
}