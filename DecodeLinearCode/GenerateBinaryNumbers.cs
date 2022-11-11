using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecodeLinearCode
{
    public class GenerateBinaryNumbers
    {
        public static List<string> GenerateBinaryNumber(int n)
        {
            Queue<string> binaryGenerationQueue = new Queue<string>();
            List<string> results = new List<string>();
            binaryGenerationQueue.Enqueue("1");

            while (n != 0)
            {
                string current = binaryGenerationQueue.Dequeue();
                results.Add(current);
                string appendZero = current + "0";
                string appendOne = current + "1";
                binaryGenerationQueue.Enqueue(appendZero);
                binaryGenerationQueue.Enqueue(appendOne);
                n--;
            }
            return results;
        }
    }
}
