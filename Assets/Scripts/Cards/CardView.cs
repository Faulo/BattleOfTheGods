using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Runtime.Cards {
    public class CardView : MonoBehaviour {

        [SerializeField] TextMeshProUGUI cost, body, header;
        [SerializeField] Image image;
        public CardInstance instance { get; private set; }
        public void Init(CardInstance instance) {
            this.instance = instance;
            this.header.text = instance.data.name;
            this.body.text = instance.body;
            this.cost.text = instance.cost.ToString();
            this.gameObject.name = instance.data.name;
            this.image.sprite = instance.data.sprite;
            UpdateVisibilityByParent();
        }

        private void OnTransformParentChanged() {
            UpdateVisibilityByParent();
        }

        private void UpdateVisibilityByParent() {
            if (this.transform.parent.gameObject.TryGetComponent<HandView>(out HandView view))
                gameObject.SetActive(true);
            else
                gameObject.SetActive(false);
        }
    }
}