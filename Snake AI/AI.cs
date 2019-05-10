using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Snake_AI {
    public partial class AI {
        public class Network {
            public Network () {
                game.dimensions = (10, 10);
                game.head = (5, 5);
                game.tail.Add((5, 6));
            }
            public Snake game = new Snake();
            public double fitness = 0;
            (int width, int height) dimensions = (21, 21);
            double randomFactor = 0.05;
            List<(int X, int Y, int flip)> level1Nodes = new List<(int X, int Y, int flip)> { };
            List<(int level1Node, int flip)> level2Nodes = new List<(int level1Node, int flip)> { };
            List<(int nodeLevel, int nodeId, int flip)>[] outputs = new List<(int nodeLevel, int nodeId, int flip)>[] {
                new List<(int nodeLevel, int nodeId, int flip)> {},
                new List<(int nodeLevel, int nodeId, int flip)> {},
                new List<(int nodeLevel, int nodeId, int flip)> {}
            };
            public int EvaluateOutput () {
                var map = game.RenderB();
                int[] values = { 0, 0, 0 };
                for (var i = 0; i < outputs.Length; i++) {
                    for (var j = 0; j < outputs[i].Count; j++) {
                        if (outputs[i][j].nodeLevel == 2) {
                            //Level 2
                            values[i] += map[
                                    level1Nodes[level2Nodes[outputs[i][j].nodeId].level1Node].X,
                                    level1Nodes[level2Nodes[outputs[i][j].nodeId].level1Node].Y
                                ] *
                                level1Nodes[level2Nodes[outputs[i][j].nodeId].level1Node].flip *
                                level2Nodes[outputs[i][j].nodeId].flip *
                                outputs[i][j].flip;
                        } else {
                            //Level 1
                            values[i] += map[
                                    level1Nodes[outputs[i][j].nodeId].X,
                                    level1Nodes[outputs[i][j].nodeId].Y
                                ] *
                                level1Nodes[outputs[i][j].nodeId].flip *
                                outputs[i][j].flip;
                        }
                    }
                }
                return Array.IndexOf(values, Math.Max(Math.Max(values[0], values[1]), values[2]));
            }
            public double Evaluate () {
                if (game.ended) {
                    var hypotenuse = Math.Sqrt(
                        game.dimensions.height *
                        game.dimensions.height +
                        game.dimensions.width *
                        game.dimensions.width);
                    var distanceToApple = Math.Sqrt(
                        (game.head.X - game.apple.X) *
                        (game.head.X - game.apple.X) +
                        (game.head.Y - game.apple.Y) *
                        (game.head.Y - game.apple.Y)
                    );
                    return game.length + distanceToApple / hypotenuse;
                }
                return -1;
            }
            public Network Clone () {
                var newNetwork = new Network();
                newNetwork.dimensions = dimensions;
                newNetwork.level1Nodes.AddRange(level1Nodes);
                newNetwork.level2Nodes.AddRange(level2Nodes);
                newNetwork.outputs[0].AddRange(outputs[0]);
                newNetwork.outputs[1].AddRange(outputs[1]);
                newNetwork.outputs[2].AddRange(outputs[2]);
                newNetwork.randomFactor = randomFactor;

                return newNetwork;
            }
            public Network Evolve () {
                var rnd = new Random();
                var newNetwork = this.Clone();
                //Create new nodes
                while (rnd.NextDouble() < randomFactor) {
                    newNetwork.level1Nodes.Add((rnd.Next() % dimensions.width, rnd.Next() % dimensions.height, (rnd.Next() % 2) == 0 ? -1 : 1));
                }
                while (rnd.NextDouble() < randomFactor && newNetwork.level1Nodes.Count > 0) {
                    newNetwork.level2Nodes.Add((rnd.Next() % newNetwork.level1Nodes.Count, (rnd.Next() % 2) == 0 ? -1 : 1));
                }
                while (rnd.NextDouble() < randomFactor) {
                    var nodeLevel = rnd.Next() % 2;
                    if ((nodeLevel == 0 && newNetwork.level1Nodes.Count > 0) || (nodeLevel == 1) && newNetwork.level2Nodes.Count > 0) {
                        newNetwork.outputs[rnd.Next() % 3].Add(
                            (
                                nodeLevel + 1,
                                nodeLevel == 0 ? rnd.Next() % newNetwork.level1Nodes.Count : rnd.Next() % newNetwork.level2Nodes.Count,
                                (rnd.Next() % 2) == 0 ? -1 : 1
                            )
                        );
                    }

                }
                //Edit old ones
                while (rnd.NextDouble() < randomFactor && newNetwork.level1Nodes.Count > 0) {
                    newNetwork.level1Nodes[rnd.Next() % newNetwork.level1Nodes.Count] = (
                        rnd.Next() % dimensions.width,
                        rnd.Next() % dimensions.height,
                        (rnd.Next() % 2) == 0 ? -1 : 1
                    );
                }
                while (rnd.NextDouble() < randomFactor && newNetwork.level2Nodes.Count > 0) {
                    newNetwork.level2Nodes[rnd.Next() % newNetwork.level2Nodes.Count] = ((rnd.Next() % newNetwork.level1Nodes.Count, (rnd.Next() % 2) == 0 ? -1 : 1));
                }
                while (rnd.NextDouble() < randomFactor) {
                    var nodeLevel = rnd.Next() % 2;
                    var output = rnd.Next() % 3;
                    if (newNetwork.outputs[output].Count > 0) {
                        newNetwork.outputs[output][rnd.Next() % newNetwork.outputs[output].Count] = (
                            nodeLevel + 1,
                            nodeLevel == 0 ? rnd.Next() % newNetwork.level1Nodes.Count : rnd.Next() % newNetwork.level2Nodes.Count,
                            (rnd.Next() % 2) == 0 ? -1 : 1
                        );
                    }
                }

                return newNetwork;
            }
        }
    }
}
