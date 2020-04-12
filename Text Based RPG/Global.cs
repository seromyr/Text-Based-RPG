using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text_Based_RPG
{
    // GAME OBJECT TYPES ----------------------------------------------------------------------------------------------- //
    enum BlockedDirection { None, Left, Right, Up, Down                                                                } //
    enum EnemyType        { Random_Robot, Common_Robot, Elite_Robot, Giant_Robot, Boss                                 } //
    enum Item             { Health_Potion_Sm, Health_Potion_Lg, Shield_Elixir_Sm, Shield_Elixir_Lg, DamageUp_Rune_Lg   } //
    enum MainMenu         { New, Continue, Quit, Credits                                                               } //
    enum GameState        { Splash, Main_Menu, Gameplay, Victory, Defeat, Quit                                         } //
    // ----------------------------------------------------------------------------------------------------------------- //
    static class Constant
    {
        public const int GAMEPLAY_CANVAS_LIMIT_LEFT = 1;
        public const int GAMEPLAY_CANVAS_LIMIT_RIGHT = 118;
        public const int GAMEPLAY_CANVAS_LIMIT_UP = 6;
        public const int GAMEPLAY_CANVAS_LIMIT_DOWN = 28;
        public const int VIEWPORT_WIDTH  = 115;
        public const int VIEWPORT_HEIGHT = 20;
    }
}
