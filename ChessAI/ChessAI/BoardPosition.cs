using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI
{
    public static class Piece
    {
        public const byte Empty     = 0b_0000_0000; // 0
        public const byte King      = 0b_0000_0001; // 1
        public const byte Queen     = 0b_0000_0010; // 2
        public const byte Bishop    = 0b_0000_0100; // 4
        public const byte Knight    = 0b_0000_1000; // 8
        public const byte Rook      = 0b_0001_0000; // 16
        public const byte Pawn      = 0b_0010_0000; // 32
        public const byte EmpPawn   = 0b_0100_0000; // 64  (when used, color should be always black so clearing it on a board copy can be done quickly with a single mask, use the index of the EmpPawn to determine color)
        public const byte White     = 0b_1000_0000; // 128

        public static readonly ulong[] initPos = {
            (((ulong)Rook) << 56) | (((ulong)Knight) << 48) | (((ulong)Bishop) << 40) | (((ulong)Queen) << 32) |
            (((ulong)King) << 24) | (((ulong)Bishop) << 16) | (((ulong)Knight) << 8) | (((ulong)Rook) << 0),
            (((ulong)Pawn) << 56) | (((ulong)Pawn) << 48) | (((ulong)Pawn) << 40) | (((ulong)Pawn) << 32) |
            (((ulong)Pawn) << 24) | (((ulong)Pawn) << 16) | (((ulong)Pawn) << 8) | (((ulong)Pawn) << 0),
            0, 0, 0, 0, 
            (((ulong)(Pawn | White)) << 56) | (((ulong)(Pawn | White)) << 48) | (((ulong)(Pawn | White)) << 40) | (((ulong)(Pawn | White)) << 32) |
            (((ulong)(Pawn | White)) << 24) | (((ulong)(Pawn | White)) << 16) | (((ulong)(Pawn | White)) << 8) | (((ulong)(Pawn | White)) << 0),
            (((ulong)(Rook | White)) << 56) | (((ulong)(Knight | White)) << 48) | (((ulong)(Bishop | White)) << 40) | (((ulong)(Queen | White)) << 32) |
            (((ulong)(King | White)) << 24) | (((ulong)(Bishop | White)) << 16) | (((ulong)(Knight | White)) << 8) | (((ulong)(Rook | White)) << 0)
        };
    }
    public class BoardPosition
    {
        /// <summary>
        /// White always starts on indexes 48-63
        /// 
        /// 
        /// Board Layout by index:
        ///  0 |  1 |  2 |  3 |  4 |  5 |  6 |  7
        /// --------------------------------------
        ///  8 |  9 | 10 | 11 | 12 | 13 | 14 | 15
        /// --------------------------------------
        /// 16 | 17 | 18 | 19 | 20 | 21 | 22 | 23
        /// --------------------------------------
        /// 24 | 25 | 26 | 27 | 28 | 29 | 30 | 31
        /// --------------------------------------
        /// 32 | 33 | 34 | 35 | 36 | 37 | 38 | 39
        /// --------------------------------------
        /// 40 | 41 | 42 | 43 | 44 | 45 | 46 | 47
        /// --------------------------------------
        /// 48 | 49 | 50 | 51 | 52 | 53 | 54 | 55
        /// --------------------------------------
        /// 56 | 57 | 58 | 59 | 60 | 61 | 62 | 63 
        /// 
        /// index MOD 8  (column id)
        ///  0 |  1 |  2 |  3 |  4 |  5 |  6 |  7
        /// </summary>
        private readonly byte[] board = new byte[8 * 8];

        /// <summary>
        /// Clears Em Passant pieces from the destination when copying the board position.
        /// </summary>
        private static void FastBoardCopy(ulong[] source, byte[] destination)
        {
            unsafe
            {
                const ulong EmpMask = ~(
                        (ulong)Piece.EmpPawn << 56 | (ulong)Piece.EmpPawn << 56 | (ulong)Piece.EmpPawn << 56 | (ulong)Piece.EmpPawn << 56 |
                        (ulong)Piece.EmpPawn << 24 | (ulong)Piece.EmpPawn << 16 | (ulong)Piece.EmpPawn << 8 | (ulong)Piece.EmpPawn << 0
                     );
                fixed (ulong* src = source)
                {
                    fixed (byte* dest = destination)
                    {
                        ulong* s = src;
                        ulong* d = (ulong*)dest;
                        *d = *src;
                        d++;
                        s++;
                        *d = *src;
                        d++;
                        s++;
                        *d = *src & EmpMask;
                        d++;
                        s++;
                        *d = *src;
                        d++;
                        s++;
                        *d = *src;
                        d++;
                        s++;
                        *d = *src & EmpMask;
                        d++;
                        s++;
                        *d = *src;
                        d++;
                        s++;
                        *d = *src;
                        d++;
                        s++;
                    }
                }
            }
        }

        /// <summary>
        /// Clears Em Passant pieces from the destination when copying the board position.
        /// </summary>
        private static void FastBoardCopy(byte[] source, byte[] destination)
        {
            unsafe
            {
                const ulong EmpMask = ~(
                        (ulong)Piece.EmpPawn << 56 | (ulong)Piece.EmpPawn << 56 | (ulong)Piece.EmpPawn << 56 | (ulong)Piece.EmpPawn << 56 |
                        (ulong)Piece.EmpPawn << 24 | (ulong)Piece.EmpPawn << 16 | (ulong)Piece.EmpPawn << 8 | (ulong)Piece.EmpPawn << 0
                     );
                fixed (byte* dest = destination, src = source)
                {
                    ulong* s = (ulong*)src;
                    ulong* d = (ulong*)dest;
                    *d = *src;
                    d++;
                    s++;
                    *d = *src;
                    d++;
                    s++;
                    *d = *src & EmpMask;
                    d++;
                    s++;
                    *d = *src;
                    d++;
                    s++;
                    *d = *src;
                    d++;
                    s++;
                    *d = *src & EmpMask;
                    d++;
                    s++;
                    *d = *src;
                    d++;
                    s++;
                    *d = *src;
                    d++;
                    s++;
                }
            }
        }

        public BoardPosition()
        {
            FastBoardCopy(Piece.initPos, board);
        }

        private BoardPosition(BoardPosition copy)
        {
            FastBoardCopy(copy.board, board);
        }

        private BoardPosition CopyMove(int sourceIdx, int destinationIdx)
        {
#if DEBUG
            if (board[sourceIdx] > 0)
                throw new Exception("No Piece at board index: " + sourceIdx);
#endif
            var bp = new BoardPosition(this);
            bp.board[destinationIdx] = bp.board[sourceIdx];
            bp.board[sourceIdx] = Piece.Empty;
            return bp;
        }
 
        /// <summary>
        /// Output all the new boards that could be produced if the pawn at the supplied index moved to a valid board location.
        /// Does not check to see if the pawn moving would cause CHECK on the king.
        /// </summary>
        /// <param name="boardIdx"></param>
        /// <returns></returns>
        public List<BoardPosition> PawnMoves(int boardIdx)
        {
#if DEBUG
            if ((board[boardIdx] & Piece.Pawn) == 0)
                throw new Exception("No Pawn at given board index: " + boardIdx);
            if ((board[boardIdx] & Piece.White) == Piece.White && boardIdx >= 56)
                throw new Exception("Illigal white pawn postion");
            if ((board[boardIdx] & Piece.White) != Piece.White && boardIdx < 8)
                throw new Exception("Illigal black pawn postion");
#endif
            var bpl = new List<BoardPosition>();

            int chkIdx;
            int idxMod = boardIdx % 8;

            // if pawn is black
            if ((board[boardIdx] & Piece.White) != Piece.White)
            {
                if (boardIdx < 56)
                {
                    chkIdx = boardIdx + 8;  // check moving forward one space
                    if (board[chkIdx] == Piece.Empty)
                        bpl.Add(CopyMove(boardIdx, chkIdx));
                    if (idxMod != 0)  // if not on the left most column
                    {
                        chkIdx = boardIdx + 7;  // check if capture to the left diagonal is possible
                        if ((board[chkIdx] & Piece.White) == Piece.White && board[chkIdx] != Piece.Empty)
                            bpl.Add(CopyMove(boardIdx, chkIdx));
                        else if (chkIdx >= 40 && (board[chkIdx] & Piece.EmpPawn) == Piece.EmpPawn)
                        {
#if DEBUG
                            if (chkIdx > 47 || (board[chkIdx - 8] & (Piece.Pawn | Piece.White)) != (Piece.Pawn | Piece.White))
                                throw new Exception("Invalid location found for EmpPawn on board.");
#endif
                            var newBoard = CopyMove(boardIdx, chkIdx);
                            newBoard.board[chkIdx - 8] = Piece.Empty;
                            bpl.Add(newBoard);
                        }
                    }
                    if (idxMod != 7)  // if not on the right most column
                    {
                        chkIdx = boardIdx + 9;  // check if capture to the right diagonal is possible
                        if ((board[chkIdx] & Piece.White) != Piece.White && board[chkIdx] != Piece.Empty)
                            bpl.Add(CopyMove(boardIdx, chkIdx));
                        else if (chkIdx >= 40 && (board[chkIdx] & Piece.EmpPawn) == Piece.EmpPawn)
                        {
#if DEBUG
                            if (chkIdx > 47 || (board[chkIdx - 8] & (Piece.Pawn | Piece.White)) != (Piece.Pawn | Piece.White))
                                throw new Exception("Invalid location found for EmpPawn on board.");
#endif
                            var newBoard = CopyMove(boardIdx, chkIdx);
                            newBoard.board[chkIdx - 8] = Piece.Empty;
                            bpl.Add(newBoard);
                        }
                    }
                }
                if (boardIdx < 16)
                {
                    chkIdx = boardIdx + 8;  // check moving forward one space
                    if (board[chkIdx] == Piece.Empty)
                    {
                        chkIdx += 8;  // check moving forward two spaces
                        if (board[chkIdx] == Piece.Empty)
                        {
                            var newBoard = CopyMove(boardIdx, chkIdx);
                            newBoard.board[boardIdx + 8] = Piece.EmpPawn;  // white color not added because this allows fast clearing of Em Passant bit when using FastBoardCopy()
                            bpl.Add(newBoard);
                        }
                    }
                }
            }
            else // if pawn is white
            {
                if (boardIdx >= 8)
                {
                    chkIdx = boardIdx - 8;  // check moving forward one space
                    if (board[chkIdx] == Piece.Empty)
                        bpl.Add(CopyMove(boardIdx, chkIdx));
                    if (idxMod != 0)  // if not on the left most column
                    {
                        chkIdx = boardIdx - 9;  // check if capture to the left diagonal is possible
                        if ((board[chkIdx] & Piece.White) == Piece.White && board[chkIdx] != Piece.Empty)
                            bpl.Add(CopyMove(boardIdx, chkIdx));
                        else if (chkIdx < 24 && (board[chkIdx] & Piece.EmpPawn) == Piece.EmpPawn)
                        {
#if DEBUG
                            if (chkIdx < 16 || (board[chkIdx + 8] & Piece.Pawn) != Piece.Pawn)
                                throw new Exception("Invalid location found for EmpPawn on board.");
#endif
                            var newBoard = CopyMove(boardIdx, chkIdx);
                            newBoard.board[chkIdx + 8] = Piece.Empty;
                            bpl.Add(newBoard);
                        }
                    }
                    if (idxMod != 7)  // if not on the right most column
                    {
                        chkIdx = boardIdx - 7;  // check if capture to the right diagonal is possible
                        if ((board[chkIdx] & Piece.White) == Piece.White && board[chkIdx] != Piece.Empty)
                            bpl.Add(CopyMove(boardIdx, chkIdx));
                        else if (chkIdx < 24 && (board[chkIdx] & Piece.EmpPawn) == Piece.EmpPawn)
                        {
#if DEBUG
                            if (chkIdx < 16 || (board[chkIdx + 8] & Piece.Pawn) != Piece.Pawn)
                                throw new Exception("Invalid location found for EmpPawn on board.");
#endif
                            var newBoard = CopyMove(boardIdx, chkIdx);
                            newBoard.board[chkIdx + 8] = Piece.Empty;
                            bpl.Add(newBoard);
                        }
                    }
                }
                if (boardIdx >= 48)
                {
                    chkIdx = boardIdx - 8;  // check moving forward one space
                    if (board[chkIdx] == Piece.Empty)
                    {
                        chkIdx -= 8;  // check moving forward two spaces
                        if (board[chkIdx] == Piece.Empty)
                        {
                            var newBoard = CopyMove(boardIdx, chkIdx);
                            newBoard.board[boardIdx - 8] = Piece.EmpPawn;
                            bpl.Add(newBoard);
                        }
                    }
                }
            }

            return null;
        }
    }
}
