using System.Runtime.CompilerServices;

namespace Chess
{
    public enum Move_Flags : byte
    {
        Quiet = 0,
        Capture = 1,
        DoublePush = 2,
        CastleKS = 3,
        CastleQS = 4,
        EnPassant = 5,
        Promotion = 6
    }
    public readonly struct Move
    {
        public uint Value { get; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Move(uint value)
        {
            Value = value;
        }
        public Move(int from, int to, int flags = 0, Piece_Bit promotion = Piece_Bit.None)
        {
            Value = (uint)(from | (to << 6) | (flags << 12) | ((int)promotion << 16));
        }
        public bool IsNull => Value == 0;
        public int From => (int)(Value & 0x3F);
        public int To => (int)((Value >> 6) & 0x3F);
        public bool IsCapture => Flag == Move_Flags.Capture || Flag == Move_Flags.EnPassant;
        public bool IsPromotion => Flag == Move_Flags.Promotion;
        public bool IsEnPassant => Flag == Move_Flags.EnPassant;
        public bool IsDoublePush => Flag == Move_Flags.DoublePush;
        public Move_Flags Flag => (Move_Flags)((Value >> 12) & 0xF);
        public Piece_Bit Promotion => (Piece_Bit)((Value >> 16) & 0xF);
        public bool IsCastle => Flag == Move_Flags.CastleKS || Flag == Move_Flags.CastleQS;
        public bool IsKingsideCastle => Flag == Move_Flags.CastleKS; 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            string promo = IsPromotion ? PieceToChar(Promotion).ToString().ToLower() : "";
            return SquareToString(From) + SquareToString(To) + promo;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string SquareToString(int sq)
        {
            return $"{(char)('a' + (sq % 8))}{(char)('1' + (sq / 8))}";
        }
        private static char PieceToChar(Piece_Bit p)
        {
            return p switch
            {
                Piece_Bit.WPawn or Piece_Bit.BPawn => 'P',
                Piece_Bit.WKnight or Piece_Bit.BKnight => 'N',
                Piece_Bit.WBishop or Piece_Bit.BBishop => 'B',
                Piece_Bit.WRook or Piece_Bit.BRook => 'R',
                Piece_Bit.WQueen or Piece_Bit.BQueen => 'Q',
                Piece_Bit.WKing or Piece_Bit.BKing => 'K',
                _ => ' ',
            };
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) => obj is Move m && Value == m.Value; 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Move left, Move right)
        {
            return left.Equals(right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Move left, Move right)
        {
            return !(left == right);
        }
    }
}