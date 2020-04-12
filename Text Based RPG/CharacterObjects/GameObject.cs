using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text_Based_RPG.CharacterObjects
{
    class GameObject : CharacterObject
    {
        protected int _health, _shield, _damage;
        public int Damage { get { return _damage; } }
        public int Health { get { return _health; } }
        public int Shield { get { return _shield; } }
    }
}
