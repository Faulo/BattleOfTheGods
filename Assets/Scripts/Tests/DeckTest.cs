using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Tests {
    [RequireComponent(typeof(CardManager))]
    public class DeckTest : MonoBehaviour {

        [SerializeField] List<Cards.CardData> deck;
        // Start is called before the first frame update
        void Start() {
            var cm = GetComponent<CardManager>();
            cm.Init(deck);
            for (int i = 0; i < 5; i++) {
                cm.Draw();
            }
        }
    }
}