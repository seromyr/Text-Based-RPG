﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text_Based_RPG.CharacterObjects
{
    class Player : GameCharacter
    {
        //Totally private variable
        private int _characterSteps;

        public Player()
        {

            Health = 100;
            Shield = 100;
            Damage = 5;
            Live = 3;
            CurrentLive = Live;
            ShieldRegenerationAllowed = true;

            X = 1;
            Y = 1;
            PreviousX = X;
            PreviousY = Y;
            Speed = 1;

            GetBattleForms(@".\Visual Data\Characters\Player");
            SetMapForms('Θ');

            Name = "[Every Day Normal Bot]";
            Color = ConsoleColor.DarkYellow;

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

            _characterSteps = GameManager.Tick;
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

        public void UpdateGameplayStatus(int steps)
        {
            //Well, only CurrentShield regenerates if it was allowed to
            //And regenerates shield every number of steps (or Ticks)
            if (GameManager.Tick == _characterSteps + steps)
            {
                _characterSteps = GameManager.Tick;
                if (ShieldRegenerationAllowed)
                {
                    if (CurrentShield > 0)
                    {
                        //if ( timeStampX)
                        CurrentShield += (int)Math.Round(ShieldRegenerationRate * Shield);
                        if (CurrentShield > 100)
                        {
                            CurrentShield = 100;
                        }
                    }
                }
            }

            if (CurrentHealth > Health)
            {
                CurrentHealth = Health;
            }
            //show log
            //Console.SetCursorPosition(5, 0);
            //Console.Write(_characterSteps);
        }
    }
}
