using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace DecodeLinearCode
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Read & Write Matrix
            Console.WriteLine("Enter number of rows in the Matrix:");
            int rows = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter number of cols in the Matrix:");
            int cols = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter Matrix G=(E|A) with spaces between each number:");
            int[,] matrixG = CreateInitialMatrix(rows, cols);

            //AllCodeWords
            List<int[]> allCodeWords = new List<int[]>();
            CodeWords(allCodeWords, matrixG);
            allCodeWords.ForEach(x => Console.WriteLine(string.Join(' ', x)));
            List<string> codeWords = CodeWordsToStrings(allCodeWords);
            Console.WriteLine("-------------------------");
            Console.WriteLine("All Code Words:");

            //H = AT|E
            int[,] matrixH = CreateH(matrixG);
            PrintMatrix(matrixH);
            Console.WriteLine("-------------------------");
            Console.WriteLine($"H = AT|E ");

            //Read Y vector
            int[] vectorY = Console.ReadLine().Split().Select(int.Parse).ToArray();
            Console.WriteLine("-------------------------");
            Console.WriteLine("Enter y Vector with spaces between each number: ");
           

            //H.Yt = if 0 all lengths => noErrors else search subClasses
            int[] HxYt = CreateVectorFromMultiplyHandVector(matrixH, vectorY);
            Console.WriteLine("-------------------------");
            Console.WriteLine($"H*Yt = {string.Join(", ", HxYt)}");

            //BinaryTable of 2 of the power of Matrix[0].length;
            List<string> table = GenerateBinaryNumber(matrixG.GetLength(1));
            Console.WriteLine("-------------------------");
            Console.WriteLine("BinaryTable:");
            Console.WriteLine(string.Join(" | ", table));


            //RemoveFromTable codeWords 
            RemoveFromTable(table, codeWords);
            Console.WriteLine("-------------------------");
            Console.WriteLine("BinaryTable:");
            Console.WriteLine(string.Join(" | ", table));

            //SubClasses
            //Classes With leaders L[]
            List<string> leaders = new List<string>();
            List<List<string>> cClasses = CreateSubClasses(table, codeWords, leaders);


            //find X = y + l; X == CodeWord
            string syndrome = FindSyndrome(leaders, HxYt, matrixH, vectorY);
            Console.WriteLine("------------------------");
            Console.WriteLine($"The syndrome is: {syndrome}.");

           
            string x = FindX(syndrome, vectorY, codeWords);
            Console.WriteLine("------------------------");
            Console.WriteLine($"X = Y({string.Join(string.Empty, vectorY)}) + leader({syndrome})");
            Console.WriteLine($"X = {x} => C{codeWords.IndexOf(x) + 1}");



        }

        private static string FindX(string syndrome, int[] vectorY, List<string> codeWords)
        {
            string yString = string.Join(string.Empty, vectorY);

            string buff = string.Empty;
            for (int i = 0; i < syndrome.Length; i++)
            {
                buff += SumChars(syndrome[i], yString[i]);
            }

            if (codeWords.Contains(buff))
            {
                return buff;
            }

            return null;

        }
        private static string FindSyndrome(List<string> leaders, int[] HxYt, int[,] matrix, int[] vectorY)
        {
            string x = string.Empty;
            string yString = string.Join(string.Empty, HxYt);

            List<string> matrixRows = new List<string>();
            List<string> results = new List<string>();


            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                string buff = string.Empty;
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    buff += matrix[i, j].ToString();
                }

                matrixRows.Add(buff);
            }


            for (int c = 0; c < leaders.Count; c++)
            {
                string result = string.Empty;
                for (int i = 0; i < matrixRows.Count; i++)
                {
                    string buff = string.Empty;
                    for (int j = 0; j < vectorY.Length; j++)
                    {
                        buff += MultiplyChars(matrixRows[i][j], leaders[c][j]);
                    }

                    char resultLeader = buff[0];
                    for (int j = 1; j < buff.Length; j++)
                    {
                        resultLeader = SumChars(resultLeader, buff[j]);
                    }

                    result += resultLeader.ToString();
                }

                results.Add(result);
                Console.WriteLine("------------------------");
                Console.WriteLine($"MatrixH * leader({leaders[c]}) = ({string.Join(',', result)})");
                if (result == yString)
                {
                    return leaders[c];
                }
            }

            return null;
        }
        private static List<List<string>> CreateSubClasses(List<string> table, List<string> codeWords, List<string> leaders)
        {
            List<List<string>> cList = new List<List<string>>();

            while (table.Count > 0)
            {
                string cName = WeightWord(table);
                List<string> classList = new List<string>();
                leaders.Add(cName);
                classList.Add(cName);
                for (int i = 0; i < codeWords.Count; i++)
                {
                    string currName = string.Empty;
                    for (int j = 0; j < cName.Length; j++)
                    {
                        currName += SumChars(cName[j], codeWords[i][j]);
                    }

                    classList.Add(currName);
                    table.Remove(currName);
                }

                cList.Add(classList);
                string clas = $"C({cName}) = ({string.Join(", ", classList)})";
                Console.WriteLine("-------------------------");
                Console.WriteLine(clas);
                //Console.WriteLine("BinaryTable:");
                //Console.WriteLine(string.Join(" | ", table));
            }

            return cList;
        }
        private static string WeightWord(List<string> table)
        {
            Dictionary<string, int> kvp = new Dictionary<string, int>();
            foreach (var s in table)
            {
                int counter = 0;
                foreach (var ch in s)
                {
                    if (ch == '1')
                    {
                        counter++;
                    }
                }

                kvp.Add(s, counter);
            }

            string word = string.Empty;
            foreach (var i in kvp.OrderBy(x => x.Value))
            {
                word = i.Key;
                break;
            }

            table.Remove(word);
            return word;

        }
        private static char SumChars(char current, char next)
        {
            if (current == '1' && next == '1')
            {
                current = '0';
            }
            else if (current == '1' && next == '0')
            {
                current = '1';
            }
            else if (current == '0' && next == '1')
            {
                current = '1';
            }
            else
            {
                current = '0';
            }

            return current;
        }
        private static char MultiplyChars(char current, char next)
        {
            if (current == '1' && next == '1')
            {
                current = '1';
            }
            else if (current == '1' && next == '0')
            {
                current = '0';
            }
            else if (current == '0' && next == '1')
            {
                current = '0';
            }
            else
            {
                current = '0';
            }

            return current;
        }
        private static void RemoveFromTable(List<string> table, List<string> codeWords)
        {
            foreach (var word in codeWords)
            {
                if (table.Contains(word))
                {
                    table.Remove(word);
                }
            }
        }
        private static List<string> CodeWordsToStrings(List<int[]> allCodeWords)
        {
            List<string> result = new List<string>();
            foreach (var word in allCodeWords)
            {
                string buff = string.Empty;
                foreach (var num in word)
                {
                    buff += num.ToString();
                }

                result.Add(buff);
            }

            return result;
        }
        public static List<string> GenerateBinaryNumber(int n)
        {
            Queue<string> binaryGenerationQueue = new Queue<string>();
            List<string> results = new List<string>();
            string zeroString = HowManyZeroesString(n);
            double length = Math.Pow(2, n);
            binaryGenerationQueue.Enqueue(zeroString + "1");

            while (length > 1)
            {
                string current = binaryGenerationQueue.Dequeue();
                if (current.Length > n)
                {
                    current = current.Remove(0, 1);
                }

                results.Add(current);
                string appendZero = current + "0";
                string appendOne = current + "1";
                binaryGenerationQueue.Enqueue(appendZero);
                binaryGenerationQueue.Enqueue(appendOne);
                length--;
            }

            return results;
        }
        private static string HowManyZeroesString(int i)
        {
            if (i == 3)
            {
                return "00";
            }

            if (i == 4)
            {
                return "000";
            }

            if (i == 5)
            {
                return "0000";
            }

            if (i == 6)
            {
                return "00000";
            }

            if (i == 7)
            {
                return "000000";
            }

            if (i == 8)
            {
                return "0000000";
            }

            if (i == 9)
            {
                return "00000000";
            }

            return "000000000";
        }
        private static int[] CreateVectorFromMultiplyHandVector(int[,] matrixH, int[] vectorY)
        {
            List<List<int>> matrixRows = new List<List<int>>();
            int[] multipliedVector = new int[matrixH.GetLength(0)];
            List<int> results = new List<int>();


            for (int i = 0; i < matrixH.GetLength(0); i++)
            {
                matrixRows.Add(new List<int>());
                for (int j = 0; j < matrixH.GetLength(1); j++)
                {
                    matrixRows[i].Add(matrixH[i, j]);
                }
            }

            foreach (var row in matrixRows)
            {
                List<int> storedNums = new List<int>();
                for (int i = 0; i < vectorY.Length; i++)
                {
                    
                    storedNums.Add(MultiplyNums(row[i], vectorY[i]));
                }

                int result = storedNums[0];
                for (int i = 1; i < storedNums.Count; i++)
                {
                    result = SumNums(result, storedNums[i]);
                }

                results.Add(result);
            }

            for (int i = 0; i < results.Count; i++)
            {
                multipliedVector[i] = results[i];
            }

            return multipliedVector;
        }
        private static int[,] CreateH(int[,] matrixG)
        {

            int[,] matrix = new int[matrixG.GetLength(1) - matrixG.GetLength(0), matrixG.GetLength(0)];
            int[,] matrixE = new int[matrixG.GetLength(1) - matrixG.GetLength(0),
                matrixG.GetLength(1) - matrixG.GetLength(0)];
            CreateMatrixE(matrixE);
            int length = matrixG.GetLength(1) - matrix.GetLength(1);
            List<List<int>> rows = new List<List<int>>();
            for (int i = 0; i < matrixG.GetLength(0); i++)
            {
                rows.Add(new List<int>());

                for (int j = matrix.GetLength(1); j < matrixG.GetLength(1); j++)
                {
                    rows[i].Add(matrixG[i, j]);
                }
            }

            for (int i = 0; i < rows[0].Count; i++)
            {
                for (int j = 0; j < rows.Count; j++)
                {
                    matrix[i, j] = rows[j][i];
                }
            }

            int[,] matrixH = new int[matrixE.GetLength(0), matrixG.GetLength(1)];

            for (int i = 0; i < matrixE.GetLength(0); i++)
            {
                for (int j = 0; j < matrixG.GetLength(1); j++)
                {
                    if (j < matrix.GetLength(1))
                    {
                        matrixH[i, j] = matrix[i, j];
                    }
                    else
                    {
                        matrixH[i, j] = matrixE[i, j - matrix.GetLength(1)];
                    }
                }
            }

            return matrixH;
        }
        private static void CodeWords(List<int[]> allCodeWords, int[,] matrixG)
        {
            int[] sumAll = new int[matrixG.GetLength(1)];

            for (int i = 0; i < matrixG.GetLength(0); i++)
            {
                int[] row = new int[matrixG.GetLength(1)];
                for (int j = 0; j < matrixG.GetLength(1); j++)
                {
                    row[j] = matrixG[i, j];
                }

                allCodeWords.Add(row);
            }

            int length = allCodeWords.Count;

            if (length == 2)
            {
                int[] row = new int[matrixG.GetLength(1)];
                for (int j = 0; j < allCodeWords[0].Length; j++)
                {
                    row[j] = SumNums(allCodeWords[0][j], allCodeWords[0 + 1][j]);
                }
                allCodeWords.Add(row);
            }

            if (length == 3)
            {
                List<int[]> tempWords = new List<int[]>();
                for (int i = 0; i < length - 1; i++)
                {
                    int[] row = new int[matrixG.GetLength(1)];
                    int[] nextRow = new int[matrixG.GetLength(1)];
                    for (int j = 0; j < allCodeWords[i].Length; j++)
                    {
                        row[j] = SumNums(allCodeWords[i][j], allCodeWords[i + 1][j]);
                        if (i + 2 < length)
                        {
                            sumAll[j] = SumNums(allCodeWords[i][j], allCodeWords[i + 1][j]);
                            sumAll[j] = SumNums(sumAll[j], allCodeWords[i + 2][j]);
                            nextRow[j] = SumNums(allCodeWords[i][j], allCodeWords[i + 2][j]);

                        }
                    }
                    tempWords.Add(row);
                    if (i + 2 < length)
                    {
                        tempWords.Add(nextRow);
                    }

                }

                foreach (var tempWord in tempWords)
                {
                    allCodeWords.Add(tempWord);
                }
            }

            if (length == 4)
            {
                List<int[]> tempWords = new List<int[]>();
                for (int i = 0; i < length - 1; i++)
                {
                    int[] row = new int[matrixG.GetLength(1)];
                    int[] nextRow = new int[matrixG.GetLength(1)];
                    int[] nextNextRow = new int[matrixG.GetLength(1)];
                    for (int j = 0; j < allCodeWords[i].Length; j++)
                    {
                        row[j] = SumNums(allCodeWords[i][j], allCodeWords[i + 1][j]);
                        if (i + 2 < length)
                        {
                            nextRow[j] = SumNums(allCodeWords[i][j], allCodeWords[i + 2][j]);
                        }

                        if (i + 3 < length)
                        {
                            sumAll[j] = SumNums(allCodeWords[i][j], allCodeWords[i + 1][j]);
                            sumAll[j] = SumNums(sumAll[j], allCodeWords[i + 2][j]);
                            sumAll[j] = SumNums(sumAll[j], allCodeWords[i + 3][j]);
                            nextNextRow[j] = SumNums(allCodeWords[i][j], allCodeWords[i + 3][j]);
                        }
                    }
                    tempWords.Add(row);
                    if (i + 2 < length)
                    {
                        tempWords.Add(nextRow);
                    }
                    if (i + 3 < length)
                    {
                        tempWords.Add(nextNextRow);
                    }

                }

                foreach (var row in tempWords)
                {

                    allCodeWords.Add(row);
                }
            }

            if (length == 5)
            {
                List<int[]> tempWords = new List<int[]>();
                for (int i = 0; i < length - 1; i++)
                {
                    int[] row = new int[matrixG.GetLength(1)];
                    int[] nextRow = new int[matrixG.GetLength(1)];
                    int[] nextNextRow = new int[matrixG.GetLength(1)];
                    int[] nextNextNextRow = new int[matrixG.GetLength(1)];
                    for (int j = 0; j < allCodeWords[i].Length; j++)
                    {
                        row[j] = SumNums(allCodeWords[i][j], allCodeWords[i + 1][j]);
                        if (i + 2 < length)
                        {
                            nextRow[j] = SumNums(allCodeWords[i][j], allCodeWords[i + 2][j]);
                        }

                        if (i + 3 < length)
                        {
                            nextNextRow[j] = SumNums(allCodeWords[i][j], allCodeWords[i + 3][j]);
                        }
                        if (i + 4 < length)
                        {
                            sumAll[j] = SumNums(allCodeWords[i][j], allCodeWords[i + 1][j]);
                            sumAll[j] = SumNums(sumAll[j], allCodeWords[i + 2][j]);
                            sumAll[j] = SumNums(sumAll[j], allCodeWords[i + 3][j]);
                            sumAll[j] = SumNums(sumAll[j], allCodeWords[i + 4][j]);
                            nextNextNextRow[j] = SumNums(allCodeWords[i][j], allCodeWords[i + 4][j]);
                        }
                    }
                    tempWords.Add(row);
                    if (i + 2 < length)
                    {
                        tempWords.Add(nextRow);
                    }
                    if (i + 3 < length)
                    {
                        tempWords.Add(nextNextRow);
                    }
                    if (i + 4 < length)
                    {
                        tempWords.Add(nextNextNextRow);
                    }

                }

                foreach (var tempWord in tempWords)
                {
                    allCodeWords.Add(tempWord);
                }
            }

            if (length == 6)
            {
                List<int[]> tempWords = new List<int[]>();
                for (int i = 0; i < length - 1; i++)
                {
                    int[] row = new int[matrixG.GetLength(1)];
                    int[] nextRow = new int[matrixG.GetLength(1)];
                    int[] nextNextRow = new int[matrixG.GetLength(1)];
                    int[] nextNextNextRow = new int[matrixG.GetLength(1)];
                    int[] nextNextNextNextRow = new int[matrixG.GetLength(1)];
                    for (int j = 0; j < allCodeWords[i].Length; j++)
                    {
                        row[j] = SumNums(allCodeWords[i][j], allCodeWords[i + 1][j]);
                        if (i + 2 < length)
                        {
                            nextRow[j] = SumNums(allCodeWords[i][j], allCodeWords[i + 2][j]);
                        }

                        if (i + 3 < length)
                        {
                            nextNextRow[j] = SumNums(allCodeWords[i][j], allCodeWords[i + 3][j]);
                        }
                        if (i + 4 < length)
                        {
                            nextNextNextRow[j] = SumNums(allCodeWords[i][j], allCodeWords[i + 4][j]);
                        }
                        if (i + 5 < length)
                        {
                            sumAll[j] = SumNums(allCodeWords[i][j], allCodeWords[i + 1][j]);
                            sumAll[j] = SumNums(sumAll[j], allCodeWords[i + 2][j]);
                            sumAll[j] = SumNums(sumAll[j], allCodeWords[i + 3][j]);
                            sumAll[j] = SumNums(sumAll[j], allCodeWords[i + 4][j]);
                            sumAll[j] = SumNums(sumAll[j], allCodeWords[i + 5][j]);
                            nextNextNextNextRow[j] = SumNums(allCodeWords[i][j], allCodeWords[i + 5][j]);
                        }
                    }
                    tempWords.Add(row);
                    if (i + 2 < length)
                    {
                        tempWords.Add(nextRow);
                    }
                    if (i + 3 < length)
                    {
                        tempWords.Add(nextNextRow);
                    }
                    if (i + 4 < length)
                    {
                        tempWords.Add(nextNextNextRow);
                    }
                    if (i + 4 < length)
                    {
                        tempWords.Add(nextNextNextNextRow);
                    }

                }

                foreach (var tempWord in tempWords)
                {
                    allCodeWords.Add(tempWord);
                }
            }

            if (length != 2)
            {
                allCodeWords.Add(sumAll);
            }
        }
        private static int SumNums(int current, int next)
        {
            if (current == 1 && next == 1)
            {
                current = 0;
            }
            else if (current == 1 && next == 0)
            {
                current = 1;
            }
            else if (current == 0 && next == 1)
            {
                current = 1;
            }
            else
            {
                current = 0;
            }
            return current;
        }
        private static int MultiplyNums(int matrixNum, int yNum)
        {
            if (matrixNum == 1 && yNum == 1)
            {
                matrixNum = 1;
            }
            else if (matrixNum == 1 && yNum == 0)
            {
                matrixNum = 0;
            }
            else if (matrixNum == 0 && yNum == 1)
            {
                matrixNum = 0;
            }
            else
            {
                matrixNum = 0;
            }
            return matrixNum;
        }
        private static void CreateMatrixE(int[,] matrixE)
        {
            for (int i = 0; i < matrixE.GetLength(0); i++)
            {
                for (int j = 0; j < matrixE.GetLength(1); j++)
                {
                    if (i == j)
                    {
                        matrixE[i, j] = 1;
                    }
                    else
                    {
                        matrixE[i, j] = 0;
                    }

                }
            }
        }
        private static void PrintMatrix(int[,] matrixG)
        {
            for (int i = 0; i < matrixG.GetLength(0); i++)
            {

                for (int j = 0; j < matrixG.GetLength(1); j++)
                {
                    Console.Write(matrixG[i, j] + " ");
                }

                Console.WriteLine();
            }
        }
        private static int[,] CreateInitialMatrix(int rows, int cols)
        {
            int[,] matrixA = new int[rows, cols];
            for (int i = 0; i < matrixA.GetLength(0); i++)
            {
                int[] intRow = Console.ReadLine().Trim().Split().Select(int.Parse).ToArray();
                for (int c = 0; c < matrixA.GetLength(1); c++)
                {
                    matrixA[i, c] = intRow[c];
                }
            }
            return matrixA;

        }
    }
}


