using System;
using ChessEngine;


namespace chessEngine.MoveGeneration;
using System.Collections.Generic;
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
        ulong enemyAttacks = EnemyAttacks.GenerateEnemyAttacks(board);
            
        ulong checkMask = CheckMask.GetCheckMask(board, enemyAttacks);
        ulong[] pinMasks = PinMasks.GetPinMasks(board);
        
        Console.WriteLine(BitboardUtils.UlongToBoard(enemyAttacks));
        
        List<Move> legalMoves = new List<Move>();

        legalMoves.AddRange(PawnMoves.GeneratePawnMoves(board, checkMask, pinMasks));
        legalMoves.AddRange(KnightMoves.GenerateKnightMoves(board, checkMask, pinMasks));
        legalMoves.AddRange(BishopMoves.GenerateBishopMoves(board, checkMask, pinMasks));
        legalMoves.AddRange(RookMoves.GenerateRookMoves(board, checkMask, pinMasks));
        legalMoves.AddRange(QueenMoves.GenerateQueenMoves(board, checkMask, pinMasks));
        legalMoves.AddRange(KingMoves.GenerateKingMoves(board, enemyAttacks));

        return legalMoves;
    }
} 

