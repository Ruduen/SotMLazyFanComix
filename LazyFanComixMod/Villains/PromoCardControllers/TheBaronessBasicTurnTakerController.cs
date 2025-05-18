using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.TheBaroness
{
  public class TheBaronessBasicTurnTakerController : TurnTakerController
  {
    public TheBaronessBasicTurnTakerController(TurnTaker turnTaker, GameController gameController)
        : base(turnTaker, gameController)
    {
    }

    public override IEnumerator StartGame()
    {
      return this.PutCardIntoPlay("Vampirism", true);
    }
  }
}