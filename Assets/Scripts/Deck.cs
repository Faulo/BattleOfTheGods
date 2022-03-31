using System.Collections.Generic;
using System.Linq;
using Runtime.Cards;
namespace Runtime {
    public class Deck {

        public List<CardData> cards;

        public Deck(IEnumerable<CardData> cards) {
            this.cards = cards.ToList();
        }
    }
}
