using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text_Based_RPG
{
    class InputManager
    {
        //Left - Right - Up - Down directions
        private int[] direction = { 1, 2, 3, 4 };
        //private int currentSelection;
        public bool keyPressed;

        private bool validInput;
        private ConsoleKey[] knownGamelayCMD =
        {
                //Locomotion
                ConsoleKey.W,       ConsoleKey.S,           ConsoleKey.A,           ConsoleKey.D,
                ConsoleKey.UpArrow, ConsoleKey.DownArrow,   ConsoleKey.LeftArrow,   ConsoleKey.RightArrow,

                //Interaction
                ConsoleKey.Spacebar, ConsoleKey.Enter, ConsoleKey.Escape
        };

        private ConsoleKey[] knownMenuCMD =
        {
                //Locomotion
                ConsoleKey.W,       ConsoleKey.S,           
                ConsoleKey.UpArrow, ConsoleKey.DownArrow,

                //Interaction
                ConsoleKey.Enter, ConsoleKey.Escape
        };

        private ConsoleKeyInfo key;

        //Constructor
        public InputManager()
        {
            validInput = false;
            keyPressed = false;
            //currentSelection = 0;
        }

        //Locomotion Input listener and direction returner
        public int InputListener_Movement(int x, int y)
        {
            keyPressed = false;
            int inputDirection = 0;

            while (!validInput)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(x, y);
                Console.Write("Press WASD or Arrow Keys to move ");

                //Record input and compare to known database
                key = Console.ReadKey(true);
                validInput = knownGamelayCMD.Contains(key.Key);

                //If input is not on the list then ask to input again
                if (!validInput)
                {
                    Console.SetCursorPosition(x, y + 1);
                    Console.Write("Invalid input!");
                }

                keyPressed = true;
            }

            switch (key.Key)
            {
                case ConsoleKey.A:
                    inputDirection = direction[0];
                    break;
                case ConsoleKey.D:
                    inputDirection = direction[1];
                    break;
                case ConsoleKey.W:
                    inputDirection = direction[2];
                    break;
                case ConsoleKey.S:
                    inputDirection = direction[3];
                    break;
                case ConsoleKey.LeftArrow:
                    inputDirection = direction[0];
                    break;
                case ConsoleKey.RightArrow:
                    inputDirection = direction[1];
                    break;
                case ConsoleKey.UpArrow:
                    inputDirection = direction[2];
                    break;
                case ConsoleKey.DownArrow:
                    inputDirection = direction[3];
                    break;
            }
            validInput = false;
            return inputDirection;
        }

        public int InputListener_Menu(int currentSelection)
        {
            keyPressed = false;
            
            while (!validInput)
            {
                //Record input and compare to known database
                key = Console.ReadKey(true);
                validInput = knownMenuCMD.Contains(key.Key);

                keyPressed = true;
            }

            switch (key.Key)
            {
                case ConsoleKey.W:
                    currentSelection--;
                    if (currentSelection < 0) currentSelection = 3;
                    break;
                case ConsoleKey.S:
                    currentSelection++;
                    if (currentSelection > 3) currentSelection = 0;
                    break;
                case ConsoleKey.UpArrow:
                    currentSelection--;
                    if (currentSelection < 0) currentSelection = 3;
                    break;
                case ConsoleKey.DownArrow:
                    currentSelection++;
                    if (currentSelection > 3) currentSelection = 0;
                    break;
                case ConsoleKey.Enter:
                    currentSelection = -1;
                    break;
                case ConsoleKey.Escape:
                    currentSelection = -2;
                    break;
            }
            validInput = false;
            return currentSelection;
        }
    }
}
