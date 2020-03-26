using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text_Based_RPG
{
    class PickUps : CharacterObject
    {
        public PickUps(HealhPotion potion)
        {
            switch (potion)
            {
                case HealhPotion.Small: //Or 0, but it is for reading comprehension
                    CreateSmallPotion();
                    break;
                case HealhPotion.Large:
                    CreateLargePotion(); //Or 1, but it is for reading comprehension
                    break;
            }

            Color = ConsoleColor.Green;
            Height = PhysicalForm.Length;
            Width = GetWidth();
        }

        private void CreateSmallPotion()
        {
            Health = 10;

            GetPhysicalAndNegativeForm(@".\Visual Data\Characters\Potion_Small.char");

            Name = "Small Health Potion";
        }

        private void CreateLargePotion()
        {
            Health = 20;

            GetPhysicalAndNegativeForm(@".\Visual Data\Characters\Potion_Large.char");

            Name = "Large Health Potion";
        }
    }
}