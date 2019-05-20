//
// Chunk.cs
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

namespace TheXDS.Sneik
{
    /// <summary>
    /// Clase base que representa un elemento interactivo dentro del juego.
    /// </summary>
    abstract class Chunk
    {
        /// <summary>
        /// Ejecuta una acción al producirse una colisión con la cabeza de la
        /// serpiente.
        /// </summary>
        /// <param name="head">Head.</param>
        public abstract void CollideAction(Chunk head);

        /// <summary>
        /// Posición X de este elemento.
        /// </summary>
        /// <value>la posición horizantal de este elemento.</value>
        public short X { get; set; }

        /// <summary>
        /// Posición Y de este elemento.
        /// </summary>
        /// <value>la posición vertical de este elemento.</value>
        public short Y { get; set; }

        /// <summary>
        /// Obtiene o establece el color de dibujado de este elemento.
        /// </summary>
        /// <value>El color a utilizar para dibujar este elemento.</value>
        public ConsoleColor Color { get; set; }

        /// <summary>
        /// Comprueba si este elemento colisiona con otro.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> si este elemento colisiona con el
        /// especificado, <see langword="false"/> en caso contrario.
        /// </returns>
        /// <param name="other">
        /// Elemento contra el cual comprobar la colisión.
        /// </param>
        public bool Collides(Chunk other)
        {
            return X == other.X && Y == other.Y;
        }

        /// <summary>
        ///     Dibuja este elemento.
        /// </summary>
        public void Draw()
        {
            Console.SetCursorPosition(X, Y);
            Console.BackgroundColor = Color;
            Console.Write("[]");
        }

        public void Clear()
        {
            Console.SetCursorPosition(X, Y);
            Console.BackgroundColor = Game.Bg;
            Console.Write("  ");
        }
    }
}