using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KopernicusExpansion
{
    namespace VertexHeightMap32
    {
        class HeightmapUtils
        {
            public const int MODE_8_BIT_NPS = 0;
            public const int MODE_16_BIT = 1;
            public const int MODE_24_BIT = 2;

            private static float SingleSample(Int32 x, Int32 y, MapSO heightMap, int mode)
            {
                // Get the Color, not the Float-Value from the Map
                Color32 c = heightMap.GetPixelColor32(x, y);

                // Get the height data from the terrain
                float height = 0;
                switch (mode)
                {
                    default:
                    case 0:
                        height = (float)c.b / (float)0xFF;
                        break;
                    case 1:
                        height = (float)((int)c.b | ((int)c.g << 8)) / (float)0xFFFF;
                        break;
                    case 2:
                        height = (float)((int)c.b | ((int)c.g << 8) | ((int)c.r << 16)) / (float)0x00FFFFFF;
                        break;
                }

                return height;
            }

            public static float SampleHeightmap(Double u, Double v, MapSO heightMap, int mode)
            {
                if (heightMap == null || !heightMap.IsCompiled) return 0;
                BilinearCoords coords = ConstructBilinearCoords(u, v, heightMap);
                return Mathf.Lerp(
                    Mathf.Lerp(
                        SingleSample(coords.xFloor, coords.yFloor, heightMap, mode),
                        SingleSample(coords.xCeiling, coords.yFloor, heightMap, mode),
                        coords.u),
                    Mathf.Lerp(
                        SingleSample(coords.xFloor, coords.yCeiling, heightMap, mode),
                        SingleSample(coords.xCeiling, coords.yCeiling, heightMap, mode),
                        coords.u),
                    coords.v);
            }

            // Function taken from https://github.com/Kopernicus/pqsmods-standalone/blob/master/KSP/MapSO.cs L340
            public static BilinearCoords ConstructBilinearCoords(Double x, Double y, MapSO heightMap)
            {
                // Create the struct
                BilinearCoords coords = new BilinearCoords();

                // Floor
                x = x - Math.Truncate(x);
                if (y >= 1) y = 1 - (y - 1);
                if (y < 0) y = -y;
                y = y - Math.Truncate(y);
                if (x < 0) x = 1.0 + x;

                // X to U
                coords.x = x * heightMap.Width;
                if (coords.x >= heightMap.Width) coords.x -= heightMap.Width;
                coords.xFloor = (Int32)Math.Floor(coords.x);
                coords.xCeiling = (Int32)Math.Ceiling(coords.x);
                coords.u = (Single)(coords.x - Math.Truncate(coords.x));
                if (coords.xCeiling >= heightMap.Width) coords.xCeiling -= heightMap.Width;

                // Y to V
                coords.y = y * heightMap.Height;
                if (coords.y >= heightMap.Height) coords.y = heightMap.Height - 1 - (coords.y - heightMap.Height);
                coords.yFloor = (Int32)Math.Floor(coords.y);
                coords.yCeiling = (Int32)Math.Ceiling(coords.y);
                coords.v = (Single)(coords.y - Math.Truncate(coords.y));
                if (coords.yCeiling >= heightMap.Height) coords.yCeiling = heightMap.Height - 1 - (coords.yCeiling - heightMap.Height);

                // We're done
                return coords;
            }

            public struct BilinearCoords
            {
                public Double x, y;
                public Int32 xCeiling, xFloor, yCeiling, yFloor;
                public Single u, v;
            }
        }
    }
}
