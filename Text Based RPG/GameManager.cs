using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Text_Based_RPG.Behaviors;
using Text_Based_RPG.DisplaySystem;
using Text_Based_RPG.CharacterObjects;

namespace Text_Based_RPG
{
    class GameManager
    {
        //Game state
        public bool gameOver, gameWin;
        public GameState gameState;
        private float damageDistributionRatio;

        //Basically time since game start
        public static int Tick { get; set; }

        private List<Enemy> enemyList;
        public int EnemyCount { get; set; }
        public int EnemyLimit { get; set; }

        //Game mechanics instances
        private GameLogic       gameLogic               = new GameLogic();
        private DisplayManager  displayManager          = new DisplayManager();
        private GameUI          gameUI                  = new GameUI();
        private InputManager    inputManager            = new InputManager();

        //Character instances
        private Player          player                  = new Player();
        private List<Items>     pickups                 = new List<Items>();
        private Items[]         potion, elixir, rune;
        private Enemy           enemy_null              = new Enemy(new MoveTowardsPlayer(), EnemyType.Giant_Robot);
        /*
                                       _                           _                  
                                      | |                         | |                 
           ___    ___    _ __    ___  | |_   _ __   _   _    ___  | |_    ___    _ __ 
          / __|  / _ \  | '_ \  / __| | __| | '__| | | | |  / __| | __|  / _ \  | '__|
         | (__  | (_) | | | | | \__ \ | |_  | |    | |_| | | (__  | |_  | (_) | | |   
          \___|  \___/  |_| |_| |___/  \__| |_|     \__,_|  \___|  \__|  \___/  |_|   

        */
        public GameManager()
        {
            //Game loop initialize
            Console.CursorVisible = false;
            Tick = 0;
            gameOver = false;
            gameWin = false;
            damageDistributionRatio = 0.1f;

            //Count enemy's instances
            enemyList = new List<Enemy>();
            EnemyCount = 0;
            EnemyLimit = 0;

            //DummyEnemy for HUD displaying only
            enemy_null.Health = 100;
            enemy_null.Shield = 100;
            enemy_null.CurrentHealth = 0;
            enemy_null.CurrentShield = 0;
            enemy_null.Damage = 0;
            enemy_null.Name = " ------";

            //Potions array initialization
            potion = new Items[255];
            elixir = new Items[255];
            rune   = new Items[255];
        }
        /*
                                                       _             _                 __                          _     _                       
                                                      | |           | |               / _|                        | |   (_)                      
           __ _    __ _   _ __ ___     ___       ___  | |_    __ _  | |_    ___      | |_   _   _   _ __     ___  | |_   _    ___    _ __    ___ 
          / _` |  / _` | | '_ ` _ \   / _ \     / __| | __|  / _` | | __|  / _ \     |  _| | | | | | '_ \   / __| | __| | |  / _ \  | '_ \  / __|
         | (_| | | (_| | | | | | | | |  __/     \__ \ | |_  | (_| | | |_  |  __/     | |   | |_| | | | | | | (__  | |_  | | | (_) | | | | | \__ \
          \__, |  \__,_| |_| |_| |_|  \___|     |___/  \__|  \__,_|  \__|  \___|     |_|    \__,_| |_| |_|  \___|  \__| |_|  \___/  |_| |_| |___/
           __/ |                                                                                                                                 
          |___/                                                                                                                                  
                                                                                                                                                                                                                                    
         */
        public void GameStart()
        {
            gameUI.DrawSplashScreen();
            gameState = GameState.Main_Menu;
        }

        public void GameMainMenu()
        {
            gameUI.DrawMainMenuScreen();
            gameUI.DrawMainMenu();
            Console.Clear();
            gameState = GameState.Gameplay;
        }

        public void GameEnd()
        {
            gameUI.DrawGameOverScreen();
            gameState = GameState.Main_Menu;
        }

        public void RunLevelOne()
        {
            // ON START
            //Instantiate the map
            Map currentMap = new Map("Dungeon_01")
            {
                X = Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 1,
                Y = Constant.GAMEPLAY_CANVAS_LIMIT_UP + 1
            };

            //Gameplay canvas is drawn once to save the framerate
            Console.ForegroundColor = ConsoleColor.Yellow;
            displayManager.DrawRectangle(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT, Constant.GAMEPLAY_CANVAS_LIMIT_UP, Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - Constant.GAMEPLAY_CANVAS_LIMIT_LEFT, Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - Constant.GAMEPLAY_CANVAS_LIMIT_UP);

            //Map is drawn once to save the framerate
            displayManager.DrawObjectAt(currentMap.X, currentMap.Y, currentMap.PhysicalForm, currentMap.Color);

            //Instantiate Player attributes
            player.CurrentHealth = 30;
            player.CurrentShield = 1;
            player.X = Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 6;
            player.Y = Constant.GAMEPLAY_CANVAS_LIMIT_UP + (Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - Constant.GAMEPLAY_CANVAS_LIMIT_UP) / 2 - 2;

            //Display level objective of this level
            gameUI.DrawConfirmationBox(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + (Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - Constant.GAMEPLAY_CANVAS_LIMIT_LEFT) / 2 - 26,
                                       Constant.GAMEPLAY_CANVAS_LIMIT_UP + (Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - Constant.GAMEPLAY_CANVAS_LIMIT_UP) / 2 - 2,
                                       40,
                                       4,
                                       "Kill the enemy! This one can't move.");

            //Instantiate Enemy attributes
            EnemyLimit = 1;

            //Spawn some Potions on start
            {
                SpawnHealthPotionAtRunTime(Item.Health_Potion_Lg, 0, Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 23, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 2);
                displayManager.DrawObjectAt(potion[0].X, potion[0].Y, potion[0].PhysicalForm, potion[0].Color);
                SpawnHealthPotionAtRunTime(Item.Health_Potion_Lg, 1, Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 42, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 2);
                displayManager.DrawObjectAt(potion[1].X, potion[1].Y, potion[1].PhysicalForm, potion[1].Color);

                SpawnHealthPotionAtRunTime(Item.Health_Potion_Lg, 2, Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 59, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 2);
                displayManager.DrawObjectAt(potion[2].X, potion[2].Y, potion[2].PhysicalForm, potion[2].Color);
                SpawnHealthPotionAtRunTime(Item.Health_Potion_Lg, 3, Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 78, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 2);
                displayManager.DrawObjectAt(potion[3].X, potion[3].Y, potion[3].PhysicalForm, potion[3].Color);

                SpawnHealthPotionAtRunTime(Item.Health_Potion_Lg, 4, Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 23, Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - 5);
                displayManager.DrawObjectAt(potion[4].X, potion[4].Y, potion[4].PhysicalForm, potion[4].Color);
                SpawnHealthPotionAtRunTime(Item.Health_Potion_Lg, 5, Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 42, Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - 5);
                displayManager.DrawObjectAt(potion[5].X, potion[5].Y, potion[5].PhysicalForm, potion[5].Color);

                SpawnHealthPotionAtRunTime(Item.Health_Potion_Lg, 6, Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 59, Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - 5);
                displayManager.DrawObjectAt(potion[6].X, potion[6].Y, potion[6].PhysicalForm, potion[6].Color);
                SpawnHealthPotionAtRunTime(Item.Health_Potion_Lg, 7, Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 78, Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - 5);
                displayManager.DrawObjectAt(potion[7].X, potion[7].Y, potion[7].PhysicalForm, potion[7].Color);

                SpawnHealthPotionAtRunTime(Item.Health_Potion_Lg, 8, Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 23, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 10);
                displayManager.DrawObjectAt(potion[8].X, potion[8].Y, potion[8].PhysicalForm, potion[8].Color);
            }
            
            SpawnEnemyAtRuntime(Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - 10, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 9, EnemyType.Common_Robot, "[   Immobile Robot   ]");

            // ON UPDATE
            while (!gameOver || !gameWin)
            {
                //Draw Player at default position
                displayManager.DrawObjectAt(player.X, player.Y, player.PhysicalForm, player.Color);

                //Update player boundaries coordinates for the collision check
                player.GetCurrentBoundaryCoordinates();

                //Perform collision check between player and alive enemy on the screen
                for (int i = 0; i < enemyList.Count; i++)
                {
                    if (!enemyList[i].IsDead)
                    {
                        //Update enemy visual position
                        displayManager.DrawObjectAt(enemyList[i].PreviousX, enemyList[i].PreviousY, enemyList[i].NegativeForm, enemyList[i].Color);
                        displayManager.DrawObjectAt(enemyList[i].X, enemyList[i].Y, enemyList[i].PhysicalForm, enemyList[i].Color);
                        enemyList[i].GetCurrentBoundaryCoordinates();

                        gameLogic.CollisionCheckInsideBounds(currentMap, enemyList[i]);
                        if (inputManager.keyPressed)
                        {
                            //Enemy movement
                            //enemyList[i].MoveTowards(player);
                        }

                        //Enemy attack player when collides
                        gameLogic.CollisionCheckOutsideBounds(enemyList[i], player);

                        gameUI.ShowStatsHUD(50, 1, enemyList[i]);

                        //Combat
                        if (enemyList[i].AttackPermission)
                        {
                            player.TakeDamageFrom(enemyList[i], damageDistributionRatio);
                            enemyList[i].TakeDamageFrom(player, damageDistributionRatio);
                        }

                    }
                    else if (enemyList[i].IsDead)
                    {
                        gameUI.ShowStatsHUD(50, 1, enemyList[i]);
                        displayManager.DrawObjectAt(enemyList[i].X, enemyList[i].Y, enemyList[i].NegativeForm, enemyList[i].Color);
                        KillEnemyAtRunTime(enemyList[i]);

                        gameUI.DrawConfirmationBox(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + (Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - Constant.GAMEPLAY_CANVAS_LIMIT_LEFT) / 2 - 26,
                               Constant.GAMEPLAY_CANVAS_LIMIT_UP + (Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - Constant.GAMEPLAY_CANVAS_LIMIT_UP) / 2 - 2,
                               26,
                               4,
                               "Enemy has been killed.");

                        EnemyCount--;
                        if (EnemyCount == 0)
                        {
                            gameWin = true;
                        }

                        //Release controls lock
                        player.BlockedHorizontally = BlockedDirection.None;
                        player.BlockedVertically = BlockedDirection.None;
                    }
                }

                if (gameWin)
                {
                    gameUI.DrawTransitionEffect_Dissolve('█', ' ');
                    ClearPreviousLevelData();
                    break;
                }

                //Collision check
                gameLogic.CollisionCheckInsideBounds(currentMap, player);
                gameLogic.CollisionCheckBetweenPlayerAndCanvasBoundaries(player);

                //Draw gameplay screens
                //------------------------------------------------ HUD
                Console.ForegroundColor = ConsoleColor.White;

                gameUI.ShowStatsHUD(1, 1, player);
                if (enemyList.Count == 0)
                {
                    gameUI.ShowStatsHUD(50, 1, enemy_null);
                }

                //Collision check between player and potions
                for (int i = 0; i < pickups.Count; i++)
                {
                    if (gameLogic.CollisionCheckTwoObjects(player, potion[i], false, Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT + 1, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 6))
                    {
                        //Increase player current health
                        DisplayManager.WriteTextAt(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT, Constant.GAMEPLAY_CANVAS_LIMIT_DOWN, $"{potion[i].Name} picked up!");
                        player.GetHealthFrom(potion[i]);

                        //Clear and remove the health potion outside the gameplay canvas
                        displayManager.DrawObjectAt(potion[i].X, potion[i].Y, potion[i].NegativeForm, potion[i].Color);
                        potion[i].X = 0;
                        potion[i].Y = 0;
                    }
                }

                //Run player controller
                PlayerController();

                if (player.CurrentLive < 0) gameOver = true;

                player.UpdateGameplayStatus(3);

                Tick++;
                //Console.SetCursorPosition(0, 0);
                //Console.WriteLine(Tick);
            }

            //Console.Clear();
            if (gameWin)
            {
                
                
                //Display level objective of this level
                gameUI.DrawConfirmationBox(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + (Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - Constant.GAMEPLAY_CANVAS_LIMIT_LEFT) / 2 - 26,
                                           Constant.GAMEPLAY_CANVAS_LIMIT_UP + (Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - Constant.GAMEPLAY_CANVAS_LIMIT_UP) / 2 - 2,
                                           44,
                                           4,
                                           "No enemy left. Proceed to the next level");
            }
        }

        public void RunLeveTwo()
        {
            // ON START
            gameOver = false;
            gameWin = false;
            //Instantiate new map
            Map currentMap = new Map("Dungeon_05")
            {
                X = Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 1,
                Y = Constant.GAMEPLAY_CANVAS_LIMIT_UP + 1
            };

            //Gameplay canvas is drawn once to save the framerate
            Console.ForegroundColor = ConsoleColor.Yellow;
            displayManager.DrawRectangle(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT, Constant.GAMEPLAY_CANVAS_LIMIT_UP, Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - Constant.GAMEPLAY_CANVAS_LIMIT_LEFT, Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - Constant.GAMEPLAY_CANVAS_LIMIT_UP);

            //Draw map
            displayManager.DrawObjectAt(currentMap.X, currentMap.Y, currentMap.PhysicalForm, currentMap.Color);

            //Player stats bring over from previous level
            player.X = Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 4;
            player.Y = Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - 7;

            //Display level objective of this level
            gameUI.DrawConfirmationBox(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + (Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - Constant.GAMEPLAY_CANVAS_LIMIT_LEFT) / 2 - 26,
                                       Constant.GAMEPLAY_CANVAS_LIMIT_UP + (Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - Constant.GAMEPLAY_CANVAS_LIMIT_UP) / 2 - 2,
                                       52,
                                       4,
                                       "Kill the enemy. This one is fast and aggressive.");

            //Draw map
            displayManager.DrawObjectAt(currentMap.X, currentMap.Y, currentMap.PhysicalForm, currentMap.Color);

            //Instantiate Enemy attributes
            EnemyLimit = 1;

            //Modify some Potions on start
            {
                //respawn the new potions
                SpawnHealthPotionAtRunTime(Item.Health_Potion_Lg, 0, Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 23, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 2);
                displayManager.DrawObjectAt(potion[0].X, potion[0].Y, potion[0].PhysicalForm, potion[0].Color);
                SpawnHealthPotionAtRunTime(Item.Health_Potion_Lg, 1, Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 42, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 2);
                displayManager.DrawObjectAt(potion[1].X, potion[1].Y, potion[1].PhysicalForm, potion[1].Color);

                SpawnShieldElixirAtRunTime(Item.Shield_Elixir_Lg, 2, Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 24, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 11);
                displayManager.DrawObjectAt(elixir[2].X, elixir[2].Y, elixir[2].PhysicalForm, elixir[2].Color);
                SpawnShieldElixirAtRunTime(Item.Shield_Elixir_Lg, 3, Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 40, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 11);
                displayManager.DrawObjectAt(elixir[3].X, elixir[3].Y, elixir[3].PhysicalForm, elixir[3].Color);
            }

            SpawnEnemyAtRuntime(Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - 10, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 3, EnemyType.Elite_Robot, "[     Elite Robot    ]");

            // ON UPDATE
            while (!gameOver || !gameWin)
            {
                //Draw Player at default position
                displayManager.DrawObjectAt(player.X, player.Y, player.PhysicalForm, player.Color);

                //Update player boundaries coordinates for the collision check
                player.GetCurrentBoundaryCoordinates();

                //Perform collision check between player and alive enemy on the screen
                for (int i = 0; i < enemyList.Count; i++)
                {
                    if (!enemyList[i].IsDead)
                    {
                        //Update enemy visual position
                        displayManager.DrawObjectAt(enemyList[i].PreviousX, enemyList[i].PreviousY, enemyList[i].NegativeForm, enemyList[i].Color);
                        displayManager.DrawObjectAt(enemyList[i].X, enemyList[i].Y, enemyList[i].PhysicalForm, enemyList[i].Color);
                        enemyList[i].GetCurrentBoundaryCoordinates();

                        gameLogic.CollisionCheckInsideBounds(currentMap, enemyList[i]);
                        if (inputManager.keyPressed)
                        {
                            //Enemy movement
                            enemyList[i].MoveTowards(player);
                        }

                        //Enemy attack player when collides
                        gameLogic.CollisionCheckOutsideBounds(enemyList[i], player);

                        gameUI.ShowStatsHUD(50, 1, enemyList[i]);

                        //Combat
                        if (enemyList[i].AttackPermission)
                        {
                            player.TakeDamageFrom(enemyList[i], damageDistributionRatio);
                            enemyList[i].TakeDamageFrom(player, damageDistributionRatio);
                        }

                    }
                    else if (enemyList[i].IsDead)
                    {
                        gameUI.ShowStatsHUD(50, 1, enemyList[i]);
                        displayManager.DrawObjectAt(enemyList[i].X, enemyList[i].Y, enemyList[i].NegativeForm, enemyList[i].Color);
                        KillEnemyAtRunTime(enemyList[i]);

                        gameUI.DrawConfirmationBox(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + (Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - Constant.GAMEPLAY_CANVAS_LIMIT_LEFT) / 2 - 26,
                               Constant.GAMEPLAY_CANVAS_LIMIT_UP + (Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - Constant.GAMEPLAY_CANVAS_LIMIT_UP) / 2 - 2,
                               26,
                               4,
                               "Enemy has been killed.");

                        EnemyCount--;
                        if (EnemyCount == 0)
                        {
                            gameWin = true;
                        }

                        //Release controls lock
                        player.BlockedHorizontally = BlockedDirection.None;
                        player.BlockedVertically = BlockedDirection.None;
                    }
                }

                if (gameWin)
                {
                    gameUI.DrawTransitionEffect_Dissolve('█', ' ');
                    ClearPreviousLevelData();

                    break;
                }

                if (gameOver)
                {
                    gameUI.DrawTransitionEffect_Dissolve('█', ' ');
                    ClearPreviousLevelData();
                    break;
                }

                //Collision check
                gameLogic.CollisionCheckInsideBounds(currentMap, player);
                gameLogic.CollisionCheckBetweenPlayerAndCanvasBoundaries(player);

                //Draw gameplay screens
                //------------------------------------------------ HUD
                Console.ForegroundColor = ConsoleColor.White;

                gameUI.ShowStatsHUD(1, 1, player);
                if (enemyList.Count == 0)
                {
                    gameUI.ShowStatsHUD(50, 1, enemy_null);
                }

                //Collision check between player and potions
                for (int i = 0; i < pickups.Count; i++)
                {
                    if (potion[i] != null && gameLogic.CollisionCheckTwoObjects(player, potion[i], false, Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT + 1, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 6))
                    {
                        //Increase player current health
                        DisplayManager.WriteTextAt(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT, Constant.GAMEPLAY_CANVAS_LIMIT_DOWN, $"{potion[i].Name} picked up!");
                        player.GetHealthFrom(potion[i]);

                        //Clear and move the health potion outside the gameplay canvas
                        displayManager.DrawObjectAt(potion[i].X, potion[i].Y, potion[i].NegativeForm, potion[i].Color);
                        potion[i].X = 0;
                        potion[i].Y = 0;
                    }
                }

                //Collision check between player and elixirs
                for (int i = 0; i < pickups.Count; i++)
                {
                    if (elixir[i] != null && gameLogic.CollisionCheckTwoObjects(player, elixir[i], false, Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT + 1, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 6))
                    {
                        //Increase player current health
                        DisplayManager.WriteTextAt(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT, Constant.GAMEPLAY_CANVAS_LIMIT_DOWN, $"{elixir[i].Name} picked up!");
                        player.GetShieldFrom(elixir[i]);

                        //Clear and move the health potion outside the gameplay canvas
                        displayManager.DrawObjectAt(elixir[i].X, elixir[i].Y, elixir[i].NegativeForm, elixir[i].Color);
                        elixir[i].X = 0;
                        elixir[i].Y = 0;
                    }
                }

                //Run player controller
                PlayerController();

                if (player.CurrentLive < 0) gameOver = true;

                player.UpdateGameplayStatus(3);

                Tick++;
            }

            Console.Clear();
            if (gameWin)
            {
                gameUI.DrawTransitionEffect_Dissolve('█', ' ');
                //Display level objective of this level
                gameUI.DrawConfirmationBox(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + (Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - Constant.GAMEPLAY_CANVAS_LIMIT_LEFT) / 2 - 26,
                                           Constant.GAMEPLAY_CANVAS_LIMIT_UP + (Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - Constant.GAMEPLAY_CANVAS_LIMIT_UP) / 2 - 2,
                                           40,
                                           4,
                                           "Good work! Proceed to the next level");
            }
        }

        public void RunLevelThree()
        {
            // ON START
            gameOver = false;
            gameWin = false;
            //Instantiate new map
            Map currentMap = new Map("Dungeon_03")
            {
                X = Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 1,
                Y = Constant.GAMEPLAY_CANVAS_LIMIT_UP + 1
            };

            //Gameplay canvas is drawn once to save the framerate
            Console.ForegroundColor = ConsoleColor.Yellow;
            displayManager.DrawRectangle(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT, Constant.GAMEPLAY_CANVAS_LIMIT_UP, Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - Constant.GAMEPLAY_CANVAS_LIMIT_LEFT, Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - Constant.GAMEPLAY_CANVAS_LIMIT_UP);

            //Draw map
            displayManager.DrawObjectAt(currentMap.X, currentMap.Y, currentMap.PhysicalForm, currentMap.Color);

            //Player stats bring over from previous level
            player.X = Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 4;
            player.Y = Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - 7;

            //Display level objective of this level
            gameUI.DrawConfirmationBox(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + (Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - Constant.GAMEPLAY_CANVAS_LIMIT_LEFT) / 2 - 26,
                                       Constant.GAMEPLAY_CANVAS_LIMIT_UP + (Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - Constant.GAMEPLAY_CANVAS_LIMIT_UP) / 2 - 2,
                                       40,
                                       4,
                                       "Kill the boss. He is slow but strong");

            //Draw map
            displayManager.DrawObjectAt(currentMap.X, currentMap.Y, currentMap.PhysicalForm, currentMap.Color);

            //Instantiate Enemy attributes
            EnemyLimit = 1;

            //Modify some Potions on start
            {
                ////respawn the new pickup items
                SpawnHealthPotionAtRunTime(Item.Health_Potion_Lg, 0, Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 23, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 2);
                displayManager.DrawObjectAt(potion[0].X, potion[0].Y, potion[0].PhysicalForm, potion[0].Color);
                SpawnHealthPotionAtRunTime(Item.Health_Potion_Lg, 1, Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 42, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 2);
                displayManager.DrawObjectAt(potion[1].X, potion[1].Y, potion[1].PhysicalForm, potion[1].Color);

                SpawnShieldElixirAtRunTime(Item.Shield_Elixir_Lg, 2, Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 24, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 11);
                displayManager.DrawObjectAt(elixir[2].X, elixir[2].Y, elixir[2].PhysicalForm, elixir[2].Color);
                SpawnShieldElixirAtRunTime(Item.Shield_Elixir_Lg, 3, Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 40, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 11);
                displayManager.DrawObjectAt(elixir[3].X, elixir[3].Y, elixir[3].PhysicalForm, elixir[3].Color);

                SpawnDamageUpRuneAtRunTime(Item.DamageUp_Rune_Lg, 0, Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 40, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 11);
                displayManager.DrawObjectAt(rune[0].X, rune[0].Y, rune[0].PhysicalForm, rune[0].Color);
            }

            SpawnEnemyAtRuntime(Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - 20, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 10, EnemyType.Giant_Robot, "[     Big Fat Bot    ]");

            // ON UPDATE
            while (!gameOver || !gameWin)
            {
                //Draw Player at default position
                displayManager.DrawObjectAt(player.X, player.Y, player.PhysicalForm, player.Color);

                //Update player boundaries coordinates for the collision check
                player.GetCurrentBoundaryCoordinates();

                //Perform collision check between player and alive enemy on the screen
                for (int i = 0; i < enemyList.Count; i++)
                {
                    if (!enemyList[i].IsDead)
                    {
                        //Update enemy visual position
                        displayManager.DrawObjectAt(enemyList[i].PreviousX, enemyList[i].PreviousY, enemyList[i].NegativeForm, enemyList[i].Color);
                        displayManager.DrawObjectAt(enemyList[i].X, enemyList[i].Y, enemyList[i].PhysicalForm, enemyList[i].Color);
                        enemyList[i].GetCurrentBoundaryCoordinates();

                        gameLogic.CollisionCheckInsideBounds(currentMap, enemyList[i]);
                        if (inputManager.keyPressed)
                        {
                            //Enemy movement
                            enemyList[i].MoveTowards(player);
                        }

                        //Enemy attack player when collides
                        gameLogic.CollisionCheckOutsideBounds(enemyList[i], player);

                        gameUI.ShowStatsHUD(50, 1, enemyList[i]);

                        //Combat
                        if (enemyList[i].AttackPermission)
                        {
                            player.TakeDamageFrom(enemyList[i], damageDistributionRatio);
                            enemyList[i].TakeDamageFrom(player, damageDistributionRatio);
                        }
                    }
                    else if (enemyList[i].IsDead)
                    {
                        gameUI.ShowStatsHUD(50, 1, enemyList[i]);
                        displayManager.DrawObjectAt(enemyList[i].X, enemyList[i].Y, enemyList[i].NegativeForm, enemyList[i].Color);
                        KillEnemyAtRunTime(enemyList[i]);

                        gameUI.DrawConfirmationBox(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + (Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - Constant.GAMEPLAY_CANVAS_LIMIT_LEFT) / 2 - 26,
                               Constant.GAMEPLAY_CANVAS_LIMIT_UP + (Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - Constant.GAMEPLAY_CANVAS_LIMIT_UP) / 2 - 2,
                               26,
                               4,
                               "Big Fat Bot defeated!");

                        EnemyCount--;
                        if (EnemyCount == 0)
                        {
                            gameWin = true;
                        }

                        //Release controls lock
                        player.BlockedHorizontally = BlockedDirection.None;
                        player.BlockedVertically = BlockedDirection.None;
                    }
                }

                if (gameWin)
                {
                    gameUI.DrawTransitionEffect_Dissolve('█', ' ');
                    ClearPreviousLevelData();

                    break;
                }

                if (gameOver)
                {
                    gameUI.DrawTransitionEffect_Dissolve('█', ' ');
                    ClearPreviousLevelData();
                    break;
                }

                //Collision check
                gameLogic.CollisionCheckInsideBounds(currentMap, player);
                gameLogic.CollisionCheckBetweenPlayerAndCanvasBoundaries(player);

                //Draw gameplay screens
                //------------------------------------------------ HUD
                Console.ForegroundColor = ConsoleColor.White;

                gameUI.ShowStatsHUD(1, 1, player);
                if (enemyList.Count == 0)
                {
                    gameUI.ShowStatsHUD(50, 1, enemy_null);
                }

                //Collision check between player and potions
                for (int i = 0; i < pickups.Count; i++)
                {
                    if (potion[i] != null && gameLogic.CollisionCheckTwoObjects(player, potion[i], false, Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT + 1, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 6))
                    {
                        //Increase player current health
                        DisplayManager.WriteTextAt(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT, Constant.GAMEPLAY_CANVAS_LIMIT_DOWN, $"{potion[i].Name} picked up!");
                        player.GetHealthFrom(potion[i]);

                        //Clear and move the health potion outside the gameplay canvas
                        displayManager.DrawObjectAt(potion[i].X, potion[i].Y, potion[i].NegativeForm, potion[i].Color);
                        potion[i].X = 0;
                        potion[i].Y = 0;
                    }
                }

                //Collision check between player and elixirs
                for (int i = 0; i < pickups.Count; i++)
                {
                    if (elixir[i] != null && gameLogic.CollisionCheckTwoObjects(player, elixir[i], false, Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT + 1, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 6))
                    {
                        //Increase player current health
                        DisplayManager.WriteTextAt(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT, Constant.GAMEPLAY_CANVAS_LIMIT_DOWN, $"{elixir[i].Name} picked up!");
                        player.GetShieldFrom(elixir[i]);

                        //Clear and move the health potion outside the gameplay canvas
                        displayManager.DrawObjectAt(elixir[i].X, elixir[i].Y, elixir[i].NegativeForm, elixir[i].Color);
                        elixir[i].X = 0;
                        elixir[i].Y = 0;
                    }
                }

                //Collision check between player and runes
                for (int i = 0; i < pickups.Count; i++)
                {
                    if (rune[i] != null && gameLogic.CollisionCheckTwoObjects(player, rune[i], false, Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT + 1, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 6))
                    {
                        //Increase player current health
                        DisplayManager.WriteTextAt(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT, Constant.GAMEPLAY_CANVAS_LIMIT_DOWN, $"{rune[i].Name} picked up!");
                        player.GetDamageFrom(rune[i]);

                        //Clear and move the health potion outside the gameplay canvas
                        displayManager.DrawObjectAt(rune[i].X, rune[i].Y, rune[i].NegativeForm, rune[i].Color);
                        rune[i].X = 0;
                        rune[i].Y = 0;
                    }
                }

                //Run player controller
                PlayerController();

                if (player.CurrentLive < 0) gameOver = true;

                player.UpdateGameplayStatus(3);

                Tick++;
            }

            Console.Clear();
            if (gameWin)
            {
                gameUI.DrawTransitionEffect_Dissolve('█', ' ');
                //Display level objective of this level
                gameUI.DrawConfirmationBox(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + (Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - Constant.GAMEPLAY_CANVAS_LIMIT_LEFT) / 2 - 26,
                                           Constant.GAMEPLAY_CANVAS_LIMIT_UP + (Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - Constant.GAMEPLAY_CANVAS_LIMIT_UP) / 2 - 2,
                                           48,
                                           4,
                                           "Congratulations! You have finished the game");
                gameState = GameState.Main_Menu;
            }
        }

        private void PlayerController()
        {
            switch (inputManager.InputListener_Movement(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT, Constant.GAMEPLAY_CANVAS_LIMIT_DOWN + 1))
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

        private void SpawnEnemyAtRuntime(int x, int y, EnemyType enemyType, string name)
        {
            enemyList.Add(new Enemy(new MoveTowardsPlayer(), enemyType));

            enemyList[EnemyCount].Name = name;
            enemyList[EnemyCount].X = x;
            enemyList[EnemyCount].Y = y;
            EnemyCount++;
        }

        private void KillEnemyAtRunTime(Enemy enemy)
        {
            enemyList.Remove(enemy);
        }

        private void SpawnHealthPotionAtRunTime(Item item, int index, int x, int y)
        { 
            switch (item)
            {
                case Item.Health_Potion_Sm:
                    potion[index] = new Items(Item.Health_Potion_Sm);
                    pickups.Add(potion[index]);
                    break;
                case Item.Health_Potion_Lg:
                    potion[index] = new Items(Item.Health_Potion_Lg);
                    pickups.Add(potion[index]);
                    break;
            }
            potion[index].X = x;
            potion[index].Y = y;
        }

        private void SpawnShieldElixirAtRunTime(Item item, int index, int x, int y)
        {
            switch (item)
            {
                case Item.Shield_Elixir_Sm:
                    elixir[index] = new Items(Item.Shield_Elixir_Sm);
                    pickups.Add(elixir[index]);
                    break;
                case Item.Shield_Elixir_Lg:
                    elixir[index] = new Items(Item.Shield_Elixir_Lg);
                    pickups.Add(elixir[index]);
                    break;
            }
            elixir[index].X = x;
            elixir[index].Y = y;
        }

        private void SpawnDamageUpRuneAtRunTime(Item item, int index, int x, int y)
        {
            rune[index] = new Items(Item.DamageUp_Rune_Lg);
            pickups.Add(rune[index]);

            rune[index].X = x;
            rune[index].Y = y;
        }

        private void ClearPreviousLevelData()
        {
            //clear the list
            pickups.Clear();

            for (int i = 0; i < potion.Length; i++)
            {
                if (potion[i] != null)
                {
                    potion[i].X = 0;
                    potion[i].Y = 0;
                }
            }

            for (int i = 0; i < elixir.Length; i++)
            {
                if (elixir[i] != null)
                {
                    elixir[i].X = 0;
                    elixir[i].Y = 0;
                }
            }
        }
    }
}
