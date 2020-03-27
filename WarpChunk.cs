/*
WarpChunk.cs

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

namespace TheXDS.Sneik
{
    /// <summary>
    /// Implementa un elemento de muro que teletransporta al jugador a otra
    /// posición.
    /// </summary>
    internal sealed class WarpChunk : Chunk
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="WarpChunk"/>.
        /// </summary>
        public WarpChunk()
        {
            Color = ConsoleColor.DarkMagenta;
            Pictograph = "()";
        }

        /// <summary>
        /// Teletransporta al jugador.
        /// </summary>
        /// <param name="head"></param>
        public override void CollideAction(Chunk head)
        {
            if (head.X == Game.Bounds.Left)
            {
                head.X = (short)(Game.Bounds.Right - 2);
            }
            if (head.X == Game.Bounds.Right)
            {
                head.X = (short)(Game.Bounds.Left + 2);
            }
            if (head.Y == Game.Bounds.Top)
            {
                head.Y = (short)(Game.Bounds.Bottom - 1);
            }
            if (head.Y == Game.Bounds.Bottom)
            {
                head.Y = (short)(Game.Bounds.Top + 1);
            }
            Game.Snake.Dequeue().Clear();
        }
    }
}