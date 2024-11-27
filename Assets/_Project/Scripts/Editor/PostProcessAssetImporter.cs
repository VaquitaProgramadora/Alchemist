using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Alchemist.Editor {
    public class PostProcessAssetImporter : AssetPostprocessor {
        // Prefijos para los diferentes tipos de activos.
        private static readonly string _textureTypePrefix = "T_";
        private static readonly string _materialTypePrefix = "M_";
        private static readonly string _videoTypesPrefix = "V_";
        private static readonly string _audioTypesPrefix = "AU_";
        private static readonly string _prefabTypePrefix = "PF_";

        // Extensiones de archivo correspondientes a cada tipo.
        private static readonly string[] _textureTypes =
            { ".png", ".jpg", ".jpeg", ".tga", ".tif", ".tiff", ".psd", ".bmp", ".iff", ".pict", ".gif", ".exr" };

        private static readonly string[] _videoTypes =
            { ".mp4", ".avi", ".mov", ".wmv", ".mpeg", ".mpg", ".m4v", ".mkv", ".flv", ".webm" };

        private static readonly string[] _audioTypes =
            { ".mp3", ".wav", ".ogg", ".aif", ".aiff", ".aifc", ".au", ".snd", ".m4a" };

        private static readonly string _materialType = ".mat";
        private static readonly string _prefabType = ".prefab";

        // Verifica si el archivo está en la carpeta del proyecto.
        private static bool IsInProjectFolder(string path) => path.Contains("Assets/_Project");

        private static bool IsTexture(string name) =>
            _textureTypes.Any(name.EndsWith) && !name.Contains("ReflectionProbe");

        private static bool HasTexturePrefix(string name) => name.StartsWith(_textureTypePrefix);

        private static bool IsMaterial(string name) => name.EndsWith(_materialType);
        private static bool HasMaterialPrefix(string name) => name.StartsWith(_materialTypePrefix);

        private static bool IsVideo(string name) => _videoTypes.Any(name.EndsWith);
        private static bool HasVideoPrefix(string name) => name.StartsWith(_videoTypesPrefix);

        private static bool IsAudio(string name) => _audioTypes.Any(name.EndsWith);
        private static bool HasAudioPrefix(string name) => name.StartsWith(_audioTypesPrefix);

        private static bool IsPrefab(string name) => name.EndsWith(_prefabType);
        private static bool HasPrefabPrefix(string name) => name.StartsWith(_prefabTypePrefix);

        // Método llamado después de importar todos los activos.
        static void OnPostprocessAllAssets(string[] importedAssetsPaths, string[] deletedAssetsPaths,
            string[] movedAssetsPaths, string[] movedFromAssetPaths) {
            foreach (var assetPath in importedAssetsPaths) {
                if (!IsInProjectFolder(assetPath)) continue;

                var fileName = Path.GetFileName(assetPath);
                if (IsTexture(assetPath) && !HasTexturePrefix(fileName)) {
                    RenameAsset(assetPath, fileName, _textureTypePrefix);
                } else if (IsMaterial(assetPath) && !HasMaterialPrefix(fileName)) {
                    RenameAsset(assetPath, fileName, _materialTypePrefix);
                } else if (IsVideo(assetPath) && !HasVideoPrefix(fileName)) {
                    RenameAsset(assetPath, fileName, _videoTypesPrefix);
                } else if (IsAudio(assetPath) && !HasAudioPrefix(fileName)) {
                    RenameAsset(assetPath, fileName, _audioTypesPrefix);
                } else if (IsPrefab(assetPath) && !HasPrefabPrefix(fileName)) {
                    RenameAsset(assetPath, fileName, _prefabTypePrefix);
                }
            }
        }

        // Método llamado antes de procesar una textura.
        private void OnPreprocessTexture() {
            if (assetPath.Contains("/256/")) {
                var textureImporter = (TextureImporter)assetImporter;
                textureImporter.spritePixelsPerUnit = 256;
                Debug.Log($"#PostProcessAssetImporter# Setting Pixels Per Unit to 256 for texture: {assetPath}");
            }
        }

        // Renombra un activo con el prefijo adecuado.
        private static void RenameAsset(string assetPath, string fileName, string prefix) {
            Debug.Log($"#PostProcessAssetImporter# Renaming asset imported: {assetPath} with prefix: {prefix}");
            var newName = $"{prefix}{fileName}";
            newName = ToPascalCase(newName);
            AssetDatabase.RenameAsset(assetPath, newName);
        }

        // Convierte un nombre a PascalCase.
        private static string ToPascalCase(string newName) {
            var words = newName.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(word => word.Substring(0, 1).ToUpper() + word.Substring(1));
            newName = String.Join("_", words);
            return newName;
        }
    }
}