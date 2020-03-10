using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//CHARACTER OBJECT SUPER CLASS
namespace Text_Based_RPG
{
    enum BlockedDirection { None, Left, Right, Up, Down }

    class CharacterObject
    {
        //DECLARATION AND ENCAPSULATION OF:
        //--------------------------------- Generic Battle Attributes
        private int damage, health, shield, currentHealth, currentShield;
        public int Damage              { get { return damage;        } set { damage        = value; } }
        public int Health              { get { return health;        } set { health        = value; } }
        public int CurrentHealth       { get { return currentHealth; } set { currentHealth = value; } }
        public int Shield              { get { return shield;        } set { shield        = value; } }
        public int CurrentShield       { get { return currentShield; } set { currentShield = value; } }

        //--------------------------------- Visual variables
        private int x, y, prevX, prevY, speed, height, width;
        public  int X                   { get { return x;                } set { x                 = value; } }
        public  int PreviousX           { get { return prevX;            } set { prevX             = value; } }
        public  int Y                   { get { return y;                } set { y                 = value; } }
        public  int PreviousY           { get { return prevY;            } set { prevY             = value; } }
        public  int Speed               { get { return speed;            } set { speed             = value; } }
        public  int Width               { get { return width;            } set { width             = value; } }
        public  int Height              { get { return height;           } set { height            = value; } }

        private string name;
        public  string Name             { get { return name;             } set { name              = value; } }

        private ConsoleColor color;
        public  ConsoleColor Color      { get { return color;            } set { color             = value; } }

        private string[] physicalForm, deadForm, negativeForm;
        public  string[] PhysicalForm   { get { return physicalForm;     } set { physicalForm      = value; } }
        public  string[] DeadForm       { get { return deadForm;         } set { deadForm          = value; } }
        public  string[] NegativeForm   { get { return negativeForm;     } set { negativeForm      = value; } }

        //--------------------------------- Collision variables
        private int[] boundaryTopX, boundaryTopY, boundaryBotX, boundaryBotY, boundaryLeftX, boundaryLeftY, boundaryRightX, boundaryRightY;
        public  int[] BoundaryTopX      { get { return boundaryTopX;     } set { boundaryTopX      = value; } }
        public  int[] BoundaryTopY      { get { return boundaryTopY;     } set { boundaryTopY      = value; } }
        public  int[] BoundaryBotX      { get { return boundaryBotX;     } set { boundaryBotX      = value; } }
        public  int[] BoundaryBotY      { get { return boundaryBotY;     } set { boundaryBotY      = value; } }
        public  int[] BoundaryLeftX     { get { return boundaryLeftX;    } set { boundaryLeftX     = value; } }
        public  int[] BoundaryLeftY     { get { return boundaryLeftY;    } set { boundaryLeftY     = value; } }
        public  int[] BoundaryRightX    { get { return boundaryRightX;   } set { boundaryRightX    = value; } }
        public  int[] BoundaryRightY    { get { return boundaryRightY;   } set { boundaryRightY    = value; } }

        private BlockedDirection blockedHorizontally = new BlockedDirection();
        private BlockedDirection blockedVertically = new BlockedDirection();
        public BlockedDirection BlockedHorizontally { get { return blockedHorizontally;   } set { blockedHorizontally = value; } }
        public BlockedDirection BlockedVertically   { get { return blockedVertically;     } set { blockedVertically = value; } }

        //--------------------------------- Combat variables
        private bool attackPermission;
        public  bool AttackPermission   { get { return attackPermission; } set { attackPermission  = value; } }

        //------------------------------------------------------------------------------------------------------

        //Constructor
        public CharacterObject()
        {
            //Hmm, instantiate everything with dummy data so that there will not be any null reference

            //Game progress attributes
            damage = 1;
            health = 1;
            shield = 1;
            currentHealth = health;
            currentShield = shield;
            attackPermission = false;

            //Visual attributes
            name = "-null-";
            x = 0;
            y = 0;
            prevX = x;
            prevY = y;
            speed = 1;
            color = ConsoleColor.White;

            physicalForm = new string[] { " " };
            deadForm = physicalForm;

            negativeForm = new string[] { ""+(char)32 };

            height = physicalForm.Length;
            width = GetWidth();

            BlockedHorizontally = BlockedDirection.None;
            BlockedVertically = BlockedDirection.None;
        }

        public void ShowStatsHUD(int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.WriteLine(" Name    {0}",Name);
            Console.SetCursorPosition(x, y + 1);
            Console.WriteLine(" Health  [                    ] {0} ", CurrentHealth);

            Console.SetCursorPosition(x + 10, y + 1);
            for (int i = 0; i < 20; i++) Console.Write("-");

            //Avoid divided by zero
            if (Health != 0)
            {
                Console.SetCursorPosition(x + 10, y + 1);
                for (int j = 0; j < (CurrentHealth * 20 / Health); j++) Console.Write("▓");
            }

            Console.SetCursorPosition(x, y + 2);
            Console.WriteLine(" Shield  [                    ] {0} ", CurrentShield);

            Console.SetCursorPosition(x + 10, y + 2);
            for (int i = 0; i < 20; i++) Console.Write("-");

            //Avoid divided by zero
            if (Shield != 0)
            {
                Console.SetCursorPosition(x + 10, y + 2);
                for (int j = 0; j < (CurrentShield * 20 / Shield); j++) Console.Write("░");
            }

            Console.SetCursorPosition(x, y + 3);
            Console.WriteLine(" Damage  {0}", Damage);
        }

        public int GetWidth()
        {
            int length = 0;
            int longesth = length;

            //Determine the longest element in the array
            for (int i = 0; i < physicalForm.Length; i++)
            {
                foreach (char c in physicalForm[i])
                {
                    length++;
                }
                if (length > longesth)
                {
                    longesth = length;
                }

                //Reset the length each loop so that won't accumulate itself
                length = 0;
            }
            return longesth;
        }

        public void TakeDamage(CharacterObject characterObject)
        {
            CurrentHealth -= characterObject.Damage; 
        }
    }
}
