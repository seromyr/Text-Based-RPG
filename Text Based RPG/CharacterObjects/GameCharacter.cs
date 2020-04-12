using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text_Based_RPG.CharacterObjects
{
    class GameCharacter : CharacterObject
    {
        //DECLARATION AND ENCAPSULATION OF:
        //--------------------------------- Generic Battle Attributes
        private int _damage, _health, _shield, _currentHealth, _currentShield, _live, _currentLive;
        public int               Damage                    { get { return _damage;             } set { _damage             = value; } }
        public int               Health                    { get { return _health;             } set { _health             = value; } }
        public int               CurrentHealth             { get { return _currentHealth;      } set { _currentHealth      = value; } }
        public int               Shield                    { get { return _shield;             } set { _shield             = value; } }
        public int               CurrentShield             { get { return _currentShield;      } set { _currentShield      = value; } }
        public int               Live                      { get { return _live;               } set { _live               = value; } }
        public int               CurrentLive               { get { return _currentLive;        } set { _currentLive        = value; } }
        public float             ShieldRegenerationRate    { get;                                                                     }
        public bool              ShieldRegenerationAllowed { get;                                set;                                 }
        public bool              IsDead                    { get;                                set;                                 }
        //--------------------------------- Combat variables
        private bool             _attackPermission;
        public bool              AttackPermission          { get { return _attackPermission;   } set { _attackPermission   = value; } }
        //--------------------------------- Visual variables
        private int              _previousX, _previousY;
        public int               PreviousX                 { get { return _previousX;          } set { _previousX          = value; } }
        public int               PreviousY                 { get { return _previousY;          } set { _previousY          = value; } }
        //--------------------------------- Collision variables
        private BlockedDirection blockedHorizontally       = new BlockedDirection();
        private BlockedDirection blockedVertically         = new BlockedDirection();
        public BlockedDirection  BlockedHorizontally       { get { return blockedHorizontally; } set { blockedHorizontally = value; } }
        public BlockedDirection  BlockedVertically         { get { return blockedVertically;   } set { blockedVertically   = value; } }

        public GameCharacter()
        {
            //fill properties with dummy data to avoid null reference
            _damage = 1;
            _health = 1;
            _shield = 1;
            _currentHealth = _health;
            _currentShield = _shield;
            
            ShieldRegenerationRate = 0.01f;
            ShieldRegenerationAllowed = false;
            _live = 0;

            _previousX = X;
            _previousY = Y;

            BlockedHorizontally = BlockedDirection.None;
            BlockedVertically = BlockedDirection.None;

            _attackPermission = false;
        }

        public void TakeDamageFrom(GameCharacter character, float damageDistributionRatio)
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
                CurrentShield -= (int)Math.Round(character.Damage * (1 - damageDistributionRatio));

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
                CurrentHealth -= character.Damage;

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
            _currentHealth += potion.Health;
            if (_currentHealth > _health)
            {
                _currentHealth = _health;
            }
        }

        public void GetShieldFrom(Items elixir)
        {
            _currentShield += elixir.Shield;
            if (_currentShield > _shield)
            {
                _currentShield = _shield;
            }
        }

        public void GetDamageFrom(Items elixir)
        {
            _damage += elixir.Damage;
        }
    }
}
