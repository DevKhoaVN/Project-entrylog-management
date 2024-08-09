using EntryLogManagement.SchoolBLL;
using EntryLogManagement.SchoolDAL;
using EntryLogManagement.SchoolPL.Utility;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMySql.Models;

namespace EntryLogManagement.SchoolPL
{
    internal class UserPL
    {
        private readonly UserService userService;
        private readonly HandleLogRepository handleLogRepository;
       

        public UserPL()
        {
            userService = new UserService();
            handleLogRepository = new HandleLogRepository();
          

        }

        public User Login()
        {
            while (true)
            {
                string UserName = InputHepler.PromptUserInput("Nhập [green]UserName: [/]");
                string Password = InputHepler.PromptUserInput("Nhập [green]Password: [/]");
                Console.WriteLine();

                var reuslt =  userService.LoginUser(UserName, Password);

                if (reuslt != null)
                {
                    AnsiConsole.MarkupLine("[green]Bạn đã đăng nhập thành công[/]");
                    AnsiConsole.WriteLine();
                    return reuslt;
                    break;
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Bạn đã nhập sai tài khoản hoặc mật khẩu[/]");
                    Console.WriteLine();
                }
                
            }
                
            
        }

        public  void Register()
        {
            while(true)
            {
                re_enter:
                string UserName = InputHepler.PromptUserInput("Nhập [green]UserName: [/]");

                if (!handleLogRepository.HandleUserName(UserName))
                {
                    AnsiConsole.Markup("[red]UserName đã tồn tại[/]");
                    Console.WriteLine();
                    goto re_enter;
                }
                string Password = InputHepler.PromptUserInput("Nhập [green]Password: [/]");
                re_enter2:
                int ID = InputHepler.GetIntPrompt("Nhập [green]ID của bạn : [/]");
               

                var result = userService.RegisterUser(UserName, Password, ID);
                if (result)
                {
                    Console.WriteLine();
                    AnsiConsole.MarkupLine("[green]Bạn đã đăng kí thành công[/]");
                    Console.WriteLine();
                    break;
                }
                else
                {
                    goto re_enter2;
                } 
                Console.WriteLine();
                
            }
           
           
        }
    }
}
