using System;
using System.Collections.Generic;
using ChessBot.Core.Core;
using chessEngine.MoveGeneration;
using System.Numerics;

namespace chessEngine.MoveSelection;

public static class ScoreEval
{
    private static readonly Dictionary<Piece, int> PieceValue = new Dictionary<Piece, int>
    {
        { Piece.Pawn, 1 }, { Piece.Knight, 3 }, { Piece.Bishop, 3 }, { Piece.Rook, 5 }, { Piece.Queen, 9 }
    };
    
    public static int Eval(Board board)
    {
        int evalScore = 0;
        
        evalScore += BitOperations.PopCount(board.whitePawns) * PieceValue[Piece.Pawn];
        evalScore += BitOperations.PopCount(board.whiteKnights) * PieceValue[Piece.Knight];
        evalScore += BitOperations.PopCount(board.whiteBishops) * PieceValue[Piece.Bishop];
        evalScore += BitOperations.PopCount(board.whiteRooks) * PieceValue[Piece.Rook];
        evalScore += BitOperations.PopCount(board.whiteQueens) * PieceValue[Piece.Queen];
        
        evalScore -= BitOperations.PopCount(board.blackPawns) * PieceValue[Piece.Pawn];
        evalScore -= BitOperations.PopCount(board.blackKnights) * PieceValue[Piece.Knight];
        evalScore -= BitOperations.PopCount(board.blackBishops) * PieceValue[Piece.Bishop];
        evalScore -= BitOperations.PopCount(board.blackRooks) * PieceValue[Piece.Rook];
        evalScore -= BitOperations.PopCount(board.blackQueens) * PieceValue[Piece.Queen];
        
        return evalScore;
    }
}