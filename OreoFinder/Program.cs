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


        public string PartitionBinToDec(string bin)
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in bin)
            {
                // This will crash for non-hex characters. You might want to handle that differently.
                try
                {
                    result.Append(partitionToCompanyPrefix[c]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error!");
                }
            }
            return result.ToString();
        }



        public int Sgtin96Vertify() {

            int numberOfNotSgtin96 = 0;
            string[] lines = File.ReadAllLines(@"tags.txt");
            foreach (string line in lines)
            {
                Console.WriteLine(line);

                /// <summary>
                ///    To find # of invalid SGTIN-96.
                /// </summary>
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
            int numberOfNotSgtin96 = 0;
            int numberOfOreos = 0;

            string[] lines = File.ReadAllLines(@"tags.txt");
            foreach (string line in lines)
            {
               // Console.WriteLine(line);

                /// <summary>
                ///    To find # of invalid SGTIN-96.
                /// </summary>
                string header = line.Substring(0, 2);
                if (header != "30")
                {
                    numberOfNotSgtin96++;
                }

                Program p = new Program();
                
                string bin = p.HexStringToBinary(line);
           //     Console.WriteLine(bin + " ovo je bin");

                header = bin.Substring(0, 8);
                string filter = bin.Substring(8,3);
                string partition = bin.Substring(11,3);

                int partitionDecValue = Convert.ToInt32(partition, 2);

                //int companyPrefixInBits;
                int itemRefInBits;

                if (partitionToCompanyPrefix.TryGetValue(partitionDecValue, out int companyPrefixInBits))
                {
               //     Console.WriteLine(companyPrefixInBits);
                    itemRefInBits = 44 - companyPrefixInBits;

                    string companyRefInDec = bin.Substring(14,companyPrefixInBits);
                    string itemRefInDec = bin.Substring(14+companyPrefixInBits, itemRefInBits);

                    long excelCompanyPrefix = Convert.ToInt64(companyRefInDec, 2);
                    long excelItemRef = Convert.ToInt64(itemRefInDec, 2);
                    var itemSerialNumber = bin.Substring(58);
                    if (excelItemRef == 1253252) {
                        numberOfOreos++;
                        Console.WriteLine("Oreos serial number: " + itemSerialNumber);
                    }

             //       Console.WriteLine("Excel company prefix: " + excelCompanyPrefix + "  excel item ref: " + excelItemRef);
                }
                else
                {
                    Console.WriteLine("Could not find the specified key.");
                }

                /// <summary>
                /// Each serial number starts at 58-th bit. (8 header + 3 filter + 3 partition + 44 company prefix with item ref).Each serial has exacly 38 bits (96 - 58).
                /// </summary>

                
               
             //   Console.WriteLine("header = "+header + " filter = " + filter + " partition = " + partitionDecValue );

            }


            //bool isheader = true;
            //var reader = new StreamReader(File.OpenRead(@"data.csv"));
            //List<string> headers = new List<string>();

            //while (!reader.EndOfStream)
            //{
            //    var line = reader.ReadLine();
            //    var values = line.Split(';');

            //    if (isheader)
            //    {
            //        isheader = false;
            //        headers = values.ToList();
            //    }
            //    else
            //    {
            //        int i = 0;
            //        for (i = 0; i < values.Length; i++)
            //        {
            //         //   Console.Write(string.Format("{0} = {1};", headers[i], values[i]));

            //        }
            //        Console.WriteLine();

            //    }
            //}


            Console.WriteLine(numberOfNotSgtin96 + " number of lines are not SGTIN-96");
            Console.WriteLine(numberOfOreos + " number of oreos in list");

            Console.ReadLine();
        }
    }
}
