using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    public static class Mask
    {
        public const ulong Board = 0xFFFFFFFFFFFFFFFFUL;
        // FILE MASKS
        public const ulong FileA = 0x0101010101010101UL;
        public const ulong FileB = 0x0202020202020202UL;
        public const ulong FileC = 0x0404040404040404UL;
        public const ulong FileD = 0x0808080808080808UL;
        public const ulong FileE = 0x1010101010101010UL;
        public const ulong FileF = 0x2020202020202020UL;
        public const ulong FileG = 0x4040404040404040UL;
        public const ulong FileH = 0x8080808080808080UL;
        // RANK MASKS
        public const ulong Rank1 = 0x00000000000000FFUL;
        public const ulong Rank2 = 0x000000000000FF00UL;
        public const ulong Rank3 = 0x0000000000FF0000UL;
        public const ulong Rank4 = 0x00000000FF000000UL;
        public const ulong Rank5 = 0x000000FF00000000UL;
        public const ulong Rank6 = 0x0000FF0000000000UL;
        public const ulong Rank7 = 0x00FF000000000000UL;
        public const ulong Rank8 = 0xFF00000000000000UL;
        // NOT FILE (tránh wrap)
        public const ulong NotFileA = ~FileA;
        public const ulong NotFileH = ~FileH;
    }
}
