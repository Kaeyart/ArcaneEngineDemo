using System;
using UnityEngine;

namespace ArcaneEngine
{
    public sealed class SpellCoreDefinition : ScriptableObject
        {
            public string id;
            public string displayName;
            [TextArea] public string description;
            public SpellDelivery delivery;
            public SpellCastMethod castMethod;
            public SpellElement element;
            public Color color = Color.white;
            public float baseDamage;
            public float manaCost;
            public float cooldown;
            public float speed;
            public float lifetime;
            public float size;
            public float radius;
            public string[] tags = new string[0];
        }
}
