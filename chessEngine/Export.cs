using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using ChessEngine;
using chessEngine.MoveGeneration;
using chessEngine.MoveGeneration.MagicBitBoards;

namespace chessEngine;

[SupportedOSPlatform("browser")]
public partial class ChessEngine
{
    private static bool _initialized = false;

    [JSExport]
    public static string GetBestMove(string fen)
    {
        if (!_initialized)
        {
            MagicBitboards.Initialize();
            _initialized = true;
        }
        
        Board board = FenBitboardParser.FenToBitboard(fen);

        List<Move> legalMoves = MoveGeneration.MoveGeneration.GenerateMoves(board);

        if (legalMoves.Count == 0)
        {
            return "";
        }

        Move bestMove = MoveSelection.MoveSelection.SelctMove(board, legalMoves);
        
        Console.WriteLine(BitboardUtils.MoveToUci(bestMove));

        return BitboardUtils.MoveToUci(bestMove);
    }
}