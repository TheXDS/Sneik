/*
FoodChunk.cs

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
using System.Linq;

namespace TheXDS.Sneik
{
    /// <summary>
    /// Implementa un elemento interactivo que controla la comida de la
    /// serpiente.
    /// </summary>
    internal sealed class FoodChunk : Chunk
    {
        private static readonly Random Rnd = new Random();

        private byte eaten;

        public void Reset()
        {
            eaten = 0;
        }

        /// <summary>
        /// Vuelve a reubicar este elemento de comida.
        /// </summary>
        public void Place()
        {
            do
            {
                X = (short)(Rnd.Next((Game.Bounds.Left + 2) / 2, Game.Bounds.Right / 2) * 2);
                Y = (short)Rnd.Next(Game.Bounds.Top + 1, Game.Bounds.Bottom);
            }
            while (Enumerable.Any(Game.Snake, p => p.Collides(this)));
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="FoodChunk"/>.
        /// </summary>
        public FoodChunk()
        {
            Color = ConsoleColor.DarkRed;
        }

        /// <summary>
        /// Ejecuta una acción al producirse una colisión con la cabeza de la
        /// serpiente.
        /// </summary>
        /// <param name="head">Head.</param>
        public override void CollideAction(Chunk head)
        {
            Game.Score += (Game.Level * 100) + 1; // +1 compensa OnGameTick()
            Place();
            Draw();
            if (eaten == 19)
            {
                eaten = 0;
                Game.Level++;
                Game.Loop.Interval = 500 / Game.Level;
            }
            else eaten++;
            Game.ReportScore();
        }

        /// <summary>
        /// Decrementa el puntaje del jugador por cada movimiento donde no se
        /// ha comido una fruta.
        /// </summary>
        protected internal override void OnGameTick()
        {
            Game.Score--;
            Game.ReportScore();
        }
    }
}
