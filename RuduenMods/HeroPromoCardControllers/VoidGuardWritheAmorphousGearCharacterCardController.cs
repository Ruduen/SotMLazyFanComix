using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;

namespace RuduenWorkshop.VoidGuardWrithe
{
    public class VoidGuardWritheAmorphousGearCharacterCardController : PromoDefaultCharacterCardController
    {
        public VoidGuardWritheAmorphousGearCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;

            // Reveal equipment until one is played.
            coroutine = this.RevealCards_MoveMatching_ReturnNonMatchingCards(this.HeroTurnTakerController, this.HeroTurnTaker.Deck, true, false, false, new LinqCardCriteria((Card c) => this.IsEquipment(c), "equipment"), 1, showMessage: true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}