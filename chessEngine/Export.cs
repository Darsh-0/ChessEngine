using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using chessEngine.MoveGeneration;
using chessEngine.MoveGeneration.MagicBitBoards;

namespace chessEngine;

[SupportedOSPlatform("browser")]
public partial class ChessEngine
{
    private static bool _initialized = false;

    [JSExport]
    public static string GetRandomMove(string fen)
    {
        if (!_initialized)
        {
            MagicBitboards.Initialize();
            _initialized = true;
        }

        FenBitboardParser parser = new FenBitboardParser();
        Board board = parser.FenToBitboard(fen);

        List<Move> legalMoves = MoveGeneration.MoveGeneration.GenerateMoves(board);

        if (legalMoves.Count == 0)
        {
            return "";
        }

        Random rand = new Random();
        Move randomMove = legalMoves[rand.Next(0, legalMoves.Count)];

        return BitboardUtils.BitToAlgebraic[randomMove.from]
               + BitboardUtils.BitToAlgebraic[randomMove.to];
    }
}