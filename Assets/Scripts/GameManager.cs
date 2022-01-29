using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Runtime.Cards;
using System;
using UnityEngine.InputSystem;
using TMPro;

namespace Runtime {
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public static Input input => instance._input;
        private Input _input;

        CardInstance currentSelectedCard;
        ICell currentSelectedCell;
        public string noCellReason { get; private set; }
        [SerializeField] TextMeshProUGUI debugText;

        private void Awake() {
            instance = this;
            _input = new Input();
            _input.Enable();
        }
        private void Update() {
            if (debugText != default)
                debugText.text = state.ToString();
        }

        private void Start() {
            StartGame();
        }

        public enum States { 
            PlayingCardsIdle, 
            PlayingCardsTargeting, 
            PlayingCardsExecuting, 
            EvaluatingTurn, 
            EndingPlayCard, 
            EndingTurn }
        public States state { get; private set; }
        void StartGame() {
            StartCoroutine(GameLoop());
        }

        public void EndTurn() 
        {
            state = States.EndingTurn;
        }

        IEnumerator GameLoop() {
            while (true) 
            {
                yield return PlayCards();
                yield return EvaluateTurn();

                if (CheckWin()) {
                    break;
                }
            }
        }

        bool CheckWin() {
            return false;
        }

        IEnumerator EvaluateTurn() {
            this.state = States.EvaluatingTurn;
            yield return null;
            this.state = States.EndingTurn;
        }

        IEnumerator PlayCards() {
            while (true) {
                WaitForPlayCard();
                yield return new WaitWhile(() => state == States.PlayingCardsIdle);
                //do highlighting?
                yield return new WaitWhile(() => state == States.PlayingCardsTargeting);
                if (currentSelectedCell != default &&
                    currentSelectedCard != default) {
                    yield return ExecuteCardRoutine();
                } else if (currentSelectedCell == default) {
                    //feedback for not selecting cell
                    state = States.PlayingCardsIdle;
                }
                yield return new WaitWhile(() => state == States.PlayingCardsExecuting);

                if (state != States.PlayingCardsExecuting ||
                    state != States.PlayingCardsExecuting ||
                    state != States.PlayingCardsTargeting)
                    break;
            }
        }

        IEnumerator ExecuteCardRoutine() {
            //only gets started if both a card and cell exist!
            CardInstance card = currentSelectedCard;
            ICell cell = currentSelectedCell;

            bool playable = true;
            foreach(var cond in card.playConditions) {
                var conditionData = new PlayCondition.PlayConditionData(cell, card);
                playable = playable && cond.Check(conditionData);
                //play visuals for condition & yield
                yield return null;
            }

            if (playable) {
                foreach(var eff in card.effects) {
                    var effectData = new CardEffect.CardEffectData(cell, card);
                    eff.OnPlay(effectData);
                    //play visuals for effect & yield
                    yield return null;
                }

                CardManager.instance.SendToGraveyard(card);
            }

            state = States.PlayingCardsIdle;
        }

        private void WaitForPlayCard() {
            CardInstance.clicked += OnCardPlay;
            this.state = States.PlayingCardsIdle;
            currentSelectedCard = default;
        }

        private void OnCardPlay(CardInstance obj) {
            CardInstance.clicked -= OnCardPlay;
            currentSelectedCard = obj;
            WorldInput.clicked += OnCellSelect;
            this.state = States.PlayingCardsTargeting;
            
        }

        private void OnCellSelect(Vector3 obj) {
            noCellReason = "";
            WorldInput.clicked -= OnCellSelect;
            Vector3Int gridPos = World.instance.WorldToGrid(obj);
            if (World.instance.TryGetCell(gridPos, out ICell cell)) {
                currentSelectedCell = cell;
            } else {
                currentSelectedCell = default;
                this.noCellReason = "Did not click on valid cell";
            }
            this.state = States.PlayingCardsExecuting;
        }
    }
}