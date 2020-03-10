using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Text_Based_RPG
{
    class DisplayManager
    {
        public ShapeFactory shapeFactory = new ShapeFactory();

        public DisplayManager()
        {
            //Hmm... Nothing to be constructed at this moment
        }

        public void DrawObjectAt(int x, int y, string[] drawObject , ConsoleColor color)
        {
            Console.ForegroundColor = color;
            
            foreach (string line in drawObject)
            {
                Console.SetCursorPosition(x, y);
                Console.WriteLine(line);
                y++;
            }            
        }

        public void DrawMasterCanvas(int length, int height)
        {
            shapeFactory.DrawRectangle(0, 0, length, height);
        }

        public void DrawRectangle(int x, int y, int length, int height)
        {
            shapeFactory.DrawRectangle(x, y, length, height);
        }

        public void DrawHint(int x, int y, string message)
        {
            Console.SetCursorPosition(x, y);
            for (int i = 0; i < 60; i++)
            {
                //Clear old message
                Console.Write((char)32);
            }
            Console.SetCursorPosition(x, y);
            Console.Write(message);
        }

        public void DrawAnimateTextboxIn(int x = 37, int y = 17, int l = 28, int h = 3)
        {

            DrawRectangle(x + 12, y, l - 24, h);
            Thread.Sleep(50);

            //(char)32
            Console.SetCursorPosition(x + 12, y + 1);
            for (int i = 0; i <= l - 22; i++) Console.Write((char)32);
            Console.SetCursorPosition(x + 12, y + 2);
            for (int i = 0; i <= l - 22; i++) Console.Write((char)32);
            Console.SetCursorPosition(x + 12, y + 3);
            for (int i = 0; i <= l - 22; i++) Console.Write((char)32);

            DrawRectangle(x + 9, y, l - 18, h);
            Thread.Sleep(50);

            Console.SetCursorPosition(x + 9, y + 1);
            for (int i = 0; i <= l - 16; i++) Console.Write((char)32);
            Console.SetCursorPosition(x + 9, y + 2);
            for (int i = 0; i <= l - 16; i++) Console.Write((char)32);
            Console.SetCursorPosition(x + 9, y + 3);
            for (int i = 0; i <= l - 16; i++) Console.Write((char)32);

            DrawRectangle(x + 4, y, l - 8, h);
            Thread.Sleep(50);

            Console.SetCursorPosition(x + 4, y + 1);
            for (int i = 0; i <= l - 6; i++) Console.Write((char)32);
            Console.SetCursorPosition(x + 4, y + 2);
            for (int i = 0; i <= l - 6; i++) Console.Write((char)32);
            Console.SetCursorPosition(x + 4, y + 3);
            for (int i = 0; i <= l - 6; i++) Console.Write((char)32);

            DrawRectangle(x, y, l, h);
        }

        public void DrawAnimateTextboxOut(int x = 37, int y = 17, int l = 28, int h = 3)
        {

            for (int j = y; j <= y + h + 1; j++)
            {
                Console.SetCursorPosition(x, j);
                for (int i = 0; i <= l + 2; i++) Console.Write((char)32);
            }

            DrawRectangle(x + 4, y, l - 8, h);
            Thread.Sleep(50);

            for (int j = y; j <= y + h + 1; j++)
            {
                Console.SetCursorPosition(x + 4, j);
                for (int i = 0; i <= l + 2 - 8; i++) Console.Write((char)32);
            }

            DrawRectangle(x + 9, y, l - 18, h);
            Thread.Sleep(50);

            for (int j = y; j <= y + h + 1; j++)
            {
                Console.SetCursorPosition(x + 9, j);
                for (int i = 0; i <= l + 2 - 18; i++) Console.Write((char)32);
            }

            DrawRectangle(x + 12, y, l - 24, h);
            Thread.Sleep(50);

            for (int j = y; j <= y + h + 1; j++)
            {
                Console.SetCursorPosition(x + 12, j);
                for (int i = 0; i <= l + 2 - 24; i++) Console.Write((char)32);
            }
        }

        public void DrawGameHint(int x = 50, int y = 18, string text = "PRESS ANY KEY")
        {
            while (!Console.KeyAvailable)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(text);
                Thread.Sleep(650);
                for (int i = 1; i <= text.Length + 2; i++)
                {
                    //(char)32 represents the backspace key
                    Console.Write("\b" + (char)32 + "\b");
                    if (i == text.Length + 2) Thread.Sleep(650);
                }
            }
        }

        public void DrawAnimatedTextBox()
        {

        }


    }
}
