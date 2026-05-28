namespace chessEngine.MoveGeneration;

public class FenBitboardParser
{
   
    // White pieces
    public Board FenToBitboard(string fen)
    {
        Board board = new Board();
        
        string[] parts = fen.Split(' ');

        if (parts[1] == "w")
        {
            board.whiteToMove = true;
        }
        else
        {
            board.whiteToMove = false;
        }
        
        int i = 63;
        foreach (char piece in parts[0])
        {
            int rank = i / 8;
            int file = i % 8;
            int bitIndex = rank * 8 + (7 - file);

            ulong bit = 1UL << bitIndex;
            switch (piece)
            {
                case 'P': // white pawn
                    board.whitePawns |= bit;

                    break;
                case 'p': // black pawn
                    board.blackPawns |= bit;

                    break;
                case 'N': // white knight
                    board.whiteKnights |= bit;

                    break;
                case 'n': // black knight
                    board.blackKnights |= bit;

                    break;
                case 'B': // white bishop
                    board.whiteBishops |= bit;

                    break;
                case 'b': // black bishop
                    board.blackBishops |= bit;

                    break;
                case 'R': // white rook
                    board.whiteRooks |= bit;

                    break;
                case 'r': // black rook
                    board.blackRooks |= bit;

                    break;
                case 'Q': // white queen
                    board.whiteQueens |= bit;

                    break;
                case 'q': // black queen
                    board.blackQueens |= bit;

                    break;
                case 'K': // white king
                    board.whiteKing |= bit;

                    break;
                case 'k': // black king
                    board.blackKing |= bit;

                    break;
                default:
                    if (char.IsDigit(piece))
                    {
                        i -= (int)piece - '0' - 1;
                    }
                    else
                    {
                        i += 1;
                    }
                    break;
            }
            i -= 1;
        }
        
        board.whitePieces = board.whitePawns | board.whiteBishops | board.whiteKnights | board.whiteRooks | board.whiteQueens | board.whiteKing;
        board.blackPieces = board.blackPawns | board.blackBishops |board.blackKnights | board.blackRooks |board.blackQueens | board.blackKing;
        board.allPieces = board.whitePieces | board.blackPieces;
        
        board.friendlyPieces = board.whiteToMove ? board.whitePieces : board.blackPieces;
        board.enemyPieces = board.whiteToMove ? board.blackPieces : board.whitePieces;

        board.whiteCastleKingSide = false;
        board.whiteCastleQueenSide = false;
        board.blackCastleKingSide = false;
        board.blackCastleQueenSide = false;

        if (parts[2].Contains('K'))
        {
            board.whiteCastleKingSide = true;
        }

        if (parts[2].Contains('Q'))
        {
            board.whiteCastleQueenSide = true;
        }

        if (parts[2].Contains('k'))
        {
            board.blackCastleKingSide = true;
        }

        if (parts[2].Contains('q'))
        {
            board.blackCastleQueenSide = true;
        }
        
        if (parts[3] != "-")
        {
            board.enPassantFile = parts[3][0] - 'a';
        }
        else
        {
            board.enPassantFile = -1; 
        }
        
        return board;
    }
}
