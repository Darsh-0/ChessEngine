using chessEngine.MoveGeneration;

public static class BoardHelper
{
    public static Board ApplyMove(Board board, Move move)
    {
        Board newBoard = board.Clone();

        // remove piece from source square
        newBoard.whitePawns   &= ~move.from;
        newBoard.blackPawns   &= ~move.from;
        newBoard.whiteKnights &= ~move.from;
        newBoard.blackKnights &= ~move.from;
        newBoard.whiteBishops &= ~move.from;
        newBoard.blackBishops &= ~move.from;
        newBoard.whiteRooks   &= ~move.from;
        newBoard.blackRooks   &= ~move.from;
        newBoard.whiteQueens  &= ~move.from;
        newBoard.blackQueens  &= ~move.from;
        newBoard.whiteKing    &= ~move.from;
        newBoard.blackKing    &= ~move.from;

        // remove any captured piece from destination
        newBoard.whitePawns   &= ~move.to;
        newBoard.blackPawns   &= ~move.to;
        newBoard.whiteKnights &= ~move.to;
        newBoard.blackKnights &= ~move.to;
        newBoard.whiteBishops &= ~move.to;
        newBoard.blackBishops &= ~move.to;
        newBoard.whiteRooks   &= ~move.to;
        newBoard.blackRooks   &= ~move.to;
        newBoard.whiteQueens  &= ~move.to;
        newBoard.blackQueens  &= ~move.to;
        newBoard.whiteKing    &= ~move.to;
        newBoard.blackKing    &= ~move.to;

        // place piece on destination
        if      ((board.whitePawns   & move.from) != 0) newBoard.whitePawns   |= move.to;
        else if ((board.blackPawns   & move.from) != 0) newBoard.blackPawns   |= move.to;
        else if ((board.whiteKnights & move.from) != 0) newBoard.whiteKnights |= move.to;
        else if ((board.blackKnights & move.from) != 0) newBoard.blackKnights |= move.to;
        else if ((board.whiteBishops & move.from) != 0) newBoard.whiteBishops |= move.to;
        else if ((board.blackBishops & move.from) != 0) newBoard.blackBishops |= move.to;
        else if ((board.whiteRooks   & move.from) != 0) newBoard.whiteRooks   |= move.to;
        else if ((board.blackRooks   & move.from) != 0) newBoard.blackRooks   |= move.to;
        else if ((board.whiteQueens  & move.from) != 0) newBoard.whiteQueens  |= move.to;
        else if ((board.blackQueens  & move.from) != 0) newBoard.blackQueens  |= move.to;
        else if ((board.whiteKing    & move.from) != 0) newBoard.whiteKing    |= move.to;
        else if ((board.blackKing    & move.from) != 0) newBoard.blackKing    |= move.to;

        // en passant: remove the captured pawn
        if (move.isEnPassant ?? false)
        {
            ulong capturedPawn = board.whiteToMove ? move.to >> 8 : move.to << 8;
            newBoard.whitePawns &= ~capturedPawn;
            newBoard.blackPawns &= ~capturedPawn;
        }

        // recalculate aggregate boards
        newBoard.whitePieces = newBoard.whitePawns | newBoard.whiteKnights | newBoard.whiteBishops
                             | newBoard.whiteRooks | newBoard.whiteQueens  | newBoard.whiteKing;
        newBoard.blackPieces = newBoard.blackPawns | newBoard.blackKnights | newBoard.blackBishops
                             | newBoard.blackRooks | newBoard.blackQueens  | newBoard.blackKing;
        newBoard.allPieces   = newBoard.whitePieces | newBoard.blackPieces;

        newBoard.whiteToMove    = !board.whiteToMove;
        newBoard.friendlyPieces = newBoard.whiteToMove ? newBoard.whitePieces : newBoard.blackPieces;
        newBoard.enemyPieces    = newBoard.whiteToMove ? newBoard.blackPieces : newBoard.whitePieces;

        return newBoard;
    }
}