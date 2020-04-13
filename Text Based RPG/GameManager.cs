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
        public bool      gameLose, gameWin, battleFinish;
        public GameState gameState;
        private float    damageDistributionRatio;
        private int      battleCount;

        //Basically time since game start
        public static int Tick { get; set; }

        private List<Enemy> enemyList;
        public int EnemyCount { get; set; }

        //Game mechanics instances
        private GameLogic       gameLogic               = new GameLogic();
        private DisplayManager  displayManager          = new DisplayManager();
        private GameUI          gameUI                  = new GameUI();
        private InputManager    inputManager            = new InputManager();
        private Camera          camera;

        //Character instances
        private Player          player                  = new Player();
        private List<Items>     pickups                 = new List<Items>();
        private Items[]         potion, elixir, rune;
        private Enemy           enemy_null              = new Enemy(new MoveTowardsPlayer(), EnemyType.Giant_Robot);

        public struct BattleScenario
        {
            public string       map;
            public int          mapX;
            public int          mapY;
            public ConsoleColor color;
            public int          playerX;
            public int          playerY;
            public string       objective;
            public int          objective_l;
            public int          objective_h;
            public int          objective_indent;
            public string       report;
            public int          report_l;
            public int          report_h;
            public int          report_indent;
            public string       debrief;
            public int          debrief_l;
            public int          debrief_h;
            public int          debrief_indent;
            public bool         stationaryEnemy;
        }

        BattleScenario scenario_00, scenario_01, scenario_02, scenario_03, scenario_04;
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
            gameLose = false;
            gameWin = false;
            battleFinish = false;
            damageDistributionRatio = 0.1f;

            //Count enemy's instances
            enemyList = new List<Enemy>();
            EnemyCount = 0;

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

            battleCount = 0;

            scenario_00 = new BattleScenario();
            {
                scenario_00.map              = "Dungeon_01";
                scenario_00.mapX             = Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 1;
                scenario_00.mapY             = Constant.GAMEPLAY_CANVAS_LIMIT_UP   + 1;
                scenario_00.color            = ConsoleColor.White;
                scenario_00.playerX          = Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 6;
                scenario_00.playerY          = Constant.GAMEPLAY_CANVAS_LIMIT_UP + (Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - Constant.GAMEPLAY_CANVAS_LIMIT_UP) / 2 - 2;
                scenario_00.objective        = " Eliminate the enemy"; //ok
                scenario_00.objective_l      = 26;
                scenario_00.objective_h      = 4;
                scenario_00.objective_indent = 2;
                scenario_00.report           = "Enemy has been eliminated"; //ok
                scenario_00.report_l         = 30;
                scenario_00.report_h         = 4;
                scenario_00.report_indent    = 4;
                scenario_00.debrief          = null; //ok
                scenario_00.debrief_l        = 0;
                scenario_00.debrief_h        = 0;
                scenario_00.debrief_indent   = 0;
                scenario_00.stationaryEnemy  = true;
            }

            scenario_01 = new BattleScenario();
            {
                scenario_01.map              = "Dungeon_01";
                scenario_01.mapX             = Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 1;
                scenario_01.mapY             = Constant.GAMEPLAY_CANVAS_LIMIT_UP + 1;
                scenario_01.color            = ConsoleColor.White;
                scenario_01.playerX          = Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 6;
                scenario_01.playerY          = Constant.GAMEPLAY_CANVAS_LIMIT_UP + (Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - Constant.GAMEPLAY_CANVAS_LIMIT_UP) / 2 - 2;
                scenario_01.objective        = "This enemy can move. Terminate it!"; //ok
                scenario_01.objective_l      = 38;
                scenario_01.objective_h      = 4;
                scenario_01.objective_indent = 8;
                scenario_01.report           = "Enemy has been killed"; //ok
                scenario_01.report_l         = 26;
                scenario_01.report_h         = 4;
                scenario_01.report_indent    = 2;
                scenario_01.debrief          = "The blockade is clear"; //ok
                scenario_01.debrief_l        = 26;
                scenario_01.debrief_h        = 4;
                scenario_01.debrief_indent   = 2;
                scenario_01.stationaryEnemy  = false;
            }

            scenario_02 = new BattleScenario();
            {
                scenario_02.map              = "Dungeon_05";
                scenario_02.mapX             = Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 1;
                scenario_02.mapY             = Constant.GAMEPLAY_CANVAS_LIMIT_UP + 1;
                scenario_02.color            = ConsoleColor.DarkMagenta;
                scenario_02.playerX          = Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 4;
                scenario_02.playerY          = Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - 7;
                scenario_02.objective        = "Caution! This enemy is fast and aggressive."; //ok
                scenario_02.objective_l      = 48;
                scenario_02.objective_h      = 4;
                scenario_02.objective_indent = 13;
                scenario_02.report           = "  You won the battle"; //ok
                scenario_02.report_l         = 26;
                scenario_02.report_h         = 4;
                scenario_02.report_indent    = 2;
                scenario_02.debrief          = "What's inside that room?"; //ok
                scenario_02.debrief_l        = 28;
                scenario_02.debrief_h        = 4;
                scenario_02.debrief_indent   = 3;
                scenario_02.stationaryEnemy  = false;
            }

            scenario_03 = new BattleScenario();
            {
                scenario_03.map              = "Dungeon_03";
                scenario_03.mapX             = Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 1;
                scenario_03.mapY             = Constant.GAMEPLAY_CANVAS_LIMIT_UP + 1;
                scenario_03.color            = ConsoleColor.DarkMagenta;
                scenario_03.playerX          = Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 4;
                scenario_03.playerY          = Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - 7;
                scenario_03.objective        = "This one is a boss, but not that big"; //ok
                scenario_03.objective_l      = 40;
                scenario_03.objective_h      = 4;
                scenario_03.objective_indent = 9;
                scenario_03.report           = "    That was tough!"; //ok
                scenario_03.report_l         = 26;
                scenario_03.report_h         = 4;
                scenario_03.report_indent    = 2;
                scenario_03.debrief          = " Yep! Not BIG BOSS!"; //ok
                scenario_03.debrief_l        = 26;
                scenario_03.debrief_h        = 4;
                scenario_03.debrief_indent   = 2;
                scenario_03.stationaryEnemy  = false;
            }

            scenario_04 = new BattleScenario();
            {
                scenario_04.map              = "Dungeon_03";
                scenario_04.mapX             = Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 1;
                scenario_04.mapY             = Constant.GAMEPLAY_CANVAS_LIMIT_UP + 1;
                scenario_04.color            = ConsoleColor.DarkMagenta;
                scenario_04.playerX          = Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 4;
                scenario_04.playerY          = Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - 7;
                scenario_04.objective        = "This is it. Prepare for the final battle!";
                scenario_04.objective_l      = 46;
                scenario_04.objective_h      = 4;
                scenario_04.objective_indent = 12;
                scenario_04.report           = "      You made it!";
                scenario_04.report_l         = 26;
                scenario_04.report_h         = 4;
                scenario_04.report_indent    = 2;
                scenario_04.debrief          = "Congratulations! You are now BIG BOSS";
                scenario_04.debrief_l        = 42;
                scenario_04.debrief_h        = 4;
                scenario_04.debrief_indent   = 11;
                scenario_04.stationaryEnemy  = false;
            }
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

        public void GameEndVictorious()
        {
            gameUI.DrawGameOverScreen();
            gameState = GameState.Main_Menu;
        }

        public void GameEndDefeated()
        {
            gameUI.DrawDefeatedScreen();
            gameState = GameState.Main_Menu;
        }

        public void GameLoop()
        {
            //ON START
            //Instantiate the viewport including the map
            camera = new Camera("World", ConsoleColor.White);

            //Load up enemy data
            SpawnEnemyAtRuntime(Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - 10, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 9 , EnemyType.Common_Robot, "[    Common Robot    ]");
            SpawnEnemyAtRuntime(Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - 10, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 9 , EnemyType.Common_Robot, "[Another Common Robot]");
            SpawnEnemyAtRuntime(Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - 10, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 3 , EnemyType.Elite_Robot , "[     Elite Robot    ]");
            SpawnEnemyAtRuntime(Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - 20, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 10, EnemyType.Giant_Robot , "[     Big Fat Bot    ]");
            SpawnEnemyAtRuntime(Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - 20, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 10, EnemyType.Boss        , "[      BIG BOSS      ]");

            //load items, enemies into camera feed
            camera.AddObject(enemyList[0].MapForm, 118, 22 );
            camera.AddObject(enemyList[1].MapForm, 42 , 10 );
            camera.AddObject(enemyList[2].MapForm, 16 , 8  );
            camera.AddObject(enemyList[3].MapForm, 165, 10 );
            camera.AddObject(enemyList[4].MapForm, 145, 61 );
            camera.AddObject('⌐'                 , 13 , 5  );
            camera.AddObject('O'                 , 212, 42 );

            //Gameplay canvas
            gameUI.DrawGameplayCanvas();

            //Instantiate Player attributes
            player.CurrentHealth = 30;
            player.CurrentShield = 1;

            //-- Player start location is at the center of the map
            player.X = Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + (int)Math.Round(Constant.VIEWPORT_WIDTH  / 2.0, 0) + 1;
            player.Y = Constant.GAMEPLAY_CANVAS_LIMIT_UP   + (int)Math.Round(Constant.VIEWPORT_HEIGHT / 2.0, 0) + 1;
            //Display level objective of this level
            gameUI.DrawConfirmationBox(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + (Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - Constant.GAMEPLAY_CANVAS_LIMIT_LEFT) / 2 - 26,
                                       Constant.GAMEPLAY_CANVAS_LIMIT_UP   + (Constant.GAMEPLAY_CANVAS_LIMIT_DOWN  - Constant.GAMEPLAY_CANVAS_LIMIT_UP)   / 2 - 2 ,
                                       28,
                                       4,
                                       "Find and defeat Big Boss", //ok
                                       3);
            camera.Update();
            //camera.TakeSnapshot();

            while (!gameLose || !gameWin)
            {
                //Show player stats
                gameUI.ShowStatsHUD(1, 1, player);

                //Draw Player at default position
                displayManager.DrawObjectAt(player.X, player.Y, player.MapForm.ToString(), player.Color);

                //Collision check
                gameLogic.CollisionCheckInsideCameraViewport(camera, player);
                gameLogic.CollisionCheckBetweenPlayerAndCanvasBoundaries(player);

                //Run player controller
                PlayerController2();

                //Update camera
                //camera.TakeSnapshot();
                camera.Update();

                //Non-enemy collision check on world map
                //------------------------- Flip the switch
                if (gameLogic.ObjectCollisionCheckOnWorldMap(camera, player, '⌐'))
                {
                    gameUI.DrawConfirmationBox(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + (Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - Constant.GAMEPLAY_CANVAS_LIMIT_LEFT) / 2 - 26,
                                               Constant.GAMEPLAY_CANVAS_LIMIT_UP   + (Constant.GAMEPLAY_CANVAS_LIMIT_DOWN  - Constant.GAMEPLAY_CANVAS_LIMIT_UP)   / 2 - 2 ,
                                               30,
                                               4,
                                               "A door is opened somewhere", //ok
                                               5);
                    camera.RemoveObject(13, 5);
                    camera.RemoveObject(147, 21);
                    camera.RemoveObject(148, 21);
                    camera.RemoveObject(147, 22);
                    camera.RemoveObject(148, 22);
                }
                //------------------------- Mega buff
                if (gameLogic.ObjectCollisionCheckOnWorldMap(camera, player, 'O'))
                {
                    gameUI.DrawConfirmationBox(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + (Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - Constant.GAMEPLAY_CANVAS_LIMIT_LEFT) / 2 - 26,
                                               Constant.GAMEPLAY_CANVAS_LIMIT_UP   + (Constant.GAMEPLAY_CANVAS_LIMIT_DOWN  - Constant.GAMEPLAY_CANVAS_LIMIT_UP)   / 2 - 2 ,
                                               36,
                                               4,
                                               "You received an powerful upgrade", //ok
                                               7);
                    camera.RemoveObject(212, 42);
                    player.Health = 100;
                    player.CurrentHealth = 100;
                    player.CurrentShield = 100;
                    player.Damage += 10;
                }

                //Battle check
                if (enemyList.Count != 0 && gameLogic.BattleCheckOnWorldMap(camera, player, enemyList[0]))
                {
                    //Display level objective of this level
                    gameUI.DrawConfirmationBox(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + (Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - Constant.GAMEPLAY_CANVAS_LIMIT_LEFT) / 2 - 26,
                                               Constant.GAMEPLAY_CANVAS_LIMIT_UP   + (Constant.GAMEPLAY_CANVAS_LIMIT_DOWN  - Constant.GAMEPLAY_CANVAS_LIMIT_UP)   / 2 - 2 ,
                                               28,
                                               4,
                                               "You encountered an enemy", //ok
                                               3); 
                    gameUI.DrawTransitionEffect_Dissolve('█', ' ');

                    switch (battleCount)
                    {
                        case 0:
                            StartBattle(scenario_00);                            
                            break;

                        case 1:
                            StartBattle(scenario_01);
                            camera.RemoveObject(41, 16);
                            camera.RemoveObject(42, 16);
                            camera.RemoveObject(43, 16);
                            break;

                        case 2:
                            StartBattle(scenario_02);
                            camera.RemoveObject(15, 6);
                            camera.RemoveObject(16, 6);
                            camera.RemoveObject(17, 6);
                            break;

                        case 3:
                            StartBattle(scenario_03);
                            camera.RemoveObject(187, 21);
                            camera.RemoveObject(188, 21);
                            camera.RemoveObject(187, 22);
                            camera.RemoveObject(188, 22);
                            break;
                        case 4:
                            StartBattle(scenario_04);                            
                            gameWin  = true;
                            break;
                    }
                    
                    //Remove defeated enemy data
                    camera.RemoveObject(enemyList[0].MapNegativeForm);
                    RemoveEnemyInstance(enemyList[0]);
                    battleFinish = false;
                    

                    //Reset player map posistion
                    player.X = Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + (int)Math.Round(Constant.VIEWPORT_WIDTH  / 2.0, 0)  + 1;
                    player.Y = Constant.GAMEPLAY_CANVAS_LIMIT_UP   + (int)Math.Round(Constant.VIEWPORT_HEIGHT / 2.0, 0)  + 1;

                    //Redrawn gameplay canvas
                    gameUI.DrawGameplayCanvas();
                }
                if (gameLose)
                {
                    gameState = GameState.Defeat;
                    ResetGameData();
                    break;
                }

                if (gameWin)
                {
                    gameState = GameState.Victory;
                    ResetGameData();
                    break;
                }
            }
            Console.Clear();
        }

        public void StartBattle(BattleScenario scenario)
        {
            // ON START
            //Instantiate the map
            Map currentMap = new Map(scenario.map)
            {
                X = scenario.mapX,
                Y = scenario.mapY
            };
            currentMap.Color = scenario.color;

            //Gameplay canvas
            gameUI.DrawGameplayCanvas();

            //Set player coordinate in battle map
            player.X = scenario.playerX;
            player.Y = scenario.playerY;

            //Display level objective of this level
            gameUI.DrawConfirmationBox(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + (Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - Constant.GAMEPLAY_CANVAS_LIMIT_LEFT) / 2 - 26,
                                       Constant.GAMEPLAY_CANVAS_LIMIT_UP   + (Constant.GAMEPLAY_CANVAS_LIMIT_DOWN  - Constant.GAMEPLAY_CANVAS_LIMIT_UP)   / 2 - 2 ,
                                       scenario.objective_l                                                                                                       ,
                                       scenario.objective_h                                                                                                       ,
                                       scenario.objective                                                                                                         ,
                                       scenario.objective_indent                                                                                                  );

            //Map is drawn once to save the framerate
            displayManager.DrawObjectAt(currentMap.X, currentMap.Y, currentMap.PhysicalForm, currentMap.Color);

            //Spawn some power up items on start
            LoadPowerUpSet(battleCount);

            // ON UPDATE
            while (!battleFinish)
            {
                //Draw Player at default position
                displayManager.DrawObjectAt(player.X, player.Y, player.PhysicalForm, player.Color);

                //Update player boundaries coordinates for the collision check
                player.GetCurrentBoundaryCoordinates();

                //Perform collision check between player and enemy in battle screen
                if (!enemyList[0].IsDead)
                {
                    //Update enemy visual position
                    displayManager.DrawObjectAt(enemyList[0].PreviousX, enemyList[0].PreviousY, enemyList[0].NegativeForm, enemyList[0].Color);
                    displayManager.DrawObjectAt(enemyList[0].X        , enemyList[0].Y        , enemyList[0].PhysicalForm, enemyList[0].Color);
                    enemyList[0].GetCurrentBoundaryCoordinates();

                    gameLogic.CollisionCheckInsideBounds(currentMap, enemyList[0]);
                    if (inputManager.keyPressed && !scenario.stationaryEnemy)
                    {
                        //Enemy movement
                        enemyList[0].MoveTowards(player);
                    }

                    //Enemy attack player when collides
                    gameLogic.CollisionCheckOutsideBounds(enemyList[0], player);

                    gameUI.ShowStatsHUD(50, 1, enemyList[0]);

                    //Combat
                    if (enemyList[0].AttackPermission)
                    {
                        player.TakeDamageFrom(enemyList[0], damageDistributionRatio);
                        enemyList[0].TakeDamageFrom(player, damageDistributionRatio);
                    }
                }
                else if (enemyList[0].IsDead)
                {
                    gameUI.ShowStatsHUD(50, 1, enemyList[0]);
                    displayManager.DrawObjectAt(enemyList[0].X, enemyList[0].Y, enemyList[0].NegativeForm, enemyList[0].Color);
                    displayManager.DrawObjectAt(enemyList[0].X, enemyList[0].Y, enemyList[0].DeadForm, enemyList[0].Color);
                        

                    gameUI.DrawConfirmationBox(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + (Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - Constant.GAMEPLAY_CANVAS_LIMIT_LEFT) / 2 - 26,
                                               Constant.GAMEPLAY_CANVAS_LIMIT_UP   + (Constant.GAMEPLAY_CANVAS_LIMIT_DOWN  - Constant.GAMEPLAY_CANVAS_LIMIT_UP)   / 2 - 2 ,
                                               scenario.report_l                                                                                                          ,
                                               scenario.report_h                                                                                                          ,
                                               scenario.report                                                                                                            ,
                                               scenario.report_indent                                                                                                     );

                    battleFinish = true;

                    //Release controls lock
                    player.BlockedHorizontally = BlockedDirection.None;
                    player.BlockedVertically   = BlockedDirection.None;
                }

                if (player.CurrentLive < 0)
                {
                    gameUI.DrawConfirmationBox(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + (Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - Constant.GAMEPLAY_CANVAS_LIMIT_LEFT) / 2 - 26,
                                               Constant.GAMEPLAY_CANVAS_LIMIT_UP + (Constant.GAMEPLAY_CANVAS_LIMIT_DOWN - Constant.GAMEPLAY_CANVAS_LIMIT_UP) / 2 - 2,
                                               26,
                                               4,
                                               "Oops! You are defeated",
                                               3);

                    //battleFinish = true;
                    gameUI.DrawTransitionEffect_Dissolve('█', ' ');
                    ClearPreviousLevelData();
                    gameLose = true;
                    break;
                }

                if (battleFinish)
                {
                    battleCount++;
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

                //Collision check between player and power ups
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

                player.UpdateGameplayStatus(3);

                Tick++;
            }

            if (battleFinish && scenario.debrief != null)
            {
                //Display level objective of this level
                gameUI.DrawConfirmationBox(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + (Constant.GAMEPLAY_CANVAS_LIMIT_RIGHT - Constant.GAMEPLAY_CANVAS_LIMIT_LEFT) / 2 - 26,
                                           Constant.GAMEPLAY_CANVAS_LIMIT_UP   + (Constant.GAMEPLAY_CANVAS_LIMIT_DOWN  - Constant.GAMEPLAY_CANVAS_LIMIT_UP)   / 2 - 2 ,
                                           scenario.debrief_l                                                                                                         ,
                                           scenario.debrief_h                                                                                                         ,
                                           scenario.debrief                                                                                                           ,  
                                           scenario.debrief_indent                                                                                                    );
            }
        }

        private void LoadPowerUpSet(int number)
        {
            switch (number)
            {
                case 0:
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
                    break;

                case 1:
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
                    break;

                case 2:
                    {
                        SpawnHealthPotionAtRunTime(Item.Health_Potion_Lg, 0, Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 23, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 2);
                        displayManager.DrawObjectAt(potion[0].X, potion[0].Y, potion[0].PhysicalForm, potion[0].Color);
                        SpawnHealthPotionAtRunTime(Item.Health_Potion_Lg, 1, Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 42, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 2);
                        displayManager.DrawObjectAt(potion[1].X, potion[1].Y, potion[1].PhysicalForm, potion[1].Color);

                        SpawnShieldElixirAtRunTime(Item.Shield_Elixir_Lg, 2, Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 24, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 11);
                        displayManager.DrawObjectAt(elixir[2].X, elixir[2].Y, elixir[2].PhysicalForm, elixir[2].Color);
                        SpawnShieldElixirAtRunTime(Item.Shield_Elixir_Lg, 3, Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 40, Constant.GAMEPLAY_CANVAS_LIMIT_UP + 11);
                        displayManager.DrawObjectAt(elixir[3].X, elixir[3].Y, elixir[3].PhysicalForm, elixir[3].Color);
                    }
                    break;

                case 3:
                    {
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
                    break;
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

        private void PlayerController2()
        {
            switch (inputManager.InputListener_Movement(Constant.GAMEPLAY_CANVAS_LIMIT_LEFT, Constant.GAMEPLAY_CANVAS_LIMIT_DOWN + 1))
            {
                case 1: //LEFT
                    if (player.BlockedHorizontally == BlockedDirection.Left) break;
                    else
                    {
                        
                        camera.ViewportDefaultX -= player.Speed;           // <= Move the map with speed
                        player.BlockedHorizontally = BlockedDirection.None;// <= Assume that there is obstacle blocking player movement horizontally
                        player.BlockedVertically = BlockedDirection.None;  // <= Assume that there is obstacle blocking player movement vertically
                    }
                    break;
                case 2: //RIGHT
                    if (player.BlockedHorizontally == BlockedDirection.Right) break;
                    else
                    {
                        
                        camera.ViewportDefaultX += player.Speed;
                        player.BlockedHorizontally = BlockedDirection.None;
                        player.BlockedVertically = BlockedDirection.None;
                    }
                    break;
                case 3: //UP
                    if (player.BlockedVertically == BlockedDirection.Up) break;
                    else
                    {
                        
                        camera.ViewportDefaultY -= player.Speed;
                        player.BlockedHorizontally = BlockedDirection.None;
                        player.BlockedVertically = BlockedDirection.None;
                    }
                    break;
                case 4: //DOWN
                    if (player.BlockedVertically == BlockedDirection.Down) break;
                    else
                    {
                        
                        camera.ViewportDefaultY += player.Speed;
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
            enemyList[EnemyCount].PreviousX = x;
            enemyList[EnemyCount].PreviousY = y;
            EnemyCount++;
        }

        private void RemoveEnemyInstance(Enemy enemy)
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

        private void ResetGameData()
        {
            enemyList.Clear();
            EnemyCount = 0;
            battleCount = 0;
            gameLose = false;
            gameWin = false;

            player.Health = 100;
            player.CurrentHealth = 30;
            player.Shield = 100;
            player.CurrentShield = 1;
            player.Damage = 5;
            player.CurrentLive = player.Live = 3;
            player.ShieldRegenerationAllowed = true;
        }
    }
}
