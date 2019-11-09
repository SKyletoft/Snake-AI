using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NewSnake {
    class NeuralNetwork {

        static Random rnd = new Random();
        static bool flag = false;

        int[] rowSizes;
        (double k, double m)[][][] weights;

        public static NeuralNetwork NewToZero (int[] rowSizes) {
            var newNetwork = new NeuralNetwork();

            newNetwork.rowSizes = rowSizes;
            newNetwork.weights = new (double k, double m)[rowSizes.Length][][];
            for (var i = 1; i < newNetwork.weights.Length; i++) {
                newNetwork.weights[i] = new (double k, double m)[rowSizes[i]][];
                for (var j = 0; j < newNetwork.weights[i].Length; j++) {
                    newNetwork.weights[i][j] = new (double k, double m)[rowSizes[i - 1]];
                    for (var k = 0; k < newNetwork.weights[i][j].Length; k++) {
                        newNetwork.weights[i][j][k] = (0.0, 0.0);
                    }
                }
            }

            return newNetwork;
        }

        public static NeuralNetwork NewToRandom (int[] rowSizes) {
            var zero = NewToZero(rowSizes);
            var child = zero.GenerateKid(1, 1);
            return child;
        }
        public static NeuralNetwork NewFromManual () {
            var newNetwork = new NeuralNetwork();
            newNetwork.rowSizes = new[] { 1, 3 };
            newNetwork.weights = new (double k, double m)[][][] {
                null,
                new (double k, double m)[][] {
                    new (double k, double m)[] {(-1.0, 0.0) },
                    new (double k, double m)[] {(0, 0.1) },
                    new (double k, double m)[] {(1.0, 0.0) }
                }
            };
            return newNetwork;
        }
        public NeuralNetwork GenerateKid (double randomness, double changeRate) {
            var newNetwork = new NeuralNetwork();

            newNetwork.rowSizes = rowSizes;
            newNetwork.weights = new (double k, double m)[rowSizes.Length][][];
            for (var i = 1; i < newNetwork.weights.Length; i++) {
                newNetwork.weights[i] = new (double k, double m)[rowSizes[i]][];
                for (var j = 0; j < newNetwork.weights[i].Length; j++) {
                    newNetwork.weights[i][j] = new (double k, double m)[rowSizes[i - 1]];
                    for (var k = 0; k < newNetwork.weights[i][j].Length; k++) {
                        newNetwork.weights[i][j][k] =
                            (weights[i][j][k].k +
                            (rnd.NextDouble() > changeRate ? 0.0 : ((rnd.NextDouble() - 0.5) * 2 * randomness)),
                            weights[i][j][k].m +
                            (rnd.NextDouble() > changeRate ? 0.0 : ((rnd.NextDouble() - 0.5) * 2 * randomness)));
                    }
                }
            }

            return newNetwork;
        }
        public double PlaySnakeOLD ((int width, int height) size, bool draw) {
            var game = new Game(size.width, size.height);
            var dir = -1;
            do {
                if (draw) {
                    game.DrawToConsole(false);
                }
                var tail = game.Tail;
                var minXDiff = 100;
                var minXDiffIndex = -1;
                var minYDiff = 100;
                var minYDiffIndex = -1;
                for (var i = 0; i < tail.Length; i++) {
                    var x = Math.Abs(game.Head.x - tail[i].x);
                    if (x < minXDiff) {
                        minXDiff = x;
                        minXDiffIndex = i;
                    }
                    var y = Math.Abs(game.Head.y - tail[i].y);
                    if (y < minYDiff) {
                        minYDiff = y;
                        minYDiffIndex = i;
                    }
                };
                var inputs = new double[6] {
                    game.Head.x - game.Apple.x,
                    game.Head.y - game.Apple.y,
                    game.Head.x - tail[minXDiffIndex].x,
                    game.Head.y - tail[minYDiffIndex].y,
                    game.Head.x - size.width,
                    game.Head.y - size.height
                };
                dir = Evaluate(inputs);
            } while (game.PlayTurn(Direction.DirectionFromIndex(dir)));
            return game.Score;
        }
        public double PlaySnake ((int width, int height) size, bool draw) {
            var game = new Game(size.width, size.height);
            var dir = 0;
            do {
                if (draw) {
                    game.DrawToConsole(false);
                }
                var tail = game.Tail;
                var currentDirection = Direction.GetDirection((game.Head.x - tail[tail.Length - 1].x, game.Head.y - tail[tail.Length - 1].y));
                var appleDiff = (x: (game.Head.x - game.Apple.x), y: (game.Head.y - game.Apple.y));
                appleDiff = Direction.TransformDirection(appleDiff, currentDirection);

                var frontCoords = (
                    game.Head.x + Direction.DirectionFromIndex(currentDirection).x,
                    game.Head.y + Direction.DirectionFromIndex(currentDirection).y
                );
                var rightCoords = (
                    game.Head.x + Direction.DirectionFromIndex(currentDirection + 1).x,
                    game.Head.y + Direction.DirectionFromIndex(currentDirection + 1).y
                );
                var leftCoords = (
                    game.Head.x + Direction.DirectionFromIndex(currentDirection - 1).x,
                    game.Head.y + Direction.DirectionFromIndex(currentDirection - 1).y
                );
                var turn = Evaluate(new double[] {
                    Math.Atan(appleDiff.y / (double) appleDiff.x),
                    Array.IndexOf(tail, frontCoords) != -1 ? 1 : 0,
                    Array.IndexOf(tail, leftCoords) != -1 ? 1 : 0,
                    Array.IndexOf(tail, rightCoords) != -1 ? 1 : 0
                }) - 1;
                dir += turn;

            } while (game.PlayTurn(Direction.DirectionFromIndex(dir)));
            return game.Score;
        }
        public int Evaluate (double[] inputs) {
            if (inputs.Length != rowSizes[0]) {
                throw new Exception("Not the correct amount of inputs!");
            }
            var results = new double[rowSizes.Length][];
            results[0] = inputs;
            for (var i = 1; i < results.Length; i++) {
                results[i] = new double[rowSizes[i]];
                for (var j = 0; j < results[i].Length; j++) {
                    results[i][j] = WeightedAverage(results[i - 1], weights[i][j]);
                }
            }

            var last = results.Length - 1;
            var greatestValue = results[last][0];
            var greatestIndex = 0;
            for (var i = 1; i < results[last].Length; i++) {
                if (results[last][i] > greatestValue) {
                    greatestValue = results[last][i];
                    greatestIndex = i;
                }
            }

            return greatestIndex;
        }

        public double WeightedAverage (double[] inputs, (double k, double m)[] weights) {
            var sum = 0.0;
            var weightSum = 0.0;
            if (inputs.Length != weights.Length) {
                throw new Exception("Not the same amount of inputs and weights!");
            }
            for (var i = 0; i < inputs.Length; i++) {
                sum += (inputs[i] * weights[i].k + weights[i].m);
                weightSum += weights[i].k;
            }
            if (weightSum != 0) {
                sum /= weightSum;
            }
            return sum;
        }

        public NeuralNetwork NextGeneration ((int width, int height) boardSize, int generationSize, double randomness, double changeRate, int gen) {
            var newGen = new NeuralNetwork[generationSize];
            var scores = new double[generationSize];
            Parallel.For(0, newGen.Length, (i) => {
            //for (var i = 0; i < newGen.Length; i++) {
                var draw = false;
                if (i == 0 && flag) {
                    draw = true;
                }
                newGen[i] = this.GenerateKid(randomness, changeRate);
                scores[i] = newGen[i].PlaySnake(boardSize, draw);
                });
            //}
            var bestScore = 0.0;
            var bestIndex = -1;
            for (var i = 0; i < scores.Length; i++) {
                if (scores[i] > bestScore) {
                    bestScore = scores[i];
                    bestIndex = i;
                }
            }
            Console.Title = String.Format("Generation {0}; Best score: {1}", gen, bestScore);
            return newGen[bestIndex];
        }
    }
}
