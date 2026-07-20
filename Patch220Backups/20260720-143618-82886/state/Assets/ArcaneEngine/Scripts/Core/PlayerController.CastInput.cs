using UnityEngine;

namespace ArcaneEngine
{
    public sealed partial class PlayerController
    {
        private readonly bool[] _castHeldPrevious =
            new bool[3];

        private void HandleCastInputStable(
            SpellSlot slot,
            bool held,
            bool released)
        {
            int index = (int)slot;

            if (index < 0 || index >= _castHeldPrevious.Length)
                return;

            CompiledSpell spell =
                GameWorld.Instance.GetSpell(slot);

            bool pressed =
                held && !_castHeldPrevious[index];

            _castHeldPrevious[index] = held;

            if (spell == null)
                return;

            if (spell.castMethod == SpellCastMethod.Charged)
            {
                if (!ProfileManager.Current.controls.holdToCharge)
                {
                    if (pressed)
                        TryManualCast(slot, 1.25f, 1f);

                    if (released)
                        _chargeTimes[index] = 0f;

                    return;
                }

                if (held && _cooldowns[index] <= 0f)
                {
                    _chargeTimes[index] =
                        Mathf.Min(
                            1.5f,
                            _chargeTimes[index] + Time.deltaTime);
                }

                if (released && _chargeTimes[index] > 0.05f)
                {
                    float normalized =
                        Mathf.Clamp01(_chargeTimes[index] / 1.5f);

                    _chargeTimes[index] = 0f;

                    TryManualCast(
                        slot,
                        Mathf.Lerp(0.75f, 2.1f, normalized),
                        Mathf.Lerp(0.7f, 1.5f, normalized));
                }

                return;
            }

            if (spell.castMethod == SpellCastMethod.Channeled)
            {
                if (held)
                    TryManualCast(slot, 1f, 1f);

                return;
            }

            // Discrete spells cast once per press, not every frame held.
            if (pressed)
                TryManualCast(slot, 1f, 1f);
        }
    }
}
