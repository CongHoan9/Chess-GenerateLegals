using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Chess
{
    public ref struct Generate_Context
    {
        public readonly PinnedInfo PinnedInfo;
        public readonly bool CapturesOnly;
        public readonly ulong TargetMask;
        public readonly Piece_Color Them;
        public readonly Board_Bit Board;
        public readonly ulong TheirOcc;
        public readonly int Kingsquare;
        public readonly ulong Checkers;
        public readonly Piece_Color Us;
        public readonly ulong Occupied;
        public readonly ulong OwnOcc;
        public readonly ulong EpBB;
        public Span<Move> Moves; 
        public int Count;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Generate_Context(Board_Bit board, Span<Move> moves, PinnedInfo pinnedinfo, ulong checkers, int kingsquare, ulong targetMask, bool capturesonly)
        {
            Count = 0;
            Board = board;
            Moves = moves;
            Us = board.Curent;
            Checkers = checkers;
            PinnedInfo = pinnedinfo;
            TargetMask = targetMask;
            Kingsquare = kingsquare;
            Occupied = board.Occupied;
            CapturesOnly = capturesonly;
            Them = Us ^ Piece_Color.Black;
            OwnOcc = Us == Piece_Color.White ? board.WhiteOcc : board.BlackOcc;
            TheirOcc = Occupied ^ OwnOcc;
            EpBB = board.EnPassantSquare >= 0 ? 1UL << board.EnPassantSquare : 0UL;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(Move m) => Moves[Count++] = m;
    }
}
