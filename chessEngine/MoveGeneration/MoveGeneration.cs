using System;

namespace chessEngine.MoveGeneration;
using System.Collections.Generic;

public struct Move
{
    public ulong from;
    public ulong to;
    public char? promotionPiece;
    public bool? isEnPassant;
    public bool? castle;
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
        List<Move> pseudoLegal = new List<Move>();

        pseudoLegal.AddRange(PawnMoves.GeneratePawnMoves(board));
        pseudoLegal.AddRange(KnightMoves.GenerateKnightMoves(board));
        pseudoLegal.AddRange(BishopMoves.GenerateBishopMoves(board));
        pseudoLegal.AddRange(RookMoves.GenerateRookMoves(board));
        pseudoLegal.AddRange(QueenMoves.GenerateQueenMoves(board));
        pseudoLegal.AddRange(KingMoves.GenerateKingMoves(board));

        List<Move> legalMoves = new List<Move>();

        foreach (Move move in pseudoLegal)
        {
            Board newBoard = BoardHelper.ApplyMove(board, move);
            if (!CheckDetection.IsInCheck(newBoard, board.whiteToMove))
            {
                Console.WriteLine(BitboardUtils.MoveToUci(move));
                legalMoves.Add(move);
            }
        }

        return legalMoves;
    }
} 

