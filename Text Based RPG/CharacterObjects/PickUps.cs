using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text_Based_RPG
{
    enum HealhPotion { small, large }
    class PickUps : CharacterObject
    {
        public PickUps(HealhPotion potion)
        {
            HealhPotion healhPotion = new HealhPotion();
            healhPotion = potion;
            switch (potion)
            {
                case HealhPotion.small: //Or 0, but it is for reading comprehension
                    CreateSmallPotion();
                    break;
                case HealhPotion.large:
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

            PhysicalForm = new string[]
            {
                "┌┐",
                "└┘"
            };

            NegativeForm = new string[]
            {
                @"" + (char)32 + (char)32,
                @"" + (char)32 + (char)32
            };

            Name = "Small Health Potion";
        }

        private void CreateLargePotion()
        {
            Health = 20;

            PhysicalForm = new string[]
            {

                "┌┼┐",
                "│││",
                "└─┘"
            };

            NegativeForm = new string[]
            {
                @"" + (char)32+ (char)32+ (char)32,
                @"" + (char)32+ (char)32+ (char)32,
                @"" + (char)32+ (char)32+ (char)32,
            };

            Name = "Large Health Potion";
        }
    }
}

/*
    ".-.-.",
    "'. .'",
    "  `"
*/
