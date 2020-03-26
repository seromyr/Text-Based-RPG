using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

//CHARACTER OBJECT SUPER CLASS
namespace Text_Based_RPG
{
    class CharacterObject
    {
        //DECLARATION AND ENCAPSULATION OF:
        //--------------------------------- Generic Battle Attributes
        private int damage, health, shield, currentHealth, currentShield, live, currentLive;
        public  int   Damage                         { get { return damage;              } set { damage              = value; } }
        public  int   Health                         { get { return health;              } set { health              = value; } }
        public  int   CurrentHealth                  { get { return currentHealth;       } set { currentHealth       = value; } }
        public  int   Shield                         { get { return shield;              } set { shield              = value; } }
        public  int   CurrentShield                  { get { return currentShield;       } set { currentShield       = value; } }
        public  int   Live                           { get { return live;                } set { live                = value; } }
        public  int   CurrentLive                    { get { return currentLive;         } set { currentLive         = value; } }
        public  float ShieldRegenerationRate         { get;                                                                     }
        public  bool  ShieldRegenerationAllowed      { get;                                set;                                 }

        private int timeStampX, timeStampY;
        public  int TimeStampX                       { get { return timeStampX;          } set { timeStampX          = value; } }
        public  int TimeStampY                       { get { return timeStampY;          } set { timeStampY          = value; } }        

        //--------------------------------- Visual variables
        private int _x, _y, _previousX, _previousY, _speed, _height, _width;
        public  int X                                { get { return _x;                   } set { _x                   = value; } }
        public  int PreviousX                        { get { return _previousX;           } set { _previousX           = value; } }
        public  int Y                                { get { return _y;                   } set { _y                   = value; } }
        public  int PreviousY                        { get { return _previousY;           } set { _previousY           = value; } }
        public  int Speed                            { get { return _speed;               } set { _speed               = value; } }
        public  int Width                            { get { return _width;               } set { _width               = value; } }
        public  int Height                           { get { return _height;              } set { _height              = value; } }

        private string _name;
        public  string Name                          { get { return _name;                } set { _name                = value; } }

        private ConsoleColor _color;
        public  ConsoleColor Color                   { get { return _color;               } set { _color               = value; } }

        private string[] _physicalForm, _deadForm, _negativeForm;
        public  string[] PhysicalForm                { get { return _physicalForm;        } }
        public  string[] DeadForm                    { get { return _deadForm;            } }
        public  string[] NegativeForm                { get { return _negativeForm;        } }

        //--------------------------------- Collision variables
        private int[] boundaryTopX, boundaryTopY, boundaryBotX, boundaryBotY, boundaryLeftX, boundaryLeftY, boundaryRightX, boundaryRightY;
        public  int[] BoundaryTopX                   { get { return boundaryTopX;        } set { boundaryTopX        = value; } }
        public  int[] BoundaryTopY                   { get { return boundaryTopY;        } set { boundaryTopY        = value; } }
        public  int[] BoundaryBotX                   { get { return boundaryBotX;        } set { boundaryBotX        = value; } }
        public  int[] BoundaryBotY                   { get { return boundaryBotY;        } set { boundaryBotY        = value; } }
        public  int[] BoundaryLeftX                  { get { return boundaryLeftX;       } set { boundaryLeftX       = value; } }
        public  int[] BoundaryLeftY                  { get { return boundaryLeftY;       } set { boundaryLeftY       = value; } }
        public  int[] BoundaryRightX                 { get { return boundaryRightX;      } set { boundaryRightX      = value; } }
        public  int[] BoundaryRightY                 { get { return boundaryRightY;      } set { boundaryRightY      = value; } }

        private BlockedDirection blockedHorizontally = new BlockedDirection();
        private BlockedDirection blockedVertically   = new BlockedDirection();
        public  BlockedDirection BlockedHorizontally { get { return blockedHorizontally; } set { blockedHorizontally = value; } }
        public  BlockedDirection BlockedVertically   { get { return blockedVertically;   } set { blockedVertically   = value; } }

        //--------------------------------- Combat variables
        private bool attackPermission;
        public  bool AttackPermission                { get { return attackPermission;    } set { attackPermission    = value; } }
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
            ShieldRegenerationRate = 0.01f;
            ShieldRegenerationAllowed = false;

            //Visual attributes
            _name = "-null-";
            _x = 0;
            _y = 0;
            _previousX = _x;
            _previousY = _y;
            _speed = 1;
            _color = ConsoleColor.White;
            _physicalForm = new string[] { " " };
            _negativeForm = new string[] { " " };
            _deadForm     = new string[] { " " };

            _height = _physicalForm.Length;
            _width  = GetWidth();

            BlockedHorizontally = BlockedDirection.None;
            BlockedVertically   = BlockedDirection.None;
        }

        //Load visual from file
        protected void GetPhysicalAndNegativeForm(string path)
        {
            _physicalForm = File.ReadAllLines(path, Encoding.ASCII);

            _negativeForm = new string[_physicalForm.Length];
            Array.Copy(_physicalForm, _negativeForm, _physicalForm.Length);

            for (int i = 0; i < _negativeForm.Length; i++)
            {
                StringBuilder sb = new StringBuilder(_negativeForm[i].Length);
                for (int j = 0; j < _negativeForm[i].Length; j++)
                {
                    sb.Append(' ');
                }
                _negativeForm[i] = sb.ToString();
            }
        }
 
        public int GetWidth()
        {
            int length = 0;
            int longesth = length;

            //Determine the longest element in the array
            for (int i = 0; i < _physicalForm.Length; i++)
            {
                foreach (char c in _physicalForm[i])
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

        public void UpdateGameplayStatus()
        {
            //Well, only CurrentShield regenerates if it was allowed too
            if (ShieldRegenerationAllowed)
            {
                if (CurrentShield > 0)
                {
                    //if ( timeStampX)
                    CurrentShield += (int)Math.Round(ShieldRegenerationRate * Shield);
                }
            }
        }

        public void TakeDamageFrom(CharacterObject characterObject, float damageDistributionRatio)
        {
            //CurrentHealth -= characterObject.Damage;

            //Shield regenerates every move
            //Damage go straight to shield first
            if (CurrentShield > 0)
            {
                if (ShieldRegenerationAllowed)
                {
                    //string text01 = "- Your shield regenerated " + (int)(ShieldRegenerationRate * Shield) + " units.";
                    //GameManager.ShowLog(text01);

                    CurrentShield += (int)(ShieldRegenerationRate * Shield);
                }

                //simple calculation when distributed damage is lesser than current health and shield
                //CurrentHealth -= (int)Math.Round(characterObject.Damage * damageDistributionRatio);
                CurrentShield -= (int)Math.Round(characterObject.Damage * (1 - damageDistributionRatio));

                //string text02 = "- Taken damage is split, " + (int)Math.Round(characterObject.Damage * damageDistributionRatio) + " went to Health, " + (int)Math.Round(dmg * (1 - damageDistributionRatio)) + " went to Shield.";
                //if (showLog) GameManager.ShowLog(text02);

                ShieldRegenerationAllowed = true;

                if (CurrentShield <= 0 && CurrentHealth > 0)
                {
                    CurrentShield = 0;
                    ShieldRegenerationAllowed = false;
                    //string text03 = "- Shield is depleted. You will take full damage next turn.";
                    //if (showLog) GameManager.ShowLog(text03);
                }

                if (CurrentShield >= Shield)
                {
                    ShieldRegenerationAllowed = false;
                    CurrentShield = Shield;
                    //string text04 = "- Shield is full!";
                    //if (showLog) GameManager.ShowLog(text04);
                }
            }
            else if (CurrentShield <= 0)
            {
                //Will take 100% damage when shield is depleted
                CurrentHealth -= characterObject.Damage;

                //Does not allow shield go below zero
                if (CurrentShield < 0)
                {
                    CurrentShield = 0;
                }

                //string text05 = "- Shield is depleted.";
                //string text06 = "- Player has taken " + dmg + " hit points.";
                //string text07 = "- Your shield regenerates " + (int)(shieldRegenRate * shield) + " points.";
                //if (showLog) GameManager.ShowLog(text05);
                //if (showLog) GameManager.ShowLog(text06);
                //if (showLog) GameManager.ShowLog(text07);

                CurrentShield += (int)(ShieldRegenerationRate * shield);
                ShieldRegenerationAllowed = true;
            }

            if (currentHealth <= 0)
            {
                CurrentHealth = Health;
                CurrentShield = Shield;
                CurrentLive--;

                if (currentLive > 0)
                {
                    //string text08 = "- Fatal hit! Player lost 01 live. Reviving...";
                    //if (showLog) GameManager.ShowLog(text08);
                }
                else
                {
                    //string text09 = "- Fatal hit. Your player died and unable to revive this time.";
                    //if (showLog) GameManager.ShowLog(text09);
                    //gameOver = true;
                }
            }
        }

        public void HealedBy(CharacterObject characterObject)
        {
            CurrentHealth += characterObject.Health;
        }
    }
}
