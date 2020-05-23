using Newtonsoft.Json;
using System;

namespace ProductOwnerSimGame.Dtos.GameView
{
    public class MissionViewWithRemainingTime : ICloneable
    {
        public string MarkDownText { get; set; }

        public int RemainingTime { get; set; }

        public bool MissionStarted { get; set; }

        public object Clone()
        {
            return new MissionViewWithRemainingTime
            {
                MarkDownText = MarkDownText,
                MissionStarted = MissionStarted,
                RemainingTime = RemainingTime
            };
        }
    }
}
