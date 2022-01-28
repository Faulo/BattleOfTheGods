using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Runtime.Cards;
using System;
using Slothsoft.UnityExtensions;
namespace Runtime {
    public class CardManager : MonoBehaviour
    {

        public List<CardInstance> hand;
        public List<CardInstance> deck;
        public List<CardInstance> grave;

        [SerializeField] Transform deckParent, handParent, graveParent;
        [SerializeField] CardInstance cardPrefab;
        public void Init(List <CardData> cards) {
            Cleanup();

            hand = new List<CardInstance>();
            deck = new List<CardInstance>();
            grave = new List<CardInstance>();

            foreach(var card in cards) {
                CardInstance instance = Instantiate(cardPrefab, deckParent);
                instance.Init(card);
                if (instance.TryGetComponent<CardView>(out CardView view))
                    view.Init(instance);
            }

            ShuffleDeck();
        }

        private void ShuffleDeck() 
        {
            deck = deck.Shuffle().ToList();
        }

        public void Draw() {
            CardInstance inst = default;
            if (deck.Count > 0) {
                inst = deck[0];
                deck.RemoveAt(0);
                hand.Add(inst);
            }

            UpdateCardParents();
        }

        private void UpdateCardParents() 
        {
            foreach (var ci in deck)
                ci.transform.parent = deckParent;
            foreach (var ci in hand)
                ci.transform.parent = handParent;
            foreach (var ci in grave)
                ci.transform.parent = graveParent;
        }

        private void Cleanup() {
            if (hand != default) {
                foreach (var ci in hand)
                    Destroy(ci.gameObject);
            }
            if (deck != default) {
                foreach (var ci in deck)
                    Destroy(ci.gameObject);
            }
            if (grave != default) {
                foreach (var ci in grave)
                    Destroy(ci.gameObject);
            }
        }
    }
}