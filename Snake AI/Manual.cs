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
    public partial class Manual:Form {
        AI.Network network = new AI.Network();
        (int, int)[] directions = new (int, int)[] { (0, -1), (1, 0), (-1, 0) };
        public Manual () {
            InitializeComponent();
            label1.Text = directions[network.EvaluateOutput()].ToString();
        }

        private void Manual_KeyDown (object sender, KeyEventArgs e) {

            switch (e.KeyCode) {
                case Keys.Left:

                    break;
            }

            label1.Text = directions[network.EvaluateOutput()].ToString();
        }
    }
}
