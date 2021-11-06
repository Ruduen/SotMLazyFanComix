using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Soulbinder
{
    public abstract class SoulbinderSharedYourTargetDamageCardController : CardController
    {
        public SoulbinderSharedYourTargetDamageCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public IEnumerator SelectYourTargetToDealDamage(List<Card> storedResults, int? damageAmount, DamageType damageType, bool excludeCharacter = false)
        {
            List<SelectCardDecision> storedDecision = new List<SelectCardDecision>();
            IEnumerator coroutine = this.GameController.SelectCardAndStoreResults(this.HeroTurnTakerController, SelectionType.CardToDealDamage,
                new LinqCardCriteria((Card c) => c.Owner == this.HeroTurnTaker && c.IsTarget && c.IsInPlayAndHasGameText && (!excludeCharacter || !c.IsCharacter)),
                storedDecision, false, false,
                new DealDamageAction(this.GetCardSource(), null, null, (Card c) => damageAmount, damageType), cardSource: this.GetCardSource()
            );
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (storedDecision.FirstOrDefault() != null)
            {
                storedResults.Add(storedDecision.FirstOrDefault().SelectedCard);
            }
        }
    }
}