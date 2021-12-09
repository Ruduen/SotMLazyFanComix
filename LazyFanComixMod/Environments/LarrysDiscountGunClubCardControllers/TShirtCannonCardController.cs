using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.LarrysDiscountGunClub
{
    public class TShirtCannonCardController : CardController
    {
        public TShirtCannonCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, StartOfTurnDamageTrigger, new TriggerType[] { TriggerType.DealDamage, TriggerType.DiscardCard, TriggerType.DestroyCard });
        }
        private IEnumerator StartOfTurnDamageTrigger(PhaseChangeAction arg)
        {
            IEnumerator coroutine;

            Func<int> AmmoNextToThis = () => this.Card.NextToLocation.Cards.Where((Card c) => c.IsAmmo).Count();
            List<DealDamageAction> ddas = new List<DealDamageAction>();

            coroutine = this.DealDamageToHighestHP(null, 1, (Card c) => c.IsHero, (Card c) => 3, DamageType.Projectile, storedResults: ddas, numberOfTargets: () => 2, damageSourceInfo: new TargetInfo(HighestLowestHP.HighestHP, 1, 1, new LinqCardCriteria((Card c) => c.IsVillainCharacterCard, "the villain character card with the highest HP")));
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Look for every instance where damage was dealt to a Hero target, get the Hero Turn Takers, and get every distinct one. 
            IEnumerable<TurnTaker> damagedHeroTurnTakers = ddas.Where((DealDamageAction dda) => dda.DidDealDamage && dda.Target.IsHero && dda.Target.Owner.IsHero).Select((DealDamageAction dda) => dda.Target.Owner).Distinct();

            if (AmmoNextToThis() > 0 && damagedHeroTurnTakers.Any())
            {
                // Select turn takers based on numbers damaged, and they should all discard.
                // TODO: Technically, this doesn't dynamically adjust if someone's mid-discard and a discard trigger destroys an ammo next to this card. But that edge case would
                // require Handelabra rewriting or manual restructuring, which just isn't worth the effort.
                coroutine = this.GameController.SelectTurnTakersAndDoAction(
                    new SelectTurnTakersDecision(this.GameController, this.DecisionMaker, new LinqTurnTakerCriteria((TurnTaker tt) => damagedHeroTurnTakers.Contains(tt)), SelectionType.DiscardCard, damagedHeroTurnTakers.Count(), false, damagedHeroTurnTakers.Count(), cardSource: this.GetCardSource()),
                    (TurnTaker tt) => this.GameController.SelectAndDiscardCards(this.GameController.FindHeroTurnTakerController(tt.ToHero()), AmmoNextToThis(), false, AmmoNextToThis(), cardSource: this.GetCardSource()),
                    cardSource: this.GetCardSource()
                    );

                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            coroutine = this.GameController.DestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsAmmo && c.Location == this.Card.NextToLocation), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}