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
    internal class ParentPL
    {
        private readonly ParentService parentService;
        private readonly AbsentreportService absentreportService;
        

        public ParentPL()
        {
            parentService = new ParentService();
            absentreportService = new AbsentreportService();
        }

        public void ShowLogStudentAll(int id)
        {
             var logforparent = parentService.ShowLogStudentALL(id);
            ShowEntrylog_Table(logforparent);
        }

        public void ShowLogStudentRangeTime(int id )
        {
            DateTime timeStart = InputHepler.GetDate("Nhập[green] ngày bắt đầu(dd/mm/yyyy): [/]");
            DateTime timeEnd = InputHepler.GetDate("Nhập[green] ngày kết thúc(dd/mm/yyyy): [/]");

            var logforparent = parentService.ShowLogStudentRangeTime(timeStart, timeEnd, id);

            ShowEntrylog_Table(logforparent);
        }

        public void SendReport(int id)
        {
            var message = InputHepler.PromptUserInput("Nhập[green] lí do vắng học: [/]");

            var result = parentService.SendAbentReport(message, id);

            if (result)
            {
                Console.WriteLine();
                AnsiConsole.MarkupLine("[green]Gửi báo cáo vắng học thành công.[/]");
                AnsiConsole.WriteLine();
            }
            else
            {
                Console.WriteLine();
                AnsiConsole.MarkupLine("[green]Gửi báo cáo vắng học thất bại.[/]");
                AnsiConsole.WriteLine();
            }
        }
        public void ShowAbsentStudentAll(int id )
        {
            var absentforparent = absentreportService.GetReportIDParent(id);

            ShowAbsentReport_Table(absentforparent);
        }


        /*public void ShowAbsentStudentRangeTime(int id)
        {
            DateTime timeStart = validateService.GetDate("Nhập[green] ngày bắt đầu(dd/mm/yyyy): [/]");
            DateTime timeEnd = validateService.GetDate("Nhập[green] ngày kết thúc(dd/mm/yyyy): [/]");

            var absentforparent = absentreportService.GetReportRangeTimeForParent(timeStart, timeEnd, id);

            ShowAbsentReport_Table(absentforparent);
        }*/

        // show bảng entrylog
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
                }
                else if (currentPage == 1 && currentPage < totalPages)
                {
                    Console.WriteLine("Nhấn [N] để xem trang tiếp theo, [Esc] để thoát.");
                }
                else if (currentPage > 1 && currentPage < totalPages)
                {
                    Console.WriteLine("Nhấn [P] để quay lại trang trước, [N] để xem trang tiếp theo, [Esc] để thoát.");
                }
                else if (currentPage > 1 && currentPage == totalPages)
                {
                    Console.WriteLine("Nhấn [P] để quay lại trang trước, [Esc] để thoát.");
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


        // Show bảng absentreport
        public void ShowAbsentReport_Table(List<Absentreport> data)
        {
            int pageSize = 15;
            int totalRecords = data.Count;
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            int currentPage = 1;

            while (true)
            {
                Console.Clear(); // Xóa màn hình trước khi hiển thị trang mới
                Console.WriteLine($"Trang {currentPage} / {totalPages}");

                // Tạo bảng và thêm các cột
                var table = new Table().Expand();
                table.Title($"[#ffff00]Bảng báo cáo vắng học[/]");
                table.AddColumn("ID học sinh");
                table.AddColumn("Tên phụ huynh");
                table.AddColumn("Lớp");
                table.AddColumn("Ngày báo cáo");
                table.AddColumn("Lý do");

                // Tính toán các dòng cần hiển thị trên trang hiện tại
                var pageData = data.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

                // Thêm các hàng vào bảng
                foreach (var report in pageData)
                {
                    table.AddRow(
                        $"{report.StudentId}",
                        $"{report.Parent.ParentName}",
                        $"{report.Parent.Students.Class}",
                        $"{report.CreateDay:yyyy-MM-dd}",
                        $"{report.Reason}"
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
