using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace RuduenWorkshop.BreachMage
{
    public class BreachMageTurnTakerController : HeroTurnTakerController
    {
        public BreachMageTurnTakerController(TurnTaker turnTaker, GameController gameController)
            : base(turnTaker, gameController)
        {
        }

        public override IEnumerator StartGame()
        {
            if (this.CharacterCardController is BreachMageSharedCharacterCardController breachMageController)
            {
                IEnumerator coroutine;
                int[] breachInitialFocus = breachMageController.BreachInitialFocus;
                Card[] breaches = this.GameController.FindCardsWhere((Card c) => c.Owner == this.HeroTurnTaker && c.DoKeywordsContain("breach")).ToArray();

                for (int i = 0; i < breachInitialFocus.Count() && i < breaches.Count(); i++)
                {
                    TokenPool focusPool = breaches[i].FindTokenPool("FocusPool");
                    if (focusPool != null)
                    {
                        if (breachInitialFocus[i] < 0 || breachInitialFocus[i] > 4)
                        {
                            // Out of bounds breach - remove the breach from the game.
                            coroutine = this.GameController.MoveCard(this, breaches[i], this.HeroTurnTaker.OutOfGame, evenIfIndestructible: true, cardSource: this.CharacterCardController.GetCardSource());
                            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        }
                        else
                        {
                            coroutine = this.GameController.RemoveTokensFromPool(focusPool, 4 - breachInitialFocus[i], cardSource: this.CharacterCardController.GetCardSource());
                            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        }
                    }
                }
            }
        }
    }
}