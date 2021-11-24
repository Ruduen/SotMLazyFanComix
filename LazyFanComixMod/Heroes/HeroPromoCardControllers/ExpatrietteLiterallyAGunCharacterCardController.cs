using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Expatriette
{
    public class ExpatrietteLiterallyAGunCharacterCardController : PromoDefaultCharacterCardController
    {
        public ExpatrietteLiterallyAGunCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<int> numerals = new List<int>()
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 1),
                this.GetPowerNumeral(2, 1),
                this.GetPowerNumeral(3, 1)
            };
            List<MoveCardAction> mcas = new List<MoveCardAction>();
            IEnumerator coroutine;

            coroutine = this.SelectTargetsAndDealMultipleInstancesOfDamage(new List<DealDamageAction>
            {
                new DealDamageAction(this.GetCardSource(), new DamageSource(this.GameController, this.Card), null, numerals[1], DamageType.Projectile),
                new DealDamageAction(this.GetCardSource(), new DamageSource(this.GameController, this.Card), null, numerals[2], DamageType.Projectile)
            }, null, null, numerals[0], numerals[0], false);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.DiscardTopCards(this.DecisionMaker, this.TurnTaker.Deck, 1, mcas, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (mcas.Count > 0 && mcas?.First().CardToMove != null && mcas.First().WasCardMoved && mcas.First().CardToMove.IsAmmo)
            {
                coroutine = this.GameController.PlayCard(this.DecisionMaker, mcas.First().CardToMove, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}