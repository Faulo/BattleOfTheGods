using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Cards {
    public abstract class PlayCondition : ScriptableObject 
    {
        public abstract bool Check(PlayConditionData data);

        public class PlayConditionData
        {
            public ICell target;

            private PlayConditionData() { }

            public PlayConditionData (ICell target) 
            {
                this.target = target;
            }
        }
    }
}