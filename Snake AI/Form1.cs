using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake_AI {

    public partial class Form1:Form {

        AI.Evolution AINetwork = new AI.Evolution();
        bool pause = true;
        bool print = true;

        public Form1 () {
            InitializeComponent();
            AINetwork.init(false);
            var frame2 = new Bitmap(Width, Height);
            var frame = AI.Network.Template().drawMap(5, ref frame2, false);
            pictureBox1.Image = frame;
        }

        private void Form1_KeyDown (object sender, KeyEventArgs e) { }
        Image scaleImage (Bitmap original, int width, int height) {
            var frame = new Bitmap(width, height);
            for (var i = 0; i < width; i++) {
                for (var j = 0; j < height; j++) {
                    frame.SetPixel(i, j, Color.Red);
                    //frame.SetPixel(i, j, original.GetPixel(i / width, j / height));
                }
            }
            for (var i = 0; i < width; i++) {
                for (var j = 0; j < height; j++) {
                    frame.SetPixel(i, j, original.GetPixel(i / 10, j / 10));
                }
            }
            return frame;
        }

        private void Timer1_Tick (object sender, EventArgs e) {
            if (!pause) {
                var frame = new Bitmap(Width, Height);
                AINetwork.cycle(ref frame, print);
                pictureBox1.Image = frame;
                Console.WriteLine(AINetwork.generation + ": " + AINetwork.maxFit + ", " + AINetwork.latestFit);
                label1.Text = AINetwork.Champions;
            }
        }

        private void Btn_pause_Click (object sender, EventArgs e) {
            pause = !pause;
            btn_pause.Text = pause ? "Continue" : "Pause";
        }

        private void Btn_consoleoutput_Click (object sender, EventArgs e) {
            print = !print;
            btn_consoleoutput.Text = print ? "Print" : "Printing";
        }
    }
}
