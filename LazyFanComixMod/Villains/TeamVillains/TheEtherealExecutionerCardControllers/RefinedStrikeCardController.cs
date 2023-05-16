using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.TheEtherealExecutionerTeam
{
    public class RefinedStrikeCardController : TheEtherealExecutionerSharedCardController
    {
        public RefinedStrikeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // Trigger to increase damage dealt to self by 1 per card.
            ITrigger tempIncrease = this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.CardSource.CardController == this, (DealDamageAction dda) => this.CountObservationCards());

            coroutine = this.DealDamageToHighestHP(this.CharacterCard, 1, (Card c) => c.IsHero, (Card c) => 2, DamageType.Melee);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            this.RemoveTrigger(tempIncrease);
        }
    }
}