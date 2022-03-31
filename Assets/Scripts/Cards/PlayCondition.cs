using UnityEngine;

namespace Runtime.Cards {
    public abstract class PlayCondition : ScriptableObject {
        public abstract bool Check(PlayConditionData data);

        public class PlayConditionData {
            public ICell cell { get; private set; }
            public CardInstance card { get; private set; }

            PlayConditionData() { }

            public PlayConditionData(ICell target, CardInstance card) {
                cell = target;
                this.card = card;
            }
        }
    }
}