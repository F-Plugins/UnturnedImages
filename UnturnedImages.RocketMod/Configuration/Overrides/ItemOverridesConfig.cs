using System.Collections.Generic;

namespace UnturnedImages.Configuration.Overrides
{
    public class ItemOverridesConfig
    {
        public ICollection<OverrideConfig> ItemOverrides { get; set; } = new List<OverrideConfig>();
    }
}
