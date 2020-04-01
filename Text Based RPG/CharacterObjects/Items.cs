using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text_Based_RPG.CharacterObjects
{
    class Items : CharacterObject
    {
        public Items(Item item)
        {
            switch (item)
            {
                case Item.Health_Potion_Sm:
                    CreateSmallHealthPotion();
                    break;
                case Item.Health_Potion_Lg:
                    CreateLargeHealthPotion();
                    break;
                case Item.Shield_Elixir_Sm:
                    CreateSmallShieldElixir();
                    break;
                case Item.Shield_Elixir_Lg:
                    CreateLargeShieldElixir();
                    break;
                case Item.DamageUp_Rune_Lg:
                    CreateLargeDamageElixir();
                    break;
            }

            
            Height = PhysicalForm.Length;
            Width = GetWidth();
        }

        private void CreateSmallHealthPotion()
        {
            Color = ConsoleColor.Green;

            Health = 10;

            GetPhysicalAndNegativeForm(@".\Visual Data\Characters\Health_Potion_Small.char");

            Name = "Small Health Potion";
        }

        private void CreateLargeHealthPotion()
        {
            Color = ConsoleColor.Green;

            Health = 20;

            GetPhysicalAndNegativeForm(@".\Visual Data\Characters\Health_Potion_Large.char");

            Name = "Large Health Potion";
        }

        private void CreateSmallShieldElixir()
        {
            Color = ConsoleColor.DarkCyan;
            Shield = 10;

            GetPhysicalAndNegativeForm(@".\Visual Data\Characters\Shield_Potion_Small.char");

            Name = "Small Shield Elixir";
        }

        private void CreateLargeShieldElixir()
        {
            Color = ConsoleColor.DarkCyan;

            Shield = 20;

            GetPhysicalAndNegativeForm(@".\Visual Data\Characters\Shield_Potion_Large.char");

            Name = "Large Shield Elixir";
        }

        private void CreateLargeDamageElixir()
        {
            Color = ConsoleColor.DarkRed;

            Damage = 20;

            GetPhysicalAndNegativeForm(@".\Visual Data\Characters\DamageUp_Elixir_Large.char");

            Name = "Large Damage Up Rune";
        }
    }
}