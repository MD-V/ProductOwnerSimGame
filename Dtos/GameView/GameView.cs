using ProductOwnerSimGame.Models;
using System;

namespace ProductOwnerSimGame.Dtos.GameView
{
    public class GameView : ICloneable
    {
        public DecisionView DecisionView { get; set; }

        public GameFinishedView GameFinishedView { get; set; }

        public InitialMissionView InitialMissionView { get; set; }

        public MissionView MissionView { get; set; }

        public MissionViewWithRemainingTime MissionViewWithRemainingTime { get; set; }

        public GameRole GameRole { get; set; }

        public object Clone()
        {
            return new GameView()
            {
                GameRole = GameRole,
                DecisionView = DecisionView?.Clone() as DecisionView,
                GameFinishedView = GameFinishedView?.Clone() as GameFinishedView,
                InitialMissionView = InitialMissionView?.Clone() as InitialMissionView,
                MissionView = MissionView?.Clone() as MissionView,
                MissionViewWithRemainingTime = MissionViewWithRemainingTime?.Clone() as MissionViewWithRemainingTime,
            };
        }
    }
}
