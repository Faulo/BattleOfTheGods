using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Runtime.Cards;
namespace Runtime {
    public class Player : MonoBehaviour {

        public CardTypes type => _type;
        [SerializeField] CardTypes _type;
        public float energy { get; set; }
        public float maxEnergy { get; set; }

        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }
    }
}