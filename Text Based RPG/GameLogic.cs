using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text_Based_RPG
{
    // GAME CONSTANTS ------------------------------------------------------------- //
    enum BlockedDirection { None, Left, Right, Up, Down }                           //
    enum EnemyType        { Random_Robot, Common_Robot, Elite_Robot, Giant_Robot }  //
    enum HealhPotion      { Small, Large }                                          //
    enum GameCanvasLimit  { Left = 1, Right = Left + 117, Up = 6, Down = Up + 22 }  //
    // ---------------------------------------------------------------------------- //

    class GameLogic
    {
        //Randomization instance
        public static Random random = new Random();

        //Unused test method
        public void CollisionCheckBetweenPlayerAndCanvasBoundaries(CharacterObject player)
        {
            //Methods
            //Constantly update every obstacle that is displayed within the gameplay canvas
            //Compare player position (outer circumference) with them. if there was any collision, player is not allowed to move in that direction => player speed = 0

            List<int> testTopX = new List<int>();
            List<int> testTopY = new List<int>();

            List<int> testBotX = new List<int>();
            List<int> testBotY = new List<int>();

            List<int> testLeftX = new List<int>();
            List<int> testLeftY = new List<int>();

            List<int> testRightX = new List<int>();
            List<int> testRightY = new List<int>();

            //This is gameplay canvas top and bot border coordinates
            for (int i = 0; i < (GameCanvasLimit.Right - GameCanvasLimit.Left); i++)
            {
                testTopX.Add((int)GameCanvasLimit.Left + i);
                testBotX.Add(testTopX[i]);
            }

            for (int i = 0; i < (GameCanvasLimit.Right - GameCanvasLimit.Left); i++)
            {
                testTopY.Add((int)GameCanvasLimit.Up);
                testBotY.Add((int)GameCanvasLimit.Down - 1);
            }

            //This is gameplay canvas left and right border coordinates
            for (int i = 0; i < (GameCanvasLimit.Down - GameCanvasLimit.Up); i++)
            {
                testLeftX.Add((int)GameCanvasLimit.Left);
                testRightX.Add((int)GameCanvasLimit.Right - 1);
            }

            for (int i = 0; i < (GameCanvasLimit.Down - GameCanvasLimit.Up); i++)
            {
                testLeftY.Add((int)GameCanvasLimit.Up + i);
                testRightY.Add(testLeftY[i]);
            }

            //Check for colision
            for (int i = 0; i < testTopY.Count; i++)
            {
                if (player.BoundaryTopX.Contains(testTopX[i]) && player.BoundaryTopY.Contains(testTopY[i]))
                {
                    //displayManager.DrawHint(gameplayCanvasLimitLeft, gameplayCanvasLimitDown, $"{ player.Name} reached top.");
                    player.BlockedVertically = BlockedDirection.Up;
                }
            }

            for (int i = 0; i < testBotY.Count; i++)
            {
                if (player.BoundaryBotX.Contains(testBotX[i]) && player.BoundaryBotY.Contains(testBotY[i]))
                {
                    //displayManager.DrawHint(gameplayCanvasLimitLeft, gameplayCanvasLimitDown, $"{ player.Name} reached bot.");
                    player.BlockedVertically = BlockedDirection.Down;
                }
            }

            for (int i = 0; i < testLeftX.Count; i++)
            {
                if (player.BoundaryLeftX.Contains(testLeftX[i]) && player.BoundaryLeftY.Contains(testLeftY[i]))
                {
                    //displayManager.DrawHint(gameplayCanvasLimitLeft, gameplayCanvasLimitDown, $"{ player.Name} reached left.");
                    player.BlockedHorizontally = BlockedDirection.Left;
                }
            }

            for (int i = 0; i < testLeftX.Count; i++)
            {
                if (player.BoundaryRightX.Contains(testRightX[i]) && player.BoundaryRightY.Contains(testRightY[i]))
                {
                    //displayManager.DrawHint(gameplayCanvasLimitLeft, gameplayCanvasLimitDown, $"{ player.Name} reached right.");
                    player.BlockedHorizontally = BlockedDirection.Right;
                }
            }
        }

        //Collision check between two object
        public bool CollisionCheckTwoObjects(CharacterObject cO1, CharacterObject cO2, bool showlog, int x, int y)
        {
            bool isCollided = false;

            //Collision check algorithm
            //Breakdown and put every visual bit of each object into separating lists
            //Loop through all list elements and find a matching pair of X and Y

            List<int> objAx = new List<int>();
            List<int> objAy = new List<int>();
            List<int> objBx = new List<int>();
            List<int> objBy = new List<int>();

            //------------- Should use a temporary variables without tampering the real variable
            int xA = cO1.X;
            int yA = cO1.Y;
            int xB = cO2.X;
            int yB = cO2.Y;

            //------------- Break down object 1
            for (int i = 0; i < cO1.Width * cO1.Height; i++)
            {
                objAx.Add(xA); xA++;
                objAy.Add(yA);

                if (xA > cO1.X + cO1.Width - 1)
                {
                    xA = cO1.X;
                    yA++;
                }
            }

            //------------- Break down object 2
            for (int i = 0; i < cO2.Width * cO2.Height; i++)
            {
                objBx.Add(xB); xB++;
                objBy.Add(yB);

                if (xB > cO2.X + cO2.Width - 1)
                {
                    xB = cO2.X;
                    yB++;
                }
            }

            //------------- X,Y comparision
            for (int i = 0; i < objBx.Count; i++)
            {
                if (objAx.Contains(objBx[i]) && objAy.Contains(objBy[i]))
                {
                    isCollided = true;
                }
            }

            //------------- Coordinates parsing display for debugging
            if (showlog)
            {
                //Player coordinates parsed log is fixed
                ParseCoordinates((int)GameCanvasLimit.Right + 1, (int)GameCanvasLimit.Up, cO1, objAx, objAy);

                ParseCoordinates(x, y, cO2, objBx, objBy);
            }

            return isCollided;
        }

        private void ParseCoordinates(int x, int y, CharacterObject characterObject, List<int> characterObjectListX, List<int> characterObjectListY)
        {
            Console.SetCursorPosition(x, y);
            Console.Write("Parsing {0}'s position", characterObject.Name);

            Console.SetCursorPosition(x, y + 1);
            Console.Write("X: ");
            for (int i = 0; i < characterObjectListX.Count; i++)
            {
                Console.Write(characterObjectListX[i] + "  ");
            }
            Console.SetCursorPosition(x, y + 2);
            Console.Write("Y: ");
            for (int i = 0; i < characterObjectListY.Count; i++)
            {
                Console.Write(characterObjectListY[i] + "  ");
            }
        }

        public void CollisionCheckInsideBounds(CharacterObject observingObject, CharacterObject observer)
        {
            //Coordinates list
            List<int> allX = new List<int>();
            List<int> allY = new List<int>();

            //Content list
            List<string> charMapList = new List<string>();

            int xB = observingObject.X;
            int yB = observingObject.Y;

            //------------- Break down observing object
            for (int i = 0; i < observingObject.Width * observingObject.Height; i++)
            {
                allX.Add(xB); xB++;
                allY.Add(yB);

                if (xB > observingObject.X + observingObject.Width - 1)
                {
                    xB = observingObject.X;
                    yB++;
                }
            }

            //Step 1: Break down observing array content into characters and put them into defined list
            for (int i = 0; i < observingObject.PhysicalForm.Length; i++)
            {
                foreach (char c in observingObject.PhysicalForm[i])
                {
                    charMapList.Add(Convert.ToString(c));
                }
            }

            //Console.SetCursorPosition(0, 0);
            //Console.Write(allX.Count + " - " + allY.Count + " - " + charMapList.Count);

            for (int i = 0; i < allY.Count; i++)
            {
                if (observer.BoundaryTopX.Contains(allX[i]) && observer.BoundaryTopY.Contains(allY[i]) && charMapList[i] != " ")
                {
                   DisplayManager.DrawHint((int)GameCanvasLimit.Left, (int)GameCanvasLimit.Down, $"{ observer.Name} encountered " + observingObject.Name + " at the top.");
                    observer.BlockedVertically = BlockedDirection.Up;
                }
            }

            for (int i = 0; i < allY.Count; i++)
            {
                if (observer.BoundaryBotX.Contains(allX[i]) && observer.BoundaryBotY.Contains(allY[i]) && charMapList[i] != " ")
                {
                    DisplayManager.DrawHint((int)GameCanvasLimit.Left, (int)GameCanvasLimit.Down, $"{ observer.Name} encountered " + observingObject.Name + " below.");
                    observer.BlockedVertically = BlockedDirection.Down;
                }
            }

            for (int i = 0; i < allX.Count; i++)
            {
                if (observer.BoundaryLeftX.Contains(allX[i]) && observer.BoundaryLeftY.Contains(allY[i]) && charMapList[i] != " ")
                {
                    DisplayManager.DrawHint((int)GameCanvasLimit.Left, (int)GameCanvasLimit.Down, $"{ observer.Name} encountered " + observingObject.Name + " on the left.");
                    observer.BlockedHorizontally = BlockedDirection.Left;
                    observingObject.AttackPermission = true;
                }
            }

            for (int i = 0; i < allX.Count; i++)
            {
                if (observer.BoundaryRightX.Contains(allX[i]) && observer.BoundaryRightY.Contains(allY[i]) && charMapList[i] != " ")
                {
                    DisplayManager.DrawHint((int)GameCanvasLimit.Left, (int)GameCanvasLimit.Down, $"{ observer.Name} encountered " + observingObject.Name + " on the right.");
                    observer.BlockedHorizontally = BlockedDirection.Right;
                    observingObject.AttackPermission = true;
                }
            }
        }

        public void CollisionCheckOutsideBounds(CharacterObject observingObject, Player player)
        {
            //Methods No.2
            //Constantly update every obstacle that is displayed within the gameplay canvas
            //Compare player position (outer circumference) with them
            //If there was any collision, player is not allowed to move in that direction

            List<int> allX = new List<int>();
            List<int> allY = new List<int>();

            int xB = observingObject.X;
            int yB = observingObject.Y;

            //This is gameplay canvas top and bot border coordinates
            //------------- Break down observing object
            for (int i = 0; i < observingObject.Width * observingObject.Height; i++)
            {
                allX.Add(xB); xB++;
                allY.Add(yB);

                if (xB > observingObject.X + observingObject.Width - 1)
                {
                    xB = observingObject.X;
                    yB++;
                }
            }

            observingObject.AttackPermission = false;

            //Check for collision
            for (int i = 0; i < allY.Count; i++)
            {
                if (player.BoundaryTopX.Contains(allX[i]) && player.BoundaryTopY.Contains(allY[i]))
                {
                    DisplayManager.DrawHint((int)GameCanvasLimit.Left, (int)GameCanvasLimit.Down, $"{ player.Name} encountered " + observingObject.Name + " at the top.");
                    player.BlockedVertically = BlockedDirection.Up;
                    observingObject.AttackPermission = true;
                }
            }

            for (int i = 0; i < allY.Count; i++)
            {
                if (player.BoundaryBotX.Contains(allX[i]) && player.BoundaryBotY.Contains(allY[i]))
                {
                    DisplayManager.DrawHint((int)GameCanvasLimit.Left, (int)GameCanvasLimit.Down, $"{ player.Name} encountered " + observingObject.Name + " below.");
                    player.BlockedVertically = BlockedDirection.Down;
                    observingObject.AttackPermission = true;
                }
            }

            for (int i = 0; i < allX.Count; i++)
            {
                if (player.BoundaryLeftX.Contains(allX[i]) && player.BoundaryLeftY.Contains(allY[i]))
                {
                    DisplayManager.DrawHint((int)GameCanvasLimit.Left, (int)GameCanvasLimit.Down, $"{ player.Name} encountered " + observingObject.Name + " on the left.");
                    player.BlockedHorizontally = BlockedDirection.Left;
                    observingObject.AttackPermission = true;
                }
            }

            for (int i = 0; i < allX.Count; i++)
            {
                if (player.BoundaryRightX.Contains(allX[i]) && player.BoundaryRightY.Contains(allY[i]))
                {
                    DisplayManager.DrawHint((int)GameCanvasLimit.Left, (int)GameCanvasLimit.Down, $"{ player.Name} encountered " + observingObject.Name + " on the right.");
                    player.BlockedHorizontally = BlockedDirection.Right;
                    observingObject.AttackPermission = true;
                }
            }
        }
    }
}
