using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Runtime.Cards;
using System;
using UnityEngine.InputSystem;
using TMPro;
using System.Linq;
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
        [SerializeField] TextMeshProUGUI log;
        [SerializeField] Config config;

        public Player player { get; private set; }
        
        private void Awake() {
            instance = this;
            _input = new Input();
            _input.Enable();
            Config.current = config;
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
            player = FindObjectOfType<Player>();

            if (player == null)
                Debug.LogError("did not find player");
            StartCoroutine(GameLoop());
        }

        public void SetTurnEvaluate() 
        {
            state = States.EvaluatingTurn;
        }

        IEnumerator GameLoop() {
            while (true) 
            {
                player.maxEnergy += Config.current.energyIncreasePerTurn;
                player.maxEnergy = Math.Min(Config.current.maxEnergy, player.maxEnergy);
                player.energy = player.maxEnergy;
                log.text += "Start turn";
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
            yield return World.instance.AdvanceSeasonRoutine();
            this.state = States.EndingTurn;
        }

        IEnumerator PlayCards() {
            while (true) {
                WaitForPlayCard();
                yield return new WaitWhile(() => state == States.PlayingCardsIdle);
                //do highlighting?
                yield return new WaitWhile(() => state == States.PlayingCardsTargeting);

                if (currentSelectedCell != default &&
                    currentSelectedCard != default) 
                {
                    yield return ExecuteCardRoutine();
                } 
                else if (currentSelectedCell == default &&
                         currentSelectedCard != default) 
                {
                    //feedback for not selecting cell
                    state = States.PlayingCardsIdle;
                }
                
                yield return new WaitWhile(() => state == States.PlayingCardsExecuting);

                if (state != States.PlayingCardsIdle &&
                    state != States.PlayingCardsExecuting &&
                    state != States.PlayingCardsTargeting)
                    break;
            }
        }

        IEnumerator ExecuteCardRoutine() {
            //only gets started if both a card and cell exist!
            CardInstance card = currentSelectedCard;
            ICell cell = currentSelectedCell;

            bool playable = true;

            if (card.cost > player.energy)
                playable = false;

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
                player.energy -= card.cost;
                CardManager.instance.SendToGraveyard(card);
            }

            state = States.PlayingCardsIdle;
        }

        private void WaitForPlayCard() {
            CardInstance.clicked += OnCardClick;
            this.state = States.PlayingCardsIdle;
            currentSelectedCard = default;
        }

        private void OnCardClick(CardInstance obj) {

            if (CanPlayCard(obj, out IEnumerable<WorldCell> legalCells, out string feedback)) {
                CardInstance.clicked -= OnCardClick;
                currentSelectedCard = obj;
                WorldInput.clicked += OnCellSelect;
                this.state = States.PlayingCardsTargeting;
            } else {
                log.text += feedback + "\n";
                this.state = States.PlayingCardsIdle;
            }
        }

        bool CanPlayCard(CardInstance card, out IEnumerable<WorldCell> legalCells, out string feedback) {
            legalCells = World.instance.cellValues.Where(c => card.playConditions.All(cond => cond.Check(new PlayCondition.PlayConditionData(c, card))));
            if (player.energy < card.cost) {
                feedback = "Energy low";
                return false;
            }
            if (legalCells.Count() <= 0) {
                feedback = "No legal target";
                return false;
            }

            feedback = $"{card.data.name} is legal cast";
            return true;
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