using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Text_Based_RPG.CharacterObjects
{
    class Map : CharacterObject
    {
        //Constructor
        public Map(string mapName)
        {
            LoadMap($@".\Visual Data\Maps\{mapName}.map");
            Name = mapName;
            Height = PhysicalForm.Length;
            Width = GetWidth();
        }
    }
}
