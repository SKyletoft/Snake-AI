using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using NewSnake;

namespace NewSnake_GUI {
    public partial class Form1 : Form {
        NeuralNetwork lastWinner = NeuralNetwork.NewToRandom(new[] { 4, 7, 7, 7, 7, 3 });
        List<(NeuralNetwork, double)> oldWinners = new List<(NeuralNetwork, double)>();
        NeuralNetwork currentlyShowing;
        bool runningAGame = false;
        Game game;
        int dir = 0;
        public Thread workerThread;
        public Form1 () {
            InitializeComponent();
            lastWinner.CentreNetwork();
            oldWinners.Add((lastWinner, 0));
            workerThread = new Thread(worker);
            workerThread.Start();
        }

        public void worker () {
            var generation = 0;
            var score = 0.0;
            while (true) {
                (lastWinner, score) = lastWinner.NextGeneration((15, 15), (int) (Math.Pow(10, score) + 500), 0.4, 0.2, generation);
                //if (lastWinner != oldWinners[oldWinners.Count - 1].Item1 || score != oldWinners[oldWinners.Count - 1].Item2) {
                    oldWinners.Add((lastWinner, score));
                //}
                generation++;
            }
        }

        private void timer1_Tick (object sender, EventArgs e) {
            for (var i = listBox1.Items.Count; i < oldWinners.Count; i++) {
                listBox1.Items.Add(String.Format("Generation {0}, {1}", i, Math.Round(oldWinners[i].Item2, 2)));
            }
        }

        private void listBox1_MouseDoubleClick (object sender, MouseEventArgs e) {
            if (listBox1.SelectedIndex == -1) {
                listBox1.SelectedIndex = 0;
            }
            currentlyShowing = oldWinners[listBox1.SelectedIndex].Item1;
            game = new Game(15, 15, (int) numericUpDown1.Value);
            dir = 0;
            runningAGame = true;
            pictureBox2.Image = Render.NeuralNetwork(pictureBox2.Width, pictureBox2.Height, currentlyShowing);
        }

        private void timer2_Tick (object sender, EventArgs e) {
            if (runningAGame) {
                runningAGame = currentlyShowing.PlayRoundOfSnake(ref game, ref dir);
                pictureBox1.Image = Render.Game(pictureBox1.Width, pictureBox1.Height, game);
            }
        }
    }
}
