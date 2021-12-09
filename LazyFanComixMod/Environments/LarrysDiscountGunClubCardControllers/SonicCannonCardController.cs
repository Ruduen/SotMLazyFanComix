using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.LarrysDiscountGunClub
{
    public class SonicCannonCardController : CardController
    {
        public SonicCannonCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, StartOfTurnDamageTrigger, new TriggerType[] { TriggerType.DealDamage, TriggerType.DestroyCard });
        }
        private IEnumerator StartOfTurnDamageTrigger(PhaseChangeAction arg)
        {
            IEnumerator coroutine;

            Func<DealDamageAction, int> AmmoNextToThisDamage = (DealDamageAction dda) => this.Card.NextToLocation.Cards.Where((Card c) => c.IsAmmo).Count() * 3;

            // Trigger to increase damage by 3 for each ammo next to this.
            ITrigger tempIncrease = this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.CardSource.CardController == this, AmmoNextToThisDamage);

            coroutine = this.DealDamageToHighestHP(null, 1, (Card c) => c.IsHero, (Card c) => 4, DamageType.Sonic, damageSourceInfo: new TargetInfo(HighestLowestHP.HighestHP, 1, 1, new LinqCardCriteria((Card c) => c.IsVillainCharacterCard, "the villain character card with the highest HP")));
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            this.RemoveTrigger(tempIncrease);

            coroutine = this.GameController.DestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c)=>c.IsAmmo && c.Location == this.Card.NextToLocation), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}