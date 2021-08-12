namespace Pathfinder.Tests
{
    public class ConsistentDieRoll : IRandomDieRoll
    {
        private readonly int _roll;

        public ConsistentDieRoll(int number) => _roll = number;

        public int GetNextNumber(int max) => _roll;
    }
}
