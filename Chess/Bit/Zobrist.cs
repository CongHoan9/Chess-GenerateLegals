namespace Chess
{
    public static class Zobrist
    {
        public static readonly ulong[,] PieceKeys = new ulong[12, 64];
        public static readonly ulong[] EnPassantKeys = new ulong[64];
        public static readonly ulong[] CastlingKeys = new ulong[16];
        public static readonly ulong SideKey;
        static Zobrist()
        {
            ulong seed = 0x123456789ABCDEF0UL;
            for (int p = 0; p < 12; p++)
            {
                for (int sq = 0; sq < 64; sq++)
                {
                    PieceKeys[p, sq] = RandomU64(ref seed);
                }
            }
            for (int sq = 0; sq < 64; sq++)
            {
                EnPassantKeys[sq] = RandomU64(ref seed);
            }
            for (int i = 0; i < 16; i++)
            {
                CastlingKeys[i] = RandomU64(ref seed);
            }
            SideKey = RandomU64(ref seed);
        }
        private static ulong RandomU64(ref ulong state)
        {
            state ^= state >> 12;
            state ^= state << 25;
            state ^= state >> 27;
            return state * 0x2545F4914F6CDD1DUL;
        }
    }
}