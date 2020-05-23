using Microsoft.OpenApi.Extensions;
using Newtonsoft.Json;
using ProductOwnerSimGame.Models;

namespace ProductOwnerSimGame.Dtos
{

    public class DecisionImpactDto
    {
        [JsonIgnore]
        public EffectCategory ImpactCategoryEnum { get; set; }

        public string ImpactCategory => ImpactCategoryEnum.GetDisplayName();

        public ImpactDto Impact { get; set; }
    }
}
