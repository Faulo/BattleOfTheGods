using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Runtime.Cards;
using System;
using UnityEngine.InputSystem;

namespace Runtime {
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public static Input input => instance._input;
        private Input _input;

        private void Awake() {
            _input = new Input();
        }

        public enum States { PlayingCardsIdle, PlayingCardsTargeting, PlayingCardsExecuting, EvaluatingTurn }
        public States state { get; private set; }
        void StartGame() {
            StartCoroutine(GameLoop());
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
        }

        IEnumerator PlayCards() {
            while (true) {
                WaitForPlayCard();
                yield return new WaitWhile(() => state == States.PlayingCardsIdle);

                yield return new WaitWhile(() => state == States.PlayingCardsTargeting);

                yield return new WaitWhile(() => state == States.PlayingCardsExecuting);

                if (state != States.PlayingCardsExecuting ||
                    state != States.PlayingCardsExecuting ||
                    state != States.PlayingCardsTargeting)
                    break;
            }
            
        }

        private void WaitForPlayCard() {
            CardInstance.clicked += PlayCard;
            this.state = States.PlayingCardsIdle;
        }

        private void PlayCard(CardInstance obj) {
            CardInstance.clicked -= PlayCard;
            this.state = States.PlayingCardsTargeting;


            
        }
    }
}