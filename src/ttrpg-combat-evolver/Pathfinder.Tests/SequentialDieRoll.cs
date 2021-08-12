namespace Pathfinder.Tests
{
    public class SequentialDieRolls : IRandomDieRoll
    {
        private readonly int[] _rolls;
        private int _index = 0;

        public SequentialDieRolls(params int[] rolls) => _rolls = rolls;

        public int GetNextNumber(int max)
        {
            return _rolls[_index++];
        }
    }
}
