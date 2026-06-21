using ChessBot.Core.Core;

namespace ChessEngine;

public struct Move
{
    public ulong from;
    public ulong to;
    public Piece? promotionPiece;
    public Piece? capturedPiece;
    public bool isEnPassant;
    public bool isCastle;
}