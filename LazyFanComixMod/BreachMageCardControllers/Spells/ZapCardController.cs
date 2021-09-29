using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.BreachMage
{
    public class ZapCardController : BreachMageSharedSpellCardController
    {
        public ZapCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override bool CanBeDestroyed
        {
            get
            {
                if (this.Card.IsInPlayAndHasGameText)
                {
                    if (this.Card.BattleZone != null)
                    {
                        if (!this.TurnTaker.IsIncapacitatedOrOutOfGame)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                return true;
            }
        }

        protected override IEnumerator ActivateCast()
        {
            IEnumerator coroutine;
            // Damage.
            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), 1, DamageType.Lightning, new int?(1), false, new int?(1), false, false, false, null, null, null, null, null, false, null, null, false, null, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override IEnumerator DestroyAttempted(DestroyCardAction d)
        {
            IEnumerator coroutine;
            coroutine = this.GameController.MoveCard(this.HeroTurnTakerController, this.Card, this.HeroTurnTaker.Hand, showMessage: true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}