using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text_Based_RPG
{
    class Program
    {
        static void Main(string[] args)
        {
            GameManager gameManager = new GameManager();

            while (!gameManager.gameOver)
            {
                gameManager.GamePlay();
            }

            Console.WriteLine("Press any key to quit");
            Console.ReadKey(true);
        }
    }
}
