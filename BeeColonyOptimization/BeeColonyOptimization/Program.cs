using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace BeeColonyOptimization
{

    public struct Bee
    {
        //status: true -> zadeklarowana, false -> niezadeklarowana
        public bool status;
        public List<UInt32> path;
        public double pathValue;
        public double normalizeValue;
        public double recruitValue;


        public void SetPath(List<UInt32> newPath)
        {
            path = newPath;
        }

        public void ResetNormalizeAndRecruitValue()
        {
            normalizeValue = 0;
            recruitValue = 0;
        }
    }

    class Program
    {

        public static Random rand = new Random();
        public static Values values;

        static void Main(string[] args)
        {
            Console.WriteLine("Set name of file: ");
            string name = Console.ReadLine();
            string nameFile = "TSP\\" + name; 

            string pathFile = nameFile;
            //try
            //{
                Point[] points = TSPFileReader.ReadTspFile(pathFile);
                Graph graph = new Graph(points);

                Console.WriteLine("Set count of bees: ");
                UInt32 bees = UInt32.Parse(Console.ReadLine());

                values = new Values(bees, pathFile, "BCO");
                values.StartTime();
                Solve(graph, bees, 1000, (UInt32)(3));
                //Solve(graph, bees, 10000, 3);

                values.StopTime();
                Console.WriteLine("End");
            //} catch (FileNotFoundException e)
            //{
                //Console.WriteLine("File not Found " + e);
            //}
            

            Console.ReadKey();

        }

        public static double FindBestPath(Bee[] hive, Graph graph)
        {
            double best = graph.CalculateRouteLength(hive[0].path);
            for (int i = 1; i < hive.Length; i++)
            {
                double nextBeePath = graph.CalculateRouteLength(hive[i].path);
                if (best > nextBeePath)
                {
                    best = nextBeePath;
                }
            }
            return best;

        }

        public static void Solve(Graph graph, UInt32 bees, UInt32 iterations, UInt32 steps)
        {
            List<UInt32> bestPath = new List<UInt32>();
            double newPath = 0;

            for (int k = 0; k < graph.dimension; k++)
            {
                bestPath.Add((UInt32)k);
            }
            double bestPathLength = graph.CalculateRouteLength(bestPath);
            Console.WriteLine("Best path is: " + bestPathLength);
            //Values.toFile.Add("-First best-" + 0 + "-" + bestPathLength);
            Values.AddNewValues(0, bestPathLength);
            Bee[] hive = new Bee[bees];
            for(int j = 1; j < iterations; j++)
            {
                Console.WriteLine("Iteration: {0}", j);
                hive = ResetHive(hive, graph);
                for (int i = 0; i < graph.dimension; i++)
                {
                    for (int s = 0; s < steps; s++)
                    {
                        for (int b = 0; b < hive.Length; b++)
                        {
                            if (hive[b].path.Count == graph.dimension) continue;
                            //if (hive[b].path.Count == graph.dimension) break;
                            UInt32 nextPosition = ChooseNextNode(graph, hive[b]);
                            hive[b].path.Add(nextPosition);
                        }
                    }

                    for (int b = 0; b < hive.Length; b++)
                    {
                        hive[b].pathValue = graph.CalculateRouteLength(hive[b].path);
                    }

                    hive = Loyalty(hive);
                    foreach (Bee bee in hive)
                    {
                        bee.ResetNormalizeAndRecruitValue();
                    }

                    hive = Recruit(hive);
                    foreach (Bee bee in hive)
                    {
                        bee.ResetNormalizeAndRecruitValue();
                    }
                //}
                }
                //Console.WriteLine("Iteration " + j.ToString());
                //HiveRoutes(hive, graph);
                //Paths(hive);
               // Console.WriteLine("-----------------------------------");
               // Console.WriteLine();

                    newPath = FindBestPath(hive, graph);

                    if (newPath < bestPathLength)
                    {
                        bestPathLength = newPath;
                        Console.WriteLine("New best found in iteration: " + j.ToString() + ", new best is: " + bestPathLength);
                        //Values.toFile.Add("New best-" + j.ToString() + "-" + bestPathLength);
                        Values.AddNewValues(j, bestPathLength);
                    }
                //}
            }
        }

        public static Bee [] ResetHive(Bee [] hive, Graph graph)
        {
            for (int i = 0; i < hive.Length; i++)
            {
                hive[i].status = false;
                hive[i].path = new List<UInt32>();
                hive[i].pathValue = 0;
                hive[i].normalizeValue = 0;
                hive[i].recruitValue = 0;
            }

            for (int b = 0; b < hive.Length; b++)
            {
                UInt32 startPosition = (UInt32)rand.Next(0, (int)graph.dimension);
                //UInt32 startPosition = (UInt32));
                hive[b].path.Add(startPosition);
            }

            return hive;
        }

        public static UInt32 ChooseNextNode(Graph g, Bee bee)
        {

            List<UInt32> nodeList = new List<UInt32>();
            for (UInt32 i = 0; i < g.dimension; i++)
            {
                nodeList.Add(i);
            }

            foreach (UInt32 node in bee.path)
            {
                nodeList.Remove(node);
            }

            /*foreach (UInt32 node in nodeList)
            {
                Console.Write(node);
                Console.Write(", ");
            }
            Console.WriteLine("___________________");*/

            int index = rand.Next(0, nodeList.Count);

            UInt32 nextNode = nodeList[index];

            return nextNode;
        }

        public static Bee [] Loyalty(Bee [] bees)
        {

            // wartość min = A, wartość max = B, znormalizowana min = a -> 0, znormalizowana max = b -> 1, x = warość
            //a + (x-A)(b-a)/(B-A)
            /*for(int i = 0; i < bees.Length; i++)
            {
                bees[i].status = false;
            }*/
            IEnumerable<Bee> sortedBees = from bee in bees orderby bee.pathValue select bee;
            bees = sortedBees.ToArray();
            double maxValue = bees[bees.Length - 1].pathValue;
            double minValue = bees[0].pathValue;

            //double maxValue = bees[0].pathValue;
            //double minValue = bees[bees.Length - 1].pathValue;

            //bees[bees.Length - 1].normalizeValue = 0;
            //bees[0].normalizeValue = 1;

            for (int i = 0; i < bees.Length; i++)
            {
                bees[i].normalizeValue = NormalizeValue(minValue, maxValue, 0, 1, bees[i].pathValue);
            }

            for (int i=0; i<bees.Length; i++)
                bees[i].status = false;

            for (int i = 0; i < bees.Length; i++)
            {
                double l = rand.NextDouble();
                double loyalty = Math.Pow(Math.E, -(1 - bees[i].normalizeValue));
                if (l < loyalty)
                {
                    bees[i].status = true;
                }
                
            }

            return bees;

        }

        public static double NormalizeValue(double minValue, double maxValue, double normalizeMin, double normalizeMax, double value)
        {
            return maxValue - minValue == 0 ? 1 : (maxValue - value) / (maxValue - minValue);
        }

        public static Bee [] Recruit(Bee [] bees)
        {
            List<Bee> recruiters = new List<Bee>();
            List<Bee> unemployed = new List<Bee>();
            foreach (Bee bee in bees)
            {
                if(bee.status == true)
                {
                    recruiters.Add(bee);
                } else
                {
                    unemployed.Add(bee);
                }
            }

            IEnumerable<Bee> sortedBees = from bee in recruiters orderby bee.pathValue select bee;
            Bee [] recruitersArrayOriginal = sortedBees.ToArray();
            Bee[] recruitersArray = BestRecruiters(recruitersArrayOriginal);
            Bee[] uneployedArray = unemployed.ToArray();
            double maxValue = recruitersArray[recruitersArray.Length - 1].pathValue;
            double minValue = recruitersArray[0].pathValue;
            recruitersArray[0].normalizeValue = 0;
            recruitersArray[recruitersArray.Length - 1].normalizeValue = 1;
            double sumValues = 0;
            for (int i = 0; i < recruitersArray.Length; i++)
            {
                recruitersArray[i].normalizeValue = NormalizeValue(minValue, maxValue, 0, 1, bees[i].pathValue);
                sumValues += recruitersArray[i].normalizeValue;
            }

            for (int i = 0; i < recruitersArray.Length; i++)
            {
                recruitersArray[i].recruitValue = recruitersArray[i].normalizeValue;
            }

            IEnumerable<Bee> sortedRecruiterBees = from bee in recruitersArray orderby bee.pathValue select bee;
            recruitersArray = sortedRecruiterBees.ToArray();


            for(int b = 0; b < uneployedArray.Length; b++)
            {
                uneployedArray[b].SetPath(ChooseRecruiter(recruitersArray));
                uneployedArray[b].status = true;


            }

            Bee[] hive = recruitersArrayOriginal.Concat(uneployedArray).ToArray();
            return hive;

        }

        public static List<UInt32> ChooseRecruiter(Bee[] bees)
        {
            double choose = rand.NextDouble();
            Bee recruiter = new Bee();

            double cumulateRecruitValue = 0;

            for (int i = 0; i < bees.Length; i++)
            {
                cumulateRecruitValue += bees[i].recruitValue;
            }
            for (int i=0; i< bees.Length; i++)
            {
                bees[i].recruitValue /= cumulateRecruitValue;
            }
            int iterator = 0;
            double actualValue = 0;
            while (iterator < bees.Length && actualValue<choose)
            {
                actualValue += bees[iterator].recruitValue;
                iterator++;
            }
            if (iterator > 0)
                iterator--;
            recruiter = bees[iterator];
            return new List<UInt32>(recruiter.path);

        }

        public static void HiveRoutes(Bee [] bees, Graph graph)
        {
            for(int i = 0; i < bees.Length; i++)
            {
                Console.WriteLine("Bee " + i.ToString());
                Console.WriteLine("Bee path " + bees[i].path.Count.ToString());
                Console.WriteLine("Bee path Length " + graph.CalculateRouteLength(bees[i].path));
                Console.WriteLine("------------------");
            }
        }

        public static void Paths(Bee [] bees)
        {
            for (int i = 0; i < bees.Length; i++)
            {
                string path = "p: ";
                foreach(UInt32 vertex in bees[i].path)
                {
                    path += vertex.ToString() + " ";
                }
                Console.WriteLine(path);
                Console.WriteLine();
            }
        }

        public static int FoundBestPath(Bee [] bees, Graph graph)
        {
            int bestBee = 0;
            double bestPath = graph.CalculateRouteLength(bees[bestBee].path);
            for (int i = 1; i < bees.Length; i++)
            {
                double beePath = graph.CalculateRouteLength(bees[i].path);
                if(beePath < bestPath)
                {
                    bestBee = i;
                    bestPath = beePath;
                }
            }

            return bestBee;


        }

        public static Bee [] BestRecruiters(Bee [] bees)
        {
            int recruitersCount = bees.Length > 10 ? (int)(bees.Length * 0.25f) : bees.Length;

            Bee[] bestRecruiters = new Bee[recruitersCount];
            for(int i = 0; i < recruitersCount; i++)
            {
                bestRecruiters[i] = bees[i];
            }

            return bestRecruiters;
        }


    }
}

