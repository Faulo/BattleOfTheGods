using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Cards {
    public abstract class CardEffect : ScriptableObject
    {
        public abstract void OnPlay(CardEffectData data);

        public class CardEffectData 
        {

        }
    }
}