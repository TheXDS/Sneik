/*
Game.cs

Author:
      César Andrés Morgan <xds_xps_ivx@hotmail.com>

Copyright (c) 2019-2020 César Andrés Morgan

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Drawing;

namespace TheXDS.Sneik
{
    internal struct GameBounds
    {
        public short Left { get; set; }

        public short Right { get; set; }

        public short Top { get; set; }

        public short Bottom { get; set; }
    }

    /// <summary>
    /// Punto de entrada y control general del juego.
    /// </summary>
    internal static class Game
    {
        private static bool Vert;
        private static bool Dir = true;
        private static readonly List<Chunk> Walls = new List<Chunk>();
        public static GameBounds Bounds = new GameBounds();
        private static IEnumerable<Chunk> Chunks => Snake.Concat(new[] { FoodChunk }).Concat(Walls);

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        private static void Main()
        {
            CleanUp();
            Loop.Elapsed+= Loop_Elapsed;
            Console.CursorVisible = false;
            var play = true;

            SetGameBounds<WarpChunk>();

            while (play)
            {
                PrepareNewGame();
                Loop.Start();

                // Timer principal
                while (Loop.Enabled) Thread.Sleep((int)Loop.Interval);

                Console.BackgroundColor = Bg;
                Console.SetCursorPosition(0, 1);
                Console.Write("Quieres volver a jugar? (s/N)");
                play = Console.ReadLine().ToLower() == "s";
                AltClear();
            }
            CleanUp();
        }

        private static void ClearTopMessage()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write(new string(' ', Console.BufferWidth * 2));
        }

        private static void ResetSnake() => ResetSnake((short)(Bounds.Left + 2), (short)(Bounds.Top + 2));

        private static void ResetSnake(in short startingX, in short startingY)
        {
            Snake.Clear();
            for (short j = startingX; j <= 12; j += 2)
            {
                Snake.Enqueue(new BodyChunk { X = j, Y = startingY });
            }
        }

        private static void SetGameBounds<TChunk>() where TChunk : Chunk, new()
        {
            Bounds.Left = 4;
            Bounds.Right = 76;
            Bounds.Top = 4;
            Bounds.Bottom = 20;
            Walls.ForEach(p => p.Clear());
            Walls.Clear();
            for (var j = Bounds.Left; j <= Bounds.Right; j+=2)
            {
                Walls.Add(new TChunk { X = j, Y = Bounds.Top });
                Walls.Add(new TChunk { X = j, Y = Bounds.Bottom });
            }
            for (var j = (short)(Bounds.Top + 1); j < Bounds.Bottom; j++)
            {
                Walls.Add(new TChunk { X = Bounds.Left, Y = j });
                Walls.Add(new TChunk { X = Bounds.Right, Y = j });
            }
        }

        private static void PrepareNewGame()
        {
            ClearTopMessage();
            Score = 0;
            Level = 1;
            Loop.Interval = 500;
            Vert = false;
            Dir = true;
            ResetSnake();
            ReportScore();
            FoodChunk.Place();
            Redraw();
        }

        /// <summary>
        /// Obtiene los segmentos activos de la serpiente.
        /// </summary>
        public static readonly Queue<Chunk> Snake = new Queue<Chunk>();

        /// <summary>
        /// Obtiene una referencia al Chunk controlador de la comida.
        /// </summary>
        public static readonly FoodChunk FoodChunk = new FoodChunk();

        /// <summary>
        /// Obtiene el controlador del ciclo del juego.
        /// </summary>
        public static System.Timers.Timer Loop = new System.Timers.Timer(500);

        /// <summary>
        /// Obtiene el puntaje actual del juego.
        /// </summary>
        public static int Score;

        /// <summary>
        /// Obtiene o establece el nivel de dificultad actual.
        /// </summary>
        public static byte Level = 1;

        /// <summary>
        /// Almacena el color de fondo predeterminado de la consola.
        /// </summary>
        public static ConsoleColor Bg = Console.BackgroundColor;

        /// <summary>
        /// Limpia y reestablece el estado de la terminal.
        /// </summary>
        private static void CleanUp()
        {
            Console.BackgroundColor = Bg;
            Console.CursorVisible = true;
            Console.Clear();
            Console.SetCursorPosition(0, 0);
        }

        /// <summary>
        /// Hace perder el juego.
        /// </summary>
        public static void Lose()
        {
            Loop.Stop();
            FoodChunk.Reset();
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = Bg;
            Console.Write("Perdiste. ");
            Thread.Sleep(3000);
            Console.Write($"Tu puntaje: {Score}. ");
            Thread.Sleep(3000);
        }

        /// <summary>
        /// Controla el ciclo principal del juego.
        /// </summary>
        /// <param name="sender">Objeto que ha generado el evento.</param>
        /// <param name="e">Argumentos del evento.</param>
        private static void Loop_Elapsed(object sender, ElapsedEventArgs e)
        {
            Parallel.Invoke(Chunks.Select<Chunk, Action>(p => p.OnGameTick).ToArray());

            var newHead = SnakeStep();

            if (newHead.X < 0 || newHead.X > Console.WindowWidth || newHead.Y < 0 || newHead.Y >= Console.WindowHeight)
            {
                Lose();
            }

            if (Chunks.FirstOrDefault(p => !(p == Chunks.First()) && p.Collides(newHead)) is Chunk c)
                c.CollideAction(newHead);
            else Snake.Dequeue().Clear();

            newHead.Draw();
            Snake.Enqueue(newHead);
        }

        private static BodyChunk SnakeStep()
        {
            var head = Snake.Last();
            ReadInput();
            var newHead = new BodyChunk();
            if (Vert)
            {
                newHead.X = head.X;
                newHead.Y = (short)(head.Y + (Dir ? 1 : -1));
            }
            else
            {
                newHead.X = (short)(head.X + (Dir ? 2 : -2));
                newHead.Y = head.Y;
            }
            return newHead;
        }

        /// <summary>
        /// Lee los comandos de entrada del jugador.
        /// </summary>
        private static void ReadInput()
        {
            ConsoleKey? k = null;
            while (Console.KeyAvailable)
            k = Console.ReadKey().Key;

            switch (k)
            {
                //case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    if (Vert) return;
                    Vert = true;
                    Dir = false;
                    break;
                //case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    if (!Vert) return;
                    Vert = false;
                    Dir = false;
                    break;
                //case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    if (Vert) return;
                    Vert = true;
                    Dir = true;
                    break;
                //case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    if (!Vert) return;
                    Vert = false;
                    Dir = true;
                    break;
                case ConsoleKey.Escape:
                    Console.SetCursorPosition(0, 0);
                    Console.BackgroundColor = Bg;
                    Console.Write("Te has retirado.");
                    Loop.Stop();
                    break;
            }
        }

        /// <summary>
        /// Redibuja la pantalla de juego.
        /// </summary>
        private static void Redraw()
        {
            foreach(var j in Chunks) j.Draw();
        }

        /// <summary>
        ///     Versión alternativa de <see cref="Console.Clear()"/> que no
        ///     limpia la pantalla haciendo Scroll en terminales de Linux.
        /// </summary>
        private static void AltClear()
        {
            Console.BackgroundColor = Bg;
            foreach (var j in Chunks)
            {
                j.Clear();
            }
        }

        /// <summary>
        /// Reporta el nivel y el puntaje actuales.
        /// </summary>
        public static void ReportScore()
        {
            Console.Title = $"Sneik - nivel {Level}, {Score} puntos";
        }
    }
}
