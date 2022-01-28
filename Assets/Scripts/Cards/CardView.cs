using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Runtime.Cards {
    public class CardView : MonoBehaviour {

        [SerializeField] TextMeshProUGUI cost, body, header;
        public CardInstance instance { get; private set; }
        public void Init(CardInstance instance) {
            this.instance = instance;
            this.header.text = instance.data.name;
            this.body.text = instance.body;
            this.cost.text = instance.cost.ToString();
        }
    }
}