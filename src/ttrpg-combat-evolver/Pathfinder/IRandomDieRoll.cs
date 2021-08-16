using System;

namespace Pathfinder
{
    public interface IRandomDieRoll
    {
        int GetNextNumber(int max);
    }

    public class RandomDieRoll : IRandomDieRoll
    {
        private readonly Random _rng = new Random();

        public int GetNextNumber(int max)
        {
            return _rng.Next(1, max);
        }
    }
}
