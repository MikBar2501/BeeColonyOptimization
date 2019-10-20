using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeColonyOptimization
{
    class Bee
    {
        //status: true -> zadeklarowana, false -> niezadeklarowana
        public bool status;
        public List<UInt32> path;
        public double pathValue;
        public Graph graph;

        public Bee(Graph graph)
        {
            status = true;
            path = new List<UInt32>();
            pathValue = 0;
            this.graph = graph;
        }


        public void SetPath(List<UInt32> newPath)
        {
            //path = newPath;
            path = new List<UInt32>();
            foreach(UInt32 node in newPath)
            {
                path.Add(node);
            }
            TotalDistance();
        }

        public void TotalDistance()
        {
            if(path.Count > 0)
            {
                pathValue = graph.CalculateRouteLength(path);
            } else
            {
                pathValue = 0;
            }
            
        }

        public void ChangeStatus(bool status)
        {
            this.status = status;
        }


        public bool IsComplete(Graph graph)
        {
            if (path.Count >= graph.dimension)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ChooseNextNode(int steps, Graph graph, Random rand)
        {
            for (int i = 0; i < steps; i++)
            {
                List<UInt32> nodeList = new List<UInt32>();
                for (UInt32 j = 0; j < graph.dimension; j++)
                {
                    nodeList.Add(j);
                }

                foreach (UInt32 node in path)
                {
                    nodeList.Remove(node);
                }

                if (IsComplete(graph))
                {
                    break;
                }
                else
                {
                    int index = rand.Next(0, nodeList.Count);
                    UInt32 nextNode = nodeList[index];
                    path.Add(nextNode);
                }
            }
            TotalDistance();
        }

    }
}
