using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Text_Based_RPG.DisplaySystem;
using Text_Based_RPG.CharacterObjects;


namespace Text_Based_RPG
{
    class GameLogic
    {
        //Randomization instance
        public static Random random = new Random();

        //Unused test method
        public void CollisionCheckBetweenPlayerAndCanvasBoundaries(Player player)
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
            for (int i = 0; i < (Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - Constant.GAMEPLAY_CANVAS_LIMIT_LEFT); i++)
            {
                testTopX.Add(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + i);
                testBotX.Add(testTopX[i]);
            }

            for (int i = 0; i < (Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - Constant.GAMEPLAY_CANVAS_LIMIT_LEFT); i++)
            {
                testTopY.Add(Constant.GAMEPLAY_CANVAS_LIMIT_UP);
                testBotY.Add(Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - 1);
            }

            //This is gameplay canvas left and right border coordinates
            for (int i = 0; i < (Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - Constant.GAMEPLAY_CANVAS_LIMIT_UP); i++)
            {
                testLeftX.Add(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT);
                testRightX.Add(Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - 1);
            }

            for (int i = 0; i < (Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - Constant.GAMEPLAY_CANVAS_LIMIT_UP); i++)
            {
                testLeftY.Add(Constant.GAMEPLAY_CANVAS_LIMIT_UP + i);
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

        //Collision check between player and item object
        public bool CollisionCheckTwoObjects(Player player, Items item, bool showlog, int x, int y)
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
            int xA = player.X;
            int yA = player.Y;
            int xB = item.X;
            int yB = item.Y;

            //------------- Break down object 1
            for (int i = 0; i < player.Width * player.Height; i++)
            {
                objAx.Add(xA); xA++;
                objAy.Add(yA);

                if (xA > player.X + player.Width - 1)
                {
                    xA = player.X;
                    yA++;
                }
            }

            //------------- Break down object 2
            for (int i = 0; i < item.Width * item.Height; i++)
            {
                objBx.Add(xB); xB++;
                objBy.Add(yB);

                if (xB > item.X + item.Width - 1)
                {
                    xB = item.X;
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
                ParseCoordinates(Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT + 1, Constant.GAMEPLAY_CANVAS_LIMIT_UP, player, objAx, objAy);

                ParseCoordinates(x, y, item, objBx, objBy);
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

        public void CollisionCheckInsideBounds(CharacterObject observingObject, GameCharacter character)
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
                if (character.BoundaryTopX.Contains(allX[i]) && character.BoundaryTopY.Contains(allY[i]) && charMapList[i] != " ")
                {
                   //DisplayManager.WriteTextAt(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT, Constant.GAMEPLAY_CANVAS_LIMIT_DOWN, $"{ observer.Name} encountered " + observingObject.Name + " at the top.");
                    character.BlockedVertically = BlockedDirection.Up;
                }
            }

            for (int i = 0; i < allY.Count; i++)
            {
                if (character.BoundaryBotX.Contains(allX[i]) && character.BoundaryBotY.Contains(allY[i]) && charMapList[i] != " ")
                {
                    //DisplayManager.WriteTextAt(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT, Constant.GAMEPLAY_CANVAS_LIMIT_DOWN, $"{ observer.Name} encountered " + observingObject.Name + " below.");
                    character.BlockedVertically = BlockedDirection.Down;
                }
            }

            for (int i = 0; i < allX.Count; i++)
            {
                if (character.BoundaryLeftX.Contains(allX[i]) && character.BoundaryLeftY.Contains(allY[i]) && charMapList[i] != " ")
                {
                    //DisplayManager.WriteTextAt(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT, Constant.GAMEPLAY_CANVAS_LIMIT_DOWN, $"{ observer.Name} encountered " + observingObject.Name + " on the left.");
                    character.BlockedHorizontally = BlockedDirection.Left;
                    //observingObject.AttackPermission = true;
                }
            }

            for (int i = 0; i < allX.Count; i++)
            {
                if (character.BoundaryRightX.Contains(allX[i]) && character.BoundaryRightY.Contains(allY[i]) && charMapList[i] != " ")
                {
                    //DisplayManager.WriteTextAt(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT, Constant.GAMEPLAY_CANVAS_LIMIT_DOWN, $"{ observer.Name} encountered " + observingObject.Name + " on the right.");
                    character.BlockedHorizontally = BlockedDirection.Right;
                    //observingObject.AttackPermission = true;
                }
            }
        }

        public void CollisionCheckInsideCameraViewport(Camera map, GameCharacter character)
        {
            int x = (int)Math.Round(Constant.VIEWPORT_WIDTH / 2.0, 0);
            int y = (int)Math.Round(Constant.VIEWPORT_HEIGHT / 2.0, 0);

            if (map.viewPort[y - 1,x] != ' ')
            {
                character.BlockedVertically = BlockedDirection.Up;
            }

            if (map.viewPort[y + 1,x] != ' ')
            {
                character.BlockedVertically = BlockedDirection.Down;
            }

            if (map.viewPort[y,x - 1] != ' ')
            {
                character.BlockedHorizontally = BlockedDirection.Left;
            }

            if (map.viewPort[y,x + 1] != ' ')
            {
                character.BlockedHorizontally = BlockedDirection.Right;
            }
        }

        public bool BattleCheckOnWorldMap(Camera map, GameCharacter character, GameCharacter enemy)
        {
            bool fight = false;
            int x = (int)Math.Round(Constant.VIEWPORT_WIDTH  / 2.0, 0);
            int y = (int)Math.Round(Constant.VIEWPORT_HEIGHT / 2.0, 0);

            if (map.viewPort[y - 1, x] == enemy.MapForm ||
                map.viewPort[y + 1, x] == enemy.MapForm ||
                map.viewPort[y, x - 1] == enemy.MapForm ||
                map.viewPort[y, x + 1] == enemy.MapForm )
            {
                fight = true;
            }

            return fight;
        }

        public bool ObjectCollisionCheckOnWorldMap(Camera map, GameCharacter character, char objectChar)
        {
            bool encounter = false;

            int x = (int)Math.Round(Constant.VIEWPORT_WIDTH  / 2.0, 0);
            int y = (int)Math.Round(Constant.VIEWPORT_HEIGHT / 2.0, 0);

            if (map.viewPort[y - 1, x] == objectChar ||
                map.viewPort[y + 1, x] == objectChar ||
                map.viewPort[y, x - 1] == objectChar ||
                map.viewPort[y, x + 1] == objectChar )
            {
                encounter = true;
            }

            return encounter;
        }

        public void CollisionCheckOutsideBounds(Enemy enemy, Player player)
        {
            //Methods No.2
            //Constantly update every obstacle that is displayed within the gameplay canvas
            //Compare player position (outer circumference) with them
            //If there was any collision, player is not allowed to move in that direction

            List<int> allX = new List<int>();
            List<int> allY = new List<int>();

            int xB = enemy.X;
            int yB = enemy.Y;

            //This is gameplay canvas top and bot border coordinates
            //------------- Break down observing object
            for (int i = 0; i < enemy.Width * enemy.Height; i++)
            {
                allX.Add(xB); xB++;
                allY.Add(yB);

                if (xB > enemy.X + enemy.Width - 1)
                {
                    xB = enemy.X;
                    yB++;
                }
            }

            enemy.AttackPermission = false;

            //Check for collision
            for (int i = 0; i < allY.Count; i++)
            {
                if (player.BoundaryTopX.Contains(allX[i]) && player.BoundaryTopY.Contains(allY[i]))
                {
                    //DisplayManager.WriteTextAt(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT, Constant.GAMEPLAY_CANVAS_LIMIT_DOWN, $"{ player.Name} encountered " + observingObject.Name + " at the top.");
                    player.BlockedVertically = BlockedDirection.Up;
                    enemy.AttackPermission = true;
                }
            }

            for (int i = 0; i < allY.Count; i++)
            {
                if (player.BoundaryBotX.Contains(allX[i]) && player.BoundaryBotY.Contains(allY[i]))
                {
                    //DisplayManager.WriteTextAt(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT, Constant.GAMEPLAY_CANVAS_LIMIT_DOWN, $"{ player.Name} encountered " + observingObject.Name + " below.");
                    player.BlockedVertically = BlockedDirection.Down;
                    enemy.AttackPermission = true;
                }
            }

            for (int i = 0; i < allX.Count; i++)
            {
                if (player.BoundaryLeftX.Contains(allX[i]) && player.BoundaryLeftY.Contains(allY[i]))
                {
                    //DisplayManager.WriteTextAt(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT, Constant.GAMEPLAY_CANVAS_LIMIT_DOWN, $"{ player.Name} encountered " + observingObject.Name + " on the left.");
                    player.BlockedHorizontally = BlockedDirection.Left;
                    enemy.AttackPermission = true;
                }
            }

            for (int i = 0; i < allX.Count; i++)
            {
                if (player.BoundaryRightX.Contains(allX[i]) && player.BoundaryRightY.Contains(allY[i]))
                {
                    //DisplayManager.WriteTextAt(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT, Constant.GAMEPLAY_CANVAS_LIMIT_DOWN, $"{ player.Name} encountered " + observingObject.Name + " on the right.");
                    player.BlockedHorizontally = BlockedDirection.Right;
                    enemy.AttackPermission = true;
                }
            }
        }

        //Randomize X and Y within the Gameplay Canvas based on the object length and height
        public int GenerateRandomGameplayCanvasCoordinates(bool xOrY, CharacterObject cO)
        {
            int generatedValue = 0;

            switch (xOrY)
            {
                //Generate X inside  gameplay canvas
                case true:
                    generatedValue = random.Next(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 1, Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT);

                    if (generatedValue > Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - cO.Width)
                    {
                        generatedValue = Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - cO.Width - 1;
                    }
                    break;

                //Generate Y inside  gameplay canvas
                case false:
                    generatedValue = random.Next(Constant.GAMEPLAY_CANVAS_LIMIT_UP + 1, Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - 1);

                    if (generatedValue > Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - cO.Height)
                    {
                        generatedValue = Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - cO.Height - 1;
                    }
                    break;
            }
            return generatedValue;
        }

        public static int GenerateRandomBetween(int a, int b)
        {
            int generatedValue = 0;

            generatedValue = random.Next(a, b);

            return generatedValue;
        }

    }
}
