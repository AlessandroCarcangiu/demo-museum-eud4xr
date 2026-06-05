using System.Collections.Generic;

namespace EUD4XR_Chatbot.TaskModelling
{
    [System.Serializable]
    public class SequenceData
    {
        public string name;
        public List<string> sequence;
    }
    
    [System.Serializable]
    public class OrderData
    {
        public string name;
        public List<string> order;
    }

    [System.Serializable]
    public class ChoiceData
    {
        public string name;
        public List<string> choice;
    }

    [System.Serializable]
    public class IterationData
    {
        public string name;
        public Iteration iteration;
    }

    [System.Serializable]
    public class Iteration
    {
        public int n_steps;
        public string expression;
    }

    [System.Serializable]
    public class ConditionalData
    {
        public string name;
        public Conditional conditional;
    }

    [System.Serializable]
    public class Conditional
    {
        public ConditionalBranch @if;
        public ConditionalBranch @else;
    }

    [System.Serializable]
    public class ConditionalBranch
    {
        public string trigger;
        public List<string> @do;
    }
}