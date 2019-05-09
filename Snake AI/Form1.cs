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
        public Form1 () {
            InitializeComponent();
        }

        private void Form1_KeyDown (object sender, KeyEventArgs e) {}
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
                    frame.SetPixel(i, j, original.GetPixel(i / 20, j / 20));
                }
            }
            return frame;
        }
    }
}
