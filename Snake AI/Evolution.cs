using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Snake_AI {
    public partial class AI {
        public class Evolution {
            List<Network> lastGenSuccesses = new List<Network> { };
            List<Network> recordMakers = new List<Network> { };
            List<Network> currentGen;
            int genSize = 25;
            public int generation = 0;
            int selectionSize = 2;
            public double maxFit = 0;
            public double latestFit = 0;
            public string Champions {
                get {
                    var output = "";
                    for (var i = 0; i < recordMakers.Count; i++) {
                        output += "[" + i + "]: " + recordMakers[i].fitness + "    ";
                    }
                    return output;
                }
            }
            public void init (bool template) {
                Network first = template ? Network.Template() : new Network();
                lastGenSuccesses.Add(first);
                lastGenSuccesses[0].fitness = -1;
            }
            public void cycle (ref Bitmap frame, bool drawToConsole) {
                generation++;
                var directions = new (int, int)[] { (0, -1), (1, 0), (-1, 0) };
                var rnd = new Random();
                currentGen = new List<Network> { };
                if (recordMakers.Count > 0 && (rnd.NextDouble() < 0.01/* || latestFit + 1 < recordMakers[0].fitness*/)) {
                    var x = rnd.Next() % recordMakers.Count;
                    if (recordMakers[x].fitness + 0.1 > maxFit) {
                        Console.WriteLine("Reappearing ancestor, ancestor: {0} last: {1}", recordMakers[x].fitness, latestFit);
                        currentGen.Add(recordMakers[x].Evolve());
                    } else {
                        recordMakers.RemoveAt(x);
                    }
                }
                if (rnd.NextDouble() < 0.00) {
                        Console.WriteLine("Pruning!");

                    for (var i = 0; i < lastGenSuccesses.Count; i++) {
                        lastGenSuccesses[i].Prune();
                    }
                } else {
                    for (var i = 0; i < genSize; i++) {
                        currentGen.Add(lastGenSuccesses[rnd.Next() % lastGenSuccesses.Count].Evolve());
                        var startTime = DateTime.Now;
                        while (!currentGen[i].game.ended) {
                            //Console.WriteLine("\n\nChose: " + directions[currentGen[i].EvaluateOutput()]);
                            currentGen[i].game.playTurn(directions[currentGen[i].EvaluateOutput()], true);
                            if (drawToConsole) {
                                //Console.Clear();
                                Console.WriteLine(generation + ": " + i);
                                currentGen[i].game.RenderSimple();
                            } else {
                                //frame = currentGen[i].game.RenderI();
                            }
                            if (DateTime.Now - startTime > new TimeSpan(0, 0, 0, 0, currentGen[i].game.length * 100)) {
                                break;
                            }
                        }
                        currentGen[i].fitness = currentGen[i].Evaluate();
                    }
                    var fitnesses = new double[genSize];
                    for (var i = 0; i < genSize; i++) {
                        fitnesses[i] = currentGen[i].fitness;
                    }
                    Array.Sort(fitnesses);
                    //if (fitnesses[0] > lastGenSuccesses[0].fitness) {
                    lastGenSuccesses = new List<Network> { };
                    for (var i = 0; i < selectionSize; i++) {
                        for (var j = 0; j < currentGen.Count; j++) {
                            if (currentGen[j].fitness == fitnesses[i]) {
                                lastGenSuccesses.Add(currentGen[i]);
                                break;
                            }
                        }
                    }
                    //}
                    if (lastGenSuccesses[0].fitness > maxFit) {
                        maxFit = lastGenSuccesses[0].fitness;
                        recordMakers.Add(lastGenSuccesses[0]);
                    }
                    latestFit = lastGenSuccesses[0].fitness;
                }
                frame = lastGenSuccesses[0].drawMap(5, ref frame, true);
            }
        }
    }
}
