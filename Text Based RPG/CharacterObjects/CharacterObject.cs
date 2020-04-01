using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

//CHARACTER OBJECT SUPER CLASS
namespace Text_Based_RPG.CharacterObjects
{
    class CharacterObject
    {
        //DECLARATION AND ENCAPSULATION OF:
        //--------------------------------- Generic Battle Attributes
        private int _damage, _health, _shield, _currentHealth, _currentShield, _live, _currentLive;
        public  int   Damage                         { get { return _damage;              } set { _damage            = value; } }
        public  int   Health                         { get { return _health;              } set { _health            = value; } }
        public  int   CurrentHealth                  { get { return _currentHealth;       } set { _currentHealth     = value; } }
        public  int   Shield                         { get { return _shield;              } set { _shield            = value; } }
        public  int   CurrentShield                  { get { return _currentShield;       } set { _currentShield     = value; } }
        public  int   Live                           { get { return _live;                } set { _live              = value; } }
        public  int   CurrentLive                    { get { return _currentLive;          } set { _currentLive        = value; } }
        public  float ShieldRegenerationRate         { get;                                                                     }
        public  bool  ShieldRegenerationAllowed      { get;                                 set;                                }
        public  bool  IsDead                        { get;                                 set;                                }

        private int timeStampX, timeStampY;
        public  int TimeStampX                       { get { return timeStampX;           } set { timeStampX         = value; } }
        public  int TimeStampY                       { get { return timeStampY;           } set { timeStampY         = value; } }        

        //--------------------------------- Visual variables
        private int _x, _y, _previousX, _previousY, _speed, _height, _width;
        public  int X                                { get { return _x;                   } set { _x                 = value; } }
        public  int PreviousX                        { get { return _previousX;           } set { _previousX         = value; } }
        public  int Y                                { get { return _y;                   } set { _y                 = value; } }
        public  int PreviousY                        { get { return _previousY;           } set { _previousY         = value; } }
        public  int Speed                            { get { return _speed;               } set { _speed             = value; } }
        public  int Width                            { get { return _width;               } set { _width             = value; } }
        public  int Height                           { get { return _height;              } set { _height            = value; } }

        private string _name;
        public  string Name                          { get { return _name;                } set { _name              = value; } }

        private ConsoleColor _color;
        public  ConsoleColor Color                   { get { return _color;               } set { _color             = value; } }

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
        private bool _attackPermission;
        public  bool AttackPermission                { get { return _attackPermission;    } set { _attackPermission  = value; } }
        //------------------------------------------------------------------------------------------------------
        
        //Totally private variable
        private int _characterSteps;

        //Constructor
        public CharacterObject()
        {
            //Hmm, instantiate everything with dummy data so that there will not be any null reference
            
            //Game progress attributes
            _damage = 1;
            _health = 1;
            _shield = 1;
            _currentHealth = _health;
            _currentShield = _shield;
            _attackPermission = false;
            ShieldRegenerationRate = 0.01f;
            ShieldRegenerationAllowed = false;
            _live = 0;

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

            _characterSteps = GameManager.Tick;
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

        public void UpdateGameplayStatus(int steps)
        {
            //Well, only CurrentShield regenerates if it was allowed to
            //And regenerates shield every number of steps (or Ticks)
            if (GameManager.Tick == _characterSteps + steps)
            {
                _characterSteps = GameManager.Tick;
                if (ShieldRegenerationAllowed)
                {
                    if (_currentShield > 0)
                    {
                        //if ( timeStampX)
                        _currentShield += (int)Math.Round(ShieldRegenerationRate * _shield);
                        if (_currentShield > 100)
                        {
                            _currentShield = 100;
                        }
                    }
                }
            }

            if (_currentHealth > _health)
            {
                _currentHealth = _health;
            }
            //show log
            //Console.SetCursorPosition(5, 0);
            //Console.Write(_characterSteps);
        }

        public void TakeDamageFrom(CharacterObject characterObject, float damageDistributionRatio)
        {
            //Shield regenerates after a few moves
            //Damage applies to shield first
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

                CurrentShield += (int)(ShieldRegenerationRate * _shield);
                ShieldRegenerationAllowed = true;
            }

            if (_currentHealth <= 0)
            {
                _currentLive--;

                if (_currentLive > 0)
                {
                    //string text08 = "- Fatal hit! Player lost 01 live. Reviving...";
                    //if (showLog) GameManager.ShowLog(text08);
                    CurrentHealth = Health;
                    CurrentShield = Shield;
                }
                else
                {
                    //string text09 = "- Fatal hit. Your player died and unable to revive this time.";
                    //if (showLog) GameManager.ShowLog(text09);
                    IsDead = true;
                }
            }
        }

        public void GetHealthFrom(Items potion)
        {
            _currentHealth += potion._health;
            if (_currentHealth > _health)
            {
                _currentHealth = _health;
            }
        }

        public void GetShieldFrom(Items elixir)
        {
            _currentShield += elixir._shield;
            if (_currentShield > _shield)
            {
                _currentShield = _shield;
            }
        }

        public void GetDamageFrom(Items elixir)
        {
            _damage += elixir._damage;
        }
    }
}
