using ProductOwnerSimGame.Models;
using System.Collections.Generic;

namespace ProductOwnerSimGame.Dtos
{
    public class GameResultDto
    {
        public List<DecisionImpactDto> DecisionImpacts { get; set; } = new List<DecisionImpactDto>();
    }
}
