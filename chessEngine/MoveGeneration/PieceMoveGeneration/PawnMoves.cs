using static chessEngine.MoveGeneration.MoveGeneration;

namespace chessEngine.MoveGeneration;

public static class PawnMoves
{


    public static List<Move> GeneratePawnMoves(Board board)
    {
        List<Move> legalMoves = new List<Move>();

        bool isWhite = board.whiteToMove;
        ulong pawns = isWhite ? board.whitePawns : board.blackPawns;
        ulong enemy = board.enemyPieces;
        ulong all = board.allPieces;

        while (pawns != 0)
        {
            ulong currentSquare = pawns & (~pawns + 1);

            ulong inFrontSquare = isWhite ? currentSquare << 8 : currentSquare >> 8;

            // move once
            if ((all & inFrontSquare) == 0)
            {
                legalMoves.Add(new Move
                {
                    from = currentSquare,
                    to = inFrontSquare
                });

                // move double on first move (unchanged logic style)
                ulong doubleInFrontSquare = isWhite ? currentSquare << 16 : currentSquare >> 16;

                bool onStartRank = isWhite ? (currentSquare & RANK_2) != 0 : (currentSquare & RANK_7) != 0;

                if (onStartRank && (all & doubleInFrontSquare) == 0)
                {
                    legalMoves.Add(new Move
                    {
                        from = currentSquare,
                        to = doubleInFrontSquare
                    });
                }
            }

            ulong captureLeft = isWhite ? currentSquare << 7 : currentSquare >> 9;
            ulong captureRight = isWhite ? currentSquare << 9 : currentSquare >> 7;

            if ((currentSquare & FILE_A) == 0 && (captureLeft & enemy) != 0)
            {
                legalMoves.Add(new Move
                {
                    from = currentSquare,
                    to = captureLeft
                });
            }

            if ((currentSquare & FILE_H) == 0 && (captureRight & enemy) != 0)
            {
                legalMoves.Add(new Move
                {
                    from = currentSquare,
                    to = captureRight
                });
            }

            if (board.enPassantFile != -1)
            {
                int epRank = isWhite ? 5 : 2;
                ulong ep = 1UL << (epRank * 8 + board.enPassantFile);

                if ((currentSquare & FILE_A) == 0 && captureLeft == ep)
                    legalMoves.Add(new Move { from = currentSquare, to = captureLeft, isEnPassant = true });

                if ((currentSquare & FILE_H) == 0 && captureRight == ep)
                    legalMoves.Add(new Move { from = currentSquare, to = captureRight, isEnPassant = true });
            }

            pawns &= pawns - 1;
        }

        return legalMoves;
    }
}