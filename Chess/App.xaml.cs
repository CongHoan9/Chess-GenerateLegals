using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Numerics;
using System.Windows;

namespace Chess
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var board = new Board_Bit();
            Stopwatch sw = Stopwatch.StartNew();
            ulong nodes = Search.Perft(board, 7, true);
            sw.Stop();
            double seconds = sw.Elapsed.TotalSeconds;
            double nps = nodes / seconds;
            Console.WriteLine("-");
            Console.WriteLine($"Count: {nodes:N0} nodes");
            Console.WriteLine($"Time: {seconds:F3}s");
            Console.WriteLine($"Speed: {nps:N0} nodes/s");
            //TestMakeUnmake();
        }
        public static void TestMakeUnmake()
        {
            Console.WriteLine("\nTestMakeUnmake...");
            var board = new Board_Bit();
            ulong initialZobrist = board.Zobrist_Instant;
            ulong initialOccupied = board.Occupied;
            var initialSide = board.Curent;
            Span<Move> moves = stackalloc Move[256];
            int cnt = Generate.GenerateMoves(board, moves);
            for (int i = 0; i < cnt; i++)
            {
                TestMakeUnmake(board, moves[i]);
            }
            Console.WriteLine("Make/Unmake OK");
        }
        public static bool TestMakeUnmake(Board_Bit board, Move move)
        {
            // Lưu trạng thái ban đầu
            ulong initialZobrist = board.Zobrist_Instant;
            ulong initialOccupied = board.Occupied;
            ulong initialWhiteOcc = board.WhiteOcc;
            ulong initialBlackOcc = board.BlackOcc;
            byte initialCastling = board.CastlingRights;
            int initialEP = board.EnPassantSquare;
            int initialHalf = board.HalfMoveClock;
            int initialFull = board.FullMoveNumber;
            var initialSide = board.Curent;
            Span<Piece_Bit> initialBoard = stackalloc Piece_Bit[64];
            for (int i = 0; i < 64; i++)
            {
                initialBoard[i] = board.Board[i];

            }
            // Thực hiện
            board.MakeMove(move);
            board.UnMakeMove();
            // So sánh
            if (board.Zobrist_Instant != initialZobrist) return false;
            if (board.Occupied != initialOccupied) return false;
            if (board.WhiteOcc != initialWhiteOcc) return false;
            if (board.BlackOcc != initialBlackOcc) return false;
            if (board.CastlingRights != initialCastling) return false;
            if (board.EnPassantSquare != initialEP) return false;
            if (board.HalfMoveClock != initialHalf) return false;
            if (board.FullMoveNumber != initialFull) return false;
            if (board.Curent != initialSide) return false;
            for (int i = 0; i < 64; i++)
            {
                if (board.Board[i] != initialBoard[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
