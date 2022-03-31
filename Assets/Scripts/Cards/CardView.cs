using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Cards {
    public class CardView : MonoBehaviour {

        [SerializeField] TextMeshProUGUI cost, body, header;
        [SerializeField] Image image;
        public CardInstance instance { get; private set; }
        public void Init(CardInstance instance) {
            this.instance = instance;
            header.text = instance.data.name;
            body.text = instance.body;
            cost.text = instance.cost.ToString();
            gameObject.name = instance.data.name;
            image.sprite = instance.data.sprite;
            UpdateVisibilityByParent();
        }

        void OnTransformParentChanged() {
            UpdateVisibilityByParent();
        }

        void UpdateVisibilityByParent() {
            if (transform.parent.gameObject.TryGetComponent<HandView>(out var view)) {
                gameObject.SetActive(true);
            } else {
                gameObject.SetActive(false);
            }
        }
    }
}