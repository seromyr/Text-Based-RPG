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
            bool exit = false;
            GameManager gameManager = new GameManager();
            gameManager.GameStart();

            //Game loop
            while (!exit)
            {
                switch (gameManager.gameState)
                {
                    case GameState.Main_Menu:
                        gameManager.GameMainMenu();
                        break;
                    case GameState.Gameplay:
                        gameManager.GameLoop();
                        break;
                    case GameState.Victory:
                        gameManager.GameEndVictorious();
                        break;
                    case GameState.Defeat:
                        gameManager.GameEndDefeated();
                        break;
                    case GameState.Quit:
                        exit = true;
                        break;
                }
            }

            Console.WriteLine("Press any key to quit");
            Console.ReadKey(true);
        }
    }
}
