using System;
using System.Collections.Generic;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;

namespace Pathfinder
{
    static class Program
    {
        private static NeatEvolutionAlgorithm<NeatGenome> _ea;

        static void Main(string[] args)
        {
            var experiment = new PowerAttackExperiment();

            _ea = experiment.CreateEvolutionAlgorithm();
            _ea.UpdateEvent += EaOnUpdateEvent;

            _ea.StartContinue();

            Console.ReadLine();
        }

        private static void EaOnUpdateEvent(object? sender, EventArgs e)
        {
            Console.WriteLine($"gen={_ea.CurrentGeneration:N0} bestFitness={_ea.Statistics._maxFitness:N6}");

            // Save the best genome to file
            var doc = NeatGenomeXmlIO.SaveComplete(new List<NeatGenome>() {_ea.CurrentChampGenome}, false);

            doc.Save("power_attack_champion.xml");
        }
    }
}
