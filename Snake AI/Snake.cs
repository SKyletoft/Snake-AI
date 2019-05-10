using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Snake_AI {
    public class Snake {
        public (int, int) currentRotation = (0, 1);
        public (int height, int width) dimensions;
        public int length = 1;
        public List<(int X, int Y)> tail = new List<(int X, int Y)> { };
        public (int X, int Y) head;
        public (int X, int Y) apple = (7, 7);
        public bool ended = false;
        Random rnd = new Random(/*123*/);
        public bool playTurn ((int X, int Y) movement, bool respectRot) {
            if (respectRot) {
                return playTurn(rotate(movement));
            } else {
                return playTurn(movement);
            }
        }
        public bool playTurn ((int X, int Y) movement) {
            var tmp = new List<(int X, int Y)> { head };
            tmp.AddRange(tail.GetRange(0, length));
            tail = tmp;
            head.X += movement.X;
            head.Y += movement.Y;
            if (head.X >= dimensions.width ||
                head.X < 0 ||
                head.Y >= dimensions.height ||
                head.Y < 0 ||
                tail.IndexOf(head) != -1
            ) {
                ended = true;
                return false;
            }

            if (head == apple) {
                length++;
                var test = apple;
                do {
                    test = (X: rnd.Next(0, dimensions.width), Y: rnd.Next(0, dimensions.height));
                } while (tail.IndexOf(test) != -1 && test != apple);
                apple = test;
            }
            ended = false;
            return true;
        }
        public int[,] RenderB () {
            var board = new int[dimensions.width * 2 + 1, dimensions.height * 2 + 1];
            var lastDirection = (X: head.X - tail[0].X, Y: head.Y - tail[0].Y);
            for (var i = 0; i < dimensions.width; i++) {
                for (var j = 0; j < dimensions.height; j++) {
                    //APPLE
                    //TAIL OR WALL OR HEAD
                    //EMPTY
                    var coords = (dimensions.width - head.X + i, dimensions.height - head.Y + j);
                    if ((i, j) == apple) {
                        board[coords.Item1, coords.Item2] = 2;
                    } else if (tail.IndexOf((i, j)) != -1 || (i, j) == head) {
                        board[coords.Item1, coords.Item2] = 0;
                    } else {
                        board[coords.Item1, coords.Item2] = 1;
                    }
                }
            }
            if (lastDirection == (0, 1)) {

            } else if (lastDirection == (1, 0)) {
                board = turnLeft(board);
            } else if (lastDirection == (-1, 0)) {
                board = turnRight(board);
            } else if (lastDirection == (0, -1)) {
                board = turn180(board);
            }
            currentRotation = lastDirection;
            return board;
        }
        public Bitmap RenderI () {
            var image = RenderB();
            var frame = new Bitmap(dimensions.width * 2 + 1, dimensions.height * 2 + 1);
            for (var i = 0; i < dimensions.width * 2; i++) {
                for (var j = 0; j < dimensions.height * 2; j++) {
                    switch (image[i, j]) {
                        case 1:
                            frame.SetPixel(i, j, Color.White);
                            break;
                        case 0:
                            frame.SetPixel(i, j, Color.Black);
                            break;
                        case 2:
                            frame.SetPixel(i, j, Color.Red);
                            break;
                    }
                }
            }
            return frame;
        }
        public Bitmap RenderSimple () {
            var frame = new Bitmap(dimensions.width, dimensions.height);
            for (var j = 0; j < dimensions.height; j++) {
                for (var i = 0; i < dimensions.width; i++) {
                    frame.SetPixel(i, j, tail.IndexOf((i, j)) != -1 ? Color.Black : Color.White);
                    if (head == (i, j)) {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    } else if (tail.IndexOf((i, j)) != -1) {
                        Console.ForegroundColor = ConsoleColor.Green;
                    } else if (apple == (i, j)) {
                        Console.ForegroundColor = ConsoleColor.Red;
                    } else {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.Write('█');
                }
                Console.WriteLine();
            }
            return frame;
        }
        int[,] turnLeft (int[,] original) {
            var newBoard = new int[dimensions.width * 2 + 1, dimensions.height * 2 + 1];
            for (var i = 0; i < dimensions.width * 2 + 1; i++) {
                for (var j = 0; j < dimensions.height * 2 + 1; j++) {
                    newBoard[i, j] = original[j, dimensions.width * 2 + 1 - (i + 1)];
                }
            }
            return newBoard;
        }
        int[,] turnRight (int[,] original) {
            var newBoard = new int[dimensions.width * 2 + 1, dimensions.height * 2 + 1];
            for (var i = 0; i < dimensions.width * 2 + 1; i++) {
                for (var j = 0; j < dimensions.height * 2 + 1; j++) {
                    newBoard[i, j] = original[dimensions.height * 2 + 1 - (j + 1), i];
                }
            }
            return newBoard;
        }
        int[,] turn180 (int[,] original) {
            var newBoard = new int[dimensions.width * 2 + 1, dimensions.height * 2 + 1];
            for (var i = 0; i < dimensions.width * 2 + 1; i++) {
                for (var j = 0; j < dimensions.height * 2 + 1; j++) {
                    newBoard[i, j] = original[dimensions.width * 2 + 1 - (i + 1), dimensions.height * 2 + 1 - (j + 1)];
                }
            }
            return newBoard;
        }
        (int X, int Y) rotate ((int X, int Y) movement) {
            var directions = new (int X, int Y)[] {
                (1,0),
                (0,-1),
                (-1,0),
                (0,1),
            };
            var currentDir = Array.IndexOf(directions, currentRotation);
            var inputDir = Array.IndexOf(directions, movement);
            return directions[((currentDir - inputDir) + 15) % 4];
        }
    }
}
