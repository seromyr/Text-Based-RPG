using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Text_Based_RPG.Behaviors;

namespace Text_Based_RPG.CharacterObjects
{
    class Enemy : GameCharacter
    {
        private IMoveBehavior moveBehavior;

        public Enemy(IMoveBehavior mB, EnemyType type)
        {
            moveBehavior = mB;           

            switch (type)
            {
                case EnemyType.Random_Robot:
                    // This random code sucks, I will try to improve it next version //
                    int     r = GameLogic.random.Next(0, 3);                         //
                    if      (r == 0) CreateCommonRobot();                            //
                    else if (r == 1) CreateEliteRobot();                             //
                    else    CreateGiantRobot();                                      //
                    //---------------------------------------------------------------//
                    break;
                case EnemyType.Common_Robot:
                    CreateCommonRobot();
                    break;
                case EnemyType.Elite_Robot:
                    CreateEliteRobot();
                    break;
                case EnemyType.Giant_Robot:
                    CreateGiantRobot();
                    break;
                case EnemyType.Boss:
                    CreateBoss();
                    break;
            }

            X = 1;
            Y = 1;
            PreviousX = X;
            PreviousY = Y;

            Height = PhysicalForm.Length;
            Width = GetWidth();

            //Instantiate boundary coordinates
            //------------------------------//
            BoundaryTopX   = new int[Width];
            BoundaryTopY   = new int[Width];
            //------------------------------//
            BoundaryBotX   = new int[Width];
            BoundaryBotY   = new int[Width];
            //------------------------------//
            BoundaryLeftX  = new int[Width];
            BoundaryLeftY  = new int[Width];
            //------------------------------//
            BoundaryRightX = new int[Width];
            BoundaryRightY = new int[Width];
            //------------------------------//
            GetCurrentBoundaryCoordinates();
        }

        private void CreateCommonRobot()
        {
            //Combat data
            Health = 30;
            CurrentHealth = 30;
            Shield = 0;
            CurrentShield = 0;
            ShieldRegenerationAllowed = false;
            Damage = 10;
            Speed = 1;
            Live = 0;
            CurrentLive = Live;

            //Visual data
            Name = "Robot ";
            GetBattleForms(@".\Visual Data\Characters\Enemy_00");
            SetMapForms('∩');
            Color = ConsoleColor.Cyan;
        }

        private void CreateEliteRobot()
        {
            //Combat data
            Health = 50;
            CurrentHealth = 50;
            Shield = 10;
            CurrentShield = 10;
            ShieldRegenerationAllowed = false;
            Damage = 15;
            Speed = 2;
            Live = 0;
            CurrentLive = Live;

            //Visual data
            Name = "Elite Robot ";
            GetBattleForms(@".\Visual Data\Characters\Enemy_01");
            SetMapForms('Ü');
            Color = ConsoleColor.DarkBlue;
        }

        private void CreateGiantRobot()
        {
            //Combat data
            Health = 100;
            CurrentHealth = 100;
            Shield = 50;
            CurrentShield = 50;
            ShieldRegenerationAllowed = false;
            Damage = 20;
            Speed = 1;
            Live = 0;
            CurrentLive = Live;

            //Visual data
            Name = "Giant Robot ";
            GetBattleForms(@".\Visual Data\Characters\Enemy_04");
            SetMapForms('Ω');
            Color = ConsoleColor.Red;
        }

        private void CreateBoss()
        {
            //Combat data
            Health = 200;
            CurrentHealth = 200;
            Shield = 100;
            CurrentShield = 100;
            ShieldRegenerationAllowed = false;
            Damage = 50;
            Speed = 1;
            Live = 0;
            CurrentLive = Live;

            //Visual data
            Name = "BIG BOSS ";
            GetBattleForms(@".\Visual Data\Characters\Enemy_03");
            SetMapForms('Σ');
            Color = ConsoleColor.Red;
        }

        public void MoveTowards(Player player)
        {
            PreviousX = X;
            PreviousY = Y;

            int HDistance = Math.Abs(X - player.X);
            int VDistance = Math.Abs(Y - player.Y);

            //Each turn, enemy decides which direction he should goes
            //and if there are multiple possible directions at the same time,
            //he will choose the longest distance direction
            //if the path is block, he will switch direction

            //Go vertically
            if (HDistance < VDistance)
            {
                //Go up when there is no obstacle at the top
                if (BlockedVertically != BlockedDirection.Up && Y > player.Y + player.Height)
                {
                    GoUp();
                }

                //Go down when there is no obstable at the bottom
                if (BlockedVertically != BlockedDirection.Down && Y + Height < player.Y)
                {
                    GoDown();
                }

                //Switch direction if got stuck
                if (BlockedVertically == BlockedDirection.Up | BlockedVertically == BlockedDirection.Down)
                {
                    //Go left when there is no obstable the left
                    if (BlockedHorizontally != BlockedDirection.Left && X > player.X + player.Width)
                    {
                        GoLeft();
                    }

                    //Go right when there is no obstable the left
                    if (BlockedHorizontally != BlockedDirection.Right && X + Width < player.X)
                    {
                        GoRight();
                    }
                }
            }

            //Go horizontally
            else if (HDistance > VDistance)
            {
                //Go left when there is no obstable the left
                if (BlockedHorizontally != BlockedDirection.Left && X > player.X + player.Width)
                {
                    GoLeft();
                }

                //Go right when there is no obstable the left
                if (BlockedHorizontally != BlockedDirection.Right && X + Width < player.X)
                {
                    GoRight();
                }

                if (BlockedHorizontally == BlockedDirection.Left || BlockedHorizontally == BlockedDirection.Right)
                {
                    //Go up when there is no obstacle at the top
                    if (BlockedVertically != BlockedDirection.Up && Y > player.Y + player.Height)
                    {
                        GoUp();
                    }

                    //Go down when there is no obstable at the bottom
                    if (BlockedVertically != BlockedDirection.Down && Y + Height < player.Y)
                    {
                        GoDown();
                    }
                }
            }

            //WILL UPDATE THIS
            //moveBehavior.Move(player); ???

            //if (X           == player.X + player.Width  ||
            //    X + Width   == player.X                 ||
            //    Y + Height  == player.Y                 ||
            //    Y           == player.Y + player.Height)
            //{
            //    AttackPermission = true;
            //}
        }

        private void GoLeft()
        {
            X -= Speed;
            BlockedHorizontally = BlockedDirection.None;
            BlockedVertically = BlockedDirection.None;
        }

        private void GoRight()
        {
            X += Speed;
            BlockedHorizontally = BlockedDirection.None;
            BlockedVertically = BlockedDirection.None;
        }

        private void GoUp()
        {
            Y -= Speed;
            BlockedHorizontally = BlockedDirection.None;
            BlockedVertically = BlockedDirection.None;
        }

        private void GoDown()
        {
            Y += Speed;
            BlockedHorizontally = BlockedDirection.None;
            BlockedVertically = BlockedDirection.None;
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
                //Console.SetCursorPosition(BoundaryTopX[i], BoundaryTopY[i]);
                //Console.Write(".");
            }

            //DOWN
            for (int i = 0; i < Width; i++)
            {
                BoundaryBotX[i] = X + i;
                BoundaryBotY[i] = Y + Height;

                //Draw boundary
                //Console.SetCursorPosition(BoundaryBotX[i], BoundaryBotY[i]);
                //Console.Write(".");
            }

            //LEFT
            for (int i = 1; i < Width; i++)
            {
                BoundaryLeftX[i] = X - 1;
                BoundaryLeftY[i] = Y + i - 1;

                //Draw boundary
                //Console.SetCursorPosition(BoundaryLeftX[i], BoundaryLeftY[i]);
                //Console.Write(".");
            }

            //RIGHT
            for (int i = 1; i < Width; i++)
            {
                BoundaryRightX[i] = X + Width;
                BoundaryRightY[i] = Y + i - 1;

                //Draw boundary
                //Console.SetCursorPosition(BoundaryRightX[i], BoundaryRightY[i]);
                //Console.Write(".");
            }
        }
    }
}
