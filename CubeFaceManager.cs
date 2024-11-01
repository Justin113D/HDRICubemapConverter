using SimpleImageIO;
using System;

namespace HDRICubemapConverter
{
    public class CubeFaceManager
    {

        public CubeFaceImage PositiveX { get; }
        public CubeFaceImage NegativeX { get; }
        public CubeFaceImage PositiveY { get; }
        public CubeFaceImage NegativeY { get; }
        public CubeFaceImage PositiveZ { get; }
        public CubeFaceImage NegativeZ { get; }

        public CubeFaceManager(Image source, string layout)
        {
            int cubeRes;

            switch(layout)
            {
                case "cube":
                    cubeRes = source.Width / 4;
                    NegativeX = new(source, cubeRes * 0, cubeRes, cubeRes, cubeRes);
                    PositiveZ = new(source, cubeRes * 1, cubeRes, cubeRes, cubeRes);
                    PositiveX = new(source, cubeRes * 2, cubeRes, cubeRes, cubeRes);
                    NegativeZ = new(source, cubeRes * 3, cubeRes, cubeRes, cubeRes);
                    PositiveY = new(source, cubeRes, 0, cubeRes, cubeRes);
                    NegativeY = new(source, cubeRes, cubeRes * 2, cubeRes, cubeRes);
                    break;
                case "line":
                    cubeRes = source.Height;
                    PositiveX = new(source, cubeRes * 0, 0, cubeRes, cubeRes);
                    NegativeX = new(source, cubeRes * 1, 0, cubeRes, cubeRes);
                    PositiveY = new(source, cubeRes * 2, 0, cubeRes, cubeRes);
                    NegativeY = new(source, cubeRes * 3, 0, cubeRes, cubeRes);
                    PositiveZ = new(source, cubeRes * 4, 0, cubeRes, cubeRes);
                    NegativeZ = new(source, cubeRes * 5, 0, cubeRes, cubeRes);
                    break;
                default:
                    throw new ArgumentException("Invalid cubemap layout! Needs to be either \"line\" or \"cube\"!", nameof(layout));
            }
        }
    }
}
