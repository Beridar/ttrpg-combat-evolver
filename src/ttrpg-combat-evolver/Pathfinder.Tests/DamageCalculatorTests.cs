using NUnit.Framework;

namespace Pathfinder.Tests
{
    public class DamageCalculatorTests
    {
        private Monster _monster;
        private PlayerCharacter _playerCharacter;

        [SetUp]
        public void Setup()
        {
            _monster = new Monster
            {
                ArmorClass = 20
            };

            _playerCharacter = new PlayerCharacter
            {
                AttackBonus = 10,
                MinRollForThreat = 20,
                StaticDamageThatCanCrit = 8,
                CriticalMultiplier = 2,
                PowerAttackPenaltyToAttack = 2,
                PowerAttackBonusToDamage = 4,
            };
        }

        [Test]
        public void It_should_do_no_damage_on_a_miss()
        {
            var rng = new ConsistentDieRoll(1);
            var damageCalculator = new DamageCalculator(() => rng);

            var damageResult = damageCalculator.Calculate(_playerCharacter, _monster);

            Assert.AreEqual(0, damageResult);
        }

        [Test]
        public void It_should_do_eight_damage_on_a_hit()
        {
            var rng = new ConsistentDieRoll(10);
            var damageCalculator = new DamageCalculator(() => rng);

            var damageResult = damageCalculator.Calculate(_playerCharacter, _monster);

            Assert.AreEqual(8, damageResult);
        }

        [Test]
        public void A_natural_one_is_always_a_miss()
        {
            var rng = new ConsistentDieRoll(1);
            var damageCalculator = new DamageCalculator(() => rng);

            _playerCharacter.AttackBonus = _monster.ArmorClass;

            var damageResult = damageCalculator.Calculate(_playerCharacter, _monster);

            Assert.AreEqual(0, damageResult);
        }

        [Test]
        public void A_natural_twenty_is_always_a_hit()
        {
            var rng = new ConsistentDieRoll(20);
            var damageCalculator = new DamageCalculator(() => rng);

            _playerCharacter.AttackBonus = _monster.ArmorClass - 20;
            _playerCharacter.MinRollForThreat = 21;

            var damageResult = damageCalculator.Calculate(_playerCharacter, _monster);

            Assert.AreEqual(8, damageResult);
        }

        [Test]
        public void An_unconfirmed_crit_does_the_same_damage_as_a_hit()
        {
            var rng = new SequentialDieRolls(20, 1);
            var damageCalculator = new DamageCalculator(() => rng);

            var damageResult = damageCalculator.Calculate(_playerCharacter, _monster);

            Assert.AreEqual(8, damageResult);
        }

        [Test]
        public void A_confirmed_crit_does_extra_damage()
        {
            var rng = new SequentialDieRolls(20, 20);
            var damageCalculator = new DamageCalculator(() => rng);

            var damageResult = damageCalculator.Calculate(_playerCharacter, _monster);

            Assert.AreEqual(16, damageResult);
        }

        [Test]
        public void It_should_use_random_numbers_for_the_crittable_damage_portion_when_calculating_hit_damage()
        {
            const int confirmation = 0,
                damageRoll1 = 6,
                damageRoll2 = 7,
                damageRoll3 = 5,
                damageRoll4 = 3,
                damageRoll5 = 9;

            var rolls = new[] { 10, confirmation, damageRoll1, damageRoll2, damageRoll3, damageRoll4, damageRoll5 };

            var rng = new SequentialDieRolls(rolls);
            var damageCalculator = new DamageCalculator(() => rng);

            _playerCharacter.RandomDamageDieCount = 5;

            var damageResult = damageCalculator.Calculate(_playerCharacter, _monster);

            var expectedDamage = 8 + 6 + 7 + 5 + 3 + 9;
            Assert.AreEqual(expectedDamage, damageResult);
        }

        [Test]
        public void It_should_use_random_numbers_for_the_non_crittable_damage_portion_when_calculating_hit_damage()
        {
            const int confirmation = 0,
                damageRoll1 = 23,
                damageRoll2 = 34;

            var rolls = new[] { 10, confirmation, damageRoll1, damageRoll2 };

            var rng = new SequentialDieRolls(rolls);
            var damageCalculator = new DamageCalculator(() => rng);

            _playerCharacter.RandomDamageDieCountThatWillNotCrit = 2;

            var damageResult = damageCalculator.Calculate(_playerCharacter, _monster);

            var expectedDamage = 8 + 23 + 34;
            Assert.AreEqual(expectedDamage, damageResult);
        }

        [Test]
        public void It_should_use_random_numbers_for_the_crittable_damage_portion_when_calculating_crit_damage()
        {
            const int confirmation = 20,
                damageRoll1 = 2,
                damageRoll2 = 3,
                critDamageRoll1 = 10,
                critDamageRoll2 = 17;

            var rolls = new[] { 20, confirmation, damageRoll1, damageRoll2, critDamageRoll1, critDamageRoll2 };

            var rng = new SequentialDieRolls(rolls);
            var damageCalculator = new DamageCalculator(() => rng);

            _playerCharacter.RandomDamageDieCount = 2;

            var damageResult = damageCalculator.Calculate(_playerCharacter, _monster);

            var expectedDamage = 8 + 8 + 2 + 3 + 10 + 17;
            Assert.AreEqual(expectedDamage, damageResult);
        }

        [Test]
        public void It_should_use_random_numbers_only_once_for_the_non_crittable_damage_portion_when_calculating_crit_damage()
        {
            const int confirmation = 20,
                damageRoll1 = 2,
                damageRoll2 = 3;

            var rolls = new[] { 20, confirmation, damageRoll1, damageRoll2 };

            var rng = new SequentialDieRolls(rolls);
            var damageCalculator = new DamageCalculator(() => rng);

            _playerCharacter.RandomDamageDieCountThatWillNotCrit = 2;

            var damageResult = damageCalculator.Calculate(_playerCharacter, _monster);

            var expectedDamage = 8 + 8 + 2 + 3;
            Assert.AreEqual(expectedDamage, damageResult);
        }

        [Test]
        public void A_critical_multiplier_of_five_means_five_separate_damage_calculations()
        {
            const int confirmation = 20,
                damageRoll1 = 9,
                damageRoll2 = 21,
                damageRoll3 = 37,
                damageRoll4 = 36,
                damageRoll5 = 3;

            var rolls = new[] { 20, confirmation, damageRoll1, damageRoll2, damageRoll3, damageRoll4, damageRoll5 };

            var rng = new SequentialDieRolls(rolls);
            var damageCalculator = new DamageCalculator(() => rng);

            _playerCharacter.CriticalMultiplier = 5;
            _playerCharacter.RandomDamageDieCount = 1;

            var damageResult = damageCalculator.Calculate(_playerCharacter, _monster);

            var expectedDamage = (8 * 5) + 9 + 21 + 37 + 36 + 3;
            Assert.AreEqual(expectedDamage, damageResult);
        }

        [Test]
        public void Precision_damage_is_included_in_hit_damage()
        {
            var rng = new SequentialDieRolls(20, 1);
            var damageCalculator = new DamageCalculator(() => rng);

            _playerCharacter.StaticDamageThatWillNotCrit = 11;

            var damageResult = damageCalculator.Calculate(_playerCharacter, _monster);

            Assert.AreEqual(19, damageResult);
        }

        [Test]
        public void The_size_of_the_random_damage_crittable_die_should_be_used_to_calculate_random_damage()
        {
            var rng = new AlwaysMaxDieRoll();
            var damageCalculator = new DamageCalculator(() => rng);

            _playerCharacter.StaticDamageThatCanCrit = 0;
            _playerCharacter.RandomDamageDieCount = 1;
            _playerCharacter.CriticalMultiplier = 1;

            _playerCharacter.RandomDamageDieSize = 23;

            var damageResult = damageCalculator.Calculate(_playerCharacter, _monster);

            Assert.AreEqual(23, damageResult);
        }

        [Test]
        public void The_size_of_the_random_damage_non_crittable_die_should_be_used_to_calculate_random_damage()
        {
            var rng = new AlwaysMaxDieRoll();
            var damageCalculator = new DamageCalculator(() => rng);

            _playerCharacter.StaticDamageThatCanCrit = 0;
            _playerCharacter.RandomDamageDieCount = 0;
            _playerCharacter.CriticalMultiplier = 1;

            _playerCharacter.RandomDamageDieCountThatWillNotCrit = 1;
            _playerCharacter.RandomDamageDieSizeThatWillNotCrit = 27;

            var damageResult = damageCalculator.Calculate(_playerCharacter, _monster);

            Assert.AreEqual(27, damageResult);
        }

        [Test]
        public void Setting_power_attack_should_decrease_the_accuracy()
        {
            _playerCharacter.IsPowerAttacking = true;

            Assert.AreEqual(8, _playerCharacter.AttackBonusAfterPowerAttack);
        }

        [Test]
        public void Setting_power_attack_should_increase_damage()
        {
            _playerCharacter.IsPowerAttacking = true;

            Assert.AreEqual(12, _playerCharacter.StaticDamageThatCanCritAfterPowerAttack);
        }

        [Test]
        public void Power_attack_damage_should_be_represented_in_a_hit()
        {
            var rng = new ConsistentDieRoll(15);
            var damageCalculator = new DamageCalculator(() => rng);

            _playerCharacter.IsPowerAttacking = true;

            var damageResult = damageCalculator.Calculate(_playerCharacter, _monster);

            Assert.AreEqual(12, damageResult);
        }

        [Test]
        public void Power_attack_damage_should_be_represented_in_a_crit()
        {
            var rng = new ConsistentDieRoll(20);
            var damageCalculator = new DamageCalculator(() => rng);

            _playerCharacter.IsPowerAttacking = true;

            var damageResult = damageCalculator.Calculate(_playerCharacter, _monster);

            Assert.AreEqual(24, damageResult);
        }

        [Test]
        public void Power_attack_attack_penalty_should_be_represented_in_every_attack()
        {
            var rng = new ConsistentDieRoll(10);
            var damageCalculator = new DamageCalculator(() => rng);

            _playerCharacter.IsPowerAttacking = true;

            var damageResult = damageCalculator.Calculate(_playerCharacter, _monster);

            Assert.AreEqual(0, damageResult);
        }
    }
}
