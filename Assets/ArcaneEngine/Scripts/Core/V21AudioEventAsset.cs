using UnityEngine;

namespace ArcaneEngine
{
    [CreateAssetMenu(menuName = "Arcane Engine/2.1/Audio Event", fileName = "AudioEvent")]
        public sealed class V21AudioEventAsset : ScriptableObject
        {
            public string stableId;
            public string category;
            public AudioClip[] clips = new AudioClip[0];
            public Vector2 pitchRange = new Vector2(0.96f, 1.04f);
            public Vector2 volumeRange = new Vector2(0.85f, 1f);
            public int maxVoices = 8;
            public bool loop;
        }
}
