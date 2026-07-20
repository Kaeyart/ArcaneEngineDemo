using System;

namespace ArcaneEngine
{
    public static class ArpgDeterminism30
    {
        public static int StableHash(string value)
        {
            unchecked
            {
                uint hash = 2166136261u;
                if (!string.IsNullOrEmpty(value))
                {
                    for (int index = 0; index < value.Length; index++)
                    {
                        char character = value[index];
                        hash ^= (byte)(character & 0xFF);
                        hash *= 16777619u;
                        hash ^= (byte)(character >> 8);
                        hash *= 16777619u;
                    }
                }
                return (int)(hash & 0x7FFFFFFF);
            }
        }

        public static int Combine(string first, string second, int third)
        {
            unchecked
            {
                uint hash = 2166136261u;
                hash = Mix(hash, StableHash(first));
                hash = Mix(hash, StableHash(second));
                hash = Mix(hash, third);
                return (int)(hash & 0x7FFFFFFF);
            }
        }

        public static int Combine(int first, int second, int third)
        {
            unchecked
            {
                uint hash = 2166136261u;
                hash = Mix(hash, first);
                hash = Mix(hash, second);
                hash = Mix(hash, third);
                return (int)(hash & 0x7FFFFFFF);
            }
        }

        public static int Positive(int value)
        {
            if (value == int.MinValue) return int.MaxValue;
            return Math.Abs(value);
        }

        public static int Index(int seed, int count)
        {
            if (count <= 0) throw new ArgumentOutOfRangeException("count", "A deterministic index requires a positive collection size.");
            return Positive(seed) % count;
        }

        private static uint Mix(uint hash, int value)
        {
            unchecked
            {
                hash ^= (byte)(value & 0xFF);
                hash *= 16777619u;
                hash ^= (byte)((value >> 8) & 0xFF);
                hash *= 16777619u;
                hash ^= (byte)((value >> 16) & 0xFF);
                hash *= 16777619u;
                hash ^= (byte)((value >> 24) & 0xFF);
                hash *= 16777619u;
                return hash;
            }
        }
    }
}
