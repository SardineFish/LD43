using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using System.IO;


namespace Assets.Editor
{
    class SpriteSheetImporter : AssetPostprocessor
    {
        void OnPostprocessTexture(Texture2D texture)
        {
            var importer = assetImporter as TextureImporter;
            importer.spritePixelsPerUnit = 16;
            importer.filterMode = FilterMode.Point;
            importer.spriteImportMode = SpriteImportMode.Multiple;
        }
    }
}
