namespace chessEngine.MoveGeneration;

public static class CheckMask
{
    public static ulong GetCheckMask(Board board, ulong enemyAttacks)
    {
        bool isWhite = board.whiteToMove;
        ulong king = isWhite ? board.whiteKing : board.blackKing;
        int kingSquare = System.Numerics.BitOperations.TrailingZeroCount(king);

        ulong checkers = GetCheckers(board, king, isWhite);
        int checkerCount = System.Numerics.BitOperations.PopCount(checkers);

        if (checkerCount == 0)
            return ulong.MaxValue; // not in check

        if (checkerCount > 1)
            return 0UL; // double check — only king moves legal

        int checkerSquare = System.Numerics.BitOperations.TrailingZeroCount(checkers);

        ulong sliders = isWhite
            ? board.blackRooks | board.blackBishops | board.blackQueens
            : board.whiteRooks | board.whiteBishops | board.whiteQueens;

        if ((checkers & sliders) != 0)
            return GetRayBetween(kingSquare, checkerSquare) | checkers;

        return checkers;
    }

    private static ulong GetCheckers(Board board, ulong king, bool isWhite)
    {
        ulong checkers = 0UL;
        int kingSquare = System.Numerics.BitOperations.TrailingZeroCount(king);

        // Pawn checkers
        ulong enemyPawns = isWhite ? board.blackPawns : board.whitePawns;
        ulong pawnAttacks = isWhite
            ? ((king & ~MoveGeneration.FILE_A) << 7) | ((king & ~MoveGeneration.FILE_H) << 9)
            : ((king & ~MoveGeneration.FILE_H) >> 7) | ((king & ~MoveGeneration.FILE_A) >> 9);
        checkers |= pawnAttacks & enemyPawns;

        // Knight checkers
        checkers |= GetKnightAttacks(king) & (isWhite ? board.blackKnights : board.whiteKnights);

        // Bishop/diagonal queen checkers
        ulong diagSliders = isWhite
            ? board.blackBishops | board.blackQueens
            : board.whiteBishops | board.whiteQueens;
        checkers |= GetBishopRays(kingSquare, board.allPieces) & diagSliders;

        // Rook/cardinal queen checkers
        ulong cardSliders = isWhite
            ? board.blackRooks | board.blackQueens
            : board.whiteRooks | board.whiteQueens;
        checkers |= GetRookRays(kingSquare, board.allPieces) & cardSliders;

        return checkers;
    }

    // Ray between two squares (exclusive of both endpoints)
    // Returns 0 if not on same rank, file, or diagonal
    private static ulong GetRayBetween(int from, int to)
    {
        ulong ray = 0UL;
        int diff = to - from;
        int step;

        int fromFile = from % 8;
        int toFile   = to % 8;
        int fromRank = from / 8;
        int toRank   = to / 8;

        if (fromRank == toRank)           step = 1;         // same rank
        else if (fromFile == toFile)      step = 8;         // same file
        else if (fromFile - toFile == fromRank - toRank) step = 9;  // diagonal
        else if (fromFile - toFile == toRank - fromRank) step = 7;  // anti-diagonal
        else return 0UL;

        if (diff < 0) step = -step;

        int current = from + step;
        while (current != to)
        {
            ray |= 1UL << current;
            current += step;
        }
        return ray;
    }

    private static ulong GetKnightAttacks(ulong knight)
    {
        return ((knight & ~MoveGeneration.FILE_A & ~MoveGeneration.FILE_B) << 6)  |
               ((knight & ~MoveGeneration.FILE_G & ~MoveGeneration.FILE_H) << 10) |
               ((knight & ~MoveGeneration.FILE_A)                          << 15) |
               ((knight & ~MoveGeneration.FILE_H)                          << 17) |
               ((knight & ~MoveGeneration.FILE_G & ~MoveGeneration.FILE_H) >> 6)  |
               ((knight & ~MoveGeneration.FILE_A & ~MoveGeneration.FILE_B) >> 10) |
               ((knight & ~MoveGeneration.FILE_H)                          >> 15) |
               ((knight & ~MoveGeneration.FILE_A)                          >> 17);
    }

    private static ulong GetBishopRays(int square, ulong occupied)
    {
        ulong attacks = 0UL;
        ulong current;

        int[] diagonalShifts = { 9, 7, -9, -7 };
        ulong[] avoidWrap    = {
            MoveGeneration.FILE_H, // going NE, stop before wrapping right
            MoveGeneration.FILE_A, // going NW, stop before wrapping left
            MoveGeneration.FILE_A, // going SW, stop before wrapping left
            MoveGeneration.FILE_H  // going SE, stop before wrapping right
        };

        for (int i = 0; i < 4; i++)
        {
            current = 1UL << square;
            while (true)
            {
                if ((current & avoidWrap[i]) != 0) break;
                current = diagonalShifts[i] > 0
                    ? current << diagonalShifts[i]
                    : current >> (-diagonalShifts[i]);
                if (current == 0) break;
                attacks |= current;
                if ((current & occupied) != 0) break;
            }
        }
        return attacks;
    }

    public static ulong GetRookRays(int square, ulong occupied)
    {
        ulong attacks = 0UL;
        ulong current;

        int[] cardinalShifts = { 8, -8, 1, -1 };
        ulong[] avoidWrap    = {
            0UL,                   // North — no wrap
            0UL,                   // South — no wrap
            MoveGeneration.FILE_H, // East — stop before wrapping right
            MoveGeneration.FILE_A  // West — stop before wrapping left
        };

        for (int i = 0; i < 4; i++)
        {
            current = 1UL << square;
            while (true)
            {
                if (avoidWrap[i] != 0 && (current & avoidWrap[i]) != 0) break;
                current = cardinalShifts[i] > 0
                    ? current << cardinalShifts[i]
                    : current >> (-cardinalShifts[i]);
                if (current == 0) break;
                attacks |= current;
                if ((current & occupied) != 0) break;
            }
        }
        return attacks;
    }
}