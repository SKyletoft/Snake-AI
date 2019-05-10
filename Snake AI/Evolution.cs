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
            List<Network> currentGen;
            int genSize = 25;
            int generation = 0;
            int selectionSize = 2;
            public void init () {
                lastGenSuccesses.Add(new Network());
            }
            public void cycle (ref Bitmap frame, bool drawToConsole) {
                generation++;
                var directions = new (int, int)[] { (1, 0), (-1, 0), (0, 1) };
                var rnd = new Random();
                currentGen = new List<Network> { };
                for (var i = 0; i < genSize; i++) {
                    currentGen.Add(lastGenSuccesses[rnd.Next() % lastGenSuccesses.Count].Evolve());
                    var startTime = DateTime.Now;
                    while (!currentGen[i].game.ended) {
                        currentGen[i].game.playTurn(directions[currentGen[i].EvaluateOutput()]);
                        if (drawToConsole) {
                            currentGen[i].game.RenderSimple();
                        } else {
                            frame = currentGen[i].game.RenderI();
                        }
                        if (DateTime.Now - startTime > new TimeSpan(0, 1, 0)) {
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
                lastGenSuccesses = new List<Network> { };
                for (var i = 0; i < selectionSize; i++) {
                    for (var j = 0; j < currentGen.Count; j++) {
                        if (currentGen[j].fitness == fitnesses[i]) {
                            lastGenSuccesses.Add(currentGen[i]);
                        }
                    }
                }
            }
        }
    }
}
