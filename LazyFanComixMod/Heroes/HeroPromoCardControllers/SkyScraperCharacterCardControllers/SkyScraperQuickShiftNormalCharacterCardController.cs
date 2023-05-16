using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.SkyScraper
{
    public class SkyScraperQuickShiftNormalCharacterCardController : SkyScraperQuickShiftSharedCharacterCardController
    {
        public SkyScraperQuickShiftNormalCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override string cardKeyword()
        {
            return "normal";
        }

        protected override string charKeyword()
        {
            return "Normal";
        }
    }
}