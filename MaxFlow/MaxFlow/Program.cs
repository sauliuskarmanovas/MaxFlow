using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security;
using System.IO;

namespace MaxFlow
{
    class Program
    {
        static bool userInput = true;
        public static Random rng = new Random();
        [STAThread]
        static void Main(string[] args)
        { 
            
            int numOfVertices = 0;
            int[][] EdgeMatrix = null;
            HashSet<int>[] neighbourList = null;
            //int numOfVertices = 0;
            int edges = 0;

            if (args.Count() < 1 || args.Count() == 2 && args[0] == "-file" && (fromFile = true))
            {
                
                if (userInput)
                    generateGraphFromUserInput(ref args, ref numOfVertices, ref edges, ref EdgeMatrix, ref neighbourList);
                else
                    generateRandomGraph(ref numOfVertices, ref edges, ref EdgeMatrix, ref neighbourList, 0);
                Console.WriteLine("Vertices: " + numOfVertices + " Edges: " + edges);
                var rr = calculateMaxFlow(EdgeMatrix, neighbourList, 0, EdgeMatrix.Length - 1);
                Console.WriteLine("maxflow: " + rr.Item1);// + " counter " + rr.Item2);
                Console.ReadKey();
                //Clipboard.SetText(sb1.Append("}").ToString());
                return;
            }
            userInput = false;
            for (int i = 0; i < args.Count(); i++)
            {
                if (args[i] == "-v" && i < args.Count() - 1)
                {
                    try
                    {
                        numOfVertices = Convert.ToInt32(args[i + 1]);
                    }
                    catch (Exception)
                    {
                        Console.Out.WriteLine("Invalid vertice count: \"" + args[i + 1]);
                    }
                }else if(args[i] == "-e" && i < args.Count() - 1)
                {
                    try
                    {
                        edges = Convert.ToInt32(args[i + 1]);
                    }
                    catch (Exception)
                    {
                        Console.Out.WriteLine("Invalid edge count: \"" + args[i + 1]);
                    }
                }
            }
            generateRandomGraph(ref numOfVertices, ref edges, ref EdgeMatrix, ref neighbourList, 0);
            Console.WriteLine("Vertices: " + numOfVertices + " Edges: " + edges);
            var r = calculateMaxFlow(EdgeMatrix, neighbourList, 0, EdgeMatrix.Length - 1);
            Console.WriteLine("maxflow: " + r.Item1);// + " counter " + r.Item2);

            /*System.Threading.Thread[] thread = new System.Threading.Thread[4];
            for (int i = 0; i < 4; i++)
            {
                int id = i;
                rng[i] = new Random(id);
                thread[i] = new System.Threading.Thread(th =>
                {
                    int edges = 0;
                    for (int j = id; j < size; j+=4)
                    {
                        for (int k = 0; k < 20; k++)
                        {
                            int[][] EdgeMatrix = null;
                            HashSet<int>[] neighbourList = null;
                            edges = 0;
                            int ff = j + 2;
                            generateRandomGraph(ref ff, ref edges, ref EdgeMatrix, ref neighbourList,id);
                            rEdges[j] += edges;
                            var r = calculateMaxFlow(EdgeMatrix, neighbourList, 0, EdgeMatrix.Length - 1);
                            rCount[j] += r.Item2;
                        }
                        rCount[j] /= 20;
                        rEdges[j] /= 20;
                        if (id==0)
                            Console.Write("\r" + j + "/"+size/4);
                    }
                });
                thread[i].Start();

            }
            for (int i = 0; i < 4; i++)
            {
                thread[i].Join();
            }
           
            StreamWriter s = new StreamWriter("vert"+ numOfVertices +".txt", true);
            s.WriteLine("vertices\t" + numOfVertices);
            s.WriteLine("Edges\tCount");
            for (int i = 0; i < size; i++)
            {
                s.WriteLine(i+"\t" + rEdges[i] + "\t" + rCount[i]);
            }
            s.Flush();*/

            Console.ReadKey();
            //Clipboard.SetText(sb1.Append("}").ToString());
        }

        private static void generateRandomGraph(ref int numOfVertices, ref int edges, ref int[][] edgeMatrix, ref HashSet<int>[] neighbourList,int id)
        {
            if (numOfVertices < 3)
            {
                numOfVertices = rng.Next(3, 100);
            }
            if(edges == 0 || edges < numOfVertices - 1 || edges > ((numOfVertices - 1) * (numOfVertices - 1)) * 2 )
            {
                edges = rng.Next(numOfVertices - 1, ((numOfVertices - 1) * (numOfVertices - 1)) * 2);
            }
            edgeMatrix = new int[numOfVertices][];
            neighbourList = new HashSet<int>[numOfVertices];
            for (int i = 0; i < numOfVertices; i++)
            {
                edgeMatrix[i] = new int[numOfVertices];
                neighbourList[i] = new HashSet<int>();
            }
            generateTree(numOfVertices, edgeMatrix, neighbourList, id);
            int unusedEdges = edges - (numOfVertices - 1);
            for (int i = 0; i < unusedEdges; i++)
            {
                int parent = rng.Next(numOfVertices);
                int child = rng.Next(numOfVertices);
                int weight = rng.Next(1, 1000);
                for (int j = 0; j < numOfVertices - 1; j++)
                {
                    bool br = false;
                    for (int k = 0; k < numOfVertices - 1; k++)
                    {
                        if(parent != child && edgeMatrix[parent][child] == 0)
                        {
                            edgeMatrix[parent][child] = weight;
                            neighbourList[parent].Add(child);
                            neighbourList[child].Add(parent);
                            br = true;
                            break;
                        }
                        child = (child + 1) % (numOfVertices - 1) + 1;
                    }
                    if (br)
                        break;
                    parent = (parent + 1) % (numOfVertices - 1);
                }
            }
        }

        private static void generateTree(int numOfVertices, int[][] edgeMatrix, HashSet<int>[] neighbourList, int id)
        {
            for (int i = 1; i < numOfVertices; i++)
            {
                int parent = rng.Next(i);
                int weight = rng.Next(1, 1000);
                edgeMatrix[parent][i] = weight;
                neighbourList[parent].Add(i);
                neighbourList[i].Add(parent);
            }
        }

        private static void generateGraphFromUserInput(ref string [] args, ref int numOfVertices, ref int edges, ref int[][] EdgeMatrix, ref HashSet<int>[] neighbourList)
        {
            if (args.Count() == 2 && args[0] == "-file")
            {
                fromFile = false;
                try
                {
                    lines = File.ReadAllText(args[1]).Split(" \n\t".ToCharArray());
                    fromFile = true;
                }
                catch (FileNotFoundException)
                {
                    
                    //return;
                }
                catch (NotSupportedException)
                { }
                catch (SecurityException)
                {

                }
                if (!fromFile)
                {
                    Console.WriteLine("Failed to open " + args[1]);
                }

            }
            if (!fromFile)
                Console.Out.WriteLine("How many vertices do you want in your graph?");
            numOfVertices = Convert.ToInt32(getNextLine());
            EdgeMatrix = new int[numOfVertices][];
            neighbourList = new HashSet<int>[numOfVertices];
            for (int i = 0; i < numOfVertices; i++)
            {
                EdgeMatrix[i] = new int[numOfVertices];
                neighbourList[i] = new HashSet<int>();
            }
            for (int k = 0; k < numOfVertices * numOfVertices; k++)
            {
                bool error;
                do
                {
                    if (!fromFile)
                        Console.Out.WriteLine("Enter the weight of edge marked with an \"X\"");
                    error = false;
                    if (!fromFile)
                    {
                        //StringBuilder sb = new StringBuilder("digraph{");
                        for (int i = 0; i < numOfVertices; i++)
                        {
                            //sb.Append(i + ";");
                            for (int j = 0; j < numOfVertices; j++)
                            {
                                if (i == k / numOfVertices && j == k % numOfVertices)
                                {
                                    Console.Write("X" + "\t");
                                    //sb.Append(i + "->" + j + "[label=\"???\"];");
                                }
                                else
                                {
                                    //if (EdgeMatrix[i][j] != 0)
                                    //    sb.Append(i + "->" + j + "[label=\"" + EdgeMatrix[i][j] + "\"];");
                                    Console.Write(EdgeMatrix[i][j] + "\t");
                                }
                            }
                            Console.WriteLine("");
                        }
                        //sb.Append("}");
                        //Clipboard.SetText(sb.ToString());
                    }
                    try
                    {
                        string line = getNextLine();
                        if (line == null)
                        {
                            Console.Out.WriteLine("Bad file input.");
                            numOfVertices = 0;
                            EdgeMatrix = null;
                            edges = 0;
                            neighbourList = null;
                            return;
                        }
                        EdgeMatrix[k / numOfVertices][k % numOfVertices] = Convert.ToInt32(line);
                        if (EdgeMatrix[k / numOfVertices][k % numOfVertices] != 0)
                        {
                            neighbourList[k / numOfVertices].Add(k % numOfVertices);
                            neighbourList[k % numOfVertices].Add(k / numOfVertices);
                            edges++;
                        }
                    }
                    catch (FormatException)
                    {
                        error = true;
                    }
                    
                } while (error);
            }
        }

        private static bool fromFile = false;
        private static String[] lines;
        private static int lineCount = 0;
        private static string getNextLine()
        {
            if (fromFile)
            {
                try
                {
                return lines[lineCount++];
                }catch (IndexOutOfRangeException)
                {
                    return null;
                }
            }else
            return Console.ReadLine();
        }

        private static Tuple<int,int> calculateMaxFlow(int[][] e, HashSet<int>[] neighbourList, int s, int t)
        {
            int maxFlow = 0;
            int[][] flow = new int[e.Length][];
            int[][] edgeMatrix = new int[e.Length][];
            int counter = 0;
            for (int i = 0; i < edgeMatrix.Length; i++)
            {
                ++counter;
                flow[i] = new int[edgeMatrix.Length];
                ++counter;
                edgeMatrix[i] = new int[edgeMatrix.Length];
                for (int j = 0; j < edgeMatrix.Length; j++)
                {
                    ++counter;
                    edgeMatrix[i][j] = e[i][j];
                }
            }
            ++counter;
            bool pathFound = false;
            do
            {
                ++counter;
                pathFound = false;
                ++counter;
                Queue<int> queue = new Queue<int>();
                ++counter;
                queue.Enqueue(s);
                ++counter;
                Dictionary<int, int> parentMap = new Dictionary<int, int>();
                ++counter;
                HashSet<int> visited = new HashSet<int>();
                ++counter;
                visited.Add(s);
                //printDebug(e,flow);
                while (queue.Count != 0)
                {
                    ++counter;
                    int v = queue.Dequeue();
                    ++counter;
                    if (v == t)
                    {
                        ++counter;
                        ArrayList route = new ArrayList();
                        ++counter;
                        int current = v;
                        ++counter;
                        int min = Int32.MaxValue;
                        ++counter;
                        int parent;
                        while (current != s)
                        {
                            ++counter;
                            route.Add(current);
                            ++counter;
                            parentMap.TryGetValue(current, out parent);
                            ++counter;
                            if (edgeMatrix[parent][current] < min)
                            {
                                ++counter;
                                min = edgeMatrix[parent][current];
                            }
                            ++counter;
                            current = parent;
                        }
                        ++counter;
                        parent = s;
                        for(int i = route.Count-1; i>=0;i--)
                        {
                            ++counter;
                            flow[parent][(int)route[i]] += min;
                            ++counter;
                            flow[(int)route[i]][parent] -= min;
                            ++counter;
                            edgeMatrix[(int)route[i]][parent] += min;
                            ++counter;
                            edgeMatrix[parent][(int)route[i]] -= min;
                            ++counter;
                            parent = (int)route[i];
                        }
                            ++counter;
                        maxFlow += min;
                            ++counter;
                        pathFound = true;
                    }
                    else foreach (int child in neighbourList[v])
                        {
                            ++counter;
                            if (!visited.Contains(child) && e[v][child] > flow[v][child])
                            {
                            ++counter;
                                visited.Add(child);
                            ++counter;
                                queue.Enqueue(child);
                            ++counter;
                                parentMap.Add(child, v);
                            }
                        }

                }
                ++counter;

            } while (pathFound);
            //if(userInput)
                printDebug(e,flow);
            ++counter;
            return new Tuple<int,int>(maxFlow, counter);
        }
        static string getStr(int[][] a)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < a.Length; i++)
            {
                for (int j = 0; j < a[i].Length; j++)
                {
                    sb.Append(a[i][j] + " ");
                }
                sb.Append("   ");
            }
            return sb.ToString();
        }
        private static StringBuilder sb1 = new StringBuilder("digraph{rankdir=LR;");
        private static StringBuilder prefix = new StringBuilder("a");
        private static void printDebug(int[][] edgeMatrix, int[][] flow)
        {
            StringBuilder sb = new StringBuilder("digraph{rankdir=LR;");
            for (int i = 0; i < edgeMatrix.Length; i++)
            {
                sb.Append(prefix.ToString() +i + ";");
                for (int j = 0; j < edgeMatrix.Length; j++)
                {
                    {
                        if (edgeMatrix[i][j] != 0 && i!= edgeMatrix.Length-1 && j != 0)
                            sb.Append(prefix.ToString()+i + "->" + prefix.ToString() + j + "[label=\"" + flow[i][j] + "/" + edgeMatrix[i][j] + "\"];");
                        Console.Write(String.Format("{0,8}",flow[i][j]+"/"+ edgeMatrix[i][j]));
                    }
                }
                Console.WriteLine("");
            }
            sb.Append("}");
            prefix[0] = (char) (prefix[0] + 1);
            //Clipboard.SetText(sb.ToString());
            sb1.Append(sb.ToString().Substring(19,sb.Length-20));
            //Console.ReadLine();
        }
    }
}
