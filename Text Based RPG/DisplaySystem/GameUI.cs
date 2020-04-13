using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using Text_Based_RPG.CharacterObjects;

namespace Text_Based_RPG.DisplaySystem
{
    class GameUI : DisplayManager
    {
        private string[] pressAnyKey         , mainMenuHeader,
                         pressAnykey_Negative, mainMenuHeader_Negative,
                         newGame             , loadGame               , quitGame         , credits         , gameOverWin         , gameOverLose         ,
                         newGame_Negative    , loadGame_Negative      , quitGame_Negative, credits_Negative, gameOverWin_Negative, gameOverLose_Negative;

        private string newGame_Normal, loadGame_Normal, quitGame_Normal, credits_Normal;

        private int inputValue, currentSelection;

        //Constructor
        public GameUI()
        {
            //Read visual elements from files
            pressAnyKey = File.ReadAllLines(@".\Visual Data\UI\pressanykey.char", Encoding.ASCII);
            pressAnykey_Negative = new string[pressAnyKey.Length];
            GetPositiveAndNegativeVisual(pressAnyKey, pressAnykey_Negative);

            mainMenuHeader = File.ReadAllLines(@".\Visual Data\UI\mainmenuheader.char", Encoding.ASCII);
            mainMenuHeader_Negative = new string[mainMenuHeader.Length];
            GetPositiveAndNegativeVisual(mainMenuHeader, mainMenuHeader_Negative);

            newGame = File.ReadAllLines(@".\Visual Data\UI\newgame.char", Encoding.ASCII);
            newGame_Negative = new string[newGame.Length];
            GetPositiveAndNegativeVisual(newGame, newGame_Negative);
            newGame_Normal = "NEW GAME";

            loadGame = File.ReadAllLines(@".\Visual Data\UI\continue.char", Encoding.ASCII);
            loadGame_Negative = new string[loadGame.Length];
            GetPositiveAndNegativeVisual(loadGame, loadGame_Negative);
            loadGame_Normal = "CONTINUE";

            quitGame = File.ReadAllLines(@".\Visual Data\UI\exitgame.char", Encoding.ASCII);
            quitGame_Negative = new string[quitGame.Length];
            GetPositiveAndNegativeVisual(quitGame, quitGame_Negative);
            quitGame_Normal = "QUIT GAME";

            credits = File.ReadAllLines(@".\Visual Data\UI\credits.char", Encoding.ASCII);
            credits_Negative = new string[credits.Length];
            GetPositiveAndNegativeVisual(credits, credits_Negative);
            credits_Normal = "CREDITS";

            gameOverWin = File.ReadAllLines(@".\Visual Data\UI\gameover_win.char", Encoding.ASCII);
            gameOverWin_Negative = new string[gameOverWin.Length];
            GetPositiveAndNegativeVisual(gameOverWin, gameOverWin_Negative);

            gameOverLose = File.ReadAllLines(@".\Visual Data\UI\gameover_lose.char", Encoding.ASCII);
            gameOverLose_Negative = new string[gameOverLose.Length];
            GetPositiveAndNegativeVisual(gameOverLose, gameOverLose_Negative);

            //Menu default function
            inputValue = 0;
            currentSelection = 0;
        }

        public void DrawTransitionEffect_Dissolve(char a, char b)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            char[,] screenMatrix = new char[120, 29];
            for (int i = 0; i < screenMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < screenMatrix.GetLength(1); j++)
                {
                    screenMatrix[i, j] = a;
                    Console.SetCursorPosition(i, j);
                }
            }

            for (int i = 0; i < screenMatrix.Length; i++)
            {
                int x = GameLogic.GenerateRandomBetween(0, screenMatrix.GetLength(0));
                int y = GameLogic.GenerateRandomBetween(0, screenMatrix.GetLength(1));

                if (screenMatrix[x, y] == a)
                {
                    screenMatrix[x, y] = b;
                    Console.SetCursorPosition(x, y);
                    Console.Write(screenMatrix[x, y]);
                }
                else i--;
            }
        }

        public void DrawGameplayCanvas()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            DrawRectangle(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT, Constant.GAMEPLAY_CANVAS_LIMIT_UP, Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - Constant.GAMEPLAY_CANVAS_LIMIT_LEFT, Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - Constant.GAMEPLAY_CANVAS_LIMIT_UP);
        }

        public void ShowStatsHUD(int x, int y, GameCharacter character)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(x, y);
            Console.WriteLine(" Name    {0}", character.Name);
            Console.SetCursorPosition(x, y + 1);
            Console.WriteLine(" Health  [                    ] {0} ", character.CurrentHealth);

            Console.SetCursorPosition(x + 10, y + 1);
            for (int i = 0; i < 20; i++) Console.Write("-");

            //Avoid divided by zero
            if (character.Health != 0)
            {
                Console.SetCursorPosition(x + 10, y + 1);
                for (int j = 0; j < (character.CurrentHealth * 20 / character.Health); j++) Console.Write("▒");
            }

            Console.SetCursorPosition(x, y + 2);
            Console.WriteLine(" Shield  [                    ] {0} ", character.CurrentShield);

            Console.SetCursorPosition(x + 10, y + 2);
            for (int i = 0; i < 20; i++) Console.Write("-");

            //Avoid divided by zero
            if (character.Shield != 0)
            {
                Console.SetCursorPosition(x + 10, y + 2);
                for (int j = 0; j < (character.CurrentShield * 20 / character.Shield); j++) Console.Write("■");
            }

            Console.SetCursorPosition(x, y + 3);
            Console.WriteLine(" Damage   {0}", character.Damage);

            Console.SetCursorPosition(x, y + 4);
            Console.WriteLine(" Live     {0}", character.CurrentLive);
        }

        public void DrawSplashScreen()
        {
            while (!Console.KeyAvailable)
            {
                for (int i = 0; i < pressAnyKey.Length; i++)
                {
                    Console.SetCursorPosition(25, i + 10);
                    Console.Write(pressAnyKey[i]);
                }
                Thread.Sleep(500);

                for (int j = 0; j < pressAnyKey.Length; j++)
                {
                    Console.SetCursorPosition(25, j + 10);
                    Console.Write(pressAnykey_Negative[j]);
                }
                Thread.Sleep(500);
            }

            Console.ReadKey(true);

            for (int i = 0; i < pressAnyKey.Length; i++)
            {
                Console.SetCursorPosition(25, i + 10);
                Console.Write(pressAnyKey[i]);
            }
            Thread.Sleep(650);

            for (int j = 0; j < pressAnyKey.Length; j++)
            {
                Console.SetCursorPosition(25, j + 10);
                Console.Write(pressAnykey_Negative[j]);
                Thread.Sleep(100);
            }
        }

        public void DrawGameOverScreen()
        {
            while (!Console.KeyAvailable)
            {
                for (int i = 0; i < gameOverWin.Length; i++)
                {
                    Console.SetCursorPosition(25, i + 10);
                    Console.Write(gameOverWin[i]);
                }
                Thread.Sleep(500);

                for (int j = 0; j < gameOverWin.Length; j++)
                {
                    Console.SetCursorPosition(25, j + 10);
                    Console.Write(gameOverWin_Negative[j]);
                }
                Thread.Sleep(500);
            }

            Console.ReadKey(true);

            for (int i = 0; i < gameOverWin.Length; i++)
            {
                Console.SetCursorPosition(25, i + 10);
                Console.Write(gameOverWin[i]);
            }
            Thread.Sleep(650);

            for (int j = 0; j < gameOverWin.Length; j++)
            {
                Console.SetCursorPosition(25, j + 10);
                Console.Write(gameOverWin_Negative[j]);
                Thread.Sleep(100);
            }
        }

        public void DrawDefeatedScreen()
        {
            while (!Console.KeyAvailable)
            {
                for (int i = 0; i < gameOverLose.Length; i++)
                {
                    Console.SetCursorPosition(25, i + 10);
                    Console.Write(gameOverLose[i]);
                }
                Thread.Sleep(500);

                for (int j = 0; j < gameOverLose.Length; j++)
                {
                    Console.SetCursorPosition(25, j + 10);
                    Console.Write(gameOverLose_Negative[j]);
                }
                Thread.Sleep(500);
            }

            Console.ReadKey(true);

            for (int i = 0; i < gameOverLose.Length; i++)
            {
                Console.SetCursorPosition(25, i + 10);
                Console.Write(gameOverLose[i]);
            }
            Thread.Sleep(650);

            for (int j = 0; j < gameOverLose.Length; j++)
            {
                Console.SetCursorPosition(25, j + 10);
                Console.Write(gameOverLose_Negative[j]);
                Thread.Sleep(100);
            }
        }

        public void DrawMainMenuScreen()
        {
            DrawTransitionEffect_Dissolve(' ', '█');
            DrawTransitionEffect_Dissolve('█', ' ');
            Console.ForegroundColor = ConsoleColor.White;
            //---------------------------------------------- draw header
            for (int i = 0; i < mainMenuHeader.Length; i++)
            {
                Console.SetCursorPosition(12, i + 1);
                Console.Write(mainMenuHeader_Negative[i]);
            }
            Thread.Sleep(650);

            for (int j = 0; j < mainMenuHeader.Length; j++)
            {
                Console.SetCursorPosition(12, j + 1);
                Console.Write(mainMenuHeader[j]);
                Thread.Sleep(100);
            }
            //---------------------------------------------- draw middle text
            Console.SetCursorPosition(20, mainMenuHeader.Length + 2);
            Console.Write("A roguelike textbased RPG by BUU NGUYEN - version 0.5b - Built on April 12th, 2020");
        }

        public void DrawMainMenu()
        {
            DrawAnimatedTextboxIn(14, 10, 92, 18);
            MainMenuFunction();
            Console.Clear();
            DrawAnimatedTextboxOut(14, 10, 92, 18);

            DrawTransitionEffect_Dissolve(' ', '█');
            DrawTransitionEffect_Dissolve('█', ' ');
        }

        private void MainMenuFunction()
        {
            InputManager inputMainMenu = new InputManager();            
            Console.ForegroundColor = ConsoleColor.White;

            int[,] mainMenuPosition = new int[4, 2];

            bool exitMainMenu = false;
            string currentMainMenuHint = "Use arrow keys to navigate, enter to confirm";

            for (int i = 0; i < mainMenuPosition.GetLength(0); i++)
            {
                mainMenuPosition[i, 0] = 25;
                mainMenuPosition[i, 1] = 14 + i;
            }

            //Default menu selection
            mainMenuPosition[0, 0] = 25;
            mainMenuPosition[0, 1] = 14;

            DrawObjectAt(mainMenuPosition[0, 0], mainMenuPosition[0, 1], newGame, ConsoleColor.Yellow);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            WriteTextAt(mainMenuPosition[0, 0], mainMenuPosition[1, 1] + newGame.GetLength(0), loadGame_Normal);
            WriteTextAt(mainMenuPosition[0, 0], mainMenuPosition[1, 1] + newGame.GetLength(0) + 1, quitGame_Normal);
            WriteTextAt(mainMenuPosition[0, 0], mainMenuPosition[1, 1] + newGame.GetLength(0) + 2, credits_Normal);
            Console.ForegroundColor = ConsoleColor.Gray;
            WriteTextAt(25, 25, currentMainMenuHint);

            while (exitMainMenu == false)
            {
                inputValue = inputMainMenu.InputListener_Menu(inputValue);

                switch (inputValue)
                {
                    case 0:
                        DrawObjectAt(mainMenuPosition[1, 0], mainMenuPosition[1, 1], loadGame_Negative, ConsoleColor.Black);
                        DrawObjectAt(mainMenuPosition[3, 0], mainMenuPosition[3, 1], credits_Negative, ConsoleColor.Black);

                        DrawObjectAt(mainMenuPosition[0, 0], mainMenuPosition[0, 1], newGame, ConsoleColor.Yellow);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        WriteTextAt(mainMenuPosition[0, 0], mainMenuPosition[1, 1] + newGame.GetLength(0), loadGame_Normal);
                        WriteTextAt(mainMenuPosition[0, 0], mainMenuPosition[1, 1] + newGame.GetLength(0) + 1, quitGame_Normal);
                        WriteTextAt(mainMenuPosition[0, 0], mainMenuPosition[1, 1] + newGame.GetLength(0) + 2, credits_Normal);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        ClearTextAt(25, 25, currentMainMenuHint);
                        currentMainMenuHint = "Start a new game";
                        WriteTextAt(25, 25, currentMainMenuHint);
                        currentSelection = (int)MainMenu.New;
                        break;
                    case 1:
                        DrawObjectAt(mainMenuPosition[0, 0], mainMenuPosition[0, 1], newGame_Negative, ConsoleColor.Black);
                        DrawObjectAt(mainMenuPosition[2, 0], mainMenuPosition[2, 1], quitGame_Negative, ConsoleColor.Black);

                        DrawObjectAt(mainMenuPosition[1, 0], mainMenuPosition[1, 1], loadGame, ConsoleColor.Yellow);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        WriteTextAt(mainMenuPosition[0, 0], mainMenuPosition[2, 1] + loadGame.GetLength(0), quitGame_Normal);
                        WriteTextAt(mainMenuPosition[0, 0], mainMenuPosition[2, 1] + loadGame.GetLength(0) + 1, credits_Normal);
                        WriteTextAt(mainMenuPosition[0, 0], mainMenuPosition[0, 1], newGame_Normal);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        ClearTextAt(25, 25, currentMainMenuHint);
                        currentMainMenuHint = "Load a previously saved game";
                        WriteTextAt(25, 25, currentMainMenuHint);
                        currentSelection = (int)MainMenu.Continue;
                        break;
                    case 2:
                        DrawObjectAt(mainMenuPosition[1, 0], mainMenuPosition[1, 1], loadGame_Negative, ConsoleColor.Black);
                        DrawObjectAt(mainMenuPosition[3, 0], mainMenuPosition[3, 1], credits_Negative, ConsoleColor.Black);

                        DrawObjectAt(mainMenuPosition[2, 0], mainMenuPosition[2, 1], quitGame, ConsoleColor.Yellow);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        WriteTextAt(mainMenuPosition[0, 0], mainMenuPosition[3, 1] + quitGame.GetLength(0), credits_Normal);
                        WriteTextAt(mainMenuPosition[0, 0], mainMenuPosition[0, 1], newGame_Normal);
                        WriteTextAt(mainMenuPosition[0, 0], mainMenuPosition[1, 1], loadGame_Normal);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        ClearTextAt(25, 25, currentMainMenuHint);
                        currentMainMenuHint = "Exit the program";
                        WriteTextAt(25, 25, currentMainMenuHint);
                        currentSelection = (int)MainMenu.Quit;
                        break;
                    case 3:
                        DrawObjectAt(mainMenuPosition[0, 0], mainMenuPosition[0, 1], newGame_Negative, ConsoleColor.Black);
                        DrawObjectAt(mainMenuPosition[2, 0], mainMenuPosition[2, 1], quitGame_Negative, ConsoleColor.Black);

                        DrawObjectAt(mainMenuPosition[3, 0], mainMenuPosition[3, 1], credits, ConsoleColor.Yellow);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        WriteTextAt(mainMenuPosition[0, 0], mainMenuPosition[0, 1], newGame_Normal);
                        WriteTextAt(mainMenuPosition[0, 0], mainMenuPosition[1, 1], loadGame_Normal);
                        WriteTextAt(mainMenuPosition[0, 0], mainMenuPosition[2, 1], quitGame_Normal);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        ClearTextAt(mainMenuPosition[0, 0], mainMenuPosition[1, 1] + newGame.GetLength(0) + 2, credits_Normal);
                        ClearTextAt(25, 25, currentMainMenuHint);
                        currentMainMenuHint = "View credits";
                        WriteTextAt(25, 25, currentMainMenuHint);
                        currentSelection = (int)MainMenu.Credits;
                        break;
                }


                if (inputValue == -1 && currentSelection == (int)MainMenu.New)
                {
                    exitMainMenu = true;
                }
                else if (inputValue == -1 && currentSelection == (int)MainMenu.Continue)
                {
                    DrawConfirmationBox(mainMenuPosition[1, 0] + loadGame[0].Length, mainMenuPosition[1, 1], 20, loadGame.GetLength(0) + 1, "Work in progress", 3);
                    inputValue = 1;
                }
                else if (inputValue == -1 && currentSelection == (int)MainMenu.Quit)
                {
                    DrawConfirmationBox(mainMenuPosition[2, 0] + quitGame[0].Length, mainMenuPosition[2, 1], 20, quitGame.GetLength(0) + 1, "Work in progress", 3);
                    inputValue = 2;
                }
                else if (inputValue == -1 && currentSelection == (int)MainMenu.Credits)
                {
                    DrawConfirmationBox(mainMenuPosition[3, 0] + credits[0].Length, mainMenuPosition[3, 1], 20, credits.GetLength(0) + 1, "Work in progress", 3);
                    inputValue = 3;
                }
            }
        }

        public void DrawConfirmationBox(int x, int y, int l, int h, string message, int line2 = 3)
        {            
            for (int i = 0; i < l; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    Console.SetCursorPosition(x + i, y + j);
                    Console.Write(' ');
                }
            }

            DrawAnimatedTextboxIn(x, y, l, h);

            Console.ForegroundColor = ConsoleColor.White;
            WriteTextAt(x + 2, y + 1, message);
            WriteTextAt(x + line2, y + 2, "Hit Enter to continue");

            while (Console.ReadKey(true).Key != ConsoleKey.Enter);

            Console.SetCursorPosition(x + 2, y + 1);
            foreach (char c in message)
            {
                Console.Write(' ');
            }
            Console.SetCursorPosition(x + line2, y + 2);
            foreach (char c in message)
            {
                Console.Write(' ');
            }

            DrawAnimatedTextboxOut(x, y, l, h);
        }
    }
}
