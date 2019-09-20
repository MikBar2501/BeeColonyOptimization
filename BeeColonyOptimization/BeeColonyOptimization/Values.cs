using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace BeeColonyOptimization
{
    class Values
    {
        double bestFunctionValue;
        UInt32 bestFunctionIteration;
        Stopwatch stopWatch;
        TimeSpan timeSpan;
        string elapsedTime;
        public static List<string> toFile = new List<string>();
        string algorithm;
        string problemName;
        string path;
        public static UInt32 beesCount;

        public Values ()
        {
            stopWatch = new Stopwatch();
        }

        //Dla BCO
        public Values(UInt32 countOfBees, string pathName, string algorithm)
        {
            stopWatch = new Stopwatch();
            this.algorithm = algorithm;
            path = pathName;
            beesCount = countOfBees;
            toFile.Add("Bee Colony Optimization for " + path);
            toFile.Add("Count of bees: |" + countOfBees);
        }

        public void StartTime()
        {
            stopWatch.Start();
        }

        public void StopTime()
        {
            stopWatch.Stop();
            timeSpan = stopWatch.Elapsed;
            elapsedTime = String.Format("{0}-{1:00}:{2:00}:{3:00}.{4:00}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
            toFile.Add("Time: " + elapsedTime);
            SaveToFile(toFile, algorithm, SplitName(path));
        }

        public static void SaveToFile(List<String> tofile, string algorithm, string startFile)
        {
            DateTime dt = DateTime.Now;
            string fileName = String.Format("{0:dd-MM-yyyy-hh-mm-ss}", dt) + "-" + algorithm + "-" + startFile + "-" + beesCount.ToString();
            File.Create(@"Files\" + fileName + ".txt").Dispose();
            using (StreamWriter sw = new StreamWriter(@"Files\" + fileName + ".txt"))
            {
                foreach (string line in tofile)
                {
                    sw.WriteLine(line);
                }
            }
        }

        public void AddToFile(string text)
        {
            toFile.Add(text);
        }

        public string SplitName(string pathFile)
        {
            string[] path = pathFile.Split('\\');
            string[] name = path[path.Length - 1].Split('.');
            return name[0];
        }

        public static void AddNewValues(int iteration, double value)
        {
            toFile.Add("-New best-" + iteration + "-" + value);
        }




    }
}
