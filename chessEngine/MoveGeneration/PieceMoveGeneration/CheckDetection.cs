using System;
using chessEngine.MoveGeneration;

public static class CheckDetection
{
    public static bool IsInCheck(Board board, bool isWhite)
    {
        ulong king = isWhite ? board.whiteKing : board.blackKing;

        Board enemyBoard = board.Clone();
        enemyBoard.whiteToMove = !isWhite;

        ulong pawnAttacks   = PawnMoves.GeneratePawnAttacks(enemyBoard);
        ulong knightAttacks = KnightMoves.GenerateKnightAttacks(enemyBoard);
        ulong bishopAttacks = BishopMoves.GenerateBishopAttacks(enemyBoard);
        ulong rookAttacks   = RookMoves.GenerateRookAttacks(enemyBoard);
        ulong queenAttacks  = QueenMoves.GenerateQueenAttacks(enemyBoard);
        ulong kingAttacks   = KingMoves.GenerateKingAttacks(enemyBoard);

        ulong enemyAttacks = pawnAttacks | knightAttacks | bishopAttacks 
                             | rookAttacks | queenAttacks  | kingAttacks;

        return (king & enemyAttacks) != 0;
    }
}