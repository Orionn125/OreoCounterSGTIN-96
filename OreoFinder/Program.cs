using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OreoFinder
{
    class Program
    {

        private static readonly Dictionary<char, string> hexCharacterToBinary = new Dictionary<char, string> {
    { '0', "0000" },
    { '1', "0001" },
    { '2', "0010" },
    { '3', "0011" },
    { '4', "0100" },
    { '5', "0101" },
    { '6', "0110" },
    { '7', "0111" },
    { '8', "1000" },
    { '9', "1001" },
    { 'A', "1010" },
    { 'B', "1011" },
    { 'C', "1100" },
    { 'D', "1101" },
    { 'E', "1110" },
    { 'F', "1111" }
};


        private static readonly Dictionary<int, int> partitionToCompanyPrefix = new Dictionary<int, int> {
    { 0 , 40 },
    { 1 , 37 },
    { 2 , 34 },
    { 3 , 30 },
    { 4 , 27 },
    { 5 , 24 },
    { 6 , 20 }
};

        public string HexStringToBinary(string hex)
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in hex)
            {
                try
                {
                    result.Append(hexCharacterToBinary[char.ToUpper(c)]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Not hex character");
                }
            }
            return result.ToString();
        }


        /// <summary>
        ///    To find number of invalid SGTIN-96.
        /// </summary>
        public int Sgtin96Vertify() {

            int numberOfNotSgtin96 = 0;
            string[] lines = File.ReadAllLines(@"tags.txt");
            foreach (string line in lines)
            {
                string header = line.Substring(0, 2);
                if (header != "30")
                {
                    numberOfNotSgtin96++;
                }
            }
                return numberOfNotSgtin96;
        }



        static void Main()
        {
            Program p = new Program();

           // int numberOfNotSgtin96 = 0;
            int numberOfOreos = 0;

            string[] lines = File.ReadAllLines(@"tags.txt");
            foreach (string line in lines){ 

                string bin = p.HexStringToBinary(line);
           //     Console.WriteLine(bin + " ovo je bin");

                string header = bin.Substring(0, 8);
                string filter = bin.Substring(8,3);
                string partition = bin.Substring(11,3);

                int partitionDecValue = Convert.ToInt32(partition, 2);

                int itemRefInBits;

                if (partitionToCompanyPrefix.TryGetValue(partitionDecValue, out int companyPrefixInBits))
                {
                    itemRefInBits = 44 - companyPrefixInBits;

                    string companyRefInDec = bin.Substring(14,companyPrefixInBits);
                    string itemRefInDec = bin.Substring(14+companyPrefixInBits, itemRefInBits);

                    //long cvsCompanyPrefix = Convert.ToInt64(companyRefInDec, 2);
                    long cvsItemRef = Convert.ToInt64(itemRefInDec, 2);

                    /// <summary>
                    /// Each serial number starts at 58-th bit. (8 header + 3 filter + 3 partition + 44 company prefix with item ref).Each serial has exacly 38 bits (96 - 58).
                    /// </summary>
                    var itemSerialNumber = bin.Substring(58);

                    // TODO: It would probably be good idea to pull this number from CVS.
                    if (cvsItemRef == 1253252) {
                        numberOfOreos++;
                        Console.WriteLine("Oreos serial number: " + itemSerialNumber);
                    }
                }
                else
                {
                    Console.WriteLine("Could not find the specified key.");
                }
            }

            Console.WriteLine(p.Sgtin96Vertify() + " - number of lines are not valid SGTIN-96");
            Console.WriteLine(numberOfOreos + " number of oreos in list");

            Console.ReadLine();
        }
    }
}