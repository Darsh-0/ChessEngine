namespace chessEngine.MoveGeneration;

public class InCheck
{
    public static bool IsInCheck(Board board, ulong enemyAttacks)
    {
        ulong friendlyKing = board.whiteToMove ? board.whiteKing : board.blackKing;
        return (friendlyKing & enemyAttacks) != 0;
    }
}