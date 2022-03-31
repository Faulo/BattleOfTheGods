using System;
using System.Collections.Generic;
using Runtime.Entities;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Runtime.Cards {
    public class CardInstance : MonoBehaviour, IPointerClickHandler {

        public static event Action<CardInstance> clicked;

        public CardData data;
        public int cost => data.cost;
        public List<CardEffect> effects => new List<CardEffect>(data.effects);
        public List<PlayCondition> playConditions => new List<PlayCondition>(data.conditions);
        public EntityData assignedEntity => data.assignedEntity;
        public string body => data.cardBody;

        public void Init(CardData data) {
            this.data = data;
        }

        public void OnPointerClick(PointerEventData eventData) {
            clicked?.Invoke(this);
        }
    }
}