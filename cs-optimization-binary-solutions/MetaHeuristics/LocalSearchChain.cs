using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryOptimization.MetaHeuristics
{
    public class LocalSearchChain
    {
        protected List<SingleTrajectoryBinarySolver> mLocalSearches=new List<SingleTrajectoryBinarySolver>();
        protected List<SingleTrajectoryBinarySolver.TerminationEvaluationMethod> mTerminationConditions = new List<SingleTrajectoryBinarySolver.TerminationEvaluationMethod>();

        public void Add(SingleTrajectoryBinarySolver local_search, SingleTrajectoryBinarySolver.TerminationEvaluationMethod termination_condition)
        {
            mLocalSearches.Add(local_search);
            mTerminationConditions.Add(termination_condition);
        }

        public int Count
        {
            get { return mLocalSearches.Count; }
        }

        public SingleTrajectoryBinarySolver GetLocalSearchAt(int index)
        {
            return mLocalSearches[index];
        }

        public SingleTrajectoryBinarySolver.TerminationEvaluationMethod GetTerminationConditionAt(int index)
        {
            return mTerminationConditions[index];
        }
    }
}
