using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace RuduenWorkshop.Synthesist
{
    public class SynthesistCharacterCardController : PromoDefaultCharacterCardController
    {
        private string[] _synthesistRelics;

        public SynthesistCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, PutRelicIntoPlayTrigger, TriggerType.FlipCard);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<int> powerNumerals = new List<int>
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 3),
                this.GetPowerNumeral(2, 1)
            };
            IEnumerator coroutine;

            List<SelectCardDecision> storedDecision = new List<SelectCardDecision>();
            coroutine = this.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.CardToDealDamage,
                new LinqCardCriteria((Card c) => c.IsHero && c.IsTarget && c.IsInPlayAndHasGameText && c != this.Card),
                storedDecision, false, false,
                new DealDamageAction(this.GetCardSource(), null, null, powerNumerals[2], DamageType.Infernal)
            );
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }


            if (storedDecision.FirstOrDefault().SelectedCard != null)
            {
                Card target = storedDecision.FirstOrDefault().SelectedCard;
                // The selected target deals another 3 damage and themselves 1 damage. 
                coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, target), powerNumerals[1], DamageType.Infernal, powerNumerals[0], false, powerNumerals[0], cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, target), target, powerNumerals[2], DamageType.Infernal, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        protected IEnumerator PutRelicIntoPlayTrigger(PhaseChangeAction action)
        {
            // If there are no other targets in play and the relic deck is not empty...
            if (this.GameController.FindCardsWhere((Card c) => c.Owner == this.HeroTurnTaker && c.IsTarget && c.IsInPlayAndNotUnderCard && c != this.Card).Count() == 0)
            {
                IEnumerator coroutine = this.GameController.SelectAndUnincapacitateHeroCharacter(this.DecisionMaker, 9, null, false, null, new LinqCardCriteria((Card c) => c.Owner == this.HeroTurnTaker && (this.GameController.IsOblivAeonMode || SynthesistRelics.Contains(c.PromoIdentifierOrIdentifier)), "incapacitated Synthesist character"), this.GetCardSource(), true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        public override IEnumerator BeforeFlipCardImmediateResponse(FlipCardAction flip)
        {
            IEnumerator coroutine;

            if (!this.CardWithoutReplacements.IsFlipped)
            {
                coroutine = this.GameController.FlipCards(this.GameController.FindCardControllersWhere((Card c) => SynthesistRelics.Contains(c.PromoIdentifierOrIdentifier) && c.IsActive));
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            // Do default flipping. 
            coroutine = base.BeforeFlipCardImmediateResponse(flip);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        }


        private string[] SynthesistRelics
        {
            get
            {
                if (_synthesistRelics == null)
                {
                    _synthesistRelics = new string[] { "BoneOfIron", "HeartOfLightning", "VialOfMercury" };
                }
                return _synthesistRelics;
            }
        }

        // TODO: Replace Incap with something more unique!
    }
}