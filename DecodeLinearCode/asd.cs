using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecodeLinearCode
{
    internal class asd
    {
//        // private static void CheckUpdateMatrixToEA(int[,] matrixG)
//        {
//            int[,] matrixE = new int[matrixG.GetLength(0), matrixG.GetLength(0)];
//        int[,] matrix = new int[matrixG.GetLength(0), matrixG.GetLength(1) - matrixE.GetLength(1)];
//        //matrixG => (matrixE|matrix)
//        CreateMatrixE(matrixE);
//        List<int> rowsToUpdate = CheckIfNeedToUpdate(matrixE, matrixG);
//            for (int i = 0; i<matrixG.GetLength(0); i++)
//            {
//                List<int> currRow = new List<int>();
//        List<int> nextRow = new List<int>();
//                if (rowsToUpdate.Contains(i))
//                {
//                    for (int j = 0; j<matrixG.GetLength(1); j++)
//                    {
//                        if (i == matrixG.GetLength(0) - 1)
//                        {
//                            currRow.Add(matrixG[i, j]);
//                            nextRow.Add(matrixG[0, j]);
//                        }
//                        else
//                        {
//                            currRow.Add(matrixG[i, j]);
//                            nextRow.Add(matrixG[i + 1, j]);
//                        }
//                    }
//                }
//                else
//{
//    continue;
//}

//for (int c = 0; c < currRow.Count; c++)
//{
//    bool shouldUpdate = false;
//    Console.WriteLine("currRow before Update:");
//    Console.WriteLine(string.Join(", ", currRow));
//    if (c < currRow.Count - matrixE.GetLength(1))
//    {
//        if (currRow[c] != matrixE[i, c])
//        {
//            currRow[c] = SumNums(currRow[c], nextRow[c]);
//            shouldUpdate = true;
//        }
//    }

//    if (shouldUpdate)
//    {
//        for (int j = 0; j < matrix.GetLength(0) - 1; j++)
//        {
//            for (int k = 0; k < matrix.GetLength(1); k++)
//            {
//                int matriELength = matrixE.GetLength(1);
//                int sum =
//                    SumNums(currRow[matriELength + k], nextRow[matriELength + k]);
//                matrix[j, k] = sum;
//                matrix[j + 1, k] = nextRow[matriELength + k];
//            }
//            Console.WriteLine("----------------------");
//            Console.WriteLine("Current row after update:");
//            Console.WriteLine(string.Join(", ", currRow));
//            Console.WriteLine("matrixA:");
//            PrintMatrix(matrix);
//        }
//    }

//}
//            }
//            for (int q = 0; q < matrixG.GetLength(0); q++)
//{
//    for (int j = 0; j < matrixG.GetLength(1); j++)
//    {
//        if (j < matrixE.GetLength(1))
//        {
//            matrixG[q, j] = matrix[q, j];
//        }
//        else
//        {
//            matrixG[q, j] = matrixE[q, j - matrixE.GetLength(1)];
//        }
//    }
//}
//        }

   }
}
