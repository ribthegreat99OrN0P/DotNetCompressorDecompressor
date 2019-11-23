using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
namespace DotNetCompressorDecompressor
{
    class Program
    {
        public static AssemblyDef assemblyDef;
        public static Assembly assemblyRef;
        public static byte[] DecompressedArray;
        static void Main(string[] args)
        {
            Console.Title = "Static DotNetCompressor Decompressor";
            Console.WriteLine("Working for versions: 1 and 1.01");
            assemblyDef = AssemblyDef.Load(args[0]);
            assemblyRef = Assembly.LoadFile(args[0]);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Loaded! Decompressing...");
            MethodDef EntryMethod = assemblyDef.ManifestModule.EntryPoint;
            for (int i = 0; i < EntryMethod.Body.Instructions.Count; i++)
            {
                Instruction ins = EntryMethod.Body.Instructions[i];
                if (ins.OpCode == OpCodes.Ldc_I4)
                {
                    int num = ins.GetLdcI4Value();
                    if (num == 5120)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Detected 1st Layered Compression!");
                        Decompress(1, 0);
                    }else if(num == 6656)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Detected 2nd Layered Compression!");
                        Decompress(2, 0);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Layered Compression Level not found! But don't worry we are static ;)");
                        Decompress(3, num);
                    }
                }
            }
            File.WriteAllBytes(Path.GetFileNameWithoutExtension(args[0]) + "-decompressed.exe", DecompressedArray);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Decompressed and Saved!");
            Console.ReadKey();
        }
        public static void Decompress(int level, int StaticNum)
        {
            ResourceManager resourceManager = new ResourceManager("resource", assemblyRef);
            byte[] buffer = (byte[])resourceManager.GetObject("app");
            if (level == 1)//First Layer Compression
            {
                int num = 5120;
                MemoryStream memoryStream = new MemoryStream(buffer);
                GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
                byte[] array = new byte[num];
                gzipStream.Read(array, 0, num);
                memoryStream.Close();
                gzipStream.Close();
                GC.Collect();
                DecompressedArray = array;
            }
            else if (level == 2)//Second Layer Compression
            {
                int num = 6656;
                MemoryStream memoryStream = new MemoryStream(buffer);
                GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
                byte[] array = new byte[num];
                gzipStream.Read(array, 0, num);
                memoryStream.Close();
                gzipStream.Close();
                GC.Collect();
                DecompressedArray = array;
            }
            else if(level == 3)//Static Layer Detection
            {
                if (StaticNum == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Sadly it looks like this version is incompatiable see you soon!!");
                    Console.ReadKey();
                }
                else
                {
                    MemoryStream memoryStream = new MemoryStream(buffer);
                    GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
                    byte[] array = new byte[StaticNum];
                    gzipStream.Read(array, 0, StaticNum);
                    memoryStream.Close();
                    gzipStream.Close();
                    GC.Collect();
                    DecompressedArray = array;
                }
               
            }
        }
    }
}
