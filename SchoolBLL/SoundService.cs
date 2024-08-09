using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using Spectre.Console;


namespace EntryLogManagement.SchoolBLL
{
    internal class SoundService
    {

        public void PlaySoundLog()
        {
            try
            {
                // Sử dụng AudioFileReader để đọc tệp âm thanh từ đường dẫn chỉ định.
                using (var audioFile = new AudioFileReader("C:\\Users\\khoav\\OneDrive\\Tài liệu\\ADO.NET\\SESSION\\Mail\\Alter.wav"))

                // WaveOutEvent được sử dụng để phát âm thanh.
                using (var outputDevice = new WaveOutEvent())
                {
                    // Khởi tạo thiết bị đầu ra với tệp âm thanh đã đọc.
                    outputDevice.Init(audioFile);

                    // Bắt đầu phát tệp âm thanh.
                    outputDevice.Play();

                    // Khởi tạo bộ đếm để theo dõi thời gian chờ người dùng nhập.
                    int IsCount = 0;

                    // Vòng lặp này tiếp tục trong khi âm thanh đang phát và bộ đếm nhỏ hơn hoặc bằng 10.
                    while (outputDevice.PlaybackState == PlaybackState.Playing && IsCount <= 10)
                    {
                        // Tăng bộ đếm mỗi lần vòng lặp thực hiện.
                        IsCount++;

                        // Chờ nhập liệu từ người dùng.
                        var key = Console.ReadLine();

                        // Thoát khỏi vòng lặp nếu người dùng nhập bất kỳ chuỗi nào không phải là "null".
                        if (key != "null")
                        {
                            break;
                        }
                    }

                    // Dừng phát âm thanh sau khi thoát khỏi vòng lặp.
                    outputDevice.Stop();
                }
            }
            // Bắt và xử lý bất kỳ ngoại lệ nào xảy ra trong quá trình phát.
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi sử dụng Spectre.Console.
                AnsiConsole.Markup("[red]Lỗi âm thanh :[/] " + ex.Message);
                AnsiConsole.WriteLine();
            }
        }

    }
}
