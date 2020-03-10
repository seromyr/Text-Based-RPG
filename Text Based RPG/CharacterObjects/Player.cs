using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text_Based_RPG
{
    class Player : CharacterObject
    {
        //Player visual boundary variables
        //public int[] boundaryTopX, boundaryTopY, boundaryBotX, boundaryBotY, boundaryLeftX, boundaryLeftY, boundaryRightX, boundaryRightY;

        public Player()
        {
            Health = 100;
            Shield = 100;
            Damage = 5;

            X = 1;
            Y = 1;
            PreviousX = X;
            PreviousY = Y;
            Speed = 1;

            PhysicalForm = new string[]
            {
                @"  Θ  ",
                @"┌─┼─┐",
                @" ┌┴┐ ",
                @" │ │ ",
            };

            NegativeForm = new string[]
            {
                ""+(char)32 + (char)32 + (char)32 + (char)32 + (char)32,
                ""+(char)32 + (char)32 + (char)32 + (char)32 + (char)32,
                ""+(char)32 + (char)32 + (char)32 + (char)32 + (char)32,
                ""+(char)32 + (char)32 + (char)32 + (char)32 + (char)32
            };

            DeadForm = new string[]
            {
                ""+(char)32
            };

            Name = "Abyssal Champion";
            Color = ConsoleColor.Red;

            Height = PhysicalForm.Length;
            Width = GetWidth();

            //Instantiate boundary coordinates
            //--------------------------------
            BoundaryTopX = new int[Width];
            BoundaryTopY = new int[Width];
            //--------------------------------
            BoundaryBotX = new int[Width];
            BoundaryBotY = new int[Width];
            //--------------------------------
            BoundaryLeftX = new int[Width];
            BoundaryLeftY = new int[Width];
            //--------------------------------
            BoundaryRightX = new int[Width];
            BoundaryRightY = new int[Width];
            //--------------------------------

            GetCurrentBoundaryCoordinates();
        }

        //For collision checking with everything on the map
        public void GetCurrentBoundaryCoordinates()
        {
            //TOP
            for (int i = 0; i < Width; i++)
            {
                BoundaryTopX[i] = X + i;
                BoundaryTopY[i] = Y - 1;
                //Draw boundary
                //Console.SetCursorPosition(boundaryTopX[i], boundaryTopY[i]);
                //Console.Write(".");
            }

            //DOWN
            for (int i = 0; i < Width; i++)
            {
                BoundaryBotX[i] = X + i;
                BoundaryBotY[i] = Y + Height;

                //Draw boundary
                //Console.SetCursorPosition(boundaryBotX[i], boundaryBotY[i]);
                //Console.Write(".");
            }

            //LEFT
            for (int i = 1; i < Width; i++)
            {
                BoundaryLeftX[i] = X - 1;
                BoundaryLeftY[i] = Y + i - 1;

                //Draw boundary
                //Console.SetCursorPosition(boundaryLeftX[i], boundaryLeftY[i]);
                //Console.Write(".");
            }

            //RIGHT
            for (int i = 1; i < Width; i++)
            {
                BoundaryRightX[i] = X + Width;
                BoundaryRightY[i] = Y + i - 1;

                //Draw boundary
                //Console.SetCursorPosition(boundaryRightX[i], boundaryRightY[i]);
                //Console.Write(".");
            }
        }
    }
}
