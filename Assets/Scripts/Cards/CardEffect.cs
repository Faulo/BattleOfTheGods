using UnityEngine;

namespace Runtime.Cards {
    public abstract class CardEffect : ScriptableObject {
        public abstract void OnPlay(CardEffectData data);

        public class CardEffectData {
            public ICell cell { get; private set; }
            public CardInstance card { get; private set; }

            public CardEffectData(ICell targetCell, CardInstance card) {
                cell = targetCell;
                this.card = card;
            }
        }
    }
}