using System.Net.Mime;
using System.Text;
using console_spec.Models;
using Spectre.Console;

namespace console_spec;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
        GameManager gameManager = new GameManager();
        gameManager.Run();
    }
}


































