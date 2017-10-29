using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryOptimization.MetaHeuristics
{
    public class LearningClassifier
    {
        protected string mCondition;
        protected int mAction;
        protected int mLastTime;
        public double Num = 1;
        public double SetSize = 1;
        public double Exp = 0.0;
        public double Pred = 10.0;
        public double Error = 0.0;

        protected double mFitness;
        public double Fitness
        {
            get { return mFitness; }
            set { mFitness = value; }
        }

        public int LastTime
        {
            get { return mLastTime; }
            set { mLastTime = value; }
        }

        public LearningClassifier(string condition, int action, int last_time, double pred = 10.0, double fitness=10.0)
        {
            mCondition = condition;
            mAction = action;
            mLastTime = last_time;

            Pred = pred;
            mFitness = fitness;
        }

        public LearningClassifier Clone()
        {
            LearningClassifier clone = new LearningClassifier(mCondition, mAction, mLastTime, Pred, Fitness);
            clone.Error = Error;
            return clone;
        }

        public int Action
        {
            get { return mAction; }
            set { mAction = value; }
        }

        public string Condition
        {
            get { return mCondition; }
            set { mCondition = value; }
        }

        public bool Match(string input)
        {
            for (int i = 0; i < mCondition.Length; ++i)
            {
                if (mCondition[i] != LearningClassifierSystem.MaskChar && input[i] != mCondition[i])
                {
                    return false;
                }
            }
            return true;
        }

    }
}
