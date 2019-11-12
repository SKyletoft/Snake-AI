using System;

namespace NewSnake {
    class Program {
        static void Main (string[] args) {
            Console.CursorVisible = false;
            Console.Clear();

            var lastWinner = NeuralNetwork.NewToZero(new[] { 904, 3 });
            //var lastWinner = NeuralNetwork.NewFromManual();
            var generation = 0;

            while (true) {
                lastWinner = lastWinner.NextGeneration((15, 15), 15000, 0.2, 0.1, generation).Item1;
                generation++;
            }

        }
    }
}
