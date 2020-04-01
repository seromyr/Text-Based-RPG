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

            gameManager.GameStart();

            //Game loop
            while (!gameManager.gameOver)
            {
                switch (gameManager.gameState)
                {
                    case GameState.Main_Menu:
                        gameManager.GameMainMenu();
                        break;
                    case GameState.Gameplay:
                        gameManager.RunLevelOne();
                        gameManager.RunLeveTwo();
                        gameManager.RunLevelThree();
                        gameManager.GameEnd();
                        break;
                    case GameState.Game_Over:
                        break;
                }
            }

            Console.WriteLine("Press any key to quit");
            Console.ReadKey(true);
        }
    }
}
