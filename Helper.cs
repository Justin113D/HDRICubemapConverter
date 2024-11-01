using SimpleImageIO;
using System;
using System.Data;

namespace HDRICubemapConverter
{
    public static class Helper
    {
        private const float _epsilonF = 1e-06f;

        public static bool IsEqualApprox(this float a, float b)
        {
            // Check for exact equality first, required to handle "infinity" values.
            if(a == b)
            {
                return true;
            }
            // Then check for approximate equality.
            float tolerance = _epsilonF * Math.Abs(a);
            if(tolerance < _epsilonF)
            {
                tolerance = _epsilonF;
            }

            return Math.Abs(a - b) < tolerance;
        }

        public static float[] SamplePixel(this Image source, float x, float y)
        {
            int left = (int)float.Floor(x);
            int right = (int)float.Ceiling(x);
            float xTime = x - left;

            int top = (int)float.Floor(y);
            int bottom = (int)float.Ceiling(y);
            float yTime = y - top;

            float[] pixelTL = source.GetPixelChannels(left, top);
            float[] pixelTR = source.GetPixelChannels(right, top);
            float[] pixelBL = source.GetPixelChannels(left, bottom);
            float[] pixelBR = source.GetPixelChannels(right, bottom);

            float tlTime = (1 - xTime) * (1 - yTime);
            float trTime = xTime * (1 - yTime);
            float blTime = (1 - xTime) * yTime;
            float brTime = xTime * yTime;

            float[] result = new float[source.NumChannels];
            for(int c = 0; c < result.Length; c++)
            {
                result[c] = (pixelTL[c] * tlTime)
                    + (pixelTR[c] * trTime)
                    + (pixelBL[c] * blTime)
                    + (pixelBR[c] * brTime);
            }

            return result;
        }


        public static float[] GetPixelChannels(this Image source, int x, int y)
        {
            float[] result = new float[source.NumChannels];
            for(int c = 0; c < source.NumChannels; c++)
            {
                result[c] = source.GetPixelChannel(x, y, c);
            }

            return result;
        }
    }
}
