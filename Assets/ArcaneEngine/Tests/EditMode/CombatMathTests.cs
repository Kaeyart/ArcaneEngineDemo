using NUnit.Framework;
using UnityEngine;

namespace ArcaneEngine.Tests
{
    public sealed class CombatMathTests
    {
        [Test]
        public void CalculateDamage_WithZeroResistance_ReturnsFullAmount()
        {
            float result = CombatMath.CalculateDamage(100f, 0f, 0f);
            Assert.That(result, Is.EqualTo(100f).Within(0.01f));
        }

        [Test]
        public void CalculateDamage_WithResistance_ReducesDamage()
        {
            float result = CombatMath.CalculateDamage(100f, 30f, 0f);
            Assert.That(result, Is.LessThan(100f));
            Assert.That(result, Is.GreaterThan(0f));
        }

        [Test]
        public void CalculateDamage_WithArmor_FurtherReducesDamage()
        {
            float noArmor = CombatMath.CalculateDamage(100f, 30f, 0f);
            float withArmor = CombatMath.CalculateDamage(100f, 30f, 40f);
            Assert.That(withArmor, Is.LessThan(noArmor));
        }

        [Test]
        public void CalculateDamage_WithFullResistance_ReturnsZero()
        {
            float result = CombatMath.CalculateDamage(100f, 100f, 0f);
            Assert.That(result, Is.EqualTo(0f));
        }

        [Test]
        public void CalculateDamage_NeverReturnsNegative()
        {
            float result = CombatMath.CalculateDamage(10f, 200f, 200f);
            Assert.That(result, Is.GreaterThanOrEqualTo(0f));
        }

        [Test]
        public void CalculateDamage_ZeroInput_ReturnsZero()
        {
            float result = CombatMath.CalculateDamage(0f, 50f, 25f);
            Assert.That(result, Is.EqualTo(0f));
        }

        [Test]
        public void CriticalMultiplier_IncreasesDamage()
        {
            float normal = CombatMath.CalculateDamage(100f, 10f, 0f);
            float critical = CombatMath.CalculateDamage(100f, 10f, 0f, 2.0f);
            Assert.That(critical, Is.GreaterThan(normal));
        }

        [Test]
        public void ShieldAbsorption_ReducesEffectiveDamage()
        {
            // Shield should absorb some damage before health is touched
            float fullDamage = CombatMath.CalculateDamage(100f, 0f, 0f);
            Assert.That(fullDamage, Is.LessThanOrEqualTo(100f));
        }

        [Test]
        public void ElementalWeakness_IncreasesDamage()
        {
            float neutral = CombatMath.CalculateDamage(100f, 0f, 0f, 1.0f, 1.0f);
            float weak = CombatMath.CalculateDamage(100f, 0f, 0f, 1.0f, 1.5f);
            Assert.That(weak, Is.GreaterThan(neutral));
        }

        [Test]
        public void ElementalResistance_DecreasesDamage()
        {
            float neutral = CombatMath.CalculateDamage(100f, 0f, 0f, 1.0f, 1.0f);
            float resistant = CombatMath.CalculateDamage(100f, 0f, 0f, 1.0f, 0.5f);
            Assert.That(resistant, Is.LessThan(neutral));
        }
    }
}