using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Text_Based_RPG.DisplaySystem
{
    class ShapeFactory
    {
        private void DrawHorizontalLine(int x, int y, int length, char character = '─')
        {
            Console.SetCursorPosition(x, y);
            for (int i = 0; i < length; i++)
            {
                Console.Write(character);
            }
        }

        private void DrawVerticalLine(int x, int y, int height, char character = '│')
        {
            for (int i = 0; i < height; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write(character);
            }
        }

        public void DrawRectangle(int x, int y, int length, int height)
        {
            //Draw top left corner
            Console.SetCursorPosition(x, y);
            Console.Write("┌");

            //Draw top horizontal line
            DrawHorizontalLine(x + 1, y, length - 2);

            //Draw top right corner
            Console.SetCursorPosition(x + length - 1, y);
            Console.WriteLine("┐");

            //Draw bottom left corner
            Console.SetCursorPosition(x, y + height - 1 );
            Console.Write("└");

            //Draw bottom horizontal line
            DrawHorizontalLine(x + 1, y + height - 1, length - 2);

            //Draw bottom right corner
            Console.SetCursorPosition(x + length - 1, y + height - 1);
            Console.WriteLine("┘");

            //Draw left & right vertical lines
            DrawVerticalLine(x, y + 1, height - 2);
            DrawVerticalLine(x + length - 1, y + 1, height - 2);
        }
        public void ClearRectangle(int x, int y, int length, int height)
        {
            //Draw top horizontal line
            DrawHorizontalLine(x, y, length, ' ');

            //Draw bottom horizontal line
            DrawHorizontalLine(x, y + height - 1, length, ' ');

            //Draw left & right vertical lines
            DrawVerticalLine(x, y + 1, height - 1, ' ');
            DrawVerticalLine(x + length - 1, y + 1, height - 1, ' ');
        }
    }
}
