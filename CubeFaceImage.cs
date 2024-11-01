using SimpleImageIO;

namespace HDRICubemapConverter
{
    public class CubeFaceImage
    {
        public Image Source { get; }
        public int FaceX { get; }
        public int FaceY { get; }
        public int FaceWidth { get; }
        public int FaceHeight { get; }

        public CubeFaceImage(Image texture, int faceX, int faceY, int faceWidth, int faceHeight)
        {
            Source = texture;
            FaceX = faceX;
            FaceY = faceY;
            FaceWidth = faceWidth;
            FaceHeight = faceHeight;
        }

        public float[] GetPixel(float u, float v)
        {
            return Source.SamplePixel(
                FaceX + float.Clamp(u * FaceWidth, 0, FaceWidth - 1),
                FaceY + float.Clamp(v * FaceHeight, 0, FaceHeight - 1)
            );
        }

        public void SetPixel(int x, int y, float[] channels)
        {
            Source.SetPixelChannels(
                FaceX + x,
                FaceY + y,
                channels
            );
        }
    }
}
