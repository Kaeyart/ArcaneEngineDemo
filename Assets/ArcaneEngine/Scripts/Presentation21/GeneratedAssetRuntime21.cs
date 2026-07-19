using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public static class GeneratedAssetRuntime21
    {
        private static readonly Dictionary<string, Mesh> Meshes = new Dictionary<string, Mesh>();
        private static readonly Dictionary<string, Material> Materials = new Dictionary<string, Material>();
        private static Texture2D _runtimeAtlas;
        private static Texture2D _runtimeGradient;

        public static Mesh Mesh(string key, int seed)
        {
            string normalized = string.IsNullOrEmpty(key) ? "core" : key.ToLowerInvariant();
            string cacheKey = normalized + ":" + (seed & 7);
            Mesh mesh;
            if (Meshes.TryGetValue(cacheKey, out mesh) && mesh != null)
                return mesh;

            mesh = Resources.Load<Mesh>("Patch210/Meshes/" + AssetName(normalized, seed));
            if (mesh == null)
                mesh = GenerateMesh(normalized, seed);
            mesh.name = "AE21 " + normalized + " " + (seed & 7);
            Meshes[cacheKey] = mesh;
            return mesh;
        }

        public static Material Material(
            BodyPartKind21 kind,
            ReactionElement element,
            Color primary,
            Color secondary,
            float emissive,
            float opacity,
            int seed)
        {
            string shaderKey = ShaderKey(kind, element);
            string cacheKey = shaderKey + ":" + element + ":" +
                              ColorUtility.ToHtmlStringRGBA(primary) + ":" +
                              Mathf.RoundToInt(emissive * 10f) + ":" +
                              Mathf.RoundToInt(opacity * 10f);
            Material cached;
            if (Materials.TryGetValue(cacheKey, out cached) && cached != null)
                return cached;

            Shader shader = Shader.Find(shaderKey);
            if (shader == null)
                shader = Shader.Find("Arcane/VFX21/Additive");
            if (shader == null)
                shader = Shader.Find("Sprites/Default");
            if (shader == null)
                shader = Shader.Find("Standard");

            Material material = new Material(shader);
            material.name = "AE21 " + element + " " + kind;
            material.hideFlags = HideFlags.DontSave;
            SetColor(material, "_Color", primary);
            SetColor(material, "_PrimaryColor", primary);
            SetColor(material, "_SecondaryColor", secondary);
            SetColor(material, "_CatalystColor", secondary);
            SetFloat(material, "_Emission", emissive);
            SetFloat(material, "_Opacity", opacity);
            SetFloat(material, "_NoiseScale", Mathf.Lerp(1.5f, 5.5f, StableSeed21.Unit(seed)));
            SetFloat(material, "_NoiseSpeed", Mathf.Lerp(0.3f, 2.4f, StableSeed21.Unit(seed + 11)));
            SetFloat(material, "_PulseFrequency", Mathf.Lerp(1f, 8f, StableSeed21.Unit(seed + 29)));
            SetTexture(material, "_MainTex", RuntimeAtlas());
            SetTexture(material, "_GradientTex", RuntimeGradient());
            Materials[cacheKey] = material;
            return material;
        }

        public static Texture2D RuntimeAtlas()
        {
            if (_runtimeAtlas != null)
                return _runtimeAtlas;

            _runtimeAtlas = Resources.Load<Texture2D>("Patch210/Textures/ArcaneVFXAtlas");
            if (_runtimeAtlas != null)
                return _runtimeAtlas;

            const int size = 128;
            _runtimeAtlas = new Texture2D(size, size, TextureFormat.RGBA32, false, true);
            _runtimeAtlas.name = "AE21 Runtime Mask Atlas";
            _runtimeAtlas.wrapMode = TextureWrapMode.Clamp;
            _runtimeAtlas.filterMode = FilterMode.Bilinear;
            Color[] pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float u = (x + 0.5f) / size * 2f - 1f;
                    float v = (y + 0.5f) / size * 2f - 1f;
                    float distance = Mathf.Sqrt(u * u + v * v);
                    float radial = Mathf.Clamp01(1f - distance);
                    float star = Mathf.Pow(Mathf.Clamp01(1f - Mathf.Abs(u * v) * 4f), 3f);
                    float noise = Hash01(x, y, 31);
                    float alpha = Mathf.Clamp01(radial * 0.72f + star * 0.20f + (noise - 0.5f) * 0.10f);
                    pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
                }
            }
            _runtimeAtlas.SetPixels(pixels);
            _runtimeAtlas.Apply(false, true);
            return _runtimeAtlas;
        }

        public static Texture2D RuntimeGradient()
        {
            if (_runtimeGradient != null)
                return _runtimeGradient;

            _runtimeGradient = Resources.Load<Texture2D>("Patch210/Textures/ArcaneGradientRamps");
            if (_runtimeGradient != null)
                return _runtimeGradient;

            _runtimeGradient = new Texture2D(128, 8, TextureFormat.RGBA32, false, true);
            _runtimeGradient.name = "AE21 Runtime Gradient Ramps";
            _runtimeGradient.wrapMode = TextureWrapMode.Clamp;
            _runtimeGradient.filterMode = FilterMode.Bilinear;
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
            for (int y = 0; y < 8; y++)
            {
                ElementVisualProfile2 profile = ElementVisualProfileRegistry2.Get(elements[y]);
                Color a = Color.white;
                Color b = profile == null ? Color.cyan : profile.secondary;
                Color c = profile == null ? Color.blue : profile.primary;
                for (int x = 0; x < 128; x++)
                {
                    float t = x / 127f;
                    Color value = t < 0.35f
                        ? Color.Lerp(a, b, t / 0.35f)
                        : Color.Lerp(b, c, (t - 0.35f) / 0.65f);
                    value.a = Mathf.SmoothStep(1f, 0f, t);
                    _runtimeGradient.SetPixel(x, y, value);
                }
            }
            _runtimeGradient.Apply(false, true);
            return _runtimeGradient;
        }

        public static Color ElementColor(ReactionElement element, bool secondary)
        {
            ElementVisualProfile2 profile = ElementVisualProfileRegistry2.Get(element);
            if (profile == null)
                return secondary ? new Color(0.8f, 0.95f, 1f) : new Color(0.4f, 0.7f, 1f);
            return secondary ? profile.secondary : profile.primary;
        }

        public static void ClearRuntimeCaches()
        {
            Meshes.Clear();
            Materials.Clear();
        }

        private static string ShaderKey(BodyPartKind21 kind, ReactionElement element)
        {
            if (kind == BodyPartKind21.Crystal || element == ReactionElement.Cold)
                return "Arcane/VFX21/Crystal";
            if (kind == BodyPartKind21.FieldPanel || kind == BodyPartKind21.Ring)
                return "Arcane/VFX21/GroundResidue";
            if (kind == BodyPartKind21.InternalPath || kind == BodyPartKind21.MotionGuide)
                return "Arcane/VFX21/Ribbon";
            if (element == ReactionElement.Void)
                return Patch210Settings21.DistortionEnabled
                    ? "Arcane/VFX21/Distortion"
                    : "Arcane/VFX21/Premultiplied";
            return kind == BodyPartKind21.Shell
                ? "Arcane/VFX21/Premultiplied"
                : "Arcane/VFX21/Additive";
        }

        private static string AssetName(string key, int seed)
        {
            string clean = key.Replace("-", "_").Replace(" ", "_");
            return char.ToUpperInvariant(clean[0]) + clean.Substring(1) + "_" + (seed & 7);
        }

        private static Mesh GenerateMesh(string key, int seed)
        {
            if (key.Contains("ring") || key.Contains("orbit") || key.Contains("countdown"))
                return GenerateTorus(28, 8, 0.5f, 0.045f);
            if (key.Contains("crystal") || key.Contains("shard") || key.Contains("spike") || key.Contains("conductor"))
                return GenerateShard(seed);
            if (key.Contains("beam") || key.Contains("tether") || key.Contains("path") || key.Contains("trail"))
                return GenerateWedge();
            if (key.Contains("ground") || key.Contains("field") || key.Contains("impact"))
                return GenerateDisc(seed, 32);
            if (key.Contains("fracture"))
                return GenerateBrokenRing(seed);
            if (key.Contains("rune"))
                return GenerateRunePlate(seed);
            if (key.Contains("shell"))
                return GenerateIcosphere(1);
            if (key.Contains("child"))
                return GenerateIcosphere(0);
            if (key.Contains("energy"))
                return GenerateIcosphere(1);
            return GenerateIcosphere(1);
        }

        private static Mesh GenerateIcosphere(int subdivisions)
        {
            Mesh mesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            float t = (1f + Mathf.Sqrt(5f)) * 0.5f;
            Vector3[] baseVertices =
            {
                new Vector3(-1, t, 0), new Vector3(1, t, 0), new Vector3(-1, -t, 0), new Vector3(1, -t, 0),
                new Vector3(0, -1, t), new Vector3(0, 1, t), new Vector3(0, -1, -t), new Vector3(0, 1, -t),
                new Vector3(t, 0, -1), new Vector3(t, 0, 1), new Vector3(-t, 0, -1), new Vector3(-t, 0, 1)
            };
            for (int i = 0; i < baseVertices.Length; i++) vertices.Add(baseVertices[i].normalized * 0.5f);
            int[] faces =
            {
                0,11,5, 0,5,1, 0,1,7, 0,7,10, 0,10,11,
                1,5,9, 5,11,4, 11,10,2, 10,7,6, 7,1,8,
                3,9,4, 3,4,2, 3,2,6, 3,6,8, 3,8,9,
                4,9,5, 2,4,11, 6,2,10, 8,6,7, 9,8,1
            };
            triangles.AddRange(faces);
            for (int level = 0; level < subdivisions; level++)
            {
                Dictionary<long, int> midpoint = new Dictionary<long, int>();
                List<int> next = new List<int>();
                for (int i = 0; i < triangles.Count; i += 3)
                {
                    int a = triangles[i]; int b = triangles[i + 1]; int c = triangles[i + 2];
                    int ab = Midpoint(vertices, midpoint, a, b);
                    int bc = Midpoint(vertices, midpoint, b, c);
                    int ca = Midpoint(vertices, midpoint, c, a);
                    next.Add(a); next.Add(ab); next.Add(ca);
                    next.Add(b); next.Add(bc); next.Add(ab);
                    next.Add(c); next.Add(ca); next.Add(bc);
                    next.Add(ab); next.Add(bc); next.Add(ca);
                }
                triangles = next;
            }
            Vector2[] uv = new Vector2[vertices.Count];
            for (int i = 0; i < vertices.Count; i++)
            {
                Vector3 n = vertices[i].normalized;
                uv[i] = new Vector2(0.5f + Mathf.Atan2(n.z, n.x) / (Mathf.PI * 2f), 0.5f - Mathf.Asin(n.y) / Mathf.PI);
            }
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.uv = uv;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        private static int Midpoint(List<Vector3> vertices, Dictionary<long, int> cache, int a, int b)
        {
            int min = Mathf.Min(a, b); int max = Mathf.Max(a, b);
            long key = ((long)min << 32) + max;
            int index;
            if (cache.TryGetValue(key, out index)) return index;
            index = vertices.Count;
            vertices.Add(((vertices[a] + vertices[b]) * 0.5f).normalized * 0.5f);
            cache[key] = index;
            return index;
        }

        private static Mesh GenerateShard(int seed)
        {
            float skewX = Mathf.Lerp(-0.16f, 0.16f, StableSeed21.Unit(seed));
            float skewZ = Mathf.Lerp(-0.16f, 0.16f, StableSeed21.Unit(seed + 5));
            Vector3[] vertices =
            {
                new Vector3(-0.16f, -0.35f, -0.12f), new Vector3(0.18f, -0.35f, -0.1f),
                new Vector3(0.12f, -0.35f, 0.16f), new Vector3(-0.14f, -0.35f, 0.12f),
                new Vector3(skewX, 0.5f, skewZ)
            };
            int[] triangles =
            {
                0,1,2, 0,2,3,
                0,4,1, 1,4,2, 2,4,3, 3,4,0
            };
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = new[]
            {
                new Vector2(0,0), new Vector2(1,0), new Vector2(1,1), new Vector2(0,1), new Vector2(0.5f,1)
            };
            mesh.RecalculateNormals(); mesh.RecalculateBounds();
            return mesh;
        }

        private static Mesh GenerateTorus(int majorSegments, int minorSegments, float radius, float tube)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();
            List<int> triangles = new List<int>();
            for (int i = 0; i <= majorSegments; i++)
            {
                float u = i / (float)majorSegments;
                float a = u * Mathf.PI * 2f;
                Vector3 center = new Vector3(Mathf.Cos(a) * radius, 0f, Mathf.Sin(a) * radius);
                for (int j = 0; j <= minorSegments; j++)
                {
                    float v = j / (float)minorSegments;
                    float b = v * Mathf.PI * 2f;
                    Vector3 offset = new Vector3(Mathf.Cos(a) * Mathf.Cos(b) * tube, Mathf.Sin(b) * tube, Mathf.Sin(a) * Mathf.Cos(b) * tube);
                    vertices.Add(center + offset);
                    uv.Add(new Vector2(u, v));
                }
            }
            int row = minorSegments + 1;
            for (int i = 0; i < majorSegments; i++)
            {
                for (int j = 0; j < minorSegments; j++)
                {
                    int a = i * row + j; int b = a + row;
                    triangles.Add(a); triangles.Add(b); triangles.Add(a + 1);
                    triangles.Add(a + 1); triangles.Add(b); triangles.Add(b + 1);
                }
            }
            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices); mesh.SetUVs(0, uv); mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals(); mesh.RecalculateBounds(); return mesh;
        }

        private static Mesh GenerateWedge()
        {
            Vector3[] vertices =
            {
                new Vector3(-0.08f,-0.08f,-0.5f), new Vector3(0.08f,-0.08f,-0.5f),
                new Vector3(0.08f,0.08f,-0.5f), new Vector3(-0.08f,0.08f,-0.5f),
                new Vector3(-0.02f,-0.02f,0.5f), new Vector3(0.02f,-0.02f,0.5f),
                new Vector3(0.02f,0.02f,0.5f), new Vector3(-0.02f,0.02f,0.5f)
            };
            int[] triangles =
            {
                0,1,2, 0,2,3, 4,6,5, 4,7,6,
                0,4,5, 0,5,1, 1,5,6, 1,6,2,
                2,6,7, 2,7,3, 3,7,4, 3,4,0
            };
            Mesh mesh = new Mesh(); mesh.vertices = vertices; mesh.triangles = triangles;
            mesh.RecalculateNormals(); mesh.RecalculateBounds(); return mesh;
        }

        private static Mesh GenerateDisc(int seed, int segments)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();
            List<int> triangles = new List<int>();
            vertices.Add(Vector3.zero); uv.Add(new Vector2(0.5f, 0.5f));
            for (int i = 0; i <= segments; i++)
            {
                float angle = Mathf.PI * 2f * i / segments;
                float irregular = Mathf.Lerp(0.82f, 1.12f, StableSeed21.Unit(StableSeed21.Combine(seed, i)));
                vertices.Add(new Vector3(Mathf.Cos(angle) * 0.5f * irregular, 0f, Mathf.Sin(angle) * 0.5f * irregular));
                uv.Add(new Vector2(Mathf.Cos(angle) * 0.5f + 0.5f, Mathf.Sin(angle) * 0.5f + 0.5f));
                if (i < segments)
                {
                    triangles.Add(0); triangles.Add(i + 1); triangles.Add(i + 2);
                }
            }
            Mesh mesh = new Mesh(); mesh.SetVertices(vertices); mesh.SetUVs(0, uv); mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals(); mesh.RecalculateBounds(); return mesh;
        }

        private static Mesh GenerateBrokenRing(int seed)
        {
            int segments = 28;
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();
            List<int> triangles = new List<int>();
            for (int i = 0; i < segments; i++)
            {
                if (StableSeed21.Unit(StableSeed21.Combine(seed, i)) < 0.17f) continue;
                float a0 = Mathf.PI * 2f * i / segments;
                float a1 = Mathf.PI * 2f * (i + 0.78f) / segments;
                float inner = 0.36f; float outer = 0.5f;
                int start = vertices.Count;
                vertices.Add(new Vector3(Mathf.Cos(a0) * inner, 0f, Mathf.Sin(a0) * inner));
                vertices.Add(new Vector3(Mathf.Cos(a0) * outer, 0f, Mathf.Sin(a0) * outer));
                vertices.Add(new Vector3(Mathf.Cos(a1) * outer, 0f, Mathf.Sin(a1) * outer));
                vertices.Add(new Vector3(Mathf.Cos(a1) * inner, 0f, Mathf.Sin(a1) * inner));
                uv.Add(Vector2.zero); uv.Add(Vector2.right); uv.Add(Vector2.one); uv.Add(Vector2.up);
                triangles.Add(start); triangles.Add(start + 1); triangles.Add(start + 2);
                triangles.Add(start); triangles.Add(start + 2); triangles.Add(start + 3);
            }
            Mesh mesh = new Mesh(); mesh.SetVertices(vertices); mesh.SetUVs(0, uv); mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals(); mesh.RecalculateBounds(); return mesh;
        }

        private static Mesh GenerateRunePlate(int seed)
        {
            int sides = 6;
            List<Vector3> vertices = new List<Vector3> { Vector3.zero };
            List<Vector2> uv = new List<Vector2> { new Vector2(0.5f, 0.5f) };
            List<int> triangles = new List<int>();
            for (int i = 0; i <= sides; i++)
            {
                float angle = Mathf.PI * 2f * i / sides + Mathf.PI / 6f;
                vertices.Add(new Vector3(Mathf.Cos(angle) * 0.5f, 0f, Mathf.Sin(angle) * 0.5f));
                uv.Add(new Vector2(Mathf.Cos(angle) * 0.5f + 0.5f, Mathf.Sin(angle) * 0.5f + 0.5f));
                if (i < sides) { triangles.Add(0); triangles.Add(i + 1); triangles.Add(i + 2); }
            }
            Mesh mesh = new Mesh(); mesh.SetVertices(vertices); mesh.SetUVs(0, uv); mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals(); mesh.RecalculateBounds(); return mesh;
        }

        private static float Hash01(int x, int y, int seed)
        {
            unchecked
            {
                int value = x * 73856093 ^ y * 19349663 ^ seed * 83492791;
                value = StableSeed21.Combine(value, value >> 7);
                return StableSeed21.Unit(value);
            }
        }

        private static void SetFloat(Material material, string name, float value)
        {
            if (material != null && material.HasProperty(name)) material.SetFloat(name, value);
        }

        private static void SetColor(Material material, string name, Color value)
        {
            if (material != null && material.HasProperty(name)) material.SetColor(name, value);
        }

        private static void SetTexture(Material material, string name, Texture value)
        {
            if (material != null && material.HasProperty(name)) material.SetTexture(name, value);
        }
    }

    public static class Patch210Settings21
    {
        private const string Prefix = "Arcane.Patch210.";

        public static bool DistortionEnabled
        {
            get { return PlayerPrefs.GetInt(Prefix + "Distortion", 1) != 0 && Patch200PresentationSettings.Distortion; }
            set { PlayerPrefs.SetInt(Prefix + "Distortion", value ? 1 : 0); PlayerPrefs.Save(); }
        }

        public static bool BloomEnabled
        {
            get { return PlayerPrefs.GetInt(Prefix + "Bloom", 1) != 0; }
            set { PlayerPrefs.SetInt(Prefix + "Bloom", value ? 1 : 0); PlayerPrefs.Save(); }
        }

        public static float FlashLimit
        {
            get { return Mathf.Clamp(PlayerPrefs.GetFloat(Prefix + "FlashLimit", 0.65f), 0.05f, 1f); }
            set { PlayerPrefs.SetFloat(Prefix + "FlashLimit", Mathf.Clamp01(value)); PlayerPrefs.Save(); }
        }

        public static bool TargetResponse
        {
            get { return PlayerPrefs.GetInt(Prefix + "TargetResponse", 1) != 0; }
            set { PlayerPrefs.SetInt(Prefix + "TargetResponse", value ? 1 : 0); PlayerPrefs.Save(); }
        }

        public static bool NearMiss
        {
            get { return PlayerPrefs.GetInt(Prefix + "NearMiss", 1) != 0; }
            set { PlayerPrefs.SetInt(Prefix + "NearMiss", value ? 1 : 0); PlayerPrefs.Save(); }
        }

        public static bool MorphologyOverlay
        {
            get { return PlayerPrefs.GetInt(Prefix + "Overlay", 0) != 0; }
            set { PlayerPrefs.SetInt(Prefix + "Overlay", value ? 1 : 0); PlayerPrefs.Save(); }
        }
    }
}
