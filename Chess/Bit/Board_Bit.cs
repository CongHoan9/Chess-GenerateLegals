using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Documents;

namespace Chess
{
    public sealed class Board_Bit
    {
        public int HalfMoveClock;
        public Piece_Color Curent;
        public byte CastlingRights;
        public ulong Zobrist_Instant;
        public int FullMoveNumber = 1;
        public int EnPassantSquare = -1;
        private readonly ulong[] PieceBB = new ulong[12];
        public ulong WhiteOcc, BlackOcc, Occupied;
        public Piece_Bit[] Board = new Piece_Bit[64];
        private readonly Stack<Board_State> PrevBoard_States = new(128);
        public ref Piece_Bit this[int square] => ref Board[square];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong GetPieceBB(Piece_Bit piece)
        {
            return PieceBB[(int)piece - 1];
        }
        public ulong GetPieceBB(int index)
        {
            return PieceBB[index];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count(Piece_Bit piece)
        {
            return BitOperations.PopCount(GetPieceBB(piece));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InCheck(Piece_Color color)
        {
            return Generate.GetCheckers(this, color) != 0;
        }
        public Board_Bit(string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")
        {
            LoadFen(fen);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PutPiece(Piece_Bit piece_bit, int square)
        {
            Board[square] = piece_bit;
            ulong bit = 1UL << square;
            PieceBB[(int)piece_bit - 1] |= bit;
            if (piece_bit <= Piece_Bit.WKing)
            {
                WhiteOcc |= bit;
            }
            else
            {
                BlackOcc |= bit;
            }
            Occupied |= bit;
            Zobrist_Instant ^= Zobrist.PieceKeys[(int)piece_bit - 1, square];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemovePiece(int square)
        {
            Piece_Bit piece_bit = Board[square];
            ulong bit = ~(1UL << square);
            Board[square] = Piece_Bit.None;
            PieceBB[(int)piece_bit - 1] &= bit;
            if (piece_bit <= Piece_Bit.WKing)
            {
                WhiteOcc &= bit;
            }
            else
            {
                BlackOcc &= bit;
            }
            Occupied &= bit;
            Zobrist_Instant ^= Zobrist.PieceKeys[(int)piece_bit - 1, square];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LoadFen(string fen)
        {
            Array.Fill(Board, Piece_Bit.None);
            Array.Clear(PieceBB, 0, PieceBB.Length);
            WhiteOcc = BlackOcc = Occupied = 0UL;
            Zobrist_Instant = 0UL;
            string[] parts = fen.Split(' ');
            int square = 56;
            foreach (char c in parts[0])
            {
                if (c == '/') 
                { 
                    square -= 16; 
                    continue;
                }
                if (char.IsDigit(c))
                {
                    square += c - '0';
                    continue; 
                }
                Piece_Bit p = Support_Bit.CharToPiece(c);
                PutPiece(p, square);
                square++;
            }
            Curent = parts[1] == "w" ? Piece_Color.White : Piece_Color.Black;
            if (Curent == Piece_Color.Black)
            {
                Zobrist_Instant ^= Zobrist.SideKey;
            }
            CastlingRights = 0;
            if (parts.Length > 2)
            {
                string cr = parts[2];
                if (cr.Contains('K'))
                {
                    CastlingRights |= 1;
                }
                if (cr.Contains('Q'))
                {
                    CastlingRights |= 2;
                }
                if (cr.Contains('k'))
                {
                    CastlingRights |= 4;
                }
                if (cr.Contains('q'))
                {
                    CastlingRights |= 8;
                }
            }
            Zobrist_Instant ^= Zobrist.CastlingKeys[CastlingRights];
            EnPassantSquare = -1;
            if (parts.Length > 3 && parts[3] != "-")
            {
                EnPassantSquare = (parts[3][0] - 'a') + (parts[3][1] - '1') * 8;
                Zobrist_Instant ^= Zobrist.EnPassantKeys[EnPassantSquare];
            }
            HalfMoveClock = parts.Length > 4 ? int.Parse(parts[4]) : 0;
            FullMoveNumber = parts.Length > 5 ? int.Parse(parts[5]) : 1;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MakeMove(Move move)
        {
            Board_State state = new()
            {
                Move = move,
                Moved = Board[move.From],
                Captured = Piece_Bit.None,
                CapturedSquare = move.To, // Default capture square
                EnPassant = EnPassantSquare,
                Castling = CastlingRights,
                HalfMove = HalfMoveClock,
                FullMove = FullMoveNumber,
                Zobrist = Zobrist_Instant,
                Current = Curent,
            };
            if (move.IsEnPassant)
            {
                int dir = Curent == Piece_Color.White ? -8 : 8;
                int capturedSq = move.To + dir;
                state.Captured = Board[capturedSq];
                state.CapturedSquare = capturedSq;
                RemovePiece(capturedSq);
            }
            else if (move.IsCapture)
            {
                state.Captured = Board[move.To];
                RemovePiece(move.To); // Nếu Board[To] là None, hàm RemovePiece mới (đã fix) sẽ bỏ qua an toàn
            }
            RemovePiece(move.From);
            PutPiece(state.Moved, move.To);
            if (move.IsPromotion)
            {
                RemovePiece(move.To); // Xóa Tốt vừa đi tới
                PutPiece(move.Promotion, move.To); // Đặt quân phong cấp vào
            }
            if (move.IsCastle)
            {
                bool ks = move.IsKingsideCastle;
                int rookFrom = ks ? move.From + 3 : move.From - 4;
                int rookTo = ks ? move.From + 1 : move.From - 1;
                Piece_Bit rook = Board[rookFrom]; // Lấy loại quân thực tế (an toàn hơn fix cứng WRook/BRook)
                RemovePiece(rookFrom);
                PutPiece(rook, rookTo);
            }
            if (CastlingRights != 0) // Chỉ tính toán nếu còn quyền nhập thành
            {
                if (state.Moved == Piece_Bit.WKing || state.Captured == Piece_Bit.WKing)
                {
                    CastlingRights &= 0b1100;
                }
                else if (state.Moved == Piece_Bit.BKing || state.Captured == Piece_Bit.BKing)
                {
                    CastlingRights &= 0b0011;
                }    
                if (state.Moved == Piece_Bit.WRook)
                {
                    if (move.From == 0)
                    {
                        CastlingRights &= 0b1101;
                    }
                    else if (move.From == 7)
                    {
                        CastlingRights &= 0b1110;
                    }
                }
                else if (state.Moved == Piece_Bit.BRook)
                {
                    if (move.From == 56)
                    {
                        CastlingRights &= 0b0111;
                    }
                    else if (move.From == 63)
                    {
                        CastlingRights &= 0b1011;
                    }
                }
                if (state.Captured == Piece_Bit.WRook)
                {
                    if (state.CapturedSquare == 0)
                    {
                        CastlingRights &= 0b1101;
                    }
                    else if (state.CapturedSquare == 7)
                    {
                        CastlingRights &= 0b1110;
                    }
                }
                else if (state.Captured == Piece_Bit.BRook)
                {
                    if (state.CapturedSquare == 56)
                    {
                        CastlingRights &= 0b0111;
                    }
                    else if (state.CapturedSquare == 63)
                    {
                        CastlingRights &= 0b1011;
                    }
                }
            }
            EnPassantSquare = -1;
            if (move.IsDoublePush)
            {
                EnPassantSquare = (move.From + move.To) >> 1;
                Zobrist_Instant ^= Zobrist.EnPassantKeys[EnPassantSquare];
            }
            HalfMoveClock = (state.Captured != Piece_Bit.None || state.Moved == Piece_Bit.WPawn || state.Moved == Piece_Bit.BPawn) ? 0 : HalfMoveClock + 1;
            if (Curent == Piece_Color.Black)
            {
                FullMoveNumber++;
            }
            Curent = Curent == Piece_Color.White ? Piece_Color.Black : Piece_Color.White;
            Zobrist_Instant ^= Zobrist.SideKey;
            Zobrist_Instant ^= Zobrist.CastlingKeys[state.Castling] ^ Zobrist.CastlingKeys[CastlingRights];
            if (state.EnPassant != -1)
            {
                Zobrist_Instant ^= Zobrist.EnPassantKeys[state.EnPassant];
            }
            PrevBoard_States.Push(state);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnMakeMove()
        {
            if (PrevBoard_States.Count == 0)
            {
                return;
            }
            Board_State s = PrevBoard_States.Pop();
            Curent = s.Current;
            CastlingRights = s.Castling;
            EnPassantSquare = s.EnPassant;
            HalfMoveClock = s.HalfMove;
            FullMoveNumber = s.FullMove;
            if (s.Move.IsCastle)
            {
                bool ks = s.Move.IsKingsideCastle;
                int rookFrom = ks ? s.Move.From + 3 : s.Move.From - 4;
                int rookTo = ks ? s.Move.From + 1 : s.Move.From - 1;
                Piece_Bit rook = Board[rookTo];
                RemovePiece(rookTo);
                PutPiece(rook, rookFrom);
            }
            RemovePiece(s.Move.To);
            PutPiece(s.Moved, s.Move.From);
            if (s.Captured != Piece_Bit.None)
            {
                PutPiece(s.Captured, s.CapturedSquare);
            }
            Zobrist_Instant = s.Zobrist;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MovePiece(int from, int to)
        {
            Piece_Bit piece = Board[from];
            Piece_Bit captured = Board[to];
            bool isCapture = captured != Piece_Bit.None;
            bool isPawnMove = piece == Piece_Bit.WPawn || piece == Piece_Bit.BPawn;
            bool isKingMove = piece == Piece_Bit.WKing || piece == Piece_Bit.BKing;
            bool isRookMove = piece == Piece_Bit.WRook || piece == Piece_Bit.BRook;

            // Lưu giá trị cũ để cập nhật Zobrist
            byte oldCastling = CastlingRights;
            int oldEnPassant = EnPassantSquare;

            // === 1. Xử lý capture trước ===
            if (isCapture)
            {
                RemovePiece(to); // cập nhật PieceBB, Occ, Zobrist
            }

            // === 2. Xóa quân từ vị trí cũ ===
            RemovePiece(from);

            // === 3. Đặt quân vào vị trí mới ===
            PutPiece(piece, to);

            // === 4. Update HalfMoveClock ===
            if (isCapture || isPawnMove)
                HalfMoveClock = 0;
            else
                HalfMoveClock++;

            // === 5. Update CastlingRights ===
            if (isKingMove)
            {
                if (piece <= Piece_Bit.WKing) // White king
                    CastlingRights &= 0b1100; // mất cả 2 quyền trắng
                else
                    CastlingRights &= 0b0011; // mất cả 2 quyền đen
            }
            else if (isRookMove)
            {
                if (piece == Piece_Bit.WRook)
                {
                    if (from == 0) CastlingRights &= 0b1101; // mất queenside white
                    if (from == 7) CastlingRights &= 0b1110; // mất kingside white
                }
                else // Black rook
                {
                    if (from == 56) CastlingRights &= 0b0111;
                    if (from == 63) CastlingRights &= 0b1011;
                }
            }

            // === 6. Reset EnPassantSquare (không hỗ trợ double push) ===
            if (EnPassantSquare != -1)
            {
                Zobrist_Instant ^= Zobrist.EnPassantKeys[EnPassantSquare];
                EnPassantSquare = -1;
            }

            // === 7. Update Zobrist (Castling + Side) ===
            Zobrist_Instant ^= Zobrist.CastlingKeys[oldCastling] ^ Zobrist.CastlingKeys[CastlingRights];
            Zobrist_Instant ^= Zobrist.SideKey; // đổi lượt đi

            // FullMoveNumber: nếu là nước đen thì tăng
            if (Curent == Piece_Color.Black)
                FullMoveNumber++;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong AttackersTo(int sq, Piece_Color side, ulong occ)
        {
            ulong attackers = 0;
            attackers |= Attacks.GetPawnAttacks(side == Piece_Color.White ? Piece_Color.Black : Piece_Color.White, sq) & GetPieceBB(Generate.GetPawn(side));
            attackers |= Attacks.GetKnightAttacks(sq) & GetPieceBB(Generate.GetKnight(side));
            attackers |= Magic.GetBishopAttacks(sq, occ) & GetPieceBB(Generate.GetBishop(side));
            attackers |= Magic.GetRookAttacks(sq, occ) & GetPieceBB(Generate.GetRook(side));
            attackers |= Magic.GetQueenAttacks(sq, occ) & GetPieceBB(Generate.GetQueen(side));
            attackers |= Attacks.GetKingAttacks(sq) & GetPieceBB(Generate.GetKing(side));
            return attackers;
        }
    }
}