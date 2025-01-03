﻿using System.Collections.Generic;
using System.Linq;
using IconsCreationTool.Editor.Utility.Extensions;
using UnityEngine;

namespace IconsCreationTool.Editor.Core
{
    public struct IconsCreatorData
    {
        public int Size { get; }
        public float Padding { get; }
        public string Prefix { get; }
        public string Suffix { get; }
        public IconBackgroundData BackgroundData { get; }
        public GameObject[] Targets { get; }
        public bool RenderShadows { get; }
        public float CameraDistance { get; }
        public float OffsetX { get; }
        public float OffsetY { get; }


        public IconsCreatorData(int size, float padding, string prefix, string suffix,
            IconBackgroundData backgroundData, List<Object> targets, bool renderShadows,
            float cameraDistance, float offsetX, float offsetY)
        {
            Size = size;
            Padding = padding;
            Prefix = prefix;
            Suffix = suffix;
            BackgroundData = backgroundData;
            Targets = targets.ExtractAllGameObjects().Where(g => g.HasVisibleMesh()).ToArray();
            RenderShadows = renderShadows;
            CameraDistance = cameraDistance;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }
    }
}