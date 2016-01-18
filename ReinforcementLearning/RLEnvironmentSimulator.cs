using System;
using System.Collections.Generic;
using System.Text;

namespace ReinforcementLearning
{
    public interface RLEnvironmentSimulator
    {
        void Simulate(double[] states, double[] actions, double dt, out double[] newStates, out double reward);
    }
}
