using System.Collections.Generic;
using Runtime.Cards;
using UnityEngine;
namespace Runtime {
    public class Player : MonoBehaviour {

        public Faction type => _type;
        [SerializeField] Faction _type;
        public float energy { get; set; }
        public float maxEnergy { get; set; }
        public List<CardData> deck;

        void Start() {

        }

        void Update() {

        }
    }
}