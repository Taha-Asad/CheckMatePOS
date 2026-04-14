using System;
using CheckMatePOS.BLL;

namespace CheckMatePOS.UI
{
    public class LoginScreen
    {
        private readonly AuthService authService;

        public LoginScreen(AuthService auth)
        {
            authService = auth;
        }

        public bool Show()
        {
            Console.WriteLine("         CheckMate POS Login        ");

            Console.Write("  Username: ");
            string username = Console.ReadLine() ?? "";

            Console.Write("  Password: ");
            string password = ReadPassword();

            return authService.Login(username, password);
        }

        private string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
                else if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }
    }
}