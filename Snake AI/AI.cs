using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Snake_AI {
    public partial class AI {
        public class Network {
            public static Network Template () {
                var newNetwork = new Network();
                //(0, -1), (1, 0), (-1, 0)
                //RIGHT: 1,0
                //UP: 0,-1
                //LEFT: -1,0
                for (var i = 0; i < newNetwork.dimensions.width; i++) {
                    for (var j = 0; j < newNetwork.dimensions.height; j++) {
                        newNetwork.level1Nodes.Add((i, j, i < newNetwork.dimensions.width / 2 ? 1 : -1)); //RIGHT
                        newNetwork.outputs[1].Add((1, newNetwork.level1Nodes.Count - 1, 1));
                        newNetwork.level1Nodes.Add((i, j, j < newNetwork.dimensions.width / 2 ? 1 : -1)); //DOWN
                        newNetwork.outputs[0].Add((1, newNetwork.level1Nodes.Count - 1, 1));
                        newNetwork.level1Nodes.Add((i, j, i > newNetwork.dimensions.width / 2 ? 1 : -1)); //LEFT
                        newNetwork.outputs[2].Add((1, newNetwork.level1Nodes.Count - 1, 1));
                    }
                }

                return newNetwork;
            }
            public Network () {
                game.dimensions = (10, 10);
                game.head = (5, 5);
                game.tail.Add((5, 6));
            }
            public Snake game = new Snake();
            public double fitness = 0;
            (int width, int height) dimensions = (21, 21);
            public double randomFactor = 0.05;
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
                try {
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
                }
                catch (Exception e) {
                    Console.WriteLine("Caught evolution error, probably divide by 0\n{0}", e);
                }
                return newNetwork;
            }
            public Bitmap drawMap (float scale, ref Bitmap frame, bool hideUnused) {
                var l1Dis = 85f / (level1Nodes.Count + 2);
                var l2Dis = 85f / (level2Nodes.Count + 2);
                var l3Dis = 85f / 4;
                var map = new Bitmap(frame.Width, frame.Height);
                var g = Graphics.FromImage(map);
                g.DrawImage(Image.FromFile("../../grid" + scale + ".png"), 0, 0);


                var used = new List<int>[] {
                    new List<int> {},
                    new List<int> {}
                };
                for (var i = 0; i < outputs.Length; i++) {
                    for (var j = 0; j < outputs[i].Count; j++) {
                        used[outputs[i][j].nodeLevel - 1].Add(outputs[i][j].nodeId);
                    }
                }

                for (var i = 0; i < level1Nodes.Count; i++) {
                    if (used[0].IndexOf(i) == -1) {
                        g.DrawLine(
                            level1Nodes[i].flip == 1 ? Pens.Red : Pens.Green,
                            scale * (4 * level1Nodes[i].X + 3),
                            scale * (4 * level1Nodes[i].Y + 3),
                            scale * 110,
                            scale * (i + 1) * l1Dis
                        );
                    } else {
                        g.DrawLine(
                            level1Nodes[i].flip == 1 ? Pens.Purple : Pens.Blue,
                            scale * (4 * level1Nodes[i].X + 3),
                            scale * (4 * level1Nodes[i].Y + 3),
                            scale * 110,
                            scale * (i + 1) * l1Dis
                        );
                    }
                }
                for (var i = 0; i < level2Nodes.Count; i++) {
                    if (used[1].IndexOf(i) == -1) {
                        g.DrawLine(
                            level2Nodes[i].flip == 1 ? Pens.Red : Pens.Green,
                            scale * 110,
                            scale * level2Nodes[i].level1Node * l1Dis,
                            scale * 135,
                            scale * (i + 1) * l2Dis
                        );
                    } else {
                        g.DrawLine(
                            level2Nodes[i].flip == 1 ? Pens.Purple : Pens.Blue,
                            scale * 110,
                            scale * level2Nodes[i].level1Node * l1Dis,
                            scale * 135,
                            scale * (i + 1) * l2Dis
                        );
                    }
                }
                for (var i = 0; i < outputs.Length; i++) {
                    for (var j = 0; j < outputs[i].Count; j++) {
                        g.DrawLine(
                            outputs[i][j].flip == 1 ? Pens.Red : Pens.Green,
                            scale * (outputs[i][j].nodeLevel == 1 ? 110 : 135),
                            scale * (outputs[i][j].nodeId * (outputs[i][j].nodeLevel == 0 ? l1Dis : l2Dis)),
                            scale * 160,
                            scale * (i + 1) * l3Dis
                        );
                    }
                }
                return map;
            }
            public void Prune () {
                var used = new List<int>[] {
                    new List<int> {},
                    new List<int> {}
                };
                for (var i = 0; i < outputs.Length; i++) {
                    for (var j = 0; j < outputs[i].Count; j++) {
                        used[outputs[i][j].nodeLevel - 1].Add(outputs[i][j].nodeId);
                    }
                }
                for (var i = 0; i < level1Nodes.Count; i++) {
                    if (used[0].IndexOf(i) == -1) {
                        level1Nodes.RemoveAt(i);
                        i--;
                    }
                }
                for (var i = 0; i < level2Nodes.Count; i++) {
                    if (used[1].IndexOf(i) == -1) {
                        level2Nodes.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    }
}
