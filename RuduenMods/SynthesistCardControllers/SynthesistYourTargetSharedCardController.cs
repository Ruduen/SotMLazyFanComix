using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Synthesist
{
    public interface ISynthesistYourTargetSharedCardController
    {
        public IEnumerator SelectYourTargetToDealDamage(List<Card> storedResults, int damageAmount, DamageType damageType);
    }
    public abstract class SynthesistYourTargetSharedCardController : CardController, ISynthesistYourTargetSharedCardController
    {
        public SynthesistYourTargetSharedCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public IEnumerator SelectYourTargetToDealDamage(List<Card> storedResults, int damageAmount, DamageType damageType)
        {
            List<SelectCardDecision> storedDecision = new List<SelectCardDecision>();
            IEnumerator coroutine = this.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.CardToDealDamage,
                new LinqCardCriteria((Card c) => c.Owner == this.HeroTurnTaker && c.IsTarget && c.IsInPlayAndHasGameText),
                storedDecision, false, false,
                new DealDamageAction(this.GetCardSource(), null, null, damageAmount, damageType)
            );
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (storedDecision.FirstOrDefault() != null)
            {
                storedResults.Add(storedDecision.FirstOrDefault().SelectedCard);
            }
        }
    }
}