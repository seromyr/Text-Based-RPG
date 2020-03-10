using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Text_Based_RPG.Behaviors;

namespace Text_Based_RPG
{
    class GameManager
    {
        //Game state
        public bool gameOver;
        //private BlockedDirection blockedHorizontally    = new BlockedDirection();
        //private BlockedDirection blockedVertically      = new BlockedDirection();

        private List<Enemy> enemyList;
        public int EnemyCount { get; set; }
        public int EnemyLimit { get; }


        //Game visual variables
        private int gameplayCanvasLimitUp, gameplayCanvasLimitDown, gameplayCanvasLimitLeft, gameplayCanvasLimitRight;

        //Game mechanics instances
        private DisplayManager  displayManager          = new DisplayManager();
        private InputManager    inputManager            = new InputManager();

        //Character instances
        private Player          player                  = new Player();
        private PickUps         potion;         //      = new PickUps(HealhPotion.small);       //or 0, but it is for reading comprehension
        private Enemy           dummyEnemy              = new Enemy(new MoveTowardsPlayer());

        //Map instance
        private Map             map                     = new Map();

        //Randomization instance
        private Random          random                  = new Random();

        //***********
        //**************
        //*****************
        //** CONSTRUCTOR *****
        //*****************
        //**************
        //***********

        public GameManager()
        {
            //Game loop initialize
            gameOver = false;

            //Set gameplay canvas border
            gameplayCanvasLimitUp = 6;
            gameplayCanvasLimitDown = gameplayCanvasLimitUp + 22;
            gameplayCanvasLimitLeft = 1;
            gameplayCanvasLimitRight = gameplayCanvasLimitLeft + 117;

            Console.CursorVisible = false;

            //Instantiate primary variables
            //------------------------------------- Player attributes
            player.CurrentHealth = 90;
            player.CurrentShield = 0;
            player.X = gameplayCanvasLimitLeft + 6;
            player.Y = gameplayCanvasLimitUp + (gameplayCanvasLimitDown - gameplayCanvasLimitUp) / 2 - 2;

            //Count enemy's instances
            enemyList = new List<Enemy>();
            EnemyCount = 0;
            EnemyLimit = 1;

            //DummyEnemy for HUD displaying only
            dummyEnemy.Health = 100;
            dummyEnemy.Shield = 100;
            dummyEnemy.CurrentHealth = 0;
            dummyEnemy.CurrentShield = 0;
            dummyEnemy.Damage = 0;
            dummyEnemy.Name = " ------";

            //Map
            map.X = gameplayCanvasLimitLeft + 1;
            map.Y = gameplayCanvasLimitUp + 1;

            //Spawn small Potion on start
            SpawnHealthPotionAtRunTime("small", gameplayCanvasLimitRight -25, gameplayCanvasLimitUp + 14);

            //------------------------------------- Input manager display position
            inputManager.x = gameplayCanvasLimitLeft;
            inputManager.y = gameplayCanvasLimitDown + 1;

            //Player controls
            //player.BlockedHorizontally = BlockedDirection.None;
            //player.BlockedVertically = BlockedDirection.None;

            //Map is drawn once on Start
            displayManager.DrawObjectAt(map.X, map.Y, map.PhysicalForm, map.Color);
        }

        //***********
        //**************
        //*****************
        //*** GAME LOOP ******
        //*****************
        //**************
        //***********

        public void GamePlay()
        {
            //Draw Player
            displayManager.DrawObjectAt(player.X, player.Y, player.PhysicalForm, player.Color);

            //Update player boundaries coordinates for the collision check
            player.GetCurrentBoundaryCoordinates();

            

            //Spawn Enemy with limited instances
            while (EnemyCount < EnemyLimit)
            {
                SpawnEnemyAtRuntime();
                if (EnemyCount > EnemyLimit)
                {
                    EnemyCount = EnemyLimit;
                }
            }

            //Perform collision check between player and every enemy on the screen
            for (int i = 0; i < enemyList.Count; i++)
            {
                enemyList[i].MoveTowards(player);
                displayManager.DrawObjectAt(enemyList[i].PreviousX , enemyList[i].PreviousY , enemyList[i].NegativeForm , enemyList[i].Color);
                displayManager.DrawObjectAt(enemyList[i].X         , enemyList[i].Y         , enemyList[i].PhysicalForm , enemyList[i].Color);
                enemyList[i].GetCurrentBoundaryCoordinates();
                CollisionCheckOutsideBounds(enemyList[i]);
                CollisionCheckInsideBounds(map, enemyList[i]);

                enemyList[i].ShowStatsHUD(50, 1);

                //Combat
                if (enemyList[i].AttackPermission)
                {
                    player.TakeDamage(enemyList[i]);
                    enemyList[i].TakeDamage(player);

                    if (enemyList[i].CurrentHealth < 0)
                    {
                        displayManager.DrawObjectAt(enemyList[i].X, enemyList[i].Y, enemyList[i].NegativeForm, enemyList[i].Color);
                        KillEnemyAtRunTime(enemyList[i]);
                        //displayManager.DrawAnimateTextboxIn();
                        EnemyCount--;
                        //Release controls lock
                        player.BlockedHorizontally = BlockedDirection.None;
                        player.BlockedVertically = BlockedDirection.None;
                    }
                }
            }

            //Map had been drawn when the gameplay started
            CollisionCheckInsideBounds(map, player);

            CollisionCheckBetweenPlayerAndCanvasBoundaries();

            //Draw gameplay screens
            //------------------------------------------------ HUD
            Console.ForegroundColor = ConsoleColor.White;
            player.ShowStatsHUD(1, 1);
            if (enemyList.Count == 0)
            {
                dummyEnemy.ShowStatsHUD(50, 1);
            }

            //------------------------------------------------ Gameplay canvas
            Console.ForegroundColor = ConsoleColor.Yellow;
            displayManager.DrawRectangle(gameplayCanvasLimitLeft, gameplayCanvasLimitUp, gameplayCanvasLimitRight - gameplayCanvasLimitLeft, gameplayCanvasLimitDown - gameplayCanvasLimitUp);

            //------------------------------------------------ Player
            //displayManager.DrawObjectAt(player.X, player.Y, player.PhysicalForm, player.Color);

            //------------------------------------------------ Health Potion
            displayManager.DrawObjectAt(potion.X, potion.Y, potion.PhysicalForm, potion.Color);

            //Collision check between player and potion
            if (CollisionCheckTwoObjects(player, potion, false, gameplayCanvasLimitRight + 1, gameplayCanvasLimitUp + 6))
            {
                //Increase player current health
                displayManager.DrawHint(gameplayCanvasLimitLeft, gameplayCanvasLimitDown, $"{potion.Name} picked up!");
                player.CurrentHealth += potion.Health;

                //Clear and move the health potion somewhere else
                displayManager.DrawObjectAt(potion.X, potion.Y, potion.NegativeForm, potion.Color);
                potion.X = GenerateRandomGameplayCanvasCoordinates(true, potion);
                potion.Y = GenerateRandomGameplayCanvasCoordinates(false, potion);
            }

            //Run player controller
            PlayerController();

            //Removed to avoid screen flickering
            //Console.Clear();

            ////------------------------------------------------ Player out of bound check
            //if (player.X < gameplayCanvasLimitLeft + 1) player.X = gameplayCanvasLimitRight - player.Width - 1;
            //else if (player.X > gameplayCanvasLimitRight - player.Width - 1) player.X = gameplayCanvasLimitLeft + 1;
            //if (player.Y < gameplayCanvasLimitUp + 1) player.Y = gameplayCanvasLimitDown - player.Height - 1;
            //else if (player.Y > gameplayCanvasLimitDown - player.Height - 1) player.Y = gameplayCanvasLimitUp + 1;
            ////------------------------------------------------ End check


        }

        void PlayerController()
        {
            switch (inputManager.InputListener())
            {
                case 1: //LEFT
                    if (player.BlockedHorizontally == BlockedDirection.Left) break;
                    else
                    {
                        player.X -= player.Speed; //                                                                   <= Move player with speed
                        player.PreviousX = player.X + player.Speed; //                                                 <= Determined player previous position
                        displayManager.DrawObjectAt(player.PreviousX, player.Y, player.NegativeForm, player.Color); // <= Clean player image in previous position
                        player.BlockedHorizontally = BlockedDirection.None; //                                                <= Assume that there is obstacle blocking player movement horizontally
                        player.BlockedVertically = BlockedDirection.None; //                                                  <= Assume that there is obstacle blocking player movement vertically
                    }
                    break;
                case 2: //RIGHT
                    if (player.BlockedHorizontally == BlockedDirection.Right) break;
                    else
                    {
                        player.X += player.Speed;
                        player.PreviousX = player.X - player.Speed;
                        displayManager.DrawObjectAt(player.PreviousX, player.Y, player.NegativeForm, player.Color);
                        player.BlockedHorizontally = BlockedDirection.None;
                        player.BlockedVertically = BlockedDirection.None;
                    }
                    break;
                case 3: //UP
                    if (player.BlockedVertically == BlockedDirection.Up) break;
                    else
                    {
                        player.Y -= player.Speed;
                        player.PreviousY = player.Y + player.Speed;
                        displayManager.DrawObjectAt(player.X, player.PreviousY, player.NegativeForm, player.Color);
                        player.BlockedHorizontally = BlockedDirection.None;
                        player.BlockedVertically = BlockedDirection.None;
                    }
                    break;
                case 4: //DOWN
                    if (player.BlockedVertically == BlockedDirection.Down) break;
                    else
                    {
                        player.Y += player.Speed;
                        player.PreviousY = player.Y - player.Speed;
                        displayManager.DrawObjectAt(player.X, player.PreviousY, player.NegativeForm, player.Color);
                        player.BlockedHorizontally = BlockedDirection.None;
                        player.BlockedVertically = BlockedDirection.None;
                    }
                    break;
            }
        }

        //Randomize X and Y within the Gameplay Canvas based on the object length and height
        private int GenerateRandomGameplayCanvasCoordinates(bool xOrY, CharacterObject cO)
        {
            int generatedValue = 0;

            switch (xOrY)
            {
                //Generate X inside  gameplay canvas
                case true:
                    generatedValue = random.Next(gameplayCanvasLimitLeft + 1, gameplayCanvasLimitRight);

                    if (generatedValue > gameplayCanvasLimitRight - cO.Width)
                    {
                        generatedValue = gameplayCanvasLimitRight - cO.Width - 1;
                    }
                    break;

                //Generate Y inside  gameplay canvas
                case false:
                    generatedValue = random.Next(gameplayCanvasLimitUp + 1, gameplayCanvasLimitDown - 1);

                    if (generatedValue > gameplayCanvasLimitDown - cO.Height)
                    {
                        generatedValue = gameplayCanvasLimitDown - cO.Height - 1;
                    }
                    break;
            }
            return generatedValue;
        }

        //Collision check between two object
        private bool CollisionCheckTwoObjects(CharacterObject cO1, CharacterObject cO2, bool showlog, int x, int y)
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
                ParseCoordinates(gameplayCanvasLimitRight + 1, gameplayCanvasLimitUp, cO1, objAx, objAy);

                ParseCoordinates(x, y, cO2, objBx, objBy);
            }
            
            return isCollided;
        }

        private void ParseCoordinates(int x, int y, CharacterObject characterObject , List<int> characterObjectListX, List<int> characterObjectListY)
        {
            Console.SetCursorPosition(x, y);
            Console.Write("Parsing {0}'s position", characterObject.Name);

            Console.SetCursorPosition(x, y +1 );
            Console.Write("X: ");
            for (int i = 0; i < characterObjectListX.Count; i++)
            {
                Console.Write(characterObjectListX[i] + "  ");
            }
            Console.SetCursorPosition(x, y +2);
            Console.Write("Y: ");
            for (int i = 0; i < characterObjectListY.Count; i++)
            {
                Console.Write(characterObjectListY[i] + "  ");
            }
        }

        //Unused test method
        private void CollisionCheckBetweenPlayerAndCanvasBoundaries()
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
            for (int i = 0; i < (gameplayCanvasLimitRight - gameplayCanvasLimitLeft); i++)
            {
                testTopX.Add(gameplayCanvasLimitLeft + i);
                testBotX.Add(testTopX[i]);
            }

            for (int i = 0; i < (gameplayCanvasLimitRight - gameplayCanvasLimitLeft); i++)
            {
                testTopY.Add(gameplayCanvasLimitUp);
                testBotY.Add(gameplayCanvasLimitDown - 1);
            }

            //This is gameplay canvas left and right border coordinates
            for (int i = 0; i < (gameplayCanvasLimitDown - gameplayCanvasLimitUp); i++)
            {
                testLeftX.Add(gameplayCanvasLimitLeft);
                testRightX.Add(gameplayCanvasLimitRight - 1);
            }

            for (int i = 0; i < (gameplayCanvasLimitDown - gameplayCanvasLimitUp); i++)
            {
                testLeftY.Add(gameplayCanvasLimitUp + i);
                testRightY.Add(testLeftY[i]);
            }

            //Check for colision
            for (int i = 0; i < testTopY.Count; i++)
            {
                if (player.BoundaryTopX.Contains(testTopX[i]) && player.BoundaryTopY.Contains(testTopY[i]))
                {
                    displayManager.DrawHint(gameplayCanvasLimitLeft, gameplayCanvasLimitDown, $"{ player.Name} reached top.");
                    player.BlockedVertically = BlockedDirection.Up;
                }
            }

            for (int i = 0; i < testBotY.Count; i++)
            {
                if (player.BoundaryBotX.Contains(testBotX[i]) && player.BoundaryBotY.Contains(testBotY[i]))
                {
                    displayManager.DrawHint(gameplayCanvasLimitLeft, gameplayCanvasLimitDown, $"{ player.Name} reached bot.");
                    player.BlockedVertically = BlockedDirection.Down;
                }
            }

            for (int i = 0; i < testLeftX.Count; i++)
            {
                if (player.BoundaryLeftX.Contains(testLeftX[i]) && player.BoundaryLeftY.Contains(testLeftY[i]))
                {
                    displayManager.DrawHint(gameplayCanvasLimitLeft, gameplayCanvasLimitDown, $"{ player.Name} reached left.");
                    player.BlockedHorizontally = BlockedDirection.Left;
                }
            }

            for (int i = 0; i < testLeftX.Count; i++)
            {
                if (player.BoundaryRightX.Contains(testRightX[i]) && player.BoundaryRightY.Contains(testRightY[i]))
                {
                    displayManager.DrawHint(gameplayCanvasLimitLeft, gameplayCanvasLimitDown, $"{ player.Name} reached right.");
                    player.BlockedHorizontally = BlockedDirection.Right;
                }
            }
        }

        private void CollisionCheckOutsideBounds(CharacterObject observingObject)
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

            //Check for colision
            for (int i = 0; i < allY.Count; i++)
            {
                if (player.BoundaryTopX.Contains(allX[i]) && player.BoundaryTopY.Contains(allY[i]))
                {
                    displayManager.DrawHint(gameplayCanvasLimitLeft, gameplayCanvasLimitDown, $"{ player.Name} encountered " + observingObject.Name + " at the top.");
                    player.BlockedVertically = BlockedDirection.Up;
                    observingObject.AttackPermission = true;
                }
            }

            for (int i = 0; i < allY.Count; i++)
            {
                if (player.BoundaryBotX.Contains(allX[i]) && player.BoundaryBotY.Contains(allY[i]))
                {
                    displayManager.DrawHint(gameplayCanvasLimitLeft, gameplayCanvasLimitDown, $"{ player.Name} encountered " + observingObject.Name + " below.");
                    player.BlockedVertically = BlockedDirection.Down;
                    observingObject.AttackPermission = true;
                }
            }

            for (int i = 0; i < allX.Count; i++)
            {
                if (player.BoundaryLeftX.Contains(allX[i]) && player.BoundaryLeftY.Contains(allY[i]))
                {
                    displayManager.DrawHint(gameplayCanvasLimitLeft, gameplayCanvasLimitDown, $"{ player.Name} encountered " + observingObject.Name + " on the left.");
                    player.BlockedHorizontally = BlockedDirection.Left;
                    observingObject.AttackPermission = true;
                }
            }

            for (int i = 0; i < allX.Count; i++)
            {
                if (player.BoundaryRightX.Contains(allX[i]) && player.BoundaryRightY.Contains(allY[i]))
                {
                    displayManager.DrawHint(gameplayCanvasLimitLeft, gameplayCanvasLimitDown, $"{ player.Name} encountered " + observingObject.Name + " on the right.");
                    player.BlockedHorizontally = BlockedDirection.Right;
                    observingObject.AttackPermission = true;
                }
            }
        }

        private void CollisionCheckInsideBounds(CharacterObject observingObject, CharacterObject observer)
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

            //Step 1: Breakdown Map array content into characters and put them into a list
            for (int i = 0; i < observingObject.PhysicalForm.Length; i++)
            {
                foreach (char c in observingObject.PhysicalForm[i])
                {
                    charMapList.Add(Convert.ToString(c));
                }
            }

            Console.SetCursorPosition(0, 0);
            Console.Write(allX.Count + " - " + allY.Count + " - " + charMapList.Count);

            for (int i = 0; i < allY.Count; i++)
            {
                if (observer.BoundaryTopX.Contains(allX[i]) && observer.BoundaryTopY.Contains(allY[i]) && charMapList[i] != " ")
                {
                    displayManager.DrawHint(gameplayCanvasLimitLeft, gameplayCanvasLimitDown, $"{ observer.Name} encountered " + observingObject.Name + " at the top.");
                    observer.BlockedVertically = BlockedDirection.Up;
                }
            }

            for (int i = 0; i < allY.Count; i++)
            {
                if (observer.BoundaryBotX.Contains(allX[i]) && observer.BoundaryBotY.Contains(allY[i]) && charMapList[i] != " ")
                {
                    displayManager.DrawHint(gameplayCanvasLimitLeft, gameplayCanvasLimitDown, $"{ observer.Name} encountered " + observingObject.Name + " below.");
                    observer.BlockedVertically = BlockedDirection.Down;
                }

            }

            for (int i = 0; i < allX.Count; i++)
            {
                if (observer.BoundaryLeftX.Contains(allX[i]) && observer.BoundaryLeftY.Contains(allY[i]) && charMapList[i] != " ")
                {
                    displayManager.DrawHint(gameplayCanvasLimitLeft, gameplayCanvasLimitDown, $"{ observer.Name} encountered " + observingObject.Name + " on the left.");
                    observer.BlockedHorizontally = BlockedDirection.Left;
                    observingObject.AttackPermission = true;
                }
            }

            for (int i = 0; i < allX.Count; i++)
            {
                if (observer.BoundaryRightX.Contains(allX[i]) && observer.BoundaryRightY.Contains(allY[i]) && charMapList[i] != " ")
                {
                    displayManager.DrawHint(gameplayCanvasLimitLeft, gameplayCanvasLimitDown, $"{ observer.Name} encountered " + observingObject.Name + " on the right.");
                    observer.BlockedHorizontally = BlockedDirection.Right;
                    observingObject.AttackPermission = true;
                }
            }
        }

        private void SpawnEnemyAtRuntime()
        {
            enemyList.Add(new Enemy(new MoveTowardsPlayer()));

            enemyList[EnemyCount].Name = "Enemy " + EnemyCount;
            enemyList[EnemyCount].X = GenerateRandomGameplayCanvasCoordinates(true, enemyList[EnemyCount]);
            enemyList[EnemyCount].Y = GenerateRandomGameplayCanvasCoordinates(false, enemyList[EnemyCount]);
            EnemyCount++;
        }

        private void KillEnemyAtRunTime(Enemy enemy)
        {
            enemyList.Remove(enemy);
        }

        private void SpawnHealthPotionAtRunTime(string type, int x, int y)
        {
            List<PickUps> potions = new List<PickUps>();
            switch (type)
            {
                case "small":
                    potion = new PickUps(HealhPotion.small);
                    potions.Add(potion);
                    break;
                case "large":
                    potion = new PickUps(HealhPotion.large);
                    potions.Add(potion);
                    break;
            }

            potion.X = x;
            potion.Y = y;
        }
    }
}
