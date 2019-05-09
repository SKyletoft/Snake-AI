using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake_AI {
    public class Evolution {
        List<AI.Network> lastGenSuccesses = new List<AI.Network> { };
        List<AI.Network> currentGen;
        int genSize;
        int generation;
        int workingOn;
        public void cycle (ref System.Drawing.Bitmap frame) {
            var directions = new (int, int)[] { (1,0), (-1,0), (0,1) };
            var rnd = new Random();
            currentGen = new List<AI.Network> { };
            for (var i = 0; i < genSize; i++) {
                currentGen.Add(lastGenSuccesses[rnd.Next() % lastGenSuccesses.Count].Evolve());
                var startTime = DateTime.Now;
                while (!currentGen[i].game.ended) {
                    currentGen[i].game.playTurn(directions[currentGen[i].EvaluateOutput()]);
                    frame = currentGen[i].game.RenderI();
                    if (DateTime.Now - startTime > new TimeSpan(0,1,0)) {
                        break;
                    }
                }
                currentGen[i].fitness = currentGen[i].Evaluate();
            }
            //TODO: Choose the best individuals

        }
    }
}
