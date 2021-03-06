using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.Cards;
using TMPro;
using UnityEngine;
namespace Runtime {
    public class GameManager : MonoBehaviour {
        public static GameManager instance;

        public static event Action playPhaseStarted;
        public static event Action playPhaseEnded;

        public static event Action targetingPhaseStarted;
        public static event Action targetingPhaseEnded;

        public delegate void OnGameOver(Faction winner);

        CardInstance currentSelectedCard;
        ICell currentClickedCell;
        public string noCellReason { get; private set; }
        TextMeshProUGUI debugText => UIController.debugText;
        TextMeshProUGUI log => UIController.log;
        [SerializeField] Config config;
        public IEnumerable<ICell> currentLegalCells;
        public Player player { get; private set; }
        public Player opponent { get; private set; }
        public WaveManager waveManager { get; private set; }
        void Awake() {
            instance = this;
            Config.current = config;
        }
        void Update() {
            if (debugText != default) {
                debugText.text = state.ToString();
            }
        }

        void Start() {
            StartGame();
        }

        public enum States {
            PlayingCardsIdle,
            PlayingCardsTargeting,
            PlayingCardsExecuting,
            EvaluatingTurn,
            EndingPlayCard,
            EndingTurn
        }
        public States state { get; private set; }
        void StartGame() {
            player = FindObjectOfType<PlayerController>().GetComponent<Player>();
            waveManager = FindObjectOfType<WaveManager>();
            player.maxEnergy = Config.current.defaultEnergy;
            if (waveManager != null) //very good code. It's true.
{
                opponent = waveManager.GetComponent<Player>();
            }

            if (player == null) {
                Debug.LogError("did not find player");
            }

            StartCoroutine(GameLoop());
        }

        public void SetTurnEvaluate() {
            state = States.EvaluatingTurn;
        }

        IEnumerator GameLoop() {
            if (player.deck != null &&
                player.deck.Count > 0) {
                CardManager.instance.Init(player.deck);
            } else {
                CardManager.instance.Init(Config.current.defaultDeck);
            }

            for (int i = 0; i < Config.current.openingHandSize; i++) {
                CardManager.instance.Draw();
            }
            Faction winner;
            int turnsWithoutCards = 0;
            while (true) {
                if (waveManager != null &&
                    opponent != null) {
                    log.text += "Opponent Action \n";
                    yield return PlayOpponentCards();
                }

                playPhaseStarted?.Invoke();

                player.maxEnergy += Config.current.energyIncreasePerTurn;
                player.maxEnergy = Math.Min(Config.current.maxEnergy, player.maxEnergy);
                player.energy = player.maxEnergy;
                CardManager.instance.Draw();
                log.text += "Start turn \n";
                yield return PlayCards();

                playPhaseEnded?.Invoke();
                log.text += "Start simulation \n";
                yield return EvaluateTurn();
                log.text += "Check win \n";
                if (CheckWin(out winner) ||
                    turnsWithoutCards >= 3) {
                    break;
                }
                log.text += "End turn \n";

                if (CardManager.instance.hand.Count == 0 &&
                    CardManager.instance.deck.Count == 0) {
                    turnsWithoutCards++;
                }
            }

            yield return EndGame(winner);
        }

        IEnumerator EndGame(Faction winner) {
            string t = $"";
            if (winner != Faction.Nobody) {
                t = $"Game Over the winner is {winner}";
            } else {
                t = $"No cards left but no win (win within 3 turns after playing last card). Equilibrium or apocalypse?";
            }
            Debug.Log(t);
            log.text += t;

            UIController.gameOverMessage.text = t;
            UIController.gameOverPanel.SetActive(true);
            BroadcastMessage(nameof(OnGameOver), winner, SendMessageOptions.DontRequireReceiver);
            yield return null;
        }

        IEnumerator PlayOpponentCards() {
            if (waveManager.currentIndex < waveManager.scenario.waves.Length) {

                yield return PlayCurrentWave();
                waveManager.currentIndex++;
            }
        }
        public IEnumerator PlayCurrentWave() {
            yield return null;
            var wave = waveManager.scenario.waves[waveManager.currentIndex];
            foreach (var tuple in wave.cardsWithTarget) {
                var card = CardManager.instance.InstantiateCard(tuple.card);
                log.text += $"opponent plays {card.data.name} at {tuple.target} \n";
                card.transform.SetParent(waveManager.cards);
                if (World.instance.TryGetCell(tuple.target, out var cell)) {

                    bool playable = true;

                    foreach (var cond in card.playConditions) {
                        var conditionData = new PlayCondition.PlayConditionData(cell, card);
                        playable = playable && cond.Check(conditionData);
                        //play visuals for condition & yield
                        yield return null;
                    }

                    if (playable) {
                        foreach (var eff in card.effects) {
                            var effectData = new CardEffect.CardEffectData(cell, card);
                            eff.OnPlay(effectData);
                            //play visuals for effect & yield
                            yield return null;
                        }
                    }
                }
            }
        }
        bool CheckWin(out Faction winner) {
            winner = Faction.Nobody;
            var cells = World.instance.cellValues;
            int totalCells = cells.Count();
            var ownCount = new Dictionary<Faction, int>();

            foreach (var cell in cells) {
                if (!ownCount.ContainsKey(cell.owningFaction)) {
                    ownCount.Add(cell.owningFaction, 0);
                }

                ownCount[cell.owningFaction]++;
            }

            //Win if any faction controls all cells!
            foreach (var f in ownCount.Keys) {
                if (f == Faction.Nobody) {
                    continue;
                }

                if (ownCount[f] >= totalCells) {
                    winner = f;
                    return true;
                }
            }

            return false;
        }

        IEnumerator EvaluateTurn() {
            state = States.EvaluatingTurn;
            yield return World.instance.AdvanceSeasonRoutine();
            state = States.EndingTurn;
        }

        IEnumerator PlayCards() {
            while (true) {
                WaitForPlayCard();
                yield return new WaitWhile(() => state == States.PlayingCardsIdle);
                currentLegalCells = new HashSet<ICell>();
                if (currentSelectedCard != null &&
                    CanPlayCard(currentSelectedCard, out currentLegalCells, out string feedback)) {
                    targetingPhaseStarted?.Invoke();

                }


                yield return new WaitWhile(() => state == States.PlayingCardsTargeting);
                targetingPhaseEnded?.Invoke();

                if (currentClickedCell != default &&
                    currentSelectedCard != default) {
                    yield return ExecuteCardRoutine(currentSelectedCard, currentClickedCell, true);
                } else if (currentClickedCell == default &&
                           currentSelectedCard != default) {
                    //feedback for not selecting cell
                    state = States.PlayingCardsIdle;
                }

                yield return new WaitWhile(() => state == States.PlayingCardsExecuting);

                if (state != States.PlayingCardsIdle &&
                    state != States.PlayingCardsExecuting &&
                    state != States.PlayingCardsTargeting) {
                    break;
                }
            }
        }

        IEnumerator ExecuteCardRoutine(CardInstance card, ICell cell, bool useEnergy = true) {
            bool playable = true;

            if (card.cost > player.energy &&
                useEnergy) {
                playable = false;
            }

            foreach (var cond in card.playConditions) {
                var conditionData = new PlayCondition.PlayConditionData(cell, card);
                playable = playable && cond.Check(conditionData);
                //play visuals for condition & yield
                yield return null;
            }

            if (playable) {
                foreach (var eff in card.effects) {
                    var effectData = new CardEffect.CardEffectData(cell, card);
                    eff.OnPlay(effectData);
                    //play visuals for effect & yield
                    yield return null;
                }
                if (useEnergy) {
                    player.energy -= card.cost;
                }

                CardManager.instance.SendToGraveyard(card);
            }
            state = States.PlayingCardsIdle;
        }

        void WaitForPlayCard() {
            CardInstance.clicked += OnCardClick;
            state = States.PlayingCardsIdle;
            currentSelectedCard = default;
        }

        void OnCardClick(CardInstance obj) {
            if (CanPlayCard(obj, out var legalCells, out string feedback)) {
                CardInstance.clicked -= OnCardClick;
                currentSelectedCard = obj;
                WorldInput.onClick += OnCellClick;
                state = States.PlayingCardsTargeting;
            } else {
                log.text += feedback + "\n";
                state = States.PlayingCardsIdle;
            }
        }

        bool CanPlayCard(CardInstance card, out IEnumerable<ICell> legalCells, out string feedback) {
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

        void OnCellClick(Vector3 position) {
            noCellReason = "";
            WorldInput.onClick -= OnCellClick;
            var gridPos = World.instance.WorldToGrid(position);
            if (World.instance.TryGetCell(gridPos, out var cell)) {
                currentClickedCell = cell;
            } else {
                currentClickedCell = default;
                noCellReason = "Did not click on valid cell";
            }
            state = States.PlayingCardsExecuting;
        }


        public enum GameOutcome {
            None,
            NatureWins,
            HumansWin
        }
    }
}