using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using NewSnake;

namespace NewSnake_GUI {
    class Render {
        public static Bitmap Game (int width, int height, Game game) {
            var frame = new Bitmap(width, height);
            var margin = 15;
            var boxWidth = (width - (2 * margin)) / game.Size.width;
            var boxHeight = (height - (2 * margin)) / game.Size.height;
            using (var g = Graphics.FromImage(frame)) {
                g.FillEllipse(
                    Brushes.Red,
                    game.Apple.x * boxWidth + margin,
                    game.Apple.y * boxHeight + margin,
                    boxWidth,
                    boxHeight
                );
                g.FillEllipse(
                    Brushes.Green,
                    game.Head.x * boxWidth + margin,
                    game.Head.y * boxHeight + margin,
                    boxWidth,
                    boxHeight
                );
                foreach (var segment in game.Tail) {
                    g.FillEllipse(
                        Brushes.DarkGreen,
                        segment.x * boxWidth + margin,
                        segment.y * boxHeight + margin,
                        boxWidth,
                        boxHeight
                    );
                }
                g.DrawString(Math.Round(game.Score, 2).ToString(), SystemFonts.DefaultFont, Brushes.Black, 10, 10);
            }
            return frame;
        }
        public static Bitmap NeuralNetwork (int width, int height, NeuralNetwork network) {
            var frame = new Bitmap(width, height);
            var scale = 25;
            using (var g = Graphics.FromImage(frame)) {
                for (var i = 1; i < network.Weights.Length; i++) {
                    for (var j = 0; j < network.Weights[i].Length; j++) {
                        for (var k = 0; k < network.Weights[i][j].Length; k++) {
                            var x1 = i * scale;
                            var y1 = j * scale;
                            var x2 = (i - 1) * scale;
                            var y2 = k * scale;
                            if (network.Weights[i][j][k].k >= 0) {
                                g.DrawLine(
                                    new Pen(Brushes.DarkGreen, (float) network.Weights[i][j][k].m),
                                    x1, y1, x2, y2
                                );
                            } else {
                                g.DrawLine(
                                    new Pen(Brushes.DarkRed, (float) -network.Weights[i][j][k].m),
                                    x1, y1, x2, y2
                                );
                            }
                            x1 += 300;
                            x2 += 300;
                            if (network.Weights[i][j][k].k >= 0) {
                                g.DrawLine(
                                    new Pen(Brushes.Green, (float) network.Weights[i][j][k].k),
                                    x1, y1, x2, y2
                                );
                            } else {
                                g.DrawLine(
                                    new Pen(Brushes.Red, (float) -network.Weights[i][j][k].k),
                                    x1, y1, x2, y2
                                );
                            }
                        }
                    }
                }
            }
            return frame;
        }
    }
}
