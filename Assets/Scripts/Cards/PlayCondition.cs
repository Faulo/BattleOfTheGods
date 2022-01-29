using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Cards {
    public abstract class PlayCondition : ScriptableObject 
    {
        public abstract bool Check(PlayConditionData data);

        public class PlayConditionData
        {
            public ICell cell { get; private set; }
            public CardInstance card { get; private set; }

            private PlayConditionData() { }

            public PlayConditionData (ICell target, CardInstance card) 
            {
                this.cell = target;
                this.card = card;
            }
        }
    }
}