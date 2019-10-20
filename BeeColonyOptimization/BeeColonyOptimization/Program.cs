using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace BeeColonyOptimization
{



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
            Point[] points = TSPFileReader.ReadTspFile(pathFile);
            Graph graph = new Graph(points);

            Console.WriteLine("Set count of bees: ");
            UInt32 bees = UInt32.Parse(Console.ReadLine());

            values = new Values(0, pathFile, "BCO");
            values.StartTime();
            //Solve(graph, bees, 1000, 3);
            Solve(graph, bees, 1000, 3);

            values.StopTime();
            Console.WriteLine("End");
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


        public static void Solve(Graph graph, UInt32 bees, UInt32 iterations, int steps)
        {
            /*List<UInt32> bestPath = new List<UInt32>();
            double newPath = 0;

           

            for (int k = 0; k < graph.dimension; k++)
            {
                bestPath.Add((UInt32)k);
            }
            double bestPathLength = graph.CalculateRouteLength(bestPath);
            Console.WriteLine("Best path is: " + bestPathLength);
            //Values.toFile.Add("-First best-" + 0 + "-" + bestPathLength);
            Values.AddNewValues(0, bestPathLength);*/
            double bestPath;
            for(int i = 0; i < iterations; i++)
            {
                
                Bee[] hive = new Bee[bees];
                Bee bestBee = new Bee(graph);

                for (int b = 0; b < hive.Length; b++)
                {
                    hive[b] = new Bee(graph);
                }

                int epochs = 0;
                while (!bestBee.IsComplete(graph))
                {
                    //forward
                    int h = 0;
                    foreach(Bee bee in hive)
                    {
                        bee.ChooseNextNode(steps, graph, rand);
                        //Console.WriteLine("Bee nr {0} path count {1}", h, bee.path.Count);
                        h++;
                    }
                    
                    //backward
                    hive = hive.OrderBy(c => c.pathValue).ToArray();
                    bestBee = hive[0];
     
                    double cMax = hive[0].pathValue;
                    double cMin = hive[hive.Length - 1].pathValue;
                    
                    List<Bee> recruiters = new List<Bee>();
                    foreach(Bee bee in hive)
                    {
                        double ob = (cMax - bee.pathValue) / (cMax - cMin);
                        double probs = Math.Pow(Math.E, (-(1 - ob) / (bee.path.Count * 0.01)));
                        double r = rand.NextDouble();
                        if(r > probs)
                        {
                            bee.ChangeStatus(true);
                            recruiters.Add(bee);
                        } else
                        {
                            bee.ChangeStatus(false);
                        }
                    }
                    if(recruiters.Count == 0)
                    {
                        //int l = hive.Length / 3;
                        for(int l = 0; l < hive.Length; l++)
                        {
                            hive[l].ChangeStatus(true);
                            recruiters.Add(hive[l]);
                        }
                    }

                    double divider = 0;
                    List<double> pr = new List<double>();
                    foreach(Bee bee in recruiters)
                    {
                        divider += (cMax - bee.pathValue) / (cMax - cMin);
                    }

                    foreach (Bee bee in recruiters)
                    {
                        pr.Add((cMax - bee.pathValue) / (cMax - cMin) / divider);
                    }

                    List<double> cumulativeProbs = new List<double>();
                    for(int x = 0; x < pr.Count; x++)
                    {
                        double cumPr = 0;
                        for(int y = x+1; y < pr.Count; y++)
                        {
                            cumPr += pr[y];
                            cumulativeProbs.Add(cumPr);
                        }
                    }

                    foreach (Bee bee in hive)
                    {
                        if(!bee.status)
                        {
                            double rndm = rand.NextDouble();
                            Bee selectedBee = new Bee(graph);
                            selectedBee = recruiters[0];
                            for (int k = 0; k < cumulativeProbs.Count; k++)
                            {
                                if(rndm < cumulativeProbs[k])
                                {
                                    selectedBee = recruiters[k];
                                    break;
                                }
                            }
                            //bee.path.Clear();
                            bee.SetPath(selectedBee.path);
                            bee.ChangeStatus(true);
                        }   
                    }
                    //Console.WriteLine("-----{0}", bestBee.path.Count);

                    epochs++;
                    if (bestBee.IsComplete(graph))
                    {
                        //CheckBestBee(bestBee);
                        //Console.WriteLine("Solve in {0} epochs", epochs);
                        Console.WriteLine("--Best path in iteration {0} is: {1}",i,bestBee.pathValue);
                        Values.AddNewValues(i, bestBee.pathValue);
                        //Console.WriteLine();
                    }

                }

                bestBee = new Bee(graph);
                for(int l = 0; l < hive.Length; l++)
                {
                    hive[l] = new Bee(graph);
                }
            }
            
        }

        public static void HiveRoutes(Bee[] bees, Graph graph)
        {
            for (int i = 0; i < bees.Length; i++)
            {
                Console.WriteLine("Bee " + i.ToString());
                Console.WriteLine("Bee path " + bees[i].path.Count.ToString());
                Console.WriteLine("Bee path Length " + graph.CalculateRouteLength(bees[i].path));
                Console.WriteLine("------------------");
            }
        }

        public static void Paths(Bee bees)
        {
            
                string path = "p: ";
                foreach (UInt32 vertex in bees.path)
                {
                    path += vertex.ToString() + " ";
                }
                Console.WriteLine(path);
                Console.WriteLine();
            
        }

        public static void CheckBestBee(Bee bee)
        {
            List<UInt32> p = bee.path;
            p.Sort();
            string lista = "p: ";
            foreach(UInt32 node in bee.path)
            {
                lista += node.ToString() + " ";
            }
            Console.WriteLine(lista);
        }
    }
}

