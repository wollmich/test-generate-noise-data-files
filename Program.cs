using System;
using System.IO;
using System.Threading;

namespace TestGenerateNoiseDataFiles
{
    class Program
    {
        static bool keepRunning = true;

        static void Main(string[] args)
        {
            Console.WriteLine("________________________________________________________________________________");
            Console.WriteLine();
            Console.WriteLine("Test Generate Noise Data Files - Michael Wollensack METAS 24.06.2020");
            Console.WriteLine("________________________________________________________________________________");
            Console.WriteLine();
            string defaultDir = AppDomain.CurrentDomain.BaseDirectory;
            Console.Write(@"Directory [" + defaultDir + "] : ");
            string stringDirPath = Console.ReadLine();
            string dirPath = !string.IsNullOrEmpty(stringDirPath) ? stringDirPath : defaultDir;
            if (!dirPath.EndsWith(Path.DirectorySeparatorChar.ToString())) dirPath += Path.DirectorySeparatorChar;
            Console.WriteLine();
            Console.Write(@"File Size (MB) [10 MB] : ");
            string stringSizeMB = Console.ReadLine();
            int sizeMB = int.TryParse(stringSizeMB, out sizeMB) ? sizeMB : 10;
            Console.WriteLine();
            Console.Write(@"Delay (ms)     [10 ms] : ");
            string stringDelay = Console.ReadLine();
            int delay = int.TryParse(stringDelay, out delay) ? delay : 10;
            Console.WriteLine();
            Console.Write(@"Delete Files    [true] : ");
            string stringDeleteFiles = Console.ReadLine();
            bool deleteFiles = bool.TryParse(stringDeleteFiles, out deleteFiles) ? deleteFiles : true;
            Console.WriteLine();
            Console.WriteLine("________________________________________________________________________________");
            Console.WriteLine();

            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
                e.Cancel = true;
                keepRunning = false;
            };

            string subDirPath = Environment.MachineName + Path.DirectorySeparatorChar;
            Directory.CreateDirectory(dirPath + subDirPath);

            int size = sizeMB * 1024 * 1024;
            byte[] buffer = new byte[size];
            byte[] buffer2;
            Random random = new Random();
            do
            {
                random.NextBytes(buffer);

                DateTime start1 = DateTime.Now;
                string fileName = subDirPath + start1.ToString("yyyy_MM_dd_HH_mm_ss_fff") + ".tmp";
                string filePath = dirPath + fileName;
                Console.Write(fileName);
                using (BinaryWriter writer = new BinaryWriter(File.Create(filePath)))
                {
                    writer.Write(buffer);
                }
                DateTime stop1 = DateTime.Now;
                double timespan1 = (stop1 - start1).TotalSeconds;
                Console.Write($" (Write {sizeMB / timespan1,5:F0} MB/s");

                DateTime start2 = DateTime.Now;
                using (BinaryReader reader = new BinaryReader(File.OpenRead(filePath)))
                {
                    buffer2 = reader.ReadBytes(size);
                }
                DateTime stop2 = DateTime.Now;
                double timespan2 = (stop2 - start2).TotalSeconds;
                Console.WriteLine($", Read {sizeMB / timespan2,5:F0} MB/s)");

                Thread.Sleep(delay);

                if (deleteFiles) File.Delete(filePath);
            }
            while (keepRunning);
            if (deleteFiles) Directory.Delete(dirPath + subDirPath);
            Console.WriteLine("________________________________________________________________________________");
            Console.WriteLine();
        }
    }
}
