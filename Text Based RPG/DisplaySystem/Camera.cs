using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Text_Based_RPG.CharacterObjects;

namespace Text_Based_RPG.DisplaySystem
{
    class Camera : DisplayManager
    {
        public char[,] viewPort, mapBuffer, viewPortNegative;
        private string viewportNegative;
        private Map currentMap;
        private int playerStartingLocationX, playerStartingLocationY, viewportDefaultX, viewportDefaultY;
        public int prevX, prevY;

        //extra object list to put on the map buffer
        private List<char> extraObject;
        private List<int>  objectX, objectY;

        public Camera(string mapName, ConsoleColor mapColor)
        {
            viewPort = new char[Constant.VIEWPORT_HEIGHT, Constant.VIEWPORT_WIDTH]; //[12,115]
            viewPortNegative = new char[Constant.VIEWPORT_HEIGHT, Constant.VIEWPORT_WIDTH]; //[12,115]
            viewportNegative = "                                                                                                                   ";

            extraObject = new List<char>();
            objectX     = new List<int>();
            objectY     = new List<int>();

            currentMap = new Map(mapName)
            {
                X = Constant.GAMEPLAY_CANVAS_LIMIT_LEFT + 1,
                Y = Constant.GAMEPLAY_CANVAS_LIMIT_UP + 1
            };
            currentMap.Color = mapColor;

            mapBuffer = new char[currentMap.Height, currentMap.Width];

            playerStartingLocationX = 0;
            playerStartingLocationY = 0;

            LoadMapToBuffer();

            viewportDefaultX = (int)(playerStartingLocationX - Math.Round(Constant.VIEWPORT_WIDTH  / 2.0, 0));
            viewportDefaultY = (int)(playerStartingLocationY - Math.Round(Constant.VIEWPORT_HEIGHT / 2.0, 0));
            prevX = viewportDefaultX;
            prevY = viewportDefaultY;

            LoadMapBufferToViewPort();

            RenderMapToCamera();
        }

        public int ViewportDefaultX { get { return viewportDefaultX; } set { viewportDefaultX = value; } }
        public int ViewportDefaultY { get { return viewportDefaultY; } set { viewportDefaultY = value; } }
        private void LoadMapToBuffer()
        {
            //Make a loop for all elements of the 1st dimension of the mapBuffer
            for (int i = 0; i < mapBuffer.GetLength(0); i++)
            {
                //Grab a string element and break it to char array
                char[] parsedCharArray = currentMap.PhysicalForm[i].ToCharArray();

                //Copy each char element to the buffer 2nd dimension of each 1st dimension
                for (int j = 0; j < parsedCharArray.Length; j++)
                {
                    mapBuffer[i, j] = parsedCharArray[j];

                    //Mark the starting position of the map
                    if (parsedCharArray[j] == 'X' || parsedCharArray[j] == 'x')
                    {
                        playerStartingLocationX = j;
                        playerStartingLocationY = i;
                        mapBuffer[i, j] = ' ';
                    }
                }
            }
        }

        private void LoadMapBufferToViewPort()
        {
            for (int j = 0; j < viewPort.GetLength(0); j++)
            {
                for (int i = 0; i < viewPort.GetLength(1); i++)
                {
                    if ((j + viewportDefaultY) > mapBuffer.GetLength(0) - 1 ||
                        (i + viewportDefaultX) > mapBuffer.GetLength(1) - 1 ||
                        (j + viewportDefaultY) < 0 ||
                        (i + viewportDefaultX) < 0 )
                    {
                        viewPort[j, i] = ' ';
                    }
                    else
                    {
                        viewPort[j, i] = mapBuffer[j + viewportDefaultY, i + viewportDefaultX];
                    }
                }
            }
        }

        private void UpdateViewport()
        {
            for (int i = 0; i < viewPort.GetLength(0); i++)
            {
                Console.SetCursorPosition(currentMap.X, currentMap.Y + i);
                Console.Write(viewportNegative);
                for (int j = 0; j < viewPort.GetLength(1); j++)
                {
                    if (viewPort[i, j] != ' ')
                    {
                        if (viewPort[i, j] == '∩')
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                        }

                        else if (viewPort[i, j] == 'Ü')
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                        }

                        else if (viewPort[i, j] == 'Ω')
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }

                        else if (viewPort[i, j] == 'Σ')
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                        }

                        else if (viewPort[i, j] == '⌐')
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                        }

                        else if (viewPort[i, j] == 'O')
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                        }

                        else Console.ForegroundColor = currentMap.Color;
                        Console.SetCursorPosition(currentMap.X + j, currentMap.Y + i);
                        Console.Write(viewPort[i, j]);
                    }
                }
            }
        }

        public void ClearViewPort()
        {
            Console.SetCursorPosition(currentMap.X, currentMap.Y);
            for (int i = 0; i < viewPort.GetLength(0); i++)
            {
                for (int j = 0; j < viewPort.GetLength(1); j++)
                {
                    if (viewPortNegative[i, j] == '+')
                    {
                        Console.SetCursorPosition(currentMap.X + j, currentMap.Y + i);
                        Console.Write(' ');
                    }
                }
            }
        }

        public void RenderMapToCamera()
        {
            //Render map through viewport, with X is the starting location
            //Go to console app window designated position to draw viewport
            Console.SetCursorPosition(currentMap.X, currentMap.Y);
            
            //Draw the portion of the map with X at the center
            UpdateViewport();
        }

        public void Update()
        {
            LoadMapBufferToViewPort();
            Console.SetCursorPosition(currentMap.X, currentMap.Y);
            //ClearViewPort();
            UpdateViewport();
            //TakeSnapshot();
        }

        public void AddObject(char mapForm, int x, int y)
        {
            //extraObject.Add(mapForm);
            objectX.Add(x);
            objectY.Add(y);

            //mapBuffer[y, x] = extraObject[extraObject.Count - 1];
            mapBuffer[y, x] = mapForm;
            Update();
        }
        
        public void RemoveObject(char mapNegativeForm)
        {
            mapBuffer[objectY[0], objectX[0]] = mapNegativeForm;
            objectX.RemoveAt(0);
            objectY.RemoveAt(0);
            Update();
        }

        public void RemoveObject(int x, int y)
        {
            mapBuffer[y, x] = ' ';
            Update();
        }

        public void TakeSnapshot()
        {
            Array.Copy(viewPort, viewPortNegative, viewPort.Length);

            for (int i = 0; i < viewPortNegative.GetLength(0); i++)
            {
                for (int j = 0; j < viewPortNegative.GetLength(1); j++)
                {
                    StringBuilder sb = new StringBuilder(viewPortNegative[i, j]);
                    if (viewPort[i, j] != ' ')
                    {
                        sb.Append('+');
                    }

                    if (viewPort[i, j] == ' ')
                    {
                        sb.Append(' ');
                    }
                    viewPortNegative[i, j] = Convert.ToChar(sb.ToString());
                }
            }
        }
    }
}
