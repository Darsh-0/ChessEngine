using static chessEngine.MoveGeneration.MoveGeneration;

namespace chessEngine.MoveGeneration;

public static class KingMoves
{
    public static List<Move> GenerateKingMoves(Board board)
    {
        List<Move> legalMoves = new List<Move>();
        
        bool isWhite = board.whiteToMove;

        ulong king = isWhite ? board.whiteKing : board.blackKing;
        ulong friendly = board.friendlyPieces;

        ulong currentSquare = king;

        ulong attackingSquares = 0;
        
        if ((currentSquare & FILE_A) == 0)
        {
            attackingSquares |= currentSquare << 9 | currentSquare << 1 | currentSquare >> 7;
        }

        if ((currentSquare & FILE_H) == 0)
        {
            attackingSquares |= currentSquare << 7 | currentSquare >> 1 | currentSquare >> 9;
        }
        
        attackingSquares |= currentSquare >> 8 | currentSquare << 8;

        Board enemyBoard = board;
        enemyBoard.whiteToMove = !isWhite;
        
        ulong EnemyAttackingSquares = PawnMoves.GeneratePawnAttacks(enemyBoard)
                                      | KnightMoves.GenerateKnightAttacks(enemyBoard)
                                      | BishopMoves.GenerateBishopAttacks(enemyBoard)
                                      | RookMoves.GenerateRookAttacks(enemyBoard)
                                      | QueenMoves.GenerateQueenAttacks(enemyBoard)
                                      | KingMoves.GenerateKingAttacks(enemyBoard);
 
        attackingSquares &= ~(friendly | EnemyAttackingSquares);
        
        while (attackingSquares != 0)
        {
            ulong to = attackingSquares & (~attackingSquares + 1);
            legalMoves.Add(new Move
            {
                from = currentSquare,
                to = to
            });
            attackingSquares &= attackingSquares - 1;
        }
        return legalMoves;
    }
    
    public static ulong GenerateKingAttacks(Board board)
    {
        bool isWhite = board.whiteToMove;
        ulong king = isWhite ? board.whiteKing : board.blackKing;
        ulong attacks = 0;

        if ((king & FILE_H) == 0)
        {
            attacks |= king << 9 | king >> 7 | king << 1;
        }
        if ((king & FILE_A) == 0)
        {
            attacks |= king << 7 | king >> 9 | king >> 1;
        }
        attacks |= king << 8 | king >> 8;

        return attacks;
    }
}