using System;
using System.Collections.Generic;

namespace ProductOwnerSimGame.Dtos.GameView
{
    public class GameFinishedView : ICloneable
    {
        public List<PhaseResultDto> PhaseResults { get; set; } = new List<PhaseResultDto>();

        public GameResultDto GameResult { get; set; }

        public object Clone()
        {
            return new GameFinishedView()
            {
                GameResult = GameResult,
                PhaseResults = PhaseResults
            };
        }
    }
}
