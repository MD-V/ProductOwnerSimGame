using Newtonsoft.Json;
using System;

namespace ProductOwnerSimGame.Dtos.GameView
{
    public class InitialMissionView : ICloneable
    {


        public string OverallMarkDownText { get; set; }

        public object Clone()
        {
            return new InitialMissionView
            {
                OverallMarkDownText = OverallMarkDownText
            };
        }
    }
}
