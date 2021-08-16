using System;
using System.Collections.Generic;

namespace Pathfinder
{
    public interface IDamageCalculator
    {
        int Calculate(PlayerCharacter playerCharacter, Monster monster);
    }

    public class DamageCalculator : IDamageCalculator
    {
        private readonly Func<IRandomDieRoll> _randomDieRoll;

        private struct Context
        {
            public PlayerCharacter PlayerCharacter { get; set; }
            public Monster Monster { get; set; }
            public List<int> AttackRolls { get; set; }
            public List<int> DamageRolls { get; set; }
        }

        public DamageCalculator(Func<IRandomDieRoll> randomDieRoll)
        {
            _randomDieRoll = randomDieRoll;
        }

        public int Calculate(PlayerCharacter playerCharacter, Monster monster)
        {
            var context = new Context
            {
                AttackRolls = new List<int>(),
                DamageRolls = new List<int>(),
                PlayerCharacter = playerCharacter,
                Monster = monster,
            };

            var attackRoll = _randomDieRoll().GetNextNumber(20);
            var confirmation = _randomDieRoll().GetNextNumber(20);

            context.AttackRolls.Add(attackRoll);
            context.AttackRolls.Add(confirmation);

            if (ThisAttackIsACrit(attackRoll, confirmation, context))
                return CalculateDamageForACrit(context);

            if (ThisAttackIsAHit(attackRoll, context))
                return CalculateDamageForAHit(context);

            return 0;
        }

        private bool ThisAttackIsAHit(int attackRoll, Context context)
        {
            switch (attackRoll)
            {
                case 1:
                    return false;
                case 20:
                    return true;
                default:
                    return context.PlayerCharacter.AttackBonusAfterPowerAttack + attackRoll >= context.Monster.ArmorClass;
            }
        }

        private int CalculateDamageForAHit(Context context)
        {
            return CalculateDamageThatMayCrit(context)
                   + CalculateDamageThatWillNotCrit(context);
        }

        private int CalculateDamageThatMayCrit(Context context)
        {
            var damage = context.PlayerCharacter.StaticDamageThatCanCritAfterPowerAttack;

            for (var i = 0; i < context.PlayerCharacter.RandomDamageDieCount; i++)
                damage += _randomDieRoll().GetNextNumber(context.PlayerCharacter.RandomDamageDieSize);

            return damage;
        }

        private int CalculateDamageThatWillNotCrit(Context context)
        {
            var damage = context.PlayerCharacter.StaticDamageThatWillNotCrit;

            for (var i = 0; i < context.PlayerCharacter.RandomDamageDieCountThatWillNotCrit; i++)
                damage += _randomDieRoll().GetNextNumber(context.PlayerCharacter.RandomDamageDieSizeThatWillNotCrit);

            return damage;
        }

        private bool ThisAttackIsACrit(int attackRoll, int possibleConfirmation, Context context)
        {
            return ThisAttackIsAHit(attackRoll, context)
                   && attackRoll >= context.PlayerCharacter.MinRollForThreat
                   && ThisAttackIsAHit(possibleConfirmation, context);
        }

        private int CalculateDamageForACrit(Context context)
        {
            var damage = CalculateDamageThatWillNotCrit(context);

            for (var i = 0; i < context.PlayerCharacter.CriticalMultiplier; i++)
                damage += CalculateDamageThatMayCrit(context);

            return damage;
        }
    }
}
