using System.Collections;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.Conflux
{
  public class ChannelEssenceCardController : ConfluxDamageTypesDealtCardController
  {
    public ChannelEssenceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, DrawResponse, TriggerType.DrawCard);
    }

    private IEnumerator DrawResponse(PhaseChangeAction action)
    {
      return this.GameController.SelectTurnTakersAndDoAction(this.HeroTurnTakerController, new LinqTurnTakerCriteria((TurnTaker tt) => !tt.IsIncapacitatedOrOutOfGame && tt.IsHero), SelectionType.DrawCard, (TurnTaker tt) => this.GameController.DrawCard(tt.ToHero(), true, cardSource: this.GetCardSource()), this.confluxDamageTypesDealt().Count(), false, 0, cardSource: this.GetCardSource());
    }


  }
}