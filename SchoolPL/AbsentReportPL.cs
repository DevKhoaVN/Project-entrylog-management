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
    internal class AbsentReportPL
    {
        private readonly AbsentreportService absentreportService;

        public AbsentReportPL()
        {
            absentreportService = new AbsentreportService();
        }

        public  void ShowAbsenReportID()
        {
            while (true)
            {
                int id = InputHepler.GetIntPrompt("Nhập [green]ID tìm kiếm: [/]");

                var absent = absentreportService.GetReportID(id);

                if (absent.Count > 0)
                {
                    ShowAbsentReport_Table(absent);
                    Console.WriteLine();
                    break; // Exit the loop once a valid report is found
                }
                else
                {
                    Console.WriteLine("[red]Không có báo cáo nào với ID đã nhập. Vui lòng nhập lại.[/]");
                    Console.WriteLine();
                }
            }
        }

        public void ShowAbsenReportAll()
        {

            var absent = absentreportService.GetReportAll();

            ShowAbsentReport_Table(absent);
        }

        public void ShowAbsenReportRangeTime()
        {
            while (true)
            {
                DateTime timeStart = InputHepler.GetDate("Nhập [green]ngày bắt đầu (dd/mm/yyyy): [/]");
                DateTime timeEnd = InputHepler.GetDate("Nhập [green]ngày kết thúc (dd/mm/yyyy): [/]");

                
                var absent = absentreportService.GetReportRangeTime(timeStart, timeEnd);

                if (absent.Count > 0)
                {
                    ShowAbsentReport_Table(absent);
                    break; // Exit the loop once valid reports are found
                }
                else
                {
                    Console.WriteLine("[red]Không có báo cáo vắng mặt nào trong khoảng thời gian này. Vui lòng nhập lại.[/]");
                }
            }
        }

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
                var table = new Table().Expand().Centered();
                table.Title($"[#ffff00]Bảng báo cáo vắng học[/]");
                table.AddColumn("ID học sinh");
                table.AddColumn("Tên học sinh");
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
                        $"{report.Parent.Students.StudentId}",
                        $"{report.Parent.Students.Name}",
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


    }
}
