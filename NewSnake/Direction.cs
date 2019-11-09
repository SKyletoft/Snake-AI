using System;
using System.Collections.Generic;
using System.Text;

namespace NewSnake {
    class Direction {
        public static readonly (int x, int y) Up = (0, -1);
        public static readonly (int x, int y) Right = (1, 0);
        public static readonly (int x, int y) Down = (0, 1);
        public static readonly (int x, int y) Left = (-1, 0);
        public static int GetDirection ((int x, int y) direction) {
            var directions = new (int x, int y)[] { (0, -1), (1, 0), (0, 1), (-1, 0) };
            var directionIndex = Array.IndexOf(directions, direction);
            if (directionIndex == -1) {
                directionIndex = 0;
            }
            return directionIndex;
        }
        public static (int x, int y) TransformDirection ((int x, int y) direction, int steps) {
            while (steps < 0) {
                steps += 4;
            }
            steps %= 4;
            while (steps > 0) {
                var tmp = direction.x;
                direction.x = direction.y * -1;
                direction.y = tmp;
                steps--;
            }
            return direction;
        }
        public static (int x, int y) DirectionFromIndex (int index) {
            while (index < 0) {
                index += 4;
            }
            switch (Math.Abs(index) % 4) {
                case 0:
                    return Up;
                case 1:
                    return Right;
                case 2:
                    return Down;
                case 3:
                    return Left;
            }
            return (0, 0);
        }
    }
}
