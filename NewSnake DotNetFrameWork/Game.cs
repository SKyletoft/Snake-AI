using System;
using System.Collections.Generic;
using System.Text;

namespace NewSnake {
    public class Game {
        (int width, int height) size;
        (int x, int y) apple;
        (int x, int y) head;
        Queue<(int x, int y)> tail = new Queue<(int x, int y)>();
        int length;
        int turns = 0;
        Random rnd = new Random(512);

        public double Score {
            get {
                return //points scores plus how close it got to the next apple
                    length;/* +
                    1 - ((
                        Math.Pow(head.x - apple.x, 2) +     
                        Math.Pow(head.y - apple.y, 2)    
                    ) / (size.width * size.height));*/
            }    
        }
        public (int width, int height) Size {
            get {
                return size;
            }
        }
        public (int x, int y) Apple {
            get {
                return apple;
            }
        }
        public (int x, int y) Head {
            get {
                return head;
            }
        }
        public (int x, int y)[] Tail {
            get {
                return tail.ToArray();
            }
        }

        public Game (int width, int height, int seed) {
            size = (width, height);
            length = 2;
            head = (width / 2, height / 2);
            tail.Enqueue((head.x, head.y + 2));
            tail.Enqueue((head.x, head.y + 1));
            //apple = (head.x, head.y - 2);
            rnd = new Random(seed);
            apple = NewApple();
        }

        public int PlayTurn ((int x, int y) dir) {
            if (turns > length * 20) {
                return 0;
            }
            turns++;

            tail.Enqueue(head);
            head.x += dir.x;
            head.y += dir.y;

            var hitException = false;
            if (head == apple) {
                length++;
                apple = NewApple();
                hitException = true;
            }
            while (tail.Count > length) {
                tail.Dequeue();
            }
            if (head.x < 0 || head.x > size.width || head.y < 0 || head.y > size.height) {
                //Console.Clear();
                //Console.WriteLine("Hit wall");
                //hit wall
                return 1;
            }
            if (tail.Contains(head) && !hitException) {
                //Console.Clear();
                //Console.WriteLine("Hit self");
                //hit itself
                return 1;
            }

            return 2;
        }

        public void DrawToConsole (bool clear) {
            if (clear) { //Less work on the CPU, but flickers
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.SetCursorPosition(apple.x, apple.y);
                Console.Write('█');
                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(head.x, head.y);
                Console.Write('█');
                Console.ForegroundColor = ConsoleColor.White;
                foreach (var place in tail) {
                    Console.SetCursorPosition(place.x, place.y);
                    Console.Write('█');
                }
            } else { //More drawing, but doesn't flicker. Might tear though?
                for (var i = 0; i < size.width; i++) {
                    for (var j = 0; j < size.height; j++) {
                        var pos = (x: i, y: j);
                        if (pos == apple) {
                            Console.ForegroundColor = ConsoleColor.Red;
                        } else if (pos == head) {
                            Console.ForegroundColor = ConsoleColor.Green;
                        } else if (tail.Contains(pos)) {
                            Console.ForegroundColor = ConsoleColor.White;
                        } else {
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        Console.SetCursorPosition(i, j);
                        Console.Write('█');
                    }
                }

            }
        }

        (int x, int y) NewApple () {
            var apple = (x: 0, y: 0);
            do {
                apple = (rnd.Next() % size.width, rnd.Next() % size.height);
            } while (tail.Contains(apple) && apple != head);
            return apple;
        }
    }
}
