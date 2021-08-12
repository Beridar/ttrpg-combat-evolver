namespace Pathfinder.Tests
{
    public class AlwaysMaxDieRoll : IRandomDieRoll
    {
        public int GetNextNumber(int max)
        {
            return max;
        }
    }
}
