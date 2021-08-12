using System;

namespace Pathfinder
{
    public interface IDamageCalculator
    {
        int Calculate(IRandomDieRoll randomDieRoll);
    }

    public class DamageCalculator : IDamageCalculator
    {
        private readonly PlayerCharacter _playerCharacter;
        private readonly Monster _monster;

        public DamageCalculator(PlayerCharacter playerCharacter, Monster monster)
        {
            _playerCharacter = playerCharacter;
            _monster = monster;
        }

        public int Calculate(IRandomDieRoll randomDieRoll)
        {
            var attackRoll = randomDieRoll.GetNextNumber(20);
            var confirmation = randomDieRoll.GetNextNumber(20);

            if (ThisAttackIsACrit(attackRoll, confirmation))
                return CalculateDamageForACrit(randomDieRoll);

            if (ThisAttackIsAHit(attackRoll))
                return CalculateDamageForAHit(randomDieRoll);

            return 0;
        }

        private bool ThisAttackIsAHit(int attackRoll)
        {
            switch (attackRoll)
            {
                case 1:
                    return false;
                case 20:
                    return true;
                default:
                    return _playerCharacter.AttackBonusAfterPowerAttack + attackRoll >= _monster.ArmorClass;
            }
        }

        private int CalculateDamageForAHit(IRandomDieRoll randomDieRoll)
        {
            return CalculateDamageThatMayCrit(randomDieRoll)
                   + CalculateDamageThatWillNotCrit(randomDieRoll);
        }

        private int CalculateDamageThatMayCrit(IRandomDieRoll randomDieRoll)
        {
            var damage = _playerCharacter.StaticDamageThatCanCritAfterPowerAttack;

            for (var i = 0; i < _playerCharacter.RandomDamageDieCount; i++)
                damage += randomDieRoll.GetNextNumber(_playerCharacter.RandomDamageDieSize);

            return damage;
        }

        private int CalculateDamageThatWillNotCrit(IRandomDieRoll randomDieRoll)
        {
            var damage = _playerCharacter.StaticDamageThatWillNotCrit;

            for (var i = 0; i < _playerCharacter.RandomDamageDieCountThatWillNotCrit; i++)
                damage += randomDieRoll.GetNextNumber(_playerCharacter.RandomDamageDieSizeThatWillNotCrit);

            return damage;
        }

        private bool ThisAttackIsACrit(int attackRoll, int possibleConfirmation)
        {
            return ThisAttackIsAHit(attackRoll)
                   && attackRoll >= _playerCharacter.MinRollForThreat
                   && ThisAttackIsAHit(possibleConfirmation);
        }

        private int CalculateDamageForACrit(IRandomDieRoll randomDieRoll)
        {
            var damage = CalculateDamageThatWillNotCrit(randomDieRoll);

            for (var i = 0; i < _playerCharacter.CriticalMultiplier; i++)
                damage += CalculateDamageThatMayCrit(randomDieRoll);

            return damage;
        }
    }
}
