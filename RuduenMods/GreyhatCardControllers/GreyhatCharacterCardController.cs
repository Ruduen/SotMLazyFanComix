using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;

// Manually tested!

namespace RuduenWorkshop.Greyhat
{
    public class GreyhatCharacterCardController : PromoDefaultCharacterCardController
    {
        public string str;

        public GreyhatCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int numeral = this.GetPowerNumeral(0, 1);
            IEnumerator coroutine;

            coroutine = this.GameController.DrawCards(this.DecisionMaker, numeral, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        // TODO: Replace Incap with something more unique!
    }
}