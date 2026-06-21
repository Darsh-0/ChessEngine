namespace chessEngine.MoveGeneration;

public static class EnemyAttacks
{
    public static ulong GenerateEnemyAttacks(Board board)
    {
        return PawnMoves.GenerateEnemyPawnAttacks(board)
               | KnightMoves.GenerateEnemyKnightAttacks(board)
               | BishopMoves.GenerateEnemyBishopAttacks(board)
               | RookMoves.GenerateEnemyRookAttacks(board)
               | QueenMoves.GenerateEnemyQueenAttacks(board)
               | KingMoves.GenerateEnemyKingAttacks(board);
    }
}