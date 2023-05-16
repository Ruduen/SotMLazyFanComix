using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.TheEtherealExecutionerTeam
{
    public class PremeditatedDisruptionCardController : TheEtherealExecutionerSharedCardController
    {
        public PremeditatedDisruptionCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            List<DealDamageAction> ddaResults = new List<DealDamageAction>();

            coroutine = this.DealDamageToLowestHP(this.CharacterCard, 1, (Card c) => c.IsHeroCharacterCard, (Card c) => 2, DamageType.Infernal, storedResults: ddaResults);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            HeroTurnTaker targetPlayer = ddaResults?.FirstOrDefault()?.Target?.Owner?.ToHero();
            if (targetPlayer != null)
            {
                coroutine = this.GameController.DiscardTopCards(this.DecisionMaker, targetPlayer.Deck, this.CountObservationCards(), cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}