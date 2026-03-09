namespace Chess
{
    public static class Rays
    {
        public static readonly ulong[,] Ray = new ulong[64, 8];
        static Rays()
        {
            for (int sq = 0; sq < 64; sq++)
            {
                int r = sq / 8;
                int f = sq % 8;
                // N
                for (int rr = r + 1; rr < 8; rr++)
                {
                    Ray[sq, 0] |= 1UL << (rr * 8 + f);
                }
                // S
                for (int rr = r - 1; rr >= 0; rr--)
                {
                    Ray[sq, 1] |= 1UL << (rr * 8 + f);
                }
                // E
                for (int ff = f + 1; ff < 8; ff++)
                {
                    Ray[sq, 2] |= 1UL << (r * 8 + ff);
                }
                // W
                for (int ff = f - 1; ff >= 0; ff--)
                {
                    Ray[sq, 3] |= 1UL << (r * 8 + ff);
                }
                // NE
                for (int rr = r + 1, ff = f + 1; rr < 8 && ff < 8; rr++, ff++)
                {
                    Ray[sq, 4] |= 1UL << (rr * 8 + ff);
                }
                // NW
                for (int rr = r + 1, ff = f - 1; rr < 8 && ff >= 0; rr++, ff--)
                {
                    Ray[sq, 5] |= 1UL << (rr * 8 + ff);
                }
                // SE
                for (int rr = r - 1, ff = f + 1; rr >= 0 && ff < 8; rr--, ff++)
                {
                    Ray[sq, 6] |= 1UL << (rr * 8 + ff);
                }
                // SW
                for (int rr = r - 1, ff = f - 1; rr >= 0 && ff >= 0; rr--, ff--)
                {
                    Ray[sq, 7] |= 1UL << (rr * 8 + ff);
                }
            }
        }
        public static bool IsPositive(int direction)
        {
            return direction == 0 || direction == 2 || direction == 4 || direction == 5;
        }
    }

}
