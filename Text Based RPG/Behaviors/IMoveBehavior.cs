using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Text_Based_RPG.CharacterObjects;

namespace Text_Based_RPG.Behaviors
{
    interface IMoveBehavior
    {
        //This object will move toward target character/object
        void Move(Player player);
    }
}
