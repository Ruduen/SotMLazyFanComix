using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.LarrysDiscountGunClub
{
    public class QuantumRoundsCardController : SharedAmmoCardController
    {
        public QuantumRoundsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator BeforeDamageResponse()
        {
            this.RemoveTrigger(this._usePowerTrigger, false);
            this.AddToTemporaryTriggerList(this.AddMakeDamageIrreducibleTrigger((DealDamageAction dda) => dda.CardSource.Card == this.GetCardThisCardIsNextTo(true)));
            this.AddToTemporaryTriggerList(this.AddTrigger<DealDamageAction>((DealDamageAction dda) => dda.CardSource.Card == this.GetCardThisCardIsNextTo(true) && dda.Target.HitPoints != null && dda.Target.HitPoints <= 4, (DealDamageAction dda) => this.GameController.DestroyCard(this.DecisionMaker, dda.Target, cardSource: this.GetCardSource()), TriggerType.DealDamage, TriggerTiming.After));
            yield return null;
        }
    }
}