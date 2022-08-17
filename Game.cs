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
using System.Timers;

#if !NETFX20
using System.Threading.Tasks;
#endif

namespace TheXDS.Sneik
{
    /// <summary>
    /// Punto de entrada y control general del juego.
    /// </summary>
    internal static class Game
    {
#if !NETFX20
        private static IEnumerable<Chunk> Chunks => Snake.Concat(new[] { FoodChunk }).Concat(Walls);
#else
        private static IEnumerable<Chunk> Chunks
        {
            get
            {
                foreach (var j in Snake) yield return j;                
                yield return FoodChunk;
                foreach (var j in Walls) yield return j;                
            }
        }

#endif

        private static bool Vert;
        private static bool Dir = true;
        private static readonly List<Chunk> Walls = new List<Chunk>();
        private static bool inPause;
        public static GameBounds Bounds = new GameBounds();
        private static readonly Dictionary<ConsoleKey, Action> KeyBindings = new Dictionary<ConsoleKey, Action>();

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        private static void Main()
        {
            CleanUp();
            Loop.Elapsed+= Loop_Elapsed;
            Console.CursorVisible = false;
            SetGameBounds<ObstacleChunk>();
            SetKeyBindings();
            while (KeepPlaying())
            {
                AltClear();
            }
            CleanUp();
        }

        private static bool KeepPlaying()
        {
            if (!inPause)
            { 
                PrepareNewGame(); 
            } 
            else
            {
                inPause = false;
                ClearTopMessage();
                Redraw();
            }

            Loop.Start();

            // Timer principal
            while (Loop.Enabled) Thread.Sleep((int)Loop.Interval);

            if (!inPause)
            {
                Thread.Sleep(3000);
                Message("Quieres volver a jugar? (s/N)");
                return Console.ReadLine().ToLower() == "s";
            }
            else 
            {
                Console.ReadKey(); 
            }
            return true;
        }

        private static void SetKeyBindings()
        {
            KeyBindings.Add(ConsoleKey.W, GoUp);
            KeyBindings.Add(ConsoleKey.UpArrow, GoUp);
            KeyBindings.Add(ConsoleKey.S, GoDown);
            KeyBindings.Add(ConsoleKey.DownArrow, GoDown);
            KeyBindings.Add(ConsoleKey.A, GoLeft);
            KeyBindings.Add(ConsoleKey.LeftArrow, GoLeft);
            KeyBindings.Add(ConsoleKey.D, GoRight);
            KeyBindings.Add(ConsoleKey.RightArrow, GoRight);
            KeyBindings.Add(ConsoleKey.Escape, Retire);
            KeyBindings.Add(ConsoleKey.Q, Retire);
            KeyBindings.Add(ConsoleKey.P, Pause);
        }

        private static void ClearTopMessage()
        {
            Console.BackgroundColor = Bg;
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
            Bounds.Bottom = 40;
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
            Message("Perdiste. ");
            Console.Write($"Tu puntaje: {Score}. ");
        }

        /// <summary>
        /// Controla el ciclo principal del juego.
        /// </summary>
        /// <param name="sender">Objeto que ha generado el evento.</param>
        /// <param name="e">Argumentos del evento.</param>
        private static void Loop_Elapsed(object sender, ElapsedEventArgs e)
        {
#if !NETFX20
            Parallel.Invoke(Chunks.Select<Chunk, Action>(p => p.OnGameTick).ToArray());
#else
            foreach (var j in Chunks)
            {
                j.OnGameTick();
            }
#endif

            var newHead = SnakeStep();

            if (!Enumerable.Any(Walls))
            {
                if (newHead.X < 0 || newHead.X > Console.WindowWidth || newHead.Y < 0 || newHead.Y >= Console.WindowHeight)
                {
                    Lose();
                }
            }

            if (Enumerable.FirstOrDefault(Chunks,p => !(p == Enumerable.First(Chunks)) && p.Collides(newHead)) is Chunk c)
                c.CollideAction(newHead);
            else Snake.Dequeue().Clear();

            Snake.Enqueue(newHead);
            if (!inPause) newHead.Draw();
        }

        private static BodyChunk SnakeStep()
        {
            var head = Enumerable.Last(Snake);
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
            while (Console.KeyAvailable) k = Console.ReadKey(true).Key;
            if (k.HasValue && KeyBindings.TryGetValue(k.Value, out var action)) action.Invoke();            
        }

        private static void GoUp()
        {
            if (Vert) return;
            Vert = true;
            Dir = false;
        }
        private static void GoDown()
        {
            if (Vert) return;
            Vert = true;
            Dir = true;
        }
        private static void GoLeft()
        {
            if (!Vert) return;
            Vert = false;
            Dir = false;
        }

        private static void GoRight()
        {
            if (!Vert) return;
            Vert = false;
            Dir = true;
        }

        private static void Retire()
        {
            Loop.Stop();
            Message("Te has retirado. ");
        }

        private static void Pause()
        {
            inPause = true;
            Loop.Stop();                
            AltClear();
            Message("Juego en pausa. Presiona cualquier tecla para continuar...");
        }

        /// <summary>
        /// Redibuja la pantalla de juego.
        /// </summary>
        private static void Redraw()
        {
            foreach(var j in Chunks) j.Draw();
        }

        /// <summary>
        /// Versión alternativa de <see cref="Console.Clear()"/> que no
        /// limpia la pantalla haciendo Scroll en terminales de Linux.
        /// </summary>
        private static void AltClear()
        {
            Console.BackgroundColor = Bg;
            foreach (var j in Chunks)
            {
                j.Clear();
            }
        }

        private static void Message(string message)
        {
            ClearTopMessage();
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = Bg;
            Console.Write(message);
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
