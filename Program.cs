﻿using SimpleImageIO;
using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.Intrinsics;

namespace HDRICubemapConverter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if(args.Length < 2)
            {
                Console.WriteLine("HDRI / HDR Cubemap converter 1.0.0 - @Justin113D");
                Console.WriteLine();
                Console.WriteLine("Usages:");
                Console.WriteLine("  HDRICubemapConverter hdri [input cubemap layout] [input cubemap filepath]");
                Console.WriteLine("  HDRICubemapConverter [output cubemap layout] [input hdri filepath]");
                Console.WriteLine();
                Console.WriteLine("Cubemap layouts:");
                Console.WriteLine("  - line: cube sides in a horizontal line:");
                Console.WriteLine("            -X+X-Y+Y-Z+Z");
                Console.WriteLine("  - cube: traditional origami-like layout:");
                Console.WriteLine("            __+Y____");
                Console.WriteLine("            -X+Z+X-Z");
                Console.WriteLine("            __-Y____");
                Console.WriteLine();
                return;
            }

            string targetType, filepath, cubemapLayout;

            targetType = args[0].ToLower();

            if(args.Length == 3)
            {
                cubemapLayout = args[1].ToLower();
                filepath = args[2];
            }
            else
            {
                cubemapLayout = "line";
                filepath = args[1];
            }

            if(!File.Exists(filepath))
            {
                Console.WriteLine("Invalid filepath!");
                return;
            }

            if(cubemapLayout is not "line" and not "cube")
            {
                Console.WriteLine("Invalid cubemap layout! Needs to be either \"line\" or \"cube\"!");
                return;
            }

            Image source = new(filepath);
            Image output;

            switch(targetType)
            {
                case "hdri":
                    output = CubemapToHDRI(source, cubemapLayout);
                    break;
                case "line":
                case "cube":
                    output = HDRIToCubemap(source, targetType);
                    break;
                default:
                    Console.WriteLine("Invalid target type!");
                    return;
            }

            int extensionIndex = filepath.IndexOf('.');
            string outputFilepath = filepath[..extensionIndex] + "_" + args[0].ToUpper() + filepath[extensionIndex..];
            output.WriteToFile(outputFilepath);
        }

        public static Image HDRIToCubemap(Image source, string cubemapLayout)
        {
            int cubeRes = source.Width / 4;
            Image result;

            switch(cubemapLayout)
            {
                case "line":
                    result = new(cubeRes * 6, cubeRes, source.NumChannels);
                    break;
                case "cube":
                    result = new(cubeRes * 4, cubeRes * 3, source.NumChannels);
                    break;
                default:
                    throw new ArgumentException();
            }

            CubeFaceManager cube = new(result, cubemapLayout);
            float resFac = 1f / cubeRes;

            void SampleCubeFace(CubeFaceImage face, Vector3 baseVector, Vector3 uVector, Vector3 vVector)
            {
                for(int y = 0; y < cubeRes; y++)
                {
                    float v = y * resFac;

                    for(int x = 0; x < cubeRes; x++)
                    {
                        float u = x * resFac;

                        Vector3 normal = Vector3.Normalize(
                            baseVector
                            + (uVector * u)
                            + (vVector * v)
                        );

                        float phi = float.Acos(-normal.Y);
                        float theta = float.Atan2(-normal.X, normal.Z);
                        float hdriU = theta / (float.Pi * 2f) % 1f;
                        float hdriV = phi / float.Pi % 1f;

                        if(hdriU < 0)
                        {
                            hdriU += 1f;
                        }

                        if(hdriV < 0)
                        {
                            hdriV += 1f;
                        }

                        face.SetPixel(x, y, 
                            source.SamplePixel(
                                hdriU * source.Width,
                                hdriV * source.Height
                            )
                        );
                    }
                }
            }

            SampleCubeFace(cube.PositiveX, new(0.5f, -0.5f, -0.5f), new(0, 0, 1), new(0, 1, 0));
            SampleCubeFace(cube.NegativeX, new(-0.5f, -0.5f, 0.5f), new(0, 0, -1), new(0, 1, 0));
            SampleCubeFace(cube.PositiveY, new(-0.5f, -0.5f, 0.5f), new(1, 0, 0), new(0, 0, -1));
            SampleCubeFace(cube.NegativeY, new(-0.5f, 0.5f, -0.5f), new(1, 0, 0), new(0, 0, 1));
            SampleCubeFace(cube.PositiveZ, new(-0.5f, -0.5f, -0.5f), new(1, 0, 0), new(0, 1, 0));
            SampleCubeFace(cube.NegativeZ, new(0.5f, -0.5f, 0.5f), new(-1, 0, 0), new(0, 1, 0));

            return result;
        }

        public static Image CubemapToHDRI(Image source, string cubemapLayout)
        {
            CubeFaceManager cube = new(source, cubemapLayout);

            Image result = new(cube.PositiveX.FaceWidth * 4, cube.PositiveX.FaceWidth * 2, source.NumChannels);

            for(int y = 0; y < result.Height; y++)
            {
                float v = 1 - (y / (float)result.Height);
                float theta = v * float.Pi;

                for(int x = 0; x < result.Width; x++)
                {
                    float u = x / (float)result.Width;
                    float phi = u * 2 * float.Pi;

                    // unit vector
                    Vector3 normal = new(
                        float.Sin(phi) * -float.Sin(theta),
                        float.Cos(theta),
                        float.Cos(phi) * -float.Sin(theta)
                    );

                    float fac = float.Max(float.Max(float.Abs(normal.X), float.Abs(normal.Y)), float.Abs(normal.Z));
                    Vector3 coord = ((normal / fac) + Vector3.One) * 0.5f;

                    float cubeU, cubeV;
                    CubeFaceImage cubeFace;

                    if(coord.X.IsEqualApprox(1))
                    {
                        cubeFace = cube.PositiveX;
                        cubeU = coord.Z - 1f;
                        cubeV = coord.Y;
                    }
                    else if(coord.X.IsEqualApprox(0))
                    {
                        cubeFace = cube.NegativeX;
                        cubeU = coord.Z;
                        cubeV = coord.Y;

                    }
                    else if(coord.Y.IsEqualApprox(1))
                    {
                        cubeFace = cube.NegativeY;
                        cubeU = coord.X;
                        cubeV = coord.Z - 1f;
                    }
                    else if(coord.Y.IsEqualApprox(0))
                    {
                        cubeFace = cube.PositiveY;
                        cubeU = coord.X;
                        cubeV = coord.Z;
                    }
                    else if(coord.Z.IsEqualApprox(1))
                    {
                        cubeFace = cube.PositiveZ;
                        cubeU = coord.X;
                        cubeV = coord.Y;
                    }
                    else if(coord.Z.IsEqualApprox(0))
                    {
                        cubeFace = cube.NegativeZ;
                        cubeU = coord.X - 1f;
                        cubeV = coord.Y;
                    }
                    else
                    {
                        Console.WriteLine("Unknown face, something went wrong");
                        continue;
                    }

                    float[] pixel = cubeFace.GetPixel(
                        float.Abs(cubeU),
                        float.Abs(cubeV));

                    result.SetPixelChannels(x, y, pixel);
                }
            }

            return result;
        }
    }
}
