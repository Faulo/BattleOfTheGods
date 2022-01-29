using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Runtime.Cards;
using UnityEngine;
namespace Runtime {
    public class Deck {

        public List<CardData> cards;

        public Deck(IEnumerable<CardData> cards) {
            this.cards = cards.ToList();
        }
    }
}
