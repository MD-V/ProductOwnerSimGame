using ProductOwnerSimGame.Dtos;
using System;
using System.Collections.Generic;

namespace ProductOwnerSimGame.Dtos.GameView
{
    public class DecisionView : ICloneable
    {

        public List<DecisionDto> Decisions { get; set; } = new List<DecisionDto>();

        public object Clone()
        {
            return new DecisionView
            {
                Decisions = Decisions
            };
        }
    }
}
