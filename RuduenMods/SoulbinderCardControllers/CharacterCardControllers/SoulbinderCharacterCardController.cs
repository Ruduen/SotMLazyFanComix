using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace RuduenWorkshop.Soulbinder
{
    public class SoulbinderCharacterCardController : SoulbinderSharedInstructionsCharacterCardController
    {
        public string str;

        public SoulbinderCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
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
                new LinqCardCriteria((Card c) => c.Owner == this.TurnTaker && c.IsTarget && c.IsInPlayAndHasGameText),
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

        // TODO: Replace Incap with something more unique!
    }
}