using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Text_Based_RPG.Behaviors;

namespace Text_Based_RPG
{
    class Enemy : CharacterObject
    {
        private IMoveBehavior moveBehavior;

        //Enemy visual boundary variables
        //public int[] boundaryTopX, boundaryTopY, boundaryBotX, boundaryBotY, boundaryLeftX, boundaryLeftY, boundaryRightX, boundaryRightY;

        public Enemy(IMoveBehavior mB)
        {
            moveBehavior = mB;

            Health = 30;
            CurrentHealth = 30;
            Shield = 100;
            CurrentShield = 0;

            X = 1;
            Y = 1;
            PreviousX = X;
            PreviousY = Y;
            Speed = 1;
            Damage = 3;

            PhysicalForm = new string[]
            {
                @")/  ",
                @"Y\_/",
                @" /~\",
            };

            NegativeForm = new string[]
            {
                ""+(char)32 + (char)32 + (char)32 + (char)32,
                ""+(char)32 + (char)32 + (char)32 + (char)32,
                ""+(char)32 + (char)32 + (char)32 + (char)32
            };

            DeadForm = new string[]
            {
                ""+(char)32
            };

            Name = "Enemy";
            Color = ConsoleColor.Cyan;

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

        public void MoveTowards(Player player)
        {
            //Measuring distance between player and enemy
            int horizontalDistance = X - player.X;

            PreviousX = X;
            PreviousY = Y;

            //In case enemy is on player's right side
            if (X > player.X + player.Width)
            {
                X -= Speed;
            }

            //In case enemy is on player's left side
            else if (X + Width < player.X)
            {
                X += Speed;
            }

            //In case enemy is above player
            if (Y + Height < player.Y)
            {
                Y += Speed;
            }

            //In case enemy is below player
            else if (Y > player.Y + player.Height)
            {
                Y -= Speed;
            }

            //moveBehavior.Move(player); ???

            //if (X           == player.X + player.Width  ||
            //    X + Width   == player.X                 ||
            //    Y + Height  == player.Y                 ||
            //    Y           == player.Y + player.Height)
            //{
            //    AttackPermission = true;
            //}
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
