using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

// Manually tested!

namespace LazyFanComix.Greyhat
{
    public class BandwidthRestrictionCardController : GreyhatSharedNetworkCardController
    {
        public BandwidthRestrictionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }


        public override void AddTriggers()
        {
            this.AddReduceDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card != null && this.CardsLinksAreNextToHeroes.Contains(dd.Target) && this.CardsLinksAreNextToNonHero.Contains(dd.DamageSource.Card), (DealDamageAction dda) => 1);
        }
    }
}