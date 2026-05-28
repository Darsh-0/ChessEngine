namespace chessEngine.MoveGeneration;

public class Board
{
    public ulong whitePawns { get; set; }
    public ulong whiteKnights { get; set; }
    public ulong whiteBishops { get; set; }
    public ulong whiteRooks { get; set; }
    public ulong whiteQueens { get; set; }
    public ulong whiteKing { get; set; }

    public ulong blackPawns { get; set; }
    public ulong blackKnights { get; set; }
    public ulong blackBishops { get; set; }
    public ulong blackRooks { get; set; }
    public ulong blackQueens { get; set; }
    public ulong blackKing { get; set; }

    public ulong whitePieces { get; set; }
    public ulong blackPieces { get; set; }
    public ulong allPieces { get; set; }

    public bool whiteToMove { get; set; }
    
    public ulong friendlyPieces { get; set; }
    public ulong enemyPieces { get; set; }

    public bool whiteCastleKingSide { get; set; }
    public bool whiteCastleQueenSide { get; set; }
    public bool blackCastleKingSide { get; set; }
    public bool blackCastleQueenSide { get; set; }

    public int enPassantFile { get; set; }

    public int halfmoveClock { get; set; }
    public int fullmoveNumber { get; set; }
    
    public Board Clone()
    {
        return new Board
        {
            whitePawns   = this.whitePawns,
            blackPawns   = this.blackPawns,
            whiteKnights = this.whiteKnights,
            blackKnights = this.blackKnights,
            whiteBishops = this.whiteBishops,
            blackBishops = this.blackBishops,
            whiteRooks   = this.whiteRooks,
            blackRooks   = this.blackRooks,
            whiteQueens  = this.whiteQueens,
            blackQueens  = this.blackQueens,
            whiteKing    = this.whiteKing,
            blackKing    = this.blackKing,
            whitePieces  = this.whitePieces,
            blackPieces  = this.blackPieces,
            allPieces    = this.allPieces,
            friendlyPieces  = this.friendlyPieces,
            enemyPieces     = this.enemyPieces,
            whiteToMove     = this.whiteToMove,
            enPassantFile   = this.enPassantFile
        };
    }
}