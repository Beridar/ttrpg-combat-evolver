using System;
using Polyhedral.Neat;
using SharpNeat.Core;
using SharpNeat.Phenomes;

namespace Pathfinder
{
    public class PowerAttackExperiment : SimpleNeatExperiment
    {
        public PowerAttackExperiment()
        {
            PhenomeEvaluator = new PowerAttackEvaluator();
        }

        public override IPhenomeEvaluator<IBlackBox> PhenomeEvaluator { get; }
        public override int InputCount => 12;
        public override int OutputCount => 1;
        public override bool EvaluateParents => false;
    }

    public class PowerAttackEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        private PlayerCharacter _playerCharacter;
        private Monster _monster;
        private DamageCalculator _damageCalculator;

        public PowerAttackEvaluator()
        {
            Reset();
        }

        public FitnessInfo Evaluate(IBlackBox phenome)
        {
            var inputs = new Func<double>[]
            {
                () => _monster.ArmorClass,
                () => _playerCharacter.AttackBonus,
                () => _playerCharacter.CriticalMultiplier,
                () => _playerCharacter.MinRollForThreat,
                () => _playerCharacter.RandomDamageDieCount,
                () => _playerCharacter.RandomDamageDieSize,
                () => _playerCharacter.PowerAttackBonusToDamage,
                () => _playerCharacter.PowerAttackPenaltyToAttack,
                () => _playerCharacter.StaticDamageThatCanCrit,
                () => _playerCharacter.StaticDamageThatWillNotCrit,
                () => _playerCharacter.RandomDamageDieCountThatWillNotCrit,
                () => _playerCharacter.RandomDamageDieSizeThatWillNotCrit,
            };

            for (int i = 0; i < phenome.InputCount && i < inputs.Length; i++)
                phenome.InputSignalArray[i] = inputs[i]();

            phenome.Activate();
            EvaluationCount++;

            var shouldPowerAttack = phenome.OutputSignalArray[0] > 0.75;

            _playerCharacter.IsPowerAttacking = shouldPowerAttack;

            var damage = _damageCalculator.Calculate(_playerCharacter, _monster);

            return new FitnessInfo(damage, damage);
        }

        public void Reset()
        {
            _playerCharacter = new PlayerCharacter
            {
                AttackBonus = 10,
                CriticalMultiplier = 2,
                MinRollForThreat = 19,
                RandomDamageDieCount = 1,
                RandomDamageDieSize = 8,
                PowerAttackBonusToDamage = 6,
                PowerAttackPenaltyToAttack = 2,
                StaticDamageThatCanCrit = 7
            };

            _monster = new Monster
            {
                ArmorClass = 15
            };

            _damageCalculator = new DamageCalculator(() => new RandomDieRoll());
        }

        public ulong EvaluationCount { get; private set; }

        public bool StopConditionSatisfied => EvaluationCount >= 1000000;
    }
}
