using System;

namespace ProductOwnerSimGame.Dtos.GameView
{
    public class MissionView : ICloneable
    {
        public string MarkDownText { get; set; }

        public bool MissionStarted { get; set; }

        public object Clone()
        {
            return new MissionView
            {
                MarkDownText = MarkDownText,
                MissionStarted = MissionStarted
            };
        }
    }
}
