using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Runtime.Cards;
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