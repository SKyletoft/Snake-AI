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
        List<(NeuralNetwork, double)> oldWinnersRan = new List<(NeuralNetwork, double)>();
        List<(NeuralNetwork, double)> oldWinnersZero = new List<(NeuralNetwork, double)>();
        List<(NeuralNetwork, double)> oldWinnersMan = new List<(NeuralNetwork, double)>();
        NeuralNetwork currentlyShowing;
        bool runningAGame = false;
        Game game;
        int dir = 0;
        public Thread workerThread1;
        public Thread workerThread2;
        public Thread workerThread3;
        public Form1 () {
            InitializeComponent();
            //lastWinner.CentreNetwork();
            oldWinnersRan.Add((NeuralNetwork.NewToRandom(new[] { 5, 7, 3}), 0));
            oldWinnersZero.Add((NeuralNetwork.NewToZero(new[] { 5, 7, 3}), 0));
            oldWinnersMan.Add((NeuralNetwork.NewFromManual(), 0));
            
            workerThread1 = new Thread(w1);
            workerThread1.Start();
            workerThread2 = new Thread(w2);
            workerThread2.Start();
            workerThread3 = new Thread(w3);
            workerThread3.Start();
            
            

        }
        public void w1 () {
            worker(ref oldWinnersRan);
        }
        public void w2 () {
            worker(ref oldWinnersZero);
        }
        public void w3 () {
            worker(ref oldWinnersMan);
        }
        public void worker (ref List<(NeuralNetwork, double)> oldWinners) {
            var lastWinner = oldWinners[oldWinners.Count - 1].Item1;
            var generation = 0;
            var score = 0.0;
            while (true) {
                (lastWinner, score) = lastWinner.NextGeneration((15, 15), 1500, 0.05, 1, generation);
                if (lastWinner != oldWinners[oldWinners.Count - 1].Item1 || score != oldWinners[oldWinners.Count - 1].Item2) {
                    oldWinners.Add((lastWinner, score));
                } else {
                    lastWinner = oldWinners[oldWinners.Count - 1].Item1;
                }
                generation++;
            }
        }

        private void timer1_Tick (object sender, EventArgs e) {
            for (var i = listBox1.Items.Count; i < oldWinnersRan.Count; i++) {
                listBox1.Items.Add(String.Format("Generation {0:00}, {1:00.00000}, {2:00.00}", i, oldWinnersRan[i].Item2, oldWinnersRan[i].Item1.PlaySnake((15, 15), false, 0)));
            }
            for (var i = listBox2.Items.Count; i < oldWinnersZero.Count; i++) {
                listBox2.Items.Add(String.Format("Generation {0:00}, {1:00.00000}, {2:00.00}", i, oldWinnersZero[i].Item2, oldWinnersZero[i].Item1.PlaySnake((15, 15), false, 0)));
            }
            for (var i = listBox3.Items.Count; i < oldWinnersMan.Count; i++) {
                listBox3.Items.Add(String.Format("Generation {0:00}, {1:00.00000}, {2:00.00}", i, oldWinnersMan[i].Item2, oldWinnersMan[i].Item1.PlaySnake((15, 15), false, 0)));
            }
        }

        private void listBox1_MouseDoubleClick (object sender, MouseEventArgs e) {
            if (listBox1.SelectedIndex == -1) {
                listBox1.SelectedIndex = 0;
            }
            currentlyShowing = oldWinnersRan[listBox1.SelectedIndex].Item1;
            game = new Game(15, 15, (int) numericUpDown1.Value);
            dir = 0;
            runningAGame = true;
            pictureBox2.Image = Render.NeuralNetwork(pictureBox2.Width, pictureBox2.Height, currentlyShowing);
            pictureBox1.Image = Render.Game(pictureBox1.Width, pictureBox1.Height, game);
        }
        private void listBox2_MouseDoubleClick (object sender, MouseEventArgs e) {
            if (listBox2.SelectedIndex == -1) {
                listBox2.SelectedIndex = 0;
            }
            currentlyShowing = oldWinnersZero[listBox2.SelectedIndex].Item1;
            game = new Game(15, 15, (int) numericUpDown1.Value);
            dir = 0;
            runningAGame = true;
            pictureBox2.Image = Render.NeuralNetwork(pictureBox2.Width, pictureBox2.Height, currentlyShowing);
            pictureBox1.Image = Render.Game(pictureBox1.Width, pictureBox1.Height, game);
        }
        private void listBox3_MouseDoubleClick (object sender, MouseEventArgs e) {
            if (listBox3.SelectedIndex == -1) {
                listBox3.SelectedIndex = 0;
            }
            currentlyShowing = oldWinnersMan[listBox3.SelectedIndex].Item1;
            game = new Game(15, 15, (int) numericUpDown1.Value);
            dir = 0;
            runningAGame = true;
            pictureBox2.Image = Render.NeuralNetwork(pictureBox2.Width, pictureBox2.Height, currentlyShowing);
            pictureBox1.Image = Render.Game(pictureBox1.Width, pictureBox1.Height, game);
        }

        private void timer2_Tick (object sender, EventArgs e) {
            if (runningAGame) {
                runningAGame = currentlyShowing.PlayRoundOfSnake(ref game, ref dir) == 2;
                pictureBox1.Image = Render.Game(pictureBox1.Width, pictureBox1.Height, game);
            }
        }

        private void button3_Click (object sender, EventArgs e) {
            workerThread3.Abort();
        }

        private void button2_Click (object sender, EventArgs e) {
            workerThread2.Abort();
        }

        private void button1_Click (object sender, EventArgs e) {
            workerThread1.Abort();
        }
    }
}
