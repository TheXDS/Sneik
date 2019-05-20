//
// Game.cs
//
// Author:
//       César Andrés Morgan <xds_xps_ivx@hotmail.com>
//
// Copyright (c) 2019 César Andrés Morgan
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace TheXDS.Sneik
{
    /// <summary>
    /// Punto de entrada y control general del juego.
    /// </summary>
    static class Game
    {
        static bool Vert;
        static bool Dir = true;
        static IEnumerable<Chunk> Chunks => Snake.Concat(new[] { FoodChunk });

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        static void Main(string[] args)
        {
            CleanUp();
            Loop.Elapsed+= Loop_Elapsed;
            Console.CursorVisible = false;
            var play = true;
            while (play)
            {
                Score = 0;
                Level = 1;
                Vert = false;
                Dir = true;
                Console.SetCursorPosition(0, 0);
                Console.Write(new string(' ', Console.BufferWidth * 2));
                Snake.Clear();
                for (byte j = 2; j <= 12; j+=2)
                    Snake.Enqueue(new BodyChunk { X = j, Y = 1});

                ReportScore();
                FoodChunk.Place();

                Redraw();
                Loop.Start();

                while (Loop.Enabled) System.Threading.Thread.Sleep((int)Loop.Interval);

                Console.BackgroundColor = Bg;
                Console.SetCursorPosition(0, 1);
                Console.Write("Quieres volver a jugar? (s/N)");
                play = Console.ReadLine().ToLower() == "s";
                AltClear();
            }
            CleanUp();
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
        public static Timer Loop = new Timer(500);

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
        ///     Limpia y reestablece el estado de la terminal.
        /// </summary>
        private static void CleanUp()
        {
            Console.BackgroundColor = Bg;
            Console.CursorVisible = true;
            Console.Clear();
            Console.SetCursorPosition(0, 0);
        }

        /// <summary>
        ///     Hace perder el juego.
        /// </summary>
        public static void Lose()
        {
            Loop.Stop();
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = Bg;
            Console.Write($"Perdiste. Tu puntaje: {Score}");
        }

        /// <summary>
        /// Controla el ciclo principal del juego.
        /// </summary>
        /// <param name="sender">Objeto que ha generado el evento.</param>
        /// <param name="e">Argumentos del evento.</param>
        static void Loop_Elapsed(object sender, ElapsedEventArgs e)
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

        /// <summary>
        /// Lee los comandos de entrada del jugador.
        /// </summary>
        static void ReadInput()
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
        static void Redraw()
        {
            foreach(var j in Chunks) j.Draw();
        }

        /// <summary>
        ///     Versión alternativa de <see cref="Console.Clear()"/> que no
        ///     limpia la pantalla haciendo Scroll en terminales de Linux.
        /// </summary>
        static void AltClear()
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
