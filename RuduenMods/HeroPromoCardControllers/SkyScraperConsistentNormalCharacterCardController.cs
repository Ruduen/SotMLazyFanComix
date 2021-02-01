using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.SkyScraper;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace RuduenWorkshop.SkyScraper
{
    public class SkyScraperConsistentNormalCharacterCardController : SkyScraperCharacterCardController
    {
        public SkyScraperConsistentNormalCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<PlayCardAction> storedResults = new List<PlayCardAction>();
            int powerNumeral = this.GetPowerNumeral(0, 2); // Amount to draw.

            IEnumerator coroutine;

            // Play card.
            coroutine = this.GameController.SelectAndPlayCardFromHand(this.HeroTurnTakerController, false, storedResults: storedResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // TO DO: Size check
        }
    }
}