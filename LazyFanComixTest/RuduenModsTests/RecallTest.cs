using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using LazyFanComix.Recall;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LazyFanComixTest
{
    [TestFixture]
    public class RecallTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
            ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(RecallCharacterCardController))); // replace with your own namespace
        }

        protected HeroTurnTakerController Recall { get { return FindHero("Recall"); } }

        [Test(Description = "Basic Setup and Health")]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Recall", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(Recall);
            Assert.IsInstanceOf(typeof(HeroTurnTakerController), Recall);
            Assert.IsInstanceOf(typeof(RecallCharacterCardController), Recall.CharacterCardController);

            Assert.AreEqual(27, Recall.CharacterCard.HitPoints);

            AssertNumberOfCardsInDeck(Recall, 36);
            AssertNumberOfCardsInHand(Recall, 4);
        }

        #region Innate Powers


        [Test()]
        public void TestInnatePowerParadoxProof()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Recall", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            DiscardAllCards(Recall);
            QuickHandStorage(Recall);
            UsePower(Recall);
            QuickHandCheck(1);

            DecisionSelectCard = PutInHand("ParadoxAnchor");
            UsePower(Recall);
            AssertIsInPlay("ParadoxAnchor");
        }


        [Test()]
        public void TestInnatePowerForecastedBlow()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Recall/RecallForecastedBlowCharacter", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card mdp = FindCardInPlay("MobileDefensePlatform");
            DecisionSelectTarget = mdp;

            QuickHPStorage(mdp);
            UsePower(Recall);
            QuickHPCheck(-1);

            QuickHPStorage(mdp);
            MoveCard(Recall, "TemporalLoop", Recall.CharacterCard.UnderLocation);
            UsePower(Recall);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestInnatePowerUnstableJump()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Recall/RecallUnstableJumpCharacter", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            UsePower(Recall);
            AssertCurrentTurnPhase(Recall, Phase.Start, true);
            GoToStartOfTurn(baron);
            // If possible, figure out how to confirm all other turns were skipped. 
            AssertCurrentTurnPhase(baron, Phase.Start, false);
            AssertNumberOfCardsUnderCard(Recall.CharacterCard, 1);

        }
        #endregion Innate Powers

        #region Tribunal Cases

        [Test()]
        public void TestTribunalRepresentativePower()
        {
            SetupGameController("BaronBlade", "Guise", "TheCelestialTribunal");

            StartGame();
            AvailableHeroes = DeckDefinition.AvailableHeroes.Concat(new string[] { "LazyFanComix.Recall" });
            SelectFromBoxForNextDecision("LazyFanComix.RecallCharacter", "LazyFanComix.Recall");

            PlayCard("RepresentativeOfEarth");

            Card representative = FindCardInPlay("RecallCharacter");
            AssertIsInPlay(representative);

            UsePower(representative);
        }

        [Test()]
        public void TestTribunalForecastedPower()
        {
            SetupGameController("BaronBlade", "Guise", "TheCelestialTribunal");

            StartGame();
            AvailableHeroes = DeckDefinition.AvailableHeroes.Concat(new string[] { "LazyFanComix.Recall" });
            SelectFromBoxForNextDecision("LazyFanComix.RecallForecastedBlowCharacter", "LazyFanComix.Recall");

            PlayCard("RepresentativeOfEarth");

            Card representative = FindCardInPlay("RecallCharacter");
            AssertIsInPlay(representative);

            UsePower(representative);
        }

        [Test()]
        public void TestTribunalUnstablePower()
        {
            SetupGameController("BaronBlade", "Guise", "TheCelestialTribunal");

            StartGame();
            AvailableHeroes = DeckDefinition.AvailableHeroes.Concat(new string[] { "LazyFanComix.Recall" });
            SelectFromBoxForNextDecision("LazyFanComix.RecallUnstableJumpCharacter", "LazyFanComix.Recall");

            PlayCard("RepresentativeOfEarth");

            Card representative = FindCardInPlay("RecallCharacter");
            AssertIsInPlay(representative);

            UsePower(representative);
        }

        #endregion Tribunal Cases

        #region Incapped Case Test
        [Test()]
        public void TestIncappedToForecastedPower()
        {
            SetupGameController("Infinitor", "LazyFanComix.BreachMage", "LazyFanComix.Recall/RecallForecastedBlowCharacter", "TheCelestialTribunal");

            StartGame();

            Card target = PlayCard("RecalescentHellion");

            DestroyCard(FindHero("BreachMage").CharacterCard);

            DecisionSelectCard = target;
            UseIncapacitatedAbility(FindHero("BreachMage").CharacterCard, 1);
        }
        #endregion Incapped Case Test

        #region Loop Cards

        [Test()]
        public void TestCardTemporalLoop()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Recall", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            GoToDrawCardPhase(Recall);
            PlayCard("TemporalLoop");
            EnterNextTurnPhase();
            EnterNextTurnPhase();
            AssertCurrentTurnPhase(Recall, Phase.Start, false);

            GoToNextTurn();
            // If possible, figure out how to confirm all other turns were skipped. 
            AssertCurrentTurnPhase(legacy, Phase.Start, false);
            AssertNumberOfCardsUnderCard(Recall.CharacterCard, 1);
        }

        [Test()]
        public void TestCardImmediateJump()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Recall", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();


            PlayCard("ImmediateJump");
            AssertCurrentTurnPhase(Recall, Phase.Start, true);
            GoToStartOfTurn(baron);
            // If possible, figure out how to confirm all other turns were skipped. 
            AssertCurrentTurnPhase(baron, Phase.Start, false);
            AssertNumberOfCardsUnderCard(Recall.CharacterCard, 1);
        }

        #endregion Loop Cards

        #region Other Cards

        [Test()]
        public void TestCardRecalled()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Recall", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            QuickHPStorage(Recall);
            Card power = PlayCard("Recalled");
            QuickHPCheck(-3);

            Card mdp = this.FindCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;

            // Start of Turn Damage.
            QuickHPStorage(mdp);
            GoToStartOfTurn(Recall);
            QuickHPCheck(-2);

            // Power damage. 
            QuickHPStorage(mdp);
            UsePower(power);
            QuickHPCheck(-4);

        }

        [Test()]
        public void TestCardDejaVu()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Recall", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            IEnumerable<Card> played = GetCards("TemporalLoop", "Recalled");

            PutInTrash(played);

            PlayCard("DejaVu");
            AssertIsInPlay(played);
        }


        [Test()]
        public void TestCardDejaVuSelf()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Recall", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card play = GetCard("DejaVu", 0);
            Card trash = GetCard("DejaVu", 1);

            PutInTrash(trash);

            AssertNextMessage("Recall tried to play Deja Vu, but it is a limited card that is already in play. Moving it to Recall's trash.");

            PlayCard(play);
            AssertInTrash(trash);
            AssertExpectedMessageWasShown();
        }


        [Test()]
        public void TestCardNoteToSelf()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Recall", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card play = PutInHand("NoteToSelf");

            MoveCard(Recall, "TemporalLoop", Recall.CharacterCard.UnderLocation);

            QuickHandStorage(Recall);
            PlayCard(play);
            QuickHandCheck(2); // 1 played, 3 drawn.
            AssertNumberOfCardsUnderCard(Recall.CharacterCard, 0);

            QuickHandStorage(Recall);
            PlayCard(play);
            QuickHandCheck(3); // 1 from trash, 3 drawn.
            AssertNumberOfCardsUnderCard(Recall.CharacterCard, 0);
        }
        [Test()]
        public void TestCardCloseTheLoop()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Recall", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card play = PutInHand("CloseTheLoop");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            MoveCard(Recall, "TemporalLoop", Recall.CharacterCard.UnderLocation);
            MoveCard(Recall, "ImmediateJump", Recall.CharacterCard.UnderLocation);

            QuickHPStorage(Recall.CharacterCard, mdp);
            PlayCard(play);
            AssertNumberOfCardsUnderCard(Recall.CharacterCard, 0);

            PlayCard(play);
            AssertNumberOfCardsUnderCard(Recall.CharacterCard, 0);
            QuickHPCheck(-4, -4);
        }


        [Test()]
        public void TestCardSeenItBefore()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Recall", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card power = PlayCard("SeenItBefore");

            QuickHandStorage(Recall);
            GoToStartOfTurn(Recall);
            QuickHandCheck(1);

            Card move = PutOnDeck("TheLegacyRing");
            DecisionMoveCardDestination = new MoveCardDestination(legacy.TurnTaker.Deck, true);
            DecisionSelectLocation = new LocationChoice(legacy.HeroTurnTaker.Deck);

            UsePower(power);
            AssertOnBottomOfDeck(move);
        }

        [Test()]
        public void TestCardSeeItAgain()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Recall", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card play = PutOnDeck("TheLegacyRing");
            Card power = PlayCard("SeeItAgain");

            DecisionSelectLocation = new LocationChoice(legacy.HeroTurnTaker.Deck);

            GoToStartOfTurn(Recall);

            DecisionSelectCard = play;

            UsePower(power);
            AssertIsInPlay(play);
        }



        [Test()]
        public void TestCardParadoxAnchor()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Recall", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            PlayCard("ParadoxAnchor");
            DestroyCard(FindCardInPlay("MobileDefensePlatform"));

            QuickHPStorage(Recall, baron);
            DealDamage(Recall, Recall, 1, DamageType.Cold);
            DealDamage(Recall, baron, 1, DamageType.Cold);
            DealDamage(Recall, baron, 1, DamageType.Cold);

            QuickHPCheck(0, -2 - 1);
        }

        [Test()]
        public void TestCardRefinedShot()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Recall", "MrFixer", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            Card power = PlayCard("RefinedShot");
            Card redirect = PlayCard("DrivingMantis");

            DecisionSelectCards = new Card[] { fixer.CharacterCard, baron.CharacterCard, fixer.CharacterCard };

            QuickHPStorage(fixer);
            UsePower(power);
            UsePower(power);
            GoToEndOfTurn(env);
            DestroyCard(redirect);
            DealDamage(Recall, fixer, 0, DamageType.Cold);
            QuickHPCheck(-2 - 5 - 0); // 0+2 damage the first time, 2+3 the second, 0 the third due to environment turn. 
        }


        [Test()]
        public void TestCardGoingThroughTheMotions()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Recall", "MrFixer", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            DestroyCard("MobileDefensePlatform");
            PlayCard("GoingThroughTheMotions");



            GoToStartOfTurn(Recall);
            DecisionSelectFunction = 1;
            UsePower(Recall);
            ResetDecisions();

            QuickHandStorage(Recall);
            GoToPlayCardPhase(Recall);
            QuickHandCheck(1); // No power usable, draw instead. 

            ResetDecisions();

            QuickHandStorage(Recall);
            PlayCard("PaparazziOnTheScene");
            GoToPlayCardPhase(Recall); // No power usable, draw.
            QuickHandCheck(1);
            DestroyCard("PaparazziOnTheScene");

            ResetDecisions();
            DecisionSelectFunctions = new int?[] { 0, 1 };
            GoToPlayCardPhase(Recall);
            AssertNotUsablePower(Recall, Recall.CharacterCard); // Use power by default.

            ResetDecisions();
            DecisionSelectFunction = 2;
            QuickHPStorage(baron);
            GoToPlayCardPhase(Recall);
            QuickHPCheck(-2); // Damage.

            ResetDecisions();
            DecisionSelectFunctions = new int?[] { null };
            GoToPlayCardPhase(Recall);
            AssertCanPerformPhaseAction();
        }

        [Test()]
        public void TestCardGoingThroughTheMotionsSkip()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Recall", "MrFixer", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            PlayCard("GoingThroughTheMotions");

            DecisionSelectFunctions = new int?[] { null };
            GoToPlayCardPhase(Recall);
            AssertPhaseActionCount(1);
        }

        [Test()]
        public void TestCardGoingThroughTheMotionsEphemeralTest()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Recall", "MrFixer", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            PlayCard("GoingThroughTheMotions");
            PlayCard("ImmediateJump");

            DecisionSelectFunctions = new int?[] { null };
            GoToPlayCardPhase(Recall);
            AssertPhaseActionCount(1);
        }


        [Test()]
        public void TestCardTemporalAnomaly()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Recall", "MrFixer", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            DestroyCard("MobileDefensePlatform");
            Card search = PutOnDeck("ParadoxAnchor");
            DecisionSelectCard = search;

            DealDamage(Recall, Recall, 5, DamageType.Melee);
            QuickHPStorage(Recall, baron);
            PlayCard("TemporalAnomaly");
            QuickHPCheck(2, -2); // 1 damage, +1 for anchor.
            AssertIsInPlay(search);
        }

        #endregion Other Cards



    }
}