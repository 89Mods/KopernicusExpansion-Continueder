﻿using Kopernicus.Components;
using Kopernicus.Configuration;
using Kopernicus.Constants;
using System;
using System.Collections.Generic;
using Kopernicus.Configuration.Parsing;
using UnityEngine;
using UnityEngine.Rendering;

namespace KopernicusExpansion
{
    namespace VertexHeightMap32
    {
        /// <summary>
        /// A heightmap PQSMod that can parse encoded 24bpp textures
        /// </summary>
        public class PQSMod_VertexHeightMap24 : PQSMod_VertexHeightMap
        {
            public override void OnVertexBuildHeight(PQS.VertexBuildData data)
            {
                // Apply it
                data.vertHeight += heightMapOffset + heightMapDeformity * HeightmapUtils.SampleHeightmap(data.u, data.v, heightMap, HeightmapUtils.MODE_24_BIT);
            }
        }
    }
}
