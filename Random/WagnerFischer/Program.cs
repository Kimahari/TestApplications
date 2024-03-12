using System.Text;

namespace WagnerFischer;

internal class Program {
    static void Main(string[] args) {
        //var word1 = "kitten";
        //var word2 = "sitting";
        //var distance = WagnerFischer(word1, word2);

        WagnerFischer("BOATS", "FLOATS");
        WagnerFischer("DOG", "BOG");
        WagnerFischer("KITTEN", "SITTING");
        WagnerFischer("KITTEN", "SItTiNG");
        WagnerFischer("SITTING", "KITTEN");
        WagnerFischer("AAAAAAAAAAA", "KITTEN");
    }

    private static int WagnerFischer(ReadOnlySpan<char> word1, ReadOnlySpan<char> word2) {
        var matrix = new int[word1.Length + 1, word2.Length + 1];

        for (var i = 0; i <= word1.Length; i++) {
            matrix[i, 0] = i;
        }

        for (var j = 0; j <= word2.Length; j++) {
            matrix[0, j] = j;
        }

        for (var i = 1; i <= word1.Length; i++) {
            for (var j = 1; j <= word2.Length; j++) {
                var cost = word1[i - 1] == word2[j - 1] ? 0 : 1;

                var topLeft = matrix[i - 1, j - 1];
                var left = matrix[i, j - 1];
                var top = matrix[i - 1, j];

                var operationsFromTopLeft = topLeft + cost;
                var operationsFromLeft = left + 1;
                var operationsFromTop = top + 1;

                var minCost = Math.Min(operationsFromTopLeft, Math.Min(operationsFromLeft, operationsFromTop));

                matrix[i, j] = minCost;
            }
        }

        //Console.Clear();

        PrintMatrix(matrix, word1, word2);

        var alteredWord = new Span<char>(new char[Math.Max(word2.Length, word1.Length)]);
        var word1Index = word1.Length;
        var word2Index = word2.Length;
        
        for (var i = 0; i < word1.Length; i++) {
            alteredWord[i] = word1[i];
        }

        var previousCost = matrix[word1Index, word2Index];

        while (word1Index > 0 && word2Index > 0) {

            var topLeft = matrix[word1Index - 1, word2Index - 1];
            var left = matrix[word1Index, word2Index - 1];
            var top = matrix[word1Index - 1, word2Index];

            var minCost = Math.Min(topLeft, Math.Min(left, top));

            var operationRequired = previousCost != minCost;
            previousCost = minCost;

            //Console.WriteLine($"'{alteredWord}' - {operationRequired}");

            if (operationRequired is false) {
                
                if (minCost == topLeft) {
                    word1Index--;
                    word2Index--;
                    continue;
                }

                if (minCost == top) {
                    word1Index--;
                    continue;
                }

                if (minCost == left) {
                    word2Index--;
                    continue;
                }
            }


            if (minCost == top) {
                //delete
                word1Index--;

                if (operationRequired) {
                    alteredWord[word1Index] = '\0';
                }

                //move cursor up
                continue;
            }

            if (minCost == left) {
                var char_to_insert = word2[word2Index - 1];
                //move cursor diagonally

                if (operationRequired) {
                    //move all chars from current index to the right and insert the char at the current index
                    for (var i = alteredWord.Length - 1; i >= word1Index; i--) {
                        alteredWord[i] = alteredWord[i - 1];
                    }

                    alteredWord[word1Index] = char_to_insert;

                }

                word2Index--;

                continue;
            }


            if (minCost == topLeft) {
                var char_to_update = word2[word2Index - 1];

                if (operationRequired) {
                    alteredWord[word1Index - 1] = char_to_update;
                }

                word1Index--;
                word2Index--;

                continue;
            }

            

            
        }

        //Console.WriteLine();
        Console.WriteLine($"'{alteredWord}'");

        return matrix[word1.Length, word2.Length];
    }

    static void PrintMatrix(int[,] matrix, ReadOnlySpan<char> word1, ReadOnlySpan<char> word2) {
        Console.Write("\t_\t");

        for (var i = 0; i < word2.Length; i++) {
            Console.Write($"{word2[i]}\t");
        }
        Console.WriteLine();

        for (var i = 0; i <= word1.Length; i++) {
            if (i == 0) {
                Console.Write("_\t");
            } else {
                Console.Write($"{word1[i - 1]}\t");
            }

            for (var j = 0; j <= word2.Length; j++) {
                Console.Write($"{matrix[i, j]}\t");
            }

            Console.WriteLine();
        }
    }
}
