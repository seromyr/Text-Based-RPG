using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;



namespace Text_Based_RPG.DisplaySystem
{
    class DisplayManager
    {
        private ShapeFactory shapeFactory = new ShapeFactory();        

        public DisplayManager()
        {
            //Hmm... Nothing to be constructed at this moment
        }

        //Draw an array type object
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

        //Draw a rectangle
        public void DrawRectangle(int x, int y, int length, int height)
        {
            shapeFactory.DrawRectangle(x, y, length, height);
        }

        //Clear a rectangle
        public void ClearRectangle(int x, int y, int length, int height)
        {
            shapeFactory.ClearRectangle(x, y, length, height);
        }

        //Write a text with a clear previous function
        public static void WriteTextAt(int x, int y, string message)
        {
            Console.SetCursorPosition(x, y);
            for (int i = 0; i < message.Length; i++)
            {
                //Clear old message
                Console.Write(' ');
            }
            Console.SetCursorPosition(x, y);
            Console.Write(message);
        }

        public void ClearTextAt(int x, int y, string message)
        {
            foreach (char c in message)
            {
                Console.SetCursorPosition(x, y);
                for (int i = 0; i < message.Length; i++)
                {
                    Console.Write(' ');
                }
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

        //Draw an animated textbox with expanding effect
        protected void DrawAnimatedTextboxIn(int x = 37, int y = 17, int l = 28, int h = 3)
        {
            //draw the smallest rectangle  at the center
            DrawRectangle(x + (l * 1 / 2) - 1, y, 2, h);
            Thread.Sleep(100);

            //then clear itself
            for (int i = 1; i < h - 1; i++)
            {
                Console.SetCursorPosition(x + (l * 1 / 2) - 1, y + i);
                Console.Write(' ');
                Console.SetCursorPosition(x + (l * 1 / 2), y + i);
                Console.Write(' ');
            }

            //draw the 2nd smallest rectangle  at the center
            DrawRectangle(x + (l * 3 / 8), y, l / 4, h);
            Thread.Sleep(100);

            //then clear itself
            for (int i = 1; i < h - 1; i++)
            {
                Console.SetCursorPosition(x + (l * 3 / 8), y + i);
                Console.Write(' ');
                Console.SetCursorPosition(x + (l * 5 / 8) - 1, y + i);
                Console.Write(' ');
            }

            //draw the 3rd smallest rectangle  at the center
            DrawRectangle(x + (l * 1 / 4), y, l / 2, h);
            Thread.Sleep(100);

            //then clear itself
            for (int i = 1; i < h - 1; i++)
            {
                Console.SetCursorPosition(x + (l * 1 / 4), y + i);
                Console.Write(' ');
                Console.SetCursorPosition(x + (l * 3 / 4) - 1, y + i);
                Console.Write(' ');
            }

            //draw the next to largest rectangle  at the center
            DrawRectangle(x + (l * 1 / 8), y, l * 3 / 4, h);
            Thread.Sleep(100);

            //then clear itself
            for (int i = 1; i < h - 1; i++)
            {
                Console.SetCursorPosition(x + (l * 1 / 8), y + i);
                Console.Write(' ');
                Console.SetCursorPosition(x + (l * 7 / 8) - 1, y + i);
                Console.Write(' ');
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            DrawRectangle(x, y, l, h);
        }

        //Draw an animated textbox with shrinking effect
        protected void DrawAnimatedTextboxOut(int x = 37, int y = 17, int l = 28, int h = 3)
        {
            //Clear the largest rectangle
            ClearRectangle(x, y, l, h);

            //Draw the next smaller rectangle
            DrawRectangle(x + (l * 1 / 8), y, l * 3 / 4, h);
            Thread.Sleep(100);

            //Clear itself
            ClearRectangle(x + (l * 1 / 8), y, l * 3 / 4, h);

            //draw the 3rd smallest rectangle  at the center
            DrawRectangle(x + (l * 1 / 4), y, l / 2, h);
            Thread.Sleep(100);

            //Clear itself
            ClearRectangle(x + (l * 1 / 4), y, l / 2, h);

            //draw the 2nd smallest rectangle  at the center
            DrawRectangle(x + (l * 3 / 8), y, l / 4, h);
            Thread.Sleep(100);

            //Clear itself
            ClearRectangle(x + (l * 3 / 8), y, l / 4, h);

            DrawRectangle(x + (l * 1 / 2) - 1, y, 2, h);
            Thread.Sleep(100);

            //Clear itself
            ClearRectangle(x + (l * 1 / 2) - 1, y, 2, h);
        }

        //Get positive and negative visual effect of an array
        protected void GetPositiveAndNegativeVisual(string[] positiveForm, string[] negativeForm)
        {
            Array.Copy(positiveForm, negativeForm, positiveForm.Length);

            for (int i = 0; i < negativeForm.Length; i++)
            {
                StringBuilder sb = new StringBuilder(negativeForm[i].Length);
                for (int j = 0; j < negativeForm[i].Length; j++)
                {
                    sb.Append(' ');
                }
                negativeForm[i] = sb.ToString();
            }
        }
    }
}
