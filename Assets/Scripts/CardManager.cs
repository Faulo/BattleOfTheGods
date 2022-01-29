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
        public static CardManager instance;

        private void Awake() {
            instance = this;
        }

        public List<CardInstance> hand;
        public List<CardInstance> deck;
        public List<CardInstance> grave;

        [SerializeField] Transform deckParent, handParent, graveParent;
        [SerializeField] CardInstance cardCiv, cardNat;
        public void Init(List <CardData> cards) {
            Cleanup();

            hand = new List<CardInstance>();
            deck = new List<CardInstance>();
            grave = new List<CardInstance>();

            foreach(var card in cards) {

                CardInstance prefab = default;

                if (card.type == CardTypes.CIV)
                    prefab = cardCiv;
                else if (card.type == CardTypes.NAT)
                    prefab = cardNat;

                CardInstance instance = Instantiate(prefab, deckParent);
                instance.Init(card);
                if (instance.TryGetComponent<CardView>(out CardView view))
                    view.Init(instance);
                deck.Add(instance);
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

        public void SendToGraveyard(CardInstance card) {
            grave.Add(card);
            if (hand.Contains(card))
                hand.Remove(card);
            if (deck.Contains(card))
                deck.Remove(card);
            UpdateCardParents();
        }

        private void UpdateCardParents() 
        {
            foreach (var ci in deck) {
                ci.transform.SetParent(deckParent, false);
            }
            foreach (var ci in hand) {
                ci.transform.SetParent(handParent, false);
            }
            foreach (var ci in grave) { 
                ci.transform.SetParent(graveParent, false); 
            }
        }

        private void Cleanup() {
            if (hand != default) {
                foreach (var ci in hand) {
                    Destroy(ci.gameObject);
                }
            }
            if (deck != default) {
                foreach (var ci in deck) {
                    Destroy(ci.gameObject);
                }
            }
            if (grave != default) {
                foreach (var ci in grave) {
                    Destroy(ci.gameObject);
                }
            }
        }
    }
}