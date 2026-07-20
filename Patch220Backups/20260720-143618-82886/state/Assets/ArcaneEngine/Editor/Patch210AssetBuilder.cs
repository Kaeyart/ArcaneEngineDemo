#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ArcaneEngine.EditorTools
{
    [InitializeOnLoad]
    public static class Patch210AssetBuilder
    {
        public const int GeneratorVersion = 21000;
        private const string Root = "Assets/ArcaneEngine/Resources/Patch210";
        private const string TextureRoot = Root + "/Textures";
        private const string MeshRoot = Root + "/Meshes";
        private const string MaterialRoot = Root + "/Materials";
        private const string ManifestPath = Root + "/generation-manifest.json";

        private static bool _scheduled;

        static Patch210AssetBuilder()
        {
            ScheduleEnsure();
        }

        [MenuItem("Arcane Engine/2.1/Generate Procedural VFX Assets")]
        public static void GenerateAll()
        {
            EnsureDirectories();
            GeneratedAssetRuntime21.ClearRuntimeCaches();
            // Texture importers and shader discovery must be allowed to complete while assets
            // are being generated. Avoid StartAssetEditing here because ImportAsset and
            // SaveAndReimport are used by the deterministic texture generator.
            GenerateAtlas();
            GenerateGradients();
            GenerateMeshes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            GenerateMaterials();
            WriteManifest();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            Debug.Log("Patch 2.1 generated procedural VFX assets at " + Root + ".");
        }

        [MenuItem("Arcane Engine/2.1/Validate Generated VFX Assets")]
        public static void ValidateMenu()
        {
            List<string> failures = Validate();
            if (failures.Count == 0)
                Debug.Log("Patch 2.1 generated VFX asset validation passed.");
            else
                Debug.LogError("Patch 2.1 generated VFX asset validation failed:\n- " + string.Join("\n- ", failures.ToArray()));
        }

        public static List<string> Validate()
        {
            List<string> failures = new List<string>();
            if (!File.Exists(ManifestPath)) failures.Add("Generation manifest is missing.");
            if (AssetDatabase.LoadAssetAtPath<Texture2D>(TextureRoot + "/ArcaneVFXAtlas.png") == null) failures.Add("Texture atlas is missing.");
            if (AssetDatabase.LoadAssetAtPath<Texture2D>(TextureRoot + "/ArcaneGradientRamps.png") == null) failures.Add("Gradient ramps are missing.");

            string[] keys = MeshKeys();
            for (int i = 0; i < keys.Length; i++)
            {
                string path = MeshRoot + "/" + AssetName(keys[i], 0) + ".asset";
                if (AssetDatabase.LoadAssetAtPath<Mesh>(path) == null) failures.Add("Mesh is missing: " + path);
            }

            string[] shaderNames =
            {
                "Arcane/VFX21/Additive",
                "Arcane/VFX21/Premultiplied",
                "Arcane/VFX21/Crystal",
                "Arcane/VFX21/Distortion",
                "Arcane/VFX21/GroundResidue",
                "Arcane/VFX21/Ribbon",
                "Arcane/VFX21/Beam",
                "Arcane/VFX21/StatusOverlay",
                "Arcane/VFX21/PostBloom"
            };
            for (int i = 0; i < shaderNames.Length; i++)
                if (Shader.Find(shaderNames[i]) == null) failures.Add("Shader is unavailable: " + shaderNames[i]);

            if (File.Exists(ManifestPath))
            {
                string manifest = File.ReadAllText(ManifestPath);
                if (manifest.IndexOf("\"generatorVersion\": " + GeneratorVersion, StringComparison.Ordinal) < 0)
                    failures.Add("Generation manifest version is stale.");
                if (manifest.IndexOf("\"contentHash\": \"" + GenerationHash() + "\"", StringComparison.Ordinal) < 0)
                    failures.Add("Generation manifest content hash is stale.");
            }
            return failures;
        }

        private static void ScheduleEnsure()
        {
            if (_scheduled) return;
            _scheduled = true;
            EditorApplication.delayCall += delegate
            {
                _scheduled = false;
                if (EditorApplication.isPlayingOrWillChangePlaymode) return;
                if (NeedsGeneration()) GenerateAll();
            };
        }

        private static bool NeedsGeneration()
        {
            if (!File.Exists(ManifestPath)) return true;
            string manifest = File.ReadAllText(ManifestPath);
            if (manifest.IndexOf("\"generatorVersion\": " + GeneratorVersion, StringComparison.Ordinal) < 0) return true;
            return Validate().Count > 0;
        }

        private static void EnsureDirectories()
        {
            Directory.CreateDirectory(TextureRoot);
            Directory.CreateDirectory(MeshRoot);
            Directory.CreateDirectory(MaterialRoot);
        }

        private static void GenerateAtlas()
        {
            const int columns = 8;
            const int rows = 4;
            const int cell = 64;
            Texture2D atlas = new Texture2D(columns * cell, rows * cell, TextureFormat.RGBA32, false, true);
            atlas.name = "ArcaneVFXAtlas";
            Color[] pixels = new Color[atlas.width * atlas.height];
            for (int index = 0; index < columns * rows; index++)
            {
                int cellX = index % columns;
                int cellY = index / columns;
                for (int y = 0; y < cell; y++)
                {
                    for (int x = 0; x < cell; x++)
                    {
                        float u = (x + 0.5f) / cell * 2f - 1f;
                        float v = (y + 0.5f) / cell * 2f - 1f;
                        float alpha = Mask(index, u, v, x, y);
                        int px = cellX * cell + x;
                        int py = (rows - 1 - cellY) * cell + y;
                        pixels[py * atlas.width + px] = new Color(1f, 1f, 1f, Mathf.Clamp01(alpha));
                    }
                }
            }
            atlas.SetPixels(pixels);
            atlas.Apply(false, false);
            string path = TextureRoot + "/ArcaneVFXAtlas.png";
            File.WriteAllBytes(path, atlas.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(atlas);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            ConfigureTexture(path, false, TextureWrapMode.Clamp);
        }

        private static void GenerateGradients()
        {
            const int width = 256;
            const int height = 16;
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false, true);
            texture.name = "ArcaneGradientRamps";
            ReactionElement[] elements =
            {
                ReactionElement.None,
                ReactionElement.Fire,
                ReactionElement.Cold,
                ReactionElement.Lightning,
                ReactionElement.Physical,
                ReactionElement.Blood,
                ReactionElement.Toxic,
                ReactionElement.Void
            };
            for (int y = 0; y < height; y++)
            {
                ReactionElement element = elements[y % elements.Length];
                ElementVisualProfile2 profile = ElementVisualProfileRegistry2.Get(element);
                Color primary = profile == null ? Color.cyan : profile.primary;
                Color secondary = profile == null ? Color.white : profile.secondary;
                Color dark = Color.Lerp(primary, Color.black, y < 8 ? 0.55f : 0.30f);
                for (int x = 0; x < width; x++)
                {
                    float t = x / (float)(width - 1);
                    Color value;
                    if (t < 0.18f) value = Color.Lerp(Color.white, secondary, t / 0.18f);
                    else if (t < 0.58f) value = Color.Lerp(secondary, primary, (t - 0.18f) / 0.40f);
                    else value = Color.Lerp(primary, dark, (t - 0.58f) / 0.42f);
                    value.a = 1f;
                    texture.SetPixel(x, y, value);
                }
            }
            texture.Apply(false, false);
            string path = TextureRoot + "/ArcaneGradientRamps.png";
            File.WriteAllBytes(path, texture.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            ConfigureTexture(path, false, TextureWrapMode.Clamp);
        }

        private static void GenerateMeshes()
        {
            string[] keys = MeshKeys();
            for (int keyIndex = 0; keyIndex < keys.Length; keyIndex++)
            {
                for (int variant = 0; variant < 8; variant++)
                {
                    string path = MeshRoot + "/" + AssetName(keys[keyIndex], variant) + ".asset";
                    if (AssetDatabase.LoadAssetAtPath<Mesh>(path) != null) AssetDatabase.DeleteAsset(path);
                    Mesh source = GeneratedAssetRuntime21.Mesh(keys[keyIndex], variant);
                    Mesh asset = UnityEngine.Object.Instantiate(source);
                    asset.name = AssetName(keys[keyIndex], variant);
                    AssetDatabase.CreateAsset(asset, path);
                }
            }
        }

        private static void GenerateMaterials()
        {
            ReactionElement[] elements =
            {
                ReactionElement.None,
                ReactionElement.Fire,
                ReactionElement.Cold,
                ReactionElement.Lightning,
                ReactionElement.Physical,
                ReactionElement.Blood,
                ReactionElement.Toxic,
                ReactionElement.Void
            };
            BodyPartKind21[] kinds =
            {
                BodyPartKind21.Core,
                BodyPartKind21.Shell,
                BodyPartKind21.InternalEnergy,
                BodyPartKind21.Crystal,
                BodyPartKind21.Ring,
                BodyPartKind21.InternalPath,
                BodyPartKind21.FieldPanel,
                BodyPartKind21.RuneNode
            };
            for (int e = 0; e < elements.Length; e++)
            {
                for (int k = 0; k < kinds.Length; k++)
                {
                    string path = MaterialRoot + "/" + elements[e] + "_" + kinds[k] + ".mat";
                    if (AssetDatabase.LoadAssetAtPath<Material>(path) != null) AssetDatabase.DeleteAsset(path);
                    Color primary = GeneratedAssetRuntime21.ElementColor(elements[e], false);
                    Color secondary = GeneratedAssetRuntime21.ElementColor(elements[e], true);
                    Material runtime = GeneratedAssetRuntime21.Material(kinds[k], elements[e], primary, secondary, 1.2f, 0.82f, e * 101 + k);
                    Material asset = new Material(runtime);
                    asset.name = elements[e] + " " + kinds[k];
                    AssetDatabase.CreateAsset(asset, path);
                }
            }
        }

        private static void WriteManifest()
        {
            string[] keys = MeshKeys();
            string json = "{\n" +
                          "  \"generatorVersion\": " + GeneratorVersion + ",\n" +
                          "  \"generatorSeed\": " + GeneratorVersion + ",\n" +
                          "  \"contentHash\": \"" + GenerationHash() + "\",\n" +
                          "  \"unityVersion\": \"" + Application.unityVersion + "\",\n" +
                          "  \"atlasCells\": 32,\n" +
                          "  \"gradientRows\": 16,\n" +
                          "  \"meshFamilies\": " + keys.Length + ",\n" +
                          "  \"meshVariantsPerFamily\": 8,\n" +
                          "  \"generatedUtc\": \"" + DateTime.UtcNow.ToString("O") + "\"\n" +
                          "}\n";
            File.WriteAllText(ManifestPath, json);
            AssetDatabase.ImportAsset(ManifestPath, ImportAssetOptions.ForceSynchronousImport);
        }

        private static void ConfigureTexture(string path, bool mipmaps, TextureWrapMode wrap)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) return;
            importer.textureType = TextureImporterType.Default;
            importer.alphaSource = TextureImporterAlphaSource.FromInput;
            importer.alphaIsTransparency = true;
            importer.sRGBTexture = true;
            importer.mipmapEnabled = mipmaps;
            importer.wrapMode = wrap;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }

        private static float Mask(int index, float u, float v, int x, int y)
        {
            float distance = Mathf.Sqrt(u * u + v * v);
            float angle = Mathf.Atan2(v, u);
            float radial = Mathf.Clamp01(1f - distance);
            float noise = Hash01(x, y, index + 31);
            switch (index)
            {
                case 0: return Mathf.SmoothStep(0f, 1f, radial);
                case 1: return distance < 0.78f ? 1f : 0f;
                case 2: return Mathf.SmoothStep(0.13f, 0f, Mathf.Abs(distance - 0.62f));
                case 3: return Mathf.Pow(Mathf.Clamp01(1f - Mathf.Min(Mathf.Abs(u), Mathf.Abs(v)) * 4f), 2f) * radial;
                case 4: return Mathf.SmoothStep(0.22f, 0f, Mathf.Abs(v)) * Mathf.SmoothStep(1f, 0.1f, Mathf.Abs(u));
                case 5: return Mathf.Clamp01(radial + Mathf.Sin(v * 11f + u * 4f) * 0.18f + v * 0.24f);
                case 6: return Mathf.Clamp01(radial * 0.8f + (noise - 0.45f) * 0.6f);
                case 7: return Mathf.Clamp01(radial * (0.45f + Cellular(u, v, 5f) * 0.8f));
                case 8: return Fracture(u, v, 9f, index);
                case 9: return Crystal(u, v);
                case 10: return Lightning(u, v, index);
                case 11: return Droplet(u, v);
                case 12: return Mathf.SmoothStep(0.09f, 0f, Mathf.Abs(distance - 0.56f)) + Mathf.SmoothStep(0.28f, 0f, distance) * 0.35f;
                case 13: return Mathf.SmoothStep(0.08f, 0f, Mathf.Abs(distance - 0.48f)) + Mathf.Pow(radial, 5f) * 0.25f;
                case 14: return RuneGlyph(u, v, 6, 0f);
                case 15: return Mathf.Pow(Mathf.Clamp01(1f - Mathf.Min(Mathf.Abs(u), Mathf.Abs(v)) * 6f), 3f) * Mathf.Pow(radial, 0.5f);
                case 16: return Fracture(u, v, 5f, index) * radial;
                case 17: return Mathf.Clamp01(radial + (noise - 0.5f) * 0.85f);
                case 18: return Mathf.Pow(radial, 2f);
                case 19: return Mathf.Pow(Mathf.Abs(Mathf.Sin(angle * 6f)), 6f) * radial;
                default:
                    int spokes = 3 + index % 8;
                    float ring = Mathf.SmoothStep(0.11f, 0f, Mathf.Abs(distance - Mathf.Lerp(0.35f, 0.72f, (index % 4) / 3f)));
                    float spoke = Mathf.Pow(Mathf.Abs(Mathf.Sin(angle * spokes + index)), 8f) * radial;
                    return Mathf.Clamp01(ring + spoke * 0.55f + (noise - 0.65f) * 0.2f);
            }
        }

        private static float Cellular(float u, float v, float scale)
        {
            float x = u * scale; float y = v * scale;
            int ix = Mathf.FloorToInt(x); int iy = Mathf.FloorToInt(y);
            float minimum = 10f;
            for (int oy = -1; oy <= 1; oy++)
            {
                for (int ox = -1; ox <= 1; ox++)
                {
                    float px = ix + ox + Hash01(ix + ox, iy + oy, 71);
                    float py = iy + oy + Hash01(ix + ox, iy + oy, 97);
                    minimum = Mathf.Min(minimum, Mathf.Sqrt((x - px) * (x - px) + (y - py) * (y - py)));
                }
            }
            return Mathf.Clamp01(1f - minimum);
        }

        private static float Fracture(float u, float v, float scale, int seed)
        {
            float x = u * scale; float y = v * scale;
            float a = Mathf.Abs(Mathf.Sin(x * 2.7f + y * 1.4f + seed));
            float b = Mathf.Abs(Mathf.Sin(y * 3.1f - x * 1.1f + seed * 0.7f));
            return Mathf.SmoothStep(0.075f, 0f, Mathf.Min(a, b));
        }

        private static float Crystal(float u, float v)
        {
            float diamond = Mathf.Abs(u) * 0.72f + Mathf.Abs(v) * 1.15f;
            float spine = Mathf.SmoothStep(0.08f, 0f, Mathf.Abs(u));
            return Mathf.Clamp01(Mathf.SmoothStep(1f, 0.58f, diamond) + spine * 0.4f);
        }

        private static float Lightning(float u, float v, int seed)
        {
            float path = Mathf.Sin((v + 1f) * 9f + seed) * 0.16f + Mathf.Sin((v + 1f) * 21f) * 0.05f;
            float main = Mathf.SmoothStep(0.055f, 0f, Mathf.Abs(u - path));
            float branch = v > 0f ? Mathf.SmoothStep(0.035f, 0f, Mathf.Abs(u + v * 0.55f - 0.2f)) * 0.65f : 0f;
            return Mathf.Clamp01(main + branch) * Mathf.SmoothStep(1f, 0.6f, Mathf.Abs(v));
        }

        private static float Droplet(float u, float v)
        {
            float y = v + 0.18f;
            float body = Mathf.SmoothStep(1f, 0.72f, Mathf.Sqrt(u * u * 1.4f + y * y));
            float tip = Mathf.SmoothStep(0.22f, 0f, Mathf.Abs(u)) * Mathf.SmoothStep(0.95f, 0.25f, v);
            return Mathf.Clamp01(body + tip * 0.55f);
        }

        private static float RuneGlyph(float u, float v, int sides, float rotation)
        {
            float angle = Mathf.Atan2(v, u) + rotation;
            float distance = Mathf.Sqrt(u * u + v * v);
            float polygon = Mathf.Cos(Mathf.Floor(0.5f + angle / (Mathf.PI * 2f / sides)) * (Mathf.PI * 2f / sides) - angle) * distance;
            float edge = Mathf.SmoothStep(0.045f, 0f, Mathf.Abs(polygon - 0.58f));
            float axis = Mathf.SmoothStep(0.035f, 0f, Mathf.Abs(u)) * Mathf.SmoothStep(0.65f, 0.12f, Mathf.Abs(v));
            return Mathf.Clamp01(edge + axis);
        }

        private static float Hash01(int x, int y, int seed)
        {
            unchecked
            {
                int value = x * 73856093 ^ y * 19349663 ^ seed * 83492791;
                return StableSeed21.Unit(StableSeed21.Combine(value, value >> 11));
            }
        }

        private static string GenerationHash()
        {
            string input = GeneratorVersion + "|32|16|8|" +
                           string.Join("|", MeshKeys()) +
                           "|Additive|Premultiplied|Crystal|Distortion|GroundResidue|Ribbon|Beam|StatusOverlay|PostBloom";
            return Hash128.Compute(input).ToString();
        }

        private static string[] MeshKeys()
        {
            return new[]
            {
                "core", "shell", "energy", "rune-glyph", "child-core",
                "orbit-rail", "orbital", "conductor", "fracture", "ground",
                "field-panel", "beam", "impact", "pierce-spike", "countdown-ring",
                "compression-ring", "crystal", "shard", "tether", "trail"
            };
        }

        private static string AssetName(string key, int variant)
        {
            string clean = key.Replace("-", "_").Replace(" ", "_");
            return char.ToUpperInvariant(clean[0]) + clean.Substring(1) + "_" + variant;
        }
    }
}
#endif
