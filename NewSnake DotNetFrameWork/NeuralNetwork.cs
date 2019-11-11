using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NewSnake {
    public class NeuralNetwork {

        static Random rnd = new Random(DateTime.Now.Millisecond + DateTime.Now.Second * 1000 + DateTime.Now.Minute * 60000);
        static bool flag = false;

        int[] rowSizes;
        (double k, double m)[][][] weights;
        public (double k, double m)[][][] Weights {
            get {
                return weights;
            }
        }

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
            newNetwork.rowSizes = new[] { 5, 3 };
            newNetwork.weights = new (double k, double m)[][][] {
                null,
                new (double k, double m)[][] {
                    new (double k, double m)[] { //LEFT
                        (1.1, 0.0), (0.0, 0.0), (0.0, 0.0), (-2.0, 0.0), (0.0, 0.0)
                    },
                    new (double k, double m)[] { //FRONT
                        (0.0, 1.0), (0.0, 0.0), (-2.0, 0.0), (0.0, 0.0), (0.0, 0.0)
                    },
                    new (double k, double m)[] { //RIGHT
                        (-1.1, 0.0), (0.0, 0.0), (0.0, 0.0), (0.0, 0.0), (-2.0, 0.0)
                    }
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
                            ((weights[i][j][k].k -
                            (rnd.NextDouble() > changeRate ? 0.0 : ((rnd.NextDouble() - 0.5) * 2 * randomness))),
                            (weights[i][j][k].m -
                            (rnd.NextDouble() > changeRate ? 0.0 : ((rnd.NextDouble() - 0.5) * 2 * randomness))));
                    }
                }
            }
            //newNetwork.CentreNetwork();

            return newNetwork;
        }
        public void CentreNetwork () {
            var sum = 0.0;
            var weightCount = 0.0;
            for (var i = 1; i < weights.Length; i++) {
                for (var j = 0; j < weights[i].Length; j++) {
                    for (var k = 0; k < weights[i][j].Length; k++) {
                        sum += weights[i][j][k].k;
                        sum += weights[i][j][k].m;
                        weightCount += 2;
                    }
                }
            }
            if (sum != 0.0) {
                sum /= weightCount;
                for (var i = 1; i < weights.Length; i++) {
                    for (var j = 0; j < weights[i].Length; j++) {
                        for (var k = 0; k < weights[i][j].Length; k++) {
                            weights[i][j][k].k -= sum;
                            weights[i][j][k].m -= sum;
                        }
                    }
                }
            }
        }
        public double PlaySnakeOLD ((int width, int height) size, bool draw) {
            var game = new Game(size.width, size.height, 0);
            var dir = -1;
            var res = 2;
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
                res = game.PlayTurn(Direction.DirectionFromIndex(dir));
            } while (res == 2);
            return game.Score * res;
        }
        public double PlaySnake ((int width, int height) size, bool draw, int seed) {
            var game = new Game(size.width, size.height, seed);
            var dir = 0;
            var res = 0;
            do {
                if (draw) {
                    game.DrawToConsole(false);
                }
                res = PlayRoundOfSnake(ref game, ref dir);
            } while (res == 2);
            return game.Score * res;
        }
        public int PlayRoundOfSnake (ref Game game, ref int dir) {
            var tail = game.Tail;
            var currentDirection = Direction.GetDirection((game.Head.x - tail[tail.Length - 1].x, game.Head.y - tail[tail.Length - 1].y));
            var appleDiff = (x: (game.Head.x - game.Apple.x), y: (game.Head.y - game.Apple.y));
            appleDiff = Direction.TransformDirection(appleDiff, 4 - currentDirection);

            var frontCoords = (
                x: game.Head.x + Direction.DirectionFromIndex(currentDirection).x,
                y: game.Head.y + Direction.DirectionFromIndex(currentDirection).y
            );
            var rightCoords = (
                x: game.Head.x + Direction.DirectionFromIndex(currentDirection + 1).x,
                y: game.Head.y + Direction.DirectionFromIndex(currentDirection + 1).y
            );
            var leftCoords = (
                x: game.Head.x + Direction.DirectionFromIndex(currentDirection - 1).x,
                y: game.Head.y + Direction.DirectionFromIndex(currentDirection - 1).y
            );
            var turn = Evaluate(new double[] {
                appleDiff.x,
                appleDiff.y,
                (Array.IndexOf(tail, frontCoords) != -1 || frontCoords.x > game.Size.width || frontCoords.x < 0 || frontCoords.y > game.Size.height || frontCoords.y < 0) ? 1 : 0,
                (Array.IndexOf(tail, leftCoords) != -1 || leftCoords.x > game.Size.width || leftCoords.x < 0 || leftCoords.y > game.Size.height || leftCoords.y < 0) ? 1 : 0,
                (Array.IndexOf(tail, rightCoords) != -1 || rightCoords.x > game.Size.width || rightCoords.x < 0 || rightCoords.y > game.Size.height || rightCoords.y < 0) ? 1 : 0
            }) - 1;
            dir += turn;
            return game.PlayTurn(Direction.DirectionFromIndex(dir));
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
                //sum /= weightSum;
            }
            return sum;
        }

        public (NeuralNetwork, double) NextGeneration ((int width, int height) boardSize, int generationSize, double randomness, double changeRate, int gen) {
            var newGen = new NeuralNetwork[generationSize];
            var scores = new double[generationSize];
            var games = 1000;
            var thisScore = 0.0;
            for (var j = 0; j < games; j++) {
                thisScore += PlaySnake(boardSize, false, j);
            }
            thisScore /= games;
            newGen[0] = this;
            scores[0] = thisScore;
            var options = new ParallelOptions();
            options.MaxDegreeOfParallelism = System.Environment.ProcessorCount - 1;
            Parallel.For(1, newGen.Length, options, (i) => {
                //for (var i = 1; i < newGen.Length - 1; i++) {
                var draw = false;
                if (i == 0 && flag) {
                    draw = true;
                }
                newGen[i] = GenerateKid(randomness, changeRate);
                var scoreSum = 0.0;
                for (var j = 0; j < games; j++) {
                    scoreSum += newGen[i].PlaySnake(boardSize, draw, j);
                }
                scores[i] = scoreSum / games;
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
            //Console.Title = String.Format("Generation {0}; Best score: {1}", gen, bestScore);
            return (newGen[bestIndex], bestScore);
        }
        public static bool operator == (NeuralNetwork left, NeuralNetwork right) {
            if (left.rowSizes.Length != right.rowSizes.Length) {
                return false;
            }
            for (var i = 0; i < left.rowSizes.Length; i++) {
                if (left.rowSizes[i] != right.rowSizes[i]) {
                    return false;
                }
            }
            if (left.weights.Length != right.weights.Length) {
                return false;
            }
            for (var i = 0; i < left.weights.Length; i++) {

            }
            for (var i = 1; i < left.weights.Length; i++) {
                if (left.weights[i].Length != right.weights[i].Length) {
                    return false;
                }
                for (var j = 0; j < left.weights[i].Length; j++) {
                    if (left.weights[i][j].Length != right.weights[i][j].Length) {
                        return false;
                    }
                    for (var k = 0; k < left.weights[i][j].Length; k++) {
                        if (left.weights[i][j][k] != right.weights[i][j][k]) {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        public static bool operator != (NeuralNetwork left, NeuralNetwork right) {
            return !(left == right);
        }
    }
}
