using System;
using System.Text;

namespace UniversityIS
{
    /// <summary>Точка входа. Ключ --demo: загрузить демо-данные и вывести отчёт.</summary>
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            try { Console.InputEncoding = Encoding.UTF8; } catch (Exception) { }

            University university = new University("Воронежский государственный университет");
            ConsoleUI ui = new ConsoleUI(university, Console.In, Console.Out);

            if (args.Length > 0 && args[0] == "--demo")
            {
                ui.LoadDemo();
                ui.ShowReport();
                return;
            }
            ui.Run();
        }
    }
}
