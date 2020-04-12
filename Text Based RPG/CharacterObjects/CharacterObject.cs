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
        //--------------------------------- Visual variables
        private int          _x, _y, _speed, _height, _width;
        public  int          X               { get { return _x;             } set { _x             = value; } }
        public  int          Y               { get { return _y;             } set { _y             = value; } }
        public  int          Speed           { get { return _speed;         } set { _speed         = value; } }
        public  int          Width           { get { return _width;         } set { _width         = value; } }
        public  int          Height          { get { return _height;        } set { _height        = value; } }

        private string       _name;
        public  string       Name            { get { return _name;          } set { _name          = value; } }

        private ConsoleColor _color;
        public  ConsoleColor Color           { get { return _color;         } set { _color         = value; } }

        private string[]     _physicalForm, _deadForm, _negativeForm;
        public  string[]     PhysicalForm    { get { return _physicalForm;                                  } }
        public  string[]     DeadForm        { get { return _deadForm;                                      } }
        public  string[]     NegativeForm    { get { return _negativeForm;                                  } }
                                                                                                           
        public char          _mapForm, _mapNegativeForm;                                                   
        public char MapForm                  { get { return _mapForm;                                       } }
        public char MapNegativeForm          { get { return _mapNegativeForm;                               } }
        //--------------------------------- Collision variables
        private int[]        boundaryTopX, boundaryTopY, boundaryBotX, boundaryBotY, boundaryLeftX, boundaryLeftY, boundaryRightX, boundaryRightY;
        public  int[]        BoundaryTopX    { get { return boundaryTopX;   } set { boundaryTopX   = value; } }
        public  int[]        BoundaryTopY    { get { return boundaryTopY;   } set { boundaryTopY   = value; } }
        public  int[]        BoundaryBotX    { get { return boundaryBotX;   } set { boundaryBotX   = value; } }
        public  int[]        BoundaryBotY    { get { return boundaryBotY;   } set { boundaryBotY   = value; } }
        public  int[]        BoundaryLeftX   { get { return boundaryLeftX;  } set { boundaryLeftX  = value; } }
        public  int[]        BoundaryLeftY   { get { return boundaryLeftY;  } set { boundaryLeftY  = value; } }
        public  int[]        BoundaryRightX  { get { return boundaryRightX; } set { boundaryRightX = value; } }
        public  int[]        BoundaryRightY  { get { return boundaryRightY; } set { boundaryRightY = value; } }
        
        //Constructor
        public CharacterObject()
        {
            //No initial property needed to be constructed here
        }

        //Load visual from file
        protected void GetBattleForms(string path)
        {
            _physicalForm = File.ReadAllLines(path + ".live", Encoding.ASCII);
            _deadForm = File.ReadAllLines(path + ".dead", Encoding.ASCII);
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

        protected void LoadMap(string path)
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


        //Define world map forms
        protected void SetMapForms(char character)
        {
            _mapForm = character;
            _mapNegativeForm = ' ';
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
    }
}
