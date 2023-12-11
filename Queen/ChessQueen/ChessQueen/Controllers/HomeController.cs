using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ChessQueen.Src;

namespace ChessQueen.Controllers
{
    public class HomeController : Controller
    {
        static int N;
        public JsonResult Index(int num = 9)
        {

            try
            {
                N = Convert.ToInt32(num);
            }
            catch (Exception)
            {
                return Json(new
                {
                    result = "Số hàng số cột cần là số nguyên và >= 3"
                }, JsonRequestBehavior.AllowGet);
            }
            int[,] board = new int[N, N];

            // Đo thời gian bắt đầu
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            bool isBoardSolved = true;
            if (N <= 25)
            {
                isBoardSolved = theBoardSolver(board, 0);
            }
            // Dừng đồng hồ và lấy thời gian chạy
            stopwatch.Stop();
            var elapsedTicks = stopwatch.ElapsedTicks;
            var elapsedMs = (double)elapsedTicks / System.Diagnostics.Stopwatch.Frequency * 1000;

            int populationSize = 50;
            double crossoverProbability = 0.8;
            double mutationProbability = 0.03;
            int maxGenerations = 1000;
            int numParallelTasks = 4;

            // Đo thời gian bắt đầu cho thuật toán GPA
            var stopwatchGPA = new System.Diagnostics.Stopwatch();
            stopwatchGPA.Start();

            GenericAlgorithm geneticAlgorithm = new GenericAlgorithm(N, populationSize, crossoverProbability, mutationProbability, maxGenerations, numParallelTasks);
            geneticAlgorithm.Run();

            // Dừng đồng hồ và lấy thời gian chạy
            stopwatch.Stop();
            var elapsedTicksGPA = stopwatchGPA.ElapsedTicks;
            var elapsedMsGPA = (double)elapsedTicksGPA / System.Diagnostics.Stopwatch.Frequency * 1000;


            if (!isBoardSolved)
            {
                return Json(new
                {
                    result = "Đã có lỗi xảy ra"
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                result = printBoard(board, elapsedMs, elapsedMsGPA),
                timeElapsed = elapsedMs, // Trả về thời gian chạy
                timeElapsedGPA = elapsedMsGPA
            }, JsonRequestBehavior.AllowGet);
        }


        static string printBoard(int[,] queens, double timeElapsed, double timeElapsedGPA)
        {
            int numRows = queens.GetLength(0);
            int numCols = queens.GetLength(1);

            // Calculate square size based on both rows and columns
            int squareSize = Math.Min(700 / numRows, 700 / numCols);

            var result = new StringBuilder();
            result.AppendLine($"<div>Thời gian chạy thuật toán Backtracking: {timeElapsed} milliseconds</div>");
            result.AppendLine($"<div>Thời gian chạy thuật toán PGA: {timeElapsedGPA} milliseconds</div>");

            result.AppendLine("<div style='display: flex; flex-wrap: wrap; width: " + (squareSize * numCols) + "px; height: " + (squareSize * numRows) + "px;'>");

            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numCols; col++)
                {
                    result.AppendLine(RenderSquare(col, row, queens[row, col], squareSize));
                }
            }

            result.AppendLine("</div>");

            return result.ToString();
        }

        static string RenderSquare(int col, int row, int isQueen, int size)
        {
            string squareColor = (row + col) % 2 == 0 ? "white" : "black";
            string queenClassName = isQueen == 1 ? "red-queen" : "";

            // Tính toán kích thước cho quân Hậu dựa trên kích thước của ô vuông
            int queenSize = size; // Kích thước của quân Hậu, có thể điều chỉnh tùy ý

            return $"<div class='square {queenClassName}' style='width: {size - 2}px; height: {size - 2}px; background-color: {squareColor}; text-align: center;'>" +
                   $"<div style='width: {queenSize}px; height: {queenSize}px; margin: auto; line-height: {queenSize}px;font-size: {queenSize * 0.6}px;'>{ (isQueen == 1 ? '♕' : ' ') }</div></div>";
        }





        static bool toPlaceOrNotToPlace(int[,] board, int row, int col)
        {
            if (N > 25)
            {
                return true;
            }
            int i, j;
            for (i = 0; i < col; i++)
            {
                if (board[row, i] == 1)
                    return false;
            }
            for (i = row, j = col; i >= 0 && j >= 0; i--, j--)
            {
                if (board[i, j] == 1)
                    return false;
            }
            for (i = row, j = col; j >= 0 && i < N; i++, j--)
            {
                if (board[i, j] == 1)
                    return false;
            }
            return true;
        }

        static bool theBoardSolver(int[,] board, int col)
        {
            if (col >= N)
                return true;
            for (int i = 0; i < N; i++)
            {
                if (toPlaceOrNotToPlace(board, i, col))
                {
                    board[i, col] = 1;
                    if (theBoardSolver(board, col + 1))
                        return true;
                    // Backtracking is important in this one.
                    board[i, col] = 0;
                }
            }
            return false;
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}