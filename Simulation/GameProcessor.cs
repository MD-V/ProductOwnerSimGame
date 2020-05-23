using AutoMapper;
using Microsoft.Azure.Cosmos;
using Microsoft.CodeAnalysis.CSharp;
using ProductOwnerSimGame.Dtos;
using ProductOwnerSimGame.Dtos.GameView;
using ProductOwnerSimGame.Models;
using ProductOwnerSimGame.Models.GameVariant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProductOwnerSimGame.Simulation
{
    public class GameProcessor : IGameProcessor
    {
        private Game _Game;
        private GameVariant _GameVariant;
        private Dictionary<string, GameRole> _PlayerDictionary = new Dictionary<string, GameRole>();

        private readonly IMapper _Mapper;

        private GamePhase _CurrentGamePhase;

        private SemaphoreSlim _UserClickedNextSemaphore = new SemaphoreSlim(1, 1);
        private List<string> _UsersClickedNext = new List<string>();

        private AutoResetEvent _WaitForProductOwner = new AutoResetEvent(false);

        private SemaphoreSlim _GameViewDictionarySemaphore = new SemaphoreSlim(1, 1);
        private Dictionary<string, GameView> _GameViewDictionary = new Dictionary<string, GameView>();

        public GameProcessor(Game game, GameVariant gameVariant, Dictionary<string, GameRole> playerDictionary, IMapper mapper)
        {
            _Game = game;
            _GameVariant = gameVariant;
            _PlayerDictionary = playerDictionary;
            _Mapper = mapper;
        }

        public Func<string, Task> GameFinishedAsyncCallback { get; set; }

        public Func<string, string, GameView, Task<bool>> GameViewUpdatedAsyncCallback { get; set; }

        public Func<string, GameStatusEnum, Task<bool>> SaveGameStatusAsyncCallback { get; set; }

        public Func<string, string, int, Task<bool>> SaveGamePhaseAsyncCallback { get; set; }

        public Func<string, Decision, Task<bool>> SaveDecisionAsyncCallback { get; set; }

        #region Game Control

        public async Task<bool> CancelAsync()
        {
            await UpdateGameStateAsync(GameStatusEnum.Cancelled).ConfigureAwait(false);

            return true;
        }

        public async Task StartAsync()
        {
            if (_Game.State == GameStatusEnum.WaitingForPlayers)
            {
                // New game
                await UpdateGameStateAsync(GameStatusEnum.InitialMissionScreen).ConfigureAwait(false);
            }
            else if (_Game.State == GameStatusEnum.Started)
            {
                // Game already started
                var madeDecs = _Game.Decisions.Count;

                if (madeDecs > 0)
                {
                    _CurrentGamePhase = _GameVariant.GamePhases.OrderBy(a => a.Sequence).Skip(madeDecs).FirstOrDefault();
                }
            }

            // Move to next phase
            MoveToNextPhaseAsync().ConfigureAwait(false);
        }

        public async Task<bool> SubmitDecisionAsync(string decisionId, string userId)
        {
            // Check if user is eligible to submit decs
            if (_PlayerDictionary.TryGetValue(userId, out var gameRole) && gameRole == GameRole.ProductOwner)
            {
                var dec = _CurrentGamePhase.Decisions.Where(a => a.DecisionId.Equals(decisionId)).FirstOrDefault();

                if (dec == null)
                {
                    return false;
                }

                _Game.Decisions.Add(dec);
                if (SaveDecisionAsyncCallback != null)
                {
                    await SaveDecisionAsyncCallback(_Game.GameId, dec).ConfigureAwait(false);
                }

                // Move to next phase
                MoveToNextPhaseAsync().ConfigureAwait(false);

                return true;
            }

            return false;
        }

        public async Task<bool> StartPhaseClickedAsync(string userId)
        {
            await _UserClickedNextSemaphore.WaitAsync();
            try
            {
                if (_Game.PlayerIds.Contains(userId) && !_UsersClickedNext.Contains(userId))
                {
                    _UsersClickedNext.Add(userId);

                    return true;
                }
            }
            finally
            {
                _UserClickedNextSemaphore.Release();
            }

            return false;
        }

        public Task<bool> PhaseDoneClickedAsync(string userId)
        {
            if (_CurrentGamePhase.Duration == PhaseDuration.Infinite)
            {
                _WaitForProductOwner.Set();

                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }


        private async Task MoveToNextPhaseAsync()
        {
            if (_Game.State == GameStatusEnum.Cancelled || _Game.State == GameStatusEnum.Finished)
            {
                return;
            }
            else if (_Game.State == GameStatusEnum.InitialMissionScreen)
            {
                // Show initial mission (initial wait screen)
                var gameView = new GameView()
                {
                    InitialMissionView = new InitialMissionView()
                    {
                        OverallMarkDownText = _GameVariant.OverallMarkDownText
                    },
                };
                await SendGameViewUpdateAsync(_PlayerDictionary.Keys, gameView).ConfigureAwait(false);

                // Wait for start all players
                await WaitForStartAsync().ConfigureAwait(false);

                // Start game
                await UpdateGameStateAsync(GameStatusEnum.Started).ConfigureAwait(false);

                // Move immediately to first phase
                MoveToNextPhaseAsync().ConfigureAwait(false);
            }
            else if (_Game.State == GameStatusEnum.Started)
            {
                if (_CurrentGamePhase != null)
                {
                    // Find next phase
                    var nextGamePhase = _GameVariant.GamePhases.Where(a => a.Sequence > _CurrentGamePhase.Sequence).OrderBy(a => a.Sequence).FirstOrDefault();

                    if (nextGamePhase != null)
                    {
                        // Next phase
                        _CurrentGamePhase = nextGamePhase;
                    }
                    else
                    {
                        // If no next phase available signal finish
                        await UpdateGameStateAsync(GameStatusEnum.Finished).ConfigureAwait(false);

                        // Build GameFinishedView
                        var gameView = new GameView()
                        {
                            GameFinishedView = BuildGameFinishedView(_Game)
                        };

                        await SendGameViewUpdateAsync(_PlayerDictionary.Keys, gameView).ConfigureAwait(false);

                        if (GameFinishedAsyncCallback != null)
                        {
                            await GameFinishedAsyncCallback(_Game.GameId).ConfigureAwait(false);
                        }
                        return;
                    }
                }
                else
                {
                    // First phase
                    _CurrentGamePhase = _GameVariant.GamePhases.OrderBy(a => a.Sequence).FirstOrDefault();
                }

                // Save phase move to database
                if (SaveGamePhaseAsyncCallback != null)
                {
                    await SaveGamePhaseAsyncCallback(_Game.GameId, _CurrentGamePhase.GamePhaseId, _CurrentGamePhase.Sequence).ConfigureAwait(false);
                }

                //Show missions to players
                await ShowMissionViewToPlayersAsync(false).ConfigureAwait(false);

                // Wait for start all players
                await WaitForStartAsync().ConfigureAwait(false);

                // Enable submission button for product owner
                await ShowMissionViewToPlayersAsync(true).ConfigureAwait(false);

                if (_CurrentGamePhase.Duration == PhaseDuration.Infinite)
                {
                    // Wait for decision list switch from po
                    await WaitForProductOwnerAsync().ConfigureAwait(false);
                }
                else if (_CurrentGamePhase.Duration == PhaseDuration.Timeout)
                {
                    // Show wait screen for everyone
                    var updateRateSeconds = 5;

                    var seconds = _CurrentGamePhase.DurationInSeconds;

                    while (seconds >= 0)
                    {
                        //Update wait screen for everyone
                        await ShowMissionViewToPlayersAsync(true, seconds).ConfigureAwait(false);

                        if(seconds == 0)
                        {
                            break;
                        }

                        await Task.Delay(TimeSpan.FromSeconds(updateRateSeconds)).ConfigureAwait(false);
                        seconds -= updateRateSeconds;
                    }
                }

                //Show decs list to po
                await ShowDecisionsViewToProductOwnerAsync().ConfigureAwait(false);
            }
        }

        private async Task ShowMissionViewToPlayersAsync(bool hasStarted, int? remainingTime = null)
        {
            // Product Owner
            var poMissionView = new GameView();

            var poText = _CurrentGamePhase.ProductOwnerMission.MarkdownText;

            if (!remainingTime.HasValue)
            {
                poMissionView.MissionView = new MissionView()
                {
                    MarkDownText = poText,
                    MissionStarted = hasStarted
                };
            }
            else
            {
                poMissionView.MissionViewWithRemainingTime = new MissionViewWithRemainingTime()
                {
                    MarkDownText = poText,
                    RemainingTime = remainingTime.Value,
                    MissionStarted = hasStarted
                };
            }

            await SendGameViewUpdateAsync(_PlayerDictionary.Where(a => a.Value == GameRole.ProductOwner).Select(a => a.Key), poMissionView).ConfigureAwait(false);

            // Stakeholder
            var stakeholderMissions = new Dictionary<string, Mission>();

            var stakeholderList = _PlayerDictionary.Where(a => a.Value == GameRole.Stakeholder).Select(a => a.Key).ToList();

            if (stakeholderList.Count == _CurrentGamePhase.StakeholderMissions.Count)
            {
                for (int i = 0; i < stakeholderList.Count; i++)
                {
                    var stakeHolderId = stakeholderList[i];
                    var mission = _CurrentGamePhase.StakeholderMissions[i];

                    stakeholderMissions[stakeHolderId] = mission;
                }
            }
            else if (stakeholderList.Count > _CurrentGamePhase.StakeholderMissions.Count)
            {
                for (int i = 0; i < stakeholderList.Count; i++)
                {
                    var stakeHolderId = stakeholderList[i];

                    Mission mission;

                    if (_CurrentGamePhase.StakeholderMissions.Count < i + 1)
                    {
                        mission = _CurrentGamePhase.StakeholderMissions.Last();
                    }
                    else
                    {
                        mission = _CurrentGamePhase.StakeholderMissions[i];
                    }

                    stakeholderMissions[stakeHolderId] = mission;
                }
            }

            foreach (var keyValuePair in stakeholderMissions)
            {
                var userId = keyValuePair.Key;
                var mission = keyValuePair.Value;

                var shMissionView = new GameView();

                var shText = mission.MarkdownText;

                if (!remainingTime.HasValue)
                {
                    shMissionView.MissionView = new MissionView()
                    {
                        MarkDownText = shText,
                        MissionStarted = hasStarted
                    };
                }
                else
                {
                    shMissionView.MissionViewWithRemainingTime = new MissionViewWithRemainingTime()
                    {
                        MarkDownText = shText,
                        RemainingTime = remainingTime.Value,
                        MissionStarted = hasStarted
                    };
                }

                await SendGameViewUpdateAsync(new[] { userId }, shMissionView).ConfigureAwait(false);
            }
        }

        private async Task ShowDecisionsViewToProductOwnerAsync()
        {
            var po = _PlayerDictionary.Where(a => a.Value == GameRole.ProductOwner).Select(a => a.Key).FirstOrDefault();

            if (po != null)
            {
                var gameView = new GameView()
                {
                    DecisionView = new DecisionView()
                    {
                        Decisions = _Mapper.Map<List<DecisionDto>>(_CurrentGamePhase.Decisions)
                    }
                };

                await SendGameViewUpdateAsync(new[] { po }, gameView).ConfigureAwait(false);
            }
        }

        private async Task WaitForStartAsync()
        {
            var waitScreen = true;

            while (waitScreen)
            {
                await _UserClickedNextSemaphore.WaitAsync();
                try
                {
                    if (_UsersClickedNext.Count == _PlayerDictionary.Count)
                    {
                        waitScreen = false;
                        _UsersClickedNext.Clear();
                    }
                }
                finally
                {
                    _UserClickedNextSemaphore.Release();
                }

                if (waitScreen)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(400)).ConfigureAwait(false);
                }
            }
        }

        private Task WaitForProductOwnerAsync()
        {
            _WaitForProductOwner.WaitOne();
            _WaitForProductOwner.Reset();

            return Task.CompletedTask;
        }

        #endregion


        #region Game updates
        private async Task UpdateGameStateAsync(GameStatusEnum newState)
        {
            _Game.State = newState;

            // Save state in DB
            if (SaveGameStatusAsyncCallback != null)
            {
                await SaveGameStatusAsyncCallback(_Game.GameId, newState).ConfigureAwait(false);
            }
        }

        private async Task SendGameViewUpdateAsync(IEnumerable<string> userIds, GameView gameView)
        {
            await _GameViewDictionarySemaphore.WaitAsync();
            try
            {
                foreach (var userId in userIds)
                {
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var clonedGameView = gameView.Clone() as GameView;

                        if (_PlayerDictionary.TryGetValue(userId, out var role))
                        {
                            clonedGameView.GameRole = role;

                            _GameViewDictionary[userId] = clonedGameView;

                            if (GameViewUpdatedAsyncCallback != null)
                            {
                                await GameViewUpdatedAsyncCallback(_Game.GameId, userId, clonedGameView).ConfigureAwait(false);
                            }
                        }
                    }
                }
            }
            finally
            {
                _GameViewDictionarySemaphore.Release();
            }
        }

        public async Task<GameView> GetCurrentGameViewAsync(string userId)
        {
            await _GameViewDictionarySemaphore.WaitAsync();
            try
            {
                if (_GameViewDictionary.TryGetValue(userId, out var gameView))
                {
                    return gameView;
                }
            }
            finally
            {
                _GameViewDictionarySemaphore.Release();
            }

            return null;
        }
        #endregion

        #region GameView

        internal static GameFinishedView BuildGameFinishedView(Game game)
        {
            var gameFinishedView = new GameFinishedView();

            var currentValues = new Dictionary<EffectCategory, int>();

            foreach (var initialValue in game.InitialValues)
            {
                currentValues[initialValue.Key] = initialValue.Value;
            }

            foreach (var madeDecision in game.Decisions)
            {
                var phaseResult = new PhaseResultDto()
                {
                    DecisionMarkdownText = madeDecision.DecisionMarkdownText,
                    ExplanationMarkdownText = madeDecision.ExplanationMarkdownText,
                };


                var newValues = new Dictionary<EffectCategory, int>();

                foreach (var currentValue in currentValues)
                {
                    var category = currentValue.Key;
                    var value = currentValue.Value;

                    var effects = madeDecision.Effects.Where(a => a.EffectCategory == category);

                    var decisionImpact = new DecisionImpactDto()
                    {
                        Impact = new ImpactDto()
                        {
                            OldValue = value,
                            NewValue = value
                        },
                        ImpactCategoryEnum = category
                    };

                    if(effects.Any())
                    {
                        foreach (var effect in effects)
                        {
                            decisionImpact.Impact.Change += effect.Value;
                            decisionImpact.Impact.NewValue += effect.Value;

                            newValues[category] = decisionImpact.Impact.NewValue;
                        }
                    }
                    else
                    {
                        newValues[category] = decisionImpact.Impact.OldValue;
                    }

                    phaseResult.DecisionImpacts.Add(decisionImpact);
                }
                currentValues = newValues;

                gameFinishedView.PhaseResults.Add(phaseResult);
            }

            var gameResult = new GameResultDto();

            foreach (var endValue in currentValues)
            {
                var category = endValue.Key;
                var endVal = endValue.Value;

                var initialValue = game.InitialValues[category];

                var decisionImpact = new DecisionImpactDto()
                {
                    Impact = new ImpactDto()
                    {
                        OldValue = initialValue,
                        NewValue = endVal,
                        Change = endVal - initialValue
                    }
                    ,
                    ImpactCategoryEnum = category
                };

                gameResult.DecisionImpacts.Add(decisionImpact);
            }

            gameFinishedView.GameResult = gameResult;

            return gameFinishedView;
        }

        #endregion

    }
}
