using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace LazyFanComix.Soulbinder
{
    public class SoulbinderCharacterCardController : HeroCharacterCardController
    {
        public string str;

        public SoulbinderCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<int> numerals = new List<int>(){
                            this.GetPowerNumeral(0, 1),   // Number of Targets
                            this.GetPowerNumeral(1, 4),   // Damage.
                            this.GetPowerNumeral(2, 2)    // Damage to deal.
            };
            List<Card> target = new List<Card>();
            IEnumerator coroutine;

            // Select target.
            coroutine = this.SelectYourTargetToDealDamage(target, numerals[1], DamageType.Infernal);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (target.Count > 0)
            {
                DamageSource targetSource = new DamageSource(this.GameController, target.FirstOrDefault());
                // That target deals 1 Target 3 Toxic Damage
                coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, targetSource, numerals[1], DamageType.Infernal, numerals[0], false, numerals[0], cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // Deals themselves 1 damage.
                coroutine = this.GameController.DealDamageToTarget(targetSource, target.FirstOrDefault(), (Card c) => numerals[2], DamageType.Infernal, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        public IEnumerator SelectYourTargetToDealDamage(List<Card> storedResults, int damageAmount, DamageType damageType)
        {
            List<SelectCardDecision> storedDecision = new List<SelectCardDecision>();
            IEnumerator coroutine = this.GameController.SelectCardAndStoreResults(this.HeroTurnTakerController, SelectionType.CardToDealDamage,
                new LinqCardCriteria((Card c) => c.Owner == this.TurnTaker && c.IsTarget && c.IsInPlayAndHasGameText),
                storedDecision, false, false,
                new DealDamageAction(this.GetCardSource(), null, null, damageAmount, damageType)
            );
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (storedDecision.FirstOrDefault() != null)
            {
                storedResults.Add(storedDecision.FirstOrDefault().SelectedCard);
            }
        }

        // TODO: Replace with something more unique!
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        coroutine = this.SelectHeroToPlayCard(this.HeroTurnTakerController);
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
                case 1:
                    {
                        coroutine = base.GameController.SelectHeroToUsePower(this.HeroTurnTakerController, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
                case 2:
                    {
                        coroutine = base.GameController.SelectHeroToDrawCard(this.HeroTurnTakerController, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
            }
            yield break;
        }
    }
}