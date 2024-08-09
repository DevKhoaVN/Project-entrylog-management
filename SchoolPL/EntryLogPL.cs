using EntryLogManagement.SchoolBLL;
using EntryLogManagement.SchoolPL.Utility;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMySql.Models;

namespace EntryLogManagement.SchoolPL
{
    internal class EntryLogPL
    {
        private readonly EntryLogService entryLogService;
        private readonly CameraService cameraService;

       public EntryLogPL()
        {
            entryLogService = new EntryLogService();
            cameraService = new CameraService();

        }

        public void ShowEntryLogID()
        {
            bool dataAvailable = false;

            while (!dataAvailable)
            {
                Console.WriteLine();
                int id = InputHepler.GetIntPrompt("Nhập [green]ID bạn muốn tìm kiếm: [/]");

                var log = entryLogService.GetEtryLogID(id);

                if (log.Count > 0)
                {
                    ShowEntrylog_Table(log);
                    dataAvailable = true; // Exit the loop as data is available
                    Console.WriteLine();
                }
                else
                {
                   
                    AnsiConsole.MarkupLine("[red]Không có dữ liệu cho ID đã nhập.[/] Vui lòng nhập lại.");
                    Console.WriteLine();
                }
            }
        }

        public void ShowEntryLogAll()
        {

            var log = entryLogService.GetEtryLogAll();

            ShowEntrylog_Table(log);
        }
        public void ShowEntryLogRangeTime()
        {
            bool dataAvailable = false;

            while (!dataAvailable)
            {
                Console.WriteLine();
                DateTime timeStart = InputHepler.GetDate("Nhập [green]ngày bắt đầu (dd/mm/yyyy): [/]");
                DateTime timeEnd = InputHepler.GetDate("Nhập [green]ngày kết thúc (dd/mm/yyyy): [/]");

                var log = entryLogService.GetEntryLogRangeTime(timeStart, timeEnd);

                if (log.Count > 0)
                {
                    ShowEntrylog_Table(log);
                    dataAvailable = true;
                    Console.WriteLine();
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Không có dữ liệu trong khoảng thời gian trên.[/] Vui lòng nhập lại.");
                    Console.WriteLine();
                }
            }
        }


        public void RecoredEntryLog()
        {
            while (true)
            {
                // Hiển thị hướng dẫn và chờ người dùng nhập dữ liệu
                AnsiConsole.MarkupLine("Nhập mã QR của sinh viên và nhấn [green]Enter[/] để ghi nhận hoặc chỉ nhấn [green]Enter[/] để thoát:\n");

                // Đọc phím nhấn từ người dùng
                var key = Console.ReadKey(intercept: true);

                // Nếu phím nhấn là Enter, thoát khỏi vòng lặp
                if (key.Key == ConsoleKey.Enter)
                {
                    AnsiConsole.MarkupLine("[green]Đã thoát khỏi chế độ ghi nhận lịch sử.[/]\n");
                    break;
                }

                // Nếu không phải phím Enter, yêu cầu nhập mã QR
                var input = AnsiConsole.Ask<string>("Nhập mã QR của sinh viên: ");

                // Chuyển đổi input thành số nguyên (ID sinh viên)
                if (int.TryParse(input, out int qrId))
                {
                    // Gọi phương thức TurnOn để ghi nhận log
                    bool isSuccess = cameraService.TurnOn(qrId);

                    // Thông báo kết quả với xuống dòng
                    if (isSuccess)
                    {
                        AnsiConsole.MarkupLine("[green]Lịch sử ra vào đã được ghi nhận thành công.[/]\n");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Lỗi khi ghi nhận lịch sử ra vào.[/]\n");
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Mã QR không hợp lệ. Vui lòng nhập lại.[/]\n");
                }
            }
        }


        public void ShowEntrylog_Table(List<Entrylog> entryLogs)
        {
            int pageSize = 15;
            int totalRecords = entryLogs.Count;
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            int currentPage = 1;

            while (true)
            {
                Console.Clear(); // Xóa màn hình trước khi hiển thị trang mới
                Console.WriteLine($"Trang {currentPage} / {totalPages}");

                // Tạo bảng và thêm các cột
                var table = new Table().Centered();
                table.Title($"[#ffff00]Danh sách học sinh ra vào[/]").HeavyEdgeBorder();
                table.AddColumn("ID học sinh");
                table.AddColumn("Tên học sinh");
                table.AddColumn("Lớp");
                table.AddColumn("Thời gian bản ghi");
                table.AddColumn("Trạng thái");

                // Tính toán các dòng cần hiển thị trên trang hiện tại
                var pageData = entryLogs.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

                // Thêm các hàng vào bảng
                foreach (var log in pageData)
                {
                    table.AddRow(
                        $"{log.StudentId}",
                        $"{log.Student.Name}",
                        $"{log.Student.Class}",
                        $"{log.LogTime:yyyy-MM-dd HH:mm:ss}",
                        $"{log.Status}"
                    );
                }

                // Hiển thị bảng
                AnsiConsole.Render(table);
                AnsiConsole.WriteLine();

                // Điều hướng người dùng
                if (totalPages == 1)
                {
                    Console.WriteLine("Nhấn [Esc] để thoát.");
                    Console.WriteLine();
                }
                else if (currentPage == 1 && currentPage < totalPages)
                {
                    Console.WriteLine("Nhấn [N] để xem trang tiếp theo, [Esc] để thoát.");
                    Console.WriteLine();
                }
                else if (currentPage > 1 && currentPage < totalPages)
                {
                    Console.WriteLine("Nhấn [P] để quay lại trang trước, [N] để xem trang tiếp theo, [Esc] để thoát.");
                    Console.WriteLine();
                }
                else if (currentPage > 1 && currentPage == totalPages)
                {
                    Console.WriteLine("Nhấn [P] để quay lại trang trước, [Esc] để thoát.");
                    Console.WriteLine();
                }

                // Nhận đầu vào từ người dùng để điều hướng
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.N && currentPage < totalPages)
                {
                    currentPage++;
                }
                else if (key.Key == ConsoleKey.P && currentPage > 1)
                {
                    currentPage--;
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    break;
                }
            }
        }


    }
}
