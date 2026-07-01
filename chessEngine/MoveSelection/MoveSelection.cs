using System;
using System.Collections.Generic;
using ChessEngine;
using chessEngine.MoveGeneration;

namespace chessEngine.MoveSelection;

public static class MoveSelection
{
    public static Move SelctMove(Board board, List<Move> moves)
    {
        Move bestMove = moves[0];
        int bestScore = int.MinValue;
        int perspective = board.whiteToMove ? 1 : -1;
        
        foreach (Move move in moves)
        {
            bool prevWhiteCastleKingSide = board.whiteCastleKingSide;
            bool prevWhiteCastleQueenSide = board.whiteCastleQueenSide;
            bool prevBlackCastleKingSide = board.blackCastleKingSide;
            bool prevBlackCastleQueenSide = board.blackCastleQueenSide;
            int prevEnPassantFile = board.enPassantFile;
            int prevHalfmoveClock = board.halfmoveClock;

            board.MakeMove(move);

            int score = perspective * ScoreEval.Eval(board);
            
            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
            
            board.UndoMove(move,
                prevWhiteCastleKingSide,
                prevWhiteCastleQueenSide,
                prevBlackCastleKingSide,
                prevBlackCastleQueenSide,
                prevEnPassantFile,
                prevHalfmoveClock);
        }
        
        return bestMove;
    }
    
}