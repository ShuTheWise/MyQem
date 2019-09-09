using System;
using System.Diagnostics;
namespace qem
{
    public class Program
    {
        private static void PrintErrorMessage(string exception)
        {
            Console.WriteLine("\nIncorrect input args. Try using [help].\n" +
                $"Details: {exception}");
        }

        private static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "help")
            {
                Console.WriteLine("STL Quadric Error Metrics Help:");
                Console.WriteLine("qem [input_path] [output_path] only STL file format is supported");
                Console.WriteLine("\nOptional args:");
                Console.WriteLine("-q [quality] approximate target number of triangles based on a 0-1 range float. Default is 0.5");
                Console.WriteLine("-t [trisCount] specific target number of triangles");
                return;
            }

            if (args.Length != 2 && args.Length != 4)
            {
                PrintErrorMessage($"Incorrect number of arguments ({args.Length}) Try using 2 or 4 args total.");
                return;
            }

            var inputPath = args[0];
            var outputPath = args[1];

            if (inputPath == outputPath)
            {
                PrintErrorMessage($"Input path same as output path. This is not permitted to prevent data loss.");
                return;
            }

            Mesh originalMesh = STLParser.LoadSTL(inputPath);
            string message = "";

            int targetCount = -1;
            if (args.Length == 2)
            {
                message = "?";
                targetCount = (int)(0.5f * originalMesh.trisCount);
            }
            else if (args.Length == 4)
            {
                message = "parsing error";
                if (args[2] == "-q")
                {
                    message += " -float";
                    float f;
                    if (float.TryParse(args[3], out f))
                    {
                        if (f > 0f && f < 1f)
                        {
                            targetCount = (int)(f * originalMesh.trisCount);
                        }
                    }
                }
                else if (args[2] == "-t")
                {
                    message += " -int";
                    int i;
                    if (int.TryParse(args[3], out i))
                    {
                        targetCount = i;
                    }
                }
            }

            if (targetCount == -1)
            {
                PrintErrorMessage(message);
                return;
            }

            if (targetCount <= 0 || targetCount >= originalMesh.trisCount)
            {
                PrintErrorMessage($"Incorrect number of target tris {targetCount}");
                return;
            }

            Console.WriteLine($"\nSimplifing to a target number of triangles -> {targetCount} <- \n");

            Run(originalMesh, outputPath, targetCount);
        }

        private static void Run(Mesh originalMesh, string outputPath, int targetCount)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            PrintMeshInfo("Input", originalMesh.vertexCount, originalMesh.trisCount);
            float t1 = stopwatch.ElapsedMilliseconds;

            Mesh simplifiedMesh = originalMesh.Simplify(targetCount);
            float t2 = stopwatch.ElapsedMilliseconds - t1;

            STLParser.SaveSTL(simplifiedMesh, outputPath);
            PrintMeshInfo("Output", simplifiedMesh.vertexCount, simplifiedMesh.trisCount);
            float t3 = stopwatch.ElapsedMilliseconds - t2;

            Console.WriteLine();

            Console.WriteLine($"Execution details [ms]." +
                $"\nOverall time: {t1 + t2 + t3}" +
                $"\n STL Loading: {t1}" +
                $"\n QEM Algorithm: {t2}" +
                $"\n STL Saving: {t3}");
        }

        private static void PrintMeshInfo(string header, int verts, int currentTrisCount)
        {
            Console.WriteLine($"| {header} | Vertices: {verts}, Tris: {currentTrisCount}");
        }
    }
}