using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Synthesist
{
    public class SynthesistTurnTakerController : HeroTurnTakerController
    {
        public SynthesistTurnTakerController(TurnTaker turnTaker, GameController gameController)
            : base(turnTaker, gameController)
        {
        }

        public override IEnumerator StartGame()
        {
            IEnumerator coroutine;

            // At the start of game, flip the relevant cards,
            IEnumerable<Card> cards = this.GameController.FindCardsWhere((Card c) => c.IsHeroCharacterCard && c.Owner == this.HeroTurnTaker && c.Identifier != "SynthesistCharacter");
            IEnumerable<CardController> cardControllers = this.GameController.FindCardControllersWhere((Card c) => cards.Contains(c));

            coroutine = this.GameController.FlipCards(cardControllers);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }


        public override bool IsIncapacitated
        {
            // Also checks odd turn taker status. Use IsIncapacitated instead of IsFlipped due to checking at the moment of incapacitation for before triggers. 
            get { 
                if(this.TurnTaker.Identifier == "Synthesist")
                {
                    return this.IncapacitationCardController.Card.IsIncapacitated;
                }
                return base.IsIncapacitated; 
            }
        }

        public override bool IsIncapacitatedOrOutOfGame
        {
            get { 
                return this.IsIncapacitated || this.IncapacitationCardController.Card.IsOutOfGame; 
            }
        }
    }
}