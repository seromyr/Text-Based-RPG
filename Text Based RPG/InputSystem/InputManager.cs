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
        private int inputDirection;
        public int x, y;
        public bool keyPressed;

        private bool validInput;
        private ConsoleKey[] knownCMD =
        {
                //Locomotion
                ConsoleKey.W,       ConsoleKey.S,           ConsoleKey.A,           ConsoleKey.D,
                ConsoleKey.UpArrow, ConsoleKey.DownArrow,   ConsoleKey.LeftArrow,   ConsoleKey.RightArrow,

                //Interation
                ConsoleKey.Spacebar
        };

        private ConsoleKeyInfo key;

        //Constructor
        public InputManager()
        {
            validInput = false;
            keyPressed = false;
        }

        //Locomotion Input listener and direction returner
        public int InputListener()
        {
            keyPressed = false;

            while (!validInput)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(x, y);
                Console.Write("Press WASD or Arrow Keys to move ");

                //Record input and compare to known database
                key = Console.ReadKey(true);
                validInput = knownCMD.Contains(key.Key);

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
    }
}
