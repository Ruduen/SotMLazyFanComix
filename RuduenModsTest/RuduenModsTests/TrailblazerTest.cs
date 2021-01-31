using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using RuduenWorkshop.Trailblazer;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RuduenModsTest
{
    [TestFixture]
    public class TrailblazerTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
            ModHelper.AddAssembly("RuduenWorkshop", Assembly.GetAssembly(typeof(TrailblazerCharacterCardController))); // replace with your own namespace
        }

        protected HeroTurnTakerController Trailblazer { get { return FindHero("Trailblazer"); } }

        [Test(Description = "Basic Setup and Health")]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.Trailblazer", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(Trailblazer);
            Assert.IsInstanceOf(typeof(HeroTurnTakerController), Trailblazer);
            Assert.IsInstanceOf(typeof(TrailblazerCharacterCardController), Trailblazer.CharacterCardController);

            Assert.AreEqual(29, Trailblazer.CharacterCard.HitPoints);
            AssertNumberOfCardsInDeck(Trailblazer, 36);
            AssertNumberOfCardsInHand(Trailblazer, 4);
        }

        [Test()]
        public void TestDriftersShotInnatePower()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Trailblazer", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card play = PutInHand("VantagePoint");
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectCardToPlay = play;

            DecisionSelectTargets = new Card[] { mdp, baron.CharacterCard, Trailblazer.CharacterCard };

            QuickHPStorage(mdp);
            UsePower(Trailblazer);
            QuickHPCheck(-1);
            AssertIsInPlay(play);
        }


        #region Positions

        [Test()]
        public void TestPositionDestroysPosition()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "CitizensHammerAndAnvilTeam", "RuduenWorkshop.Trailblazer", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            List<Card> positions = new List<Card> {
                FindCardsWhere((Card c) => c.IsPosition && c.IsInPlay && c.Owner == hammeranvilTeam.TurnTaker).FirstOrDefault(),
                PutInHand("VantagePoint"),
                PutInHand("DefensiveBulwark"),
                FindCardsWhere((Card c) => c.IsPosition && !c.IsInPlay && c.Owner == hammeranvilTeam.TurnTaker).FirstOrDefault()
            };

            AssertIsInPlay(positions[0]);

            // Destroys other Position. 
            PlayCard(positions[1]);
            AssertIsInPlay(positions[1]);
            AssertInTrash(positions[0]);

            // Destroys same Position. 
            PlayCard(positions[2]);
            AssertIsInPlay(positions[2]);
            AssertInTrash(positions[1]);

            // Is Destroyed by Position.
            PlayCard(positions[3]);
            AssertIsInPlay(positions[3]);
            AssertInTrash(positions[2]);
        }

        [Test()]
        public void TestPositionVantagePoint()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Trailblazer", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card play = PutInHand("VantagePoint");

            DecisionSelectTurnTaker = legacy.TurnTaker;

            QuickHandStorage(legacy);
            GoToPlayCardPhase(Trailblazer);
            PlayCard(play);
            GoToUsePowerPhase(Trailblazer);
            UsePower(play);
            GoToDrawCardPhase(Trailblazer);
            QuickHandCheck(2); // Legacy drew 2. 
            AssertPhaseActionCount(2); // 2 Draws for Trailblazer on turn.
        }

        [Test()]
        public void TestPositionDefensiveBulwark()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Trailblazer", "MrFixer", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            DealDamage(baron.CharacterCard, (Card c) => c.IsHero, 10, DamageType.Melee);

            Card play = PutInHand("DefensiveBulwark");

            QuickHPStorage(Trailblazer, fixer);
            PlayCard(play);
            UsePower(play);
            DealDamage(baron.CharacterCard, Trailblazer.CharacterCard, 3, DamageType.Melee);
            DealDamage(baron.CharacterCard, Trailblazer.CharacterCard, 3, DamageType.Melee);
            QuickHPCheck(2 - 1 - 3, 2); // Fixer just heals 2. Trailblazer Healed 2, but took 1 and 3 damage.

        }
        [Test()]
        public void TestPositionStrikingZone()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Trailblazer", "MrFixer", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            DealDamage(baron.CharacterCard, (Card c) => c.IsHero, 10, DamageType.Melee);

            Card play = PutInHand("StrikingZone");
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectTargets = new Card[] { mdp, baron.CharacterCard };

            QuickHPStorage(mdp);
            PlayCard(play);
            UsePower(play);
            QuickHPCheck(-2); // Base 1, increased by 1. 

        }
        #endregion Positions

        #region OnPositionPlay

        [Test()]
        public void TestOnPositionPlayClimbingHarnessWithMultiTrigger()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Trailblazer", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            Card position = PutInHand("VantagePoint");
            PlayCard("ClimbingHarness");

            DecisionSelectTurnTaker = legacy.TurnTaker;

            QuickHPStorage(mdp);
            PlayCard(position);
            PutInHand(position);
            PlayCard(position);
            GoToStartOfTurn(Trailblazer);
            PutInHand(position);
            PlayCard(position);
            DealDamage(Trailblazer, mdp, 0, DamageType.Melee); // Deal 0+2+2 Damage.
            DealDamage(Trailblazer, mdp, 0, DamageType.Melee); // Deal 0 Damage.
            QuickHPCheck(-4 - 0);
        }

        [Test()]
        public void TestOnPositionPlaySupplyPack()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Trailblazer", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card position = PutInHand("VantagePoint");
            PlayCard("SupplyPack");

            DecisionSelectTurnTaker = legacy.TurnTaker;

            PlayCard(position);
            AssertNotUsablePower(Trailblazer, position); // Power used. 
            PutInHand(position);
            PlayCard(position);
            AssertUsablePower(Trailblazer, position); // Power reset and usable.
        }

        [Test()]
        public void TestOnPositionPlayWornBinoculars()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Trailblazer", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card position = PutInHand("VantagePoint");
            PlayCard("WornBinoculars");

            DecisionSelectTurnTaker = baron.TurnTaker;
            DecisionMoveCardDestinations = new MoveCardDestination[] {
                new MoveCardDestination(baron.TurnTaker.Deck, false, false, false),
                new MoveCardDestination(baron.TurnTaker.Deck, true, false, false)
            };
            Card revealedCard = baron.TurnTaker.Deck.TopCard;

            // Moved to top without error.
            PlayCard(position);
            AssertOnTopOfDeck(revealedCard);

            // Move BB's top card to the bottom. 
            GoToStartOfTurn(Trailblazer);
            PutInHand(position);
            PlayCard(position);
            AssertOnBottomOfDeck(revealedCard);
        }
        #endregion OnPositionPlay

        #region OnPositionDestroy
        [Test()]
        public void TestOnPositionDestroyRapidRepositioning()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Trailblazer", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card position = PutInHand("VantagePoint");
            Card play = PutInHand("SupplyPack");

            DecisionSelectCardToPlay = play;

            PlayCard("RapidRepositioning");

            PlayCard(position);
            DestroyCard(position);

            AssertIsInPlay(play);
        }

        [Test()]
        public void TestOnPositionDestroyPartingGift()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Trailblazer", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card position = PutInHand("VantagePoint");
            Card mdp = GetCardInPlay("MobileDefensePlatform");


            DecisionSelectCards = new Card[] { mdp, baron.CharacterCard };

            PlayCard("PartingGift");

            QuickHPStorage(mdp);
            PlayCard(position);
            DestroyCard(position);
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestOnPositionDestroyWastelandWanderer()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Trailblazer", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card position = PutInHand("VantagePoint");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card power = PlayCard("WastelandWanderer");

            DecisionSelectTarget = mdp;

            PlayCard(position);
            DestroyCard(position);
            AssertTokenPoolCount(power.TokenPools.FirstOrDefault(), 1); // One token added.

            QuickHPStorage(mdp);
            UsePower(power);
            QuickHPCheck(-3); // One token, two bonus tokens.
        }
        #endregion OnPositionDestroy

        #region Ungrouped Cards
        [Test()]
        public void TestUngroupedLeadTheWay()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Trailblazer", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card position = PutOnDeck("VantagePoint", true);
            Card play = PutInHand("SupplyPack");
            DecisionSelectCards = new Card[] { position, play };

            PlayCard("LeadTheWay");
            AssertIsInPlay(position, play);
        }

        [Test()]
        public void TestUngroupedUnchartedGrounds()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Trailblazer", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            QuickHPStorage(mdp);
            PlayCard("UnchartedGrounds");
            AssertNumberOfCardsAtLocation(Trailblazer.TurnTaker.PlayArea, 2); // Confirm character card + position is now in play. 
            QuickHPCheck(-1); // Two damaged, MDP should be included.
        }

        [Test()]
        public void TestUngroupedPreparedAmbush()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Trailblazer", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card[] destroyed = new Card[] { PlayCard("VantagePoint"), PlayCard("ImpendingCasualty"), PlayCard("LivingForceField") };

            QuickHPStorage(mdp);
            PlayCard("PreparedAmbush");
            AssertNotUsablePower(Trailblazer, Trailblazer.CharacterCard);
            AssertNotUsablePower(legacy, legacy.CharacterCard);
            AssertInTrash(destroyed);

        }

        [Test()]
        public void TestUngroupedTacticalWithdrawl()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Trailblazer", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card position = PlayCard("VantagePoint");

            QuickHandStorage(Trailblazer);
            PlayCard("TacticalWithdrawl");
            QuickHandCheck(4); // 4 Cards from using power twice. 
            AssertInTrash(position);

        }

        [Test()]
        public void TestUngroupedTacticalWithdrawlNoPosition()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Trailblazer", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            QuickHandStorage(Trailblazer);
            PlayCard("TacticalWithdrawl");
            QuickHandCheck(0); // No Change. 

        }

        [Test()]
        public void TestUngroupedTrailOfAshes()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Trailblazer", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();


            Card position = PlayCard("VantagePoint");
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            QuickHandStorage(Trailblazer);
            QuickHPStorage(mdp, Trailblazer.CharacterCard);
            PlayCard("TrailOfAshes");
            QuickHandCheck(2); // Draws. 
            AssertInTrash(position);
            QuickHPCheck(-2, -2);
        }

        [Test()]
        public void TestUngroupedTrailOfAshesNoPosition()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Trailblazer", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            QuickHandStorage(Trailblazer);
            QuickHPStorage(mdp, Trailblazer.CharacterCard);
            PlayCard("TrailOfAshes");
            QuickHandCheck(0); // No Draws. 
            QuickHPCheck(-2, -2);
        }

        [Test()]
        public void TestUngroupedGuidedOdysseyNoTrashes()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Trailblazer", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card[] cards = new Card[] { GetCardWithLittleEffect(Trailblazer), GetCardWithLittleEffect(legacy) };

            PutOnDeck(Trailblazer, cards[0]);
            PutOnDeck(legacy, cards[1]);
            PlayCard("GuidedOdyssey");
            AssertIsInPlay(cards);
        }

        [Test()]
        public void TestUngroupedGuidedOdysseyTrashes()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Trailblazer", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card[] cards = new Card[] { GetCard("SupplyPack"), GetCardWithLittleEffect(legacy), PutInTrash("VantagePoint"), PutInTrash("ImpendingCasualty") };

            PutOnDeck(Trailblazer, cards[0]);
            PutOnDeck(legacy, cards[1]);

            PlayCard("GuidedOdyssey");
            AssertIsInPlay(cards);
        }

        #endregion Ungrouped Cards
    }
}