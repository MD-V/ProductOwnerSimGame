using ProductOwnerSimGame.Models;
using System.Collections.Generic;

namespace ProductOwnerSimGame.Dtos
{
    public class PhaseResultDto
    {
        public string DecisionMarkdownText { get; set; }

        public string ExplanationMarkdownText { get; set; }

        public List<DecisionImpactDto> DecisionImpacts { get; set; } = new List<DecisionImpactDto>();
    }
}
