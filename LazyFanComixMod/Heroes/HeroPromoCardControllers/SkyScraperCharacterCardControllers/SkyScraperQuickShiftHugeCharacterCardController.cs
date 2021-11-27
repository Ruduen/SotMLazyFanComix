using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.SkyScraper;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.SkyScraper
{
    public class SkyScraperQuickShiftHugeCharacterCardController : SkyScraperQuickShiftSharedCharacterCardController
    {
        public SkyScraperQuickShiftHugeCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override string cardKeyword()
        {
            return "huge";
        }

        protected override string charKeyword()
        {
            return "Huge";
        }
    }
}