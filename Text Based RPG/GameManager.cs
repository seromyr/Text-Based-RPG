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
        private float damageDistributionRatio;
        public static int Tick { get; set; }

        private List<Enemy> enemyList;
        public int EnemyCount { get; set; }
        public int EnemyLimit { get; }

        //Game mechanics instances
        private GameLogic       gameLogic               = new GameLogic();
        private DisplayManager  displayManager          = new DisplayManager();
        private InputManager    inputManager            = new InputManager();

        //Character instances
        private Player          player                  = new Player();
        private PickUps         potion;
        private Enemy           enemy_null              = new Enemy(new MoveTowardsPlayer(), EnemyType.Giant_Robot);

        //Map instance
        private Map             map                     = new Map();

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
            Tick = 0;
            gameOver = false;
            damageDistributionRatio = 0.1f;

            Console.CursorVisible = false;

            //Instantiate primary variables
            //------------------------------------- Player attributes
            player.CurrentHealth = 90;
            player.CurrentShield = 50;
            player.X = (int)GameCanvasLimit.Left + 6;
            player.Y = (int)GameCanvasLimit.Up + ((int)GameCanvasLimit.Down - (int)GameCanvasLimit.Up) / 2 - 2;

            //Count enemy's instances
            enemyList = new List<Enemy>();
            EnemyCount = 0;
            EnemyLimit = 2;

            //DummyEnemy for HUD displaying only
            enemy_null.Health = 100;
            enemy_null.Shield = 100;
            enemy_null.CurrentHealth = 0;
            enemy_null.CurrentShield = 0;
            enemy_null.Damage = 0;
            enemy_null.Name = " ------";

            //Map
            map.X = (int)GameCanvasLimit.Left + 1;
            map.Y = (int)GameCanvasLimit.Up + 1;

            //Spawn small Potion on start
            SpawnHealthPotionAtRunTime("small", (int)GameCanvasLimit.Right - 25, (int)GameCanvasLimit.Up + 14);

            //------------------------------------- Input manager display position
            inputManager.x = (int)GameCanvasLimit.Left;
            inputManager.y = (int)GameCanvasLimit.Down + 1;

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
                SpawnCommonRobotAtRuntime();
                if (EnemyCount > EnemyLimit)
                {
                    EnemyCount = EnemyLimit;
                }
            }

            //Perform collision check between player and every enemy on the screen
            for (int i = 0; i < enemyList.Count; i++)
            {
                displayManager.DrawObjectAt(enemyList[i].PreviousX , enemyList[i].PreviousY , enemyList[i].NegativeForm , enemyList[i].Color);
                displayManager.DrawObjectAt(enemyList[i].X         , enemyList[i].Y         , enemyList[i].PhysicalForm , enemyList[i].Color);
                enemyList[i].GetCurrentBoundaryCoordinates();

                gameLogic.CollisionCheckInsideBounds(map, enemyList[i]);
                if (inputManager.keyPressed)
                {
                    enemyList[i].MoveTowards(player);
                }
                
                //Enemy attack player when collides
                gameLogic.CollisionCheckOutsideBounds(enemyList[i], player);

                displayManager.ShowStatsHUD(50, 1, enemyList[i]);

                //Combat
                if (enemyList[i].AttackPermission)
                {
                    player.TakeDamageFrom(enemyList[i], damageDistributionRatio);
                    enemyList[i].TakeDamageFrom(player, damageDistributionRatio);

                    if (enemyList[i].CurrentHealth < 0)
                    {
                        displayManager.DrawObjectAt(enemyList[i].X, enemyList[i].Y, enemyList[i].NegativeForm, enemyList[i].Color);
                        KillEnemyAtRunTime(enemyList[i]);
                        displayManager.DrawAnimatedTextBox();
                        EnemyCount--;
                        //Release controls lock
                        player.BlockedHorizontally = BlockedDirection.None;
                        player.BlockedVertically = BlockedDirection.None;
                    }
                }
            }

            //Map had been drawn when the gameplay started
            gameLogic.CollisionCheckInsideBounds(map, player);

            gameLogic.CollisionCheckBetweenPlayerAndCanvasBoundaries(player);
            //Draw gameplay screens
            //------------------------------------------------ HUD
            Console.ForegroundColor = ConsoleColor.White;

            displayManager.ShowStatsHUD(1, 1, player);
            if (enemyList.Count == 0)
            {
                displayManager.ShowStatsHUD(50, 1, enemy_null);
            }

            //------------------------------------------------ Gameplay canvas
            Console.ForegroundColor = ConsoleColor.Yellow;
            displayManager.DrawRectangle((int)GameCanvasLimit.Left, (int)GameCanvasLimit.Up, GameCanvasLimit.Right - GameCanvasLimit.Left, GameCanvasLimit.Down - GameCanvasLimit.Up);

            //------------------------------------------------ Health Potion
            displayManager.DrawObjectAt(potion.X, potion.Y, potion.PhysicalForm, potion.Color);

            //Collision check between player and potion
            if (gameLogic.CollisionCheckTwoObjects(player, potion, false, (int)GameCanvasLimit.Right + 1, (int)GameCanvasLimit.Up + 6))
            {
                //Increase player current health
                DisplayManager.DrawHint((int)GameCanvasLimit.Left, (int)GameCanvasLimit.Down, $"{potion.Name} picked up!");
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

            if (player.CurrentLive < 0) gameOver = true;

            player.UpdateGameplayStatus();

            Tick++;
            Console.SetCursorPosition(0, 0);
            Console.WriteLine(Tick);
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
                        player.BlockedHorizontally = BlockedDirection.None; //                                         <= Assume that there is obstacle blocking player movement horizontally
                        player.BlockedVertically = BlockedDirection.None; //                                           <= Assume that there is obstacle blocking player movement vertically
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
                    generatedValue = GameLogic.random.Next((int)GameCanvasLimit.Left + 1, (int)GameCanvasLimit.Right);

                    if (generatedValue > (int)GameCanvasLimit.Right - cO.Width)
                    {
                        generatedValue = (int)GameCanvasLimit.Right - cO.Width - 1;
                    }
                    break;

                //Generate Y inside  gameplay canvas
                case false:
                    generatedValue = GameLogic.random.Next((int)GameCanvasLimit.Up + 1, (int)GameCanvasLimit.Down - 1);

                    if (generatedValue > (int)GameCanvasLimit.Down - cO.Height)
                    {
                        generatedValue = (int)GameCanvasLimit.Down - cO.Height - 1;
                    }
                    break;
            }
            return generatedValue;
        }

        private void SpawnCommonRobotAtRuntime()
        {
            enemyList.Add(new Enemy(new MoveTowardsPlayer(), EnemyType.Random_Robot));

            enemyList[EnemyCount].Name += EnemyCount;
            enemyList[EnemyCount].X = (int)GameCanvasLimit.Right - enemyList[EnemyCount].Width - 5;
            enemyList[EnemyCount].Y = (int)GameCanvasLimit.Up + 4;
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
                    potion = new PickUps(HealhPotion.Small);
                    potions.Add(potion);
                    break;
                case "large":
                    potion = new PickUps(HealhPotion.Large);
                    potions.Add(potion);
                    break;
            }

            potion.X = x;
            potion.Y = y;
        }
    }
}
