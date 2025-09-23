using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.ShellShock
{
  public class ShellGliderCardController : VehicleSharedCardController
  {
    public ShellGliderCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      base.AddTriggers();
      this.AddReduceDamageTrigger((Card c) => c == this.CharacterCard, 1);
    }

  }
}