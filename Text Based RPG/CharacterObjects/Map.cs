using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Text_Based_RPG
{
    class Map : CharacterObject
    {
        private Random random = new Random();
        private string[] map_01, map_02, map_03;

        //Constructor
        public Map()
        {
            //Load map from file
            map_01 = File.ReadAllLines(@".\Visual Data\Maps\Dungeon_01.map", Encoding.ASCII);
            map_02 = File.ReadAllLines(@".\Visual Data\Maps\Dungeon_02.map", Encoding.ASCII);
            map_03 = File.ReadAllLines(@".\Visual Data\Maps\Dungeon_03.map", Encoding.ASCII);

            GetPhysicalAndNegativeForm(@".\Visual Data\Maps\Dungeon_03.map");
            Name = "map";
            Height = PhysicalForm.Length;
            Width = GetWidth();
        }

        //Deprecated
        private string[] LoadNewMap()
        {
            string[] currentMap;
            
            if (random.Next(0, 2) == 0)
            {
                currentMap = map_01;
            }

            else currentMap = map_02;

            return currentMap;
        }
    }
}
