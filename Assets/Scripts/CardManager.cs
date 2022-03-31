using System.Collections.Generic;
using System.Linq;
using Runtime.Cards;
using Slothsoft.UnityExtensions;
using UnityEngine;
namespace Runtime {
    public class CardManager : MonoBehaviour {
        public static CardManager instance;
        public static Transform waveParent => instance._waveParent;
        [SerializeField] Transform _waveParent;
        void Awake() {
            instance = this;
        }

        public List<CardInstance> hand;
        public List<CardInstance> deck;
        public List<CardInstance> grave;

        [SerializeField] Transform deckParent, handParent, graveParent;
        [SerializeField] CardInstance cardCiv, cardNat;
        public void Init(List<CardData> cards) {
            Cleanup();

            hand = new List<CardInstance>();
            deck = new List<CardInstance>();
            grave = new List<CardInstance>();

            foreach (var card in cards) {
                var instance = InstantiateCard(card);
                if (instance.TryGetComponent<CardView>(out var view)) {
                    view.Init(instance);
                }

                deck.Add(instance);
            }

            ShuffleDeck();
        }

        public CardInstance InstantiateCard(CardData card) {
            CardInstance prefab = default;

            if (card.type == Faction.Civilization) {
                prefab = cardCiv;
            } else if (card.type == Faction.Nature) {
                prefab = cardNat;
            }

            var instance = Instantiate(prefab, deckParent);
            instance.Init(card);
            return instance;
        }

        void ShuffleDeck() {
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
            if (hand.Contains(card)) {
                hand.Remove(card);
            }

            if (deck.Contains(card)) {
                deck.Remove(card);
            }

            UpdateCardParents();
        }

        void UpdateCardParents() {
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

        void Cleanup() {
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