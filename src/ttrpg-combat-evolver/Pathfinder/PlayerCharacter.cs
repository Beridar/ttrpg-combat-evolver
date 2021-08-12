using System;

namespace Pathfinder
{
    public class PlayerCharacter
    {
        public int AttackBonus { get; set; }
        public int PowerAttackPenaltyToAttack { get; set; }
        public int StaticDamageThatCanCrit { get; set; }
        public int PowerAttackBonusToDamage { get; set; }
        public int StaticDamageThatWillNotCrit { get; set; }
        public int RandomDamageDieSize { get; set; }
        public int RandomDamageDieCount { get; set; }
        public int RandomDamageDieSizeThatWillNotCrit { get; set; }
        public int RandomDamageDieCountThatWillNotCrit { get; set; }
        public int MinRollForThreat { get; set; }
        public int CriticalMultiplier { get; set; }
        public bool IsPowerAttacking { get; set; }

        public int AttackBonusAfterPowerAttack => IsPowerAttacking
            ? AttackBonus - PowerAttackPenaltyToAttack
            : AttackBonus;

        public int StaticDamageThatCanCritAfterPowerAttack => IsPowerAttacking
            ? StaticDamageThatCanCrit + PowerAttackBonusToDamage
            : StaticDamageThatCanCrit;
    }
}
