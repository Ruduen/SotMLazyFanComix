using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using LazyFanComix.Thri;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LazyFanComixTest
{
    [TestFixture]
    public class ThriTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
            ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(ThriCharacterCardController))); // replace with your own namespace
        }

        protected HeroTurnTakerController Thri
        { get { return FindHero("Thri"); } }

        [Test(Description = "Basic Setup and Health")]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Thri", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(Thri);
            Assert.IsInstanceOf(typeof(HeroTurnTakerController), Thri);
            Assert.IsInstanceOf(typeof(ThriCharacterCardController), Thri.CharacterCardController);

            Assert.AreEqual(27, Thri.CharacterCard.HitPoints);
            AssertNumberOfCardsInDeck(Thri, 36);
            AssertNumberOfCardsInHand(Thri, 4);
        }

        #region Innate Tests


        [Test()]
        public void TestBaseInnatePower()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Thri", "Bunker", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();

            DestroyNonCharacterVillainCards();

            QuickHPStorage(baron);
            UsePower(Thri);
            QuickHPCheck(-2);
            UsePower(Thri);
            QuickHPCheck(-2);
            UsePower(Thri);
            QuickHPCheck(-2 - 3);
            UsePower(Thri);
            QuickHPCheck(-2);

            GoToStartOfTurn(bunker);
            UsePower(Thri);
            QuickHPCheck(-2);
            UsePower(bunker);
            QuickHPCheck(0);
            UsePower(Thri);
            QuickHPCheck(-2 - 3);
            UsePower(Thri);
            QuickHPCheck(-2);
        }


        [Test()]
        public void TestInnatePowerTribunal()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "Tempest", "Guise", "Legacy/AmericasGreatestLegacyCharacter", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();

            DestroyNonCharacterVillainCards();

            QuickHPStorage(baron);
            UsePower(tempest);
            QuickHPCheck(-1);
            UsePower(tempest);
            QuickHPCheck(-1);

            SelectFromBoxForNextDecision("LazyFanComix.ThriCharacter", "LazyFanComix.Thri");
            PlayCard("CalledToJudgement");
            QuickHPCheck(-2 - 3);

            GoToStartOfTurn(guise);
            UsePower(tempest);
            QuickHPCheck(-1);
            UsePower(tempest);
            QuickHPCheck(-1);
            UsePower(FindCardInPlay("ThriCharacter"));
            QuickHPCheck(-2 - 3);
        }

        #endregion Innate Tests

        #region Equipment Tests

        [Test()]
        public void TestEquipAnchor()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Thri", "Bunker", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();

            DestroyNonCharacterVillainCards();

            UsePower(Thri);

            Card ongoing = PlayCard("BacklashField");

            QuickHPStorage(baron);
            Card equip = PlayCard("LoadoutAnchor");
            QuickHPCheck(-1 - 1);
            UsePower(equip, 0);
            QuickHPCheck(-1 - 1 - 1);
            AssertInTrash(ongoing);
            UsePower(equip);
            QuickHPCheck(-1 - 1);

        }

        [Test()]
        public void TestEquipSlick()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Thri", "Bunker", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();

            DestroyNonCharacterVillainCards();

            UsePower(Thri);

            Card[] targets = new Card[] { PlayCard("MobileDefensePlatform", 0), PlayCard("MobileDefensePlatform", 1), PlayCard("BladeBattalion") };

            DecisionSelectTargets = new Card[] { targets[0], targets[0], targets[1], targets[2] };

            QuickHPStorage(targets);
            Card equip = PlayCard("LoadoutSlick");
            QuickHPCheck(-3, -0, -0);
            UsePower(equip, 0);
            QuickHPCheck(-3, -3, -3);
            UsePower(equip);
            QuickHPCheck(-3, -0, -0);

        }


        [Test()]
        public void TestEquipWhisper()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Thri", "Bunker", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();

            DestroyNonCharacterVillainCards();

            UsePower(Thri);

            Card[] targets = new Card[] { PlayCard("MobileDefensePlatform", 0), PlayCard("MobileDefensePlatform", 1) };

            QuickHandStorage(Thri);
            QuickHPStorage(targets);
            Card equip = PlayCard("LoadoutWhisper");
            QuickHPCheck(-1, -1);
            QuickHandCheck(0);
            UsePower(equip);
            QuickHPCheck(-1, -1);
            QuickHandCheck(3);
            UsePower(equip);
            QuickHPCheck(-1, -1);
            QuickHandCheck(0);
        }

        #endregion Equipment Tests

        #region Ongoing Tests

        [Test()]
        public void TestOngoingRuleOfThree()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Thri", "Bunker", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();

            Card equip = PlayCard("LoadoutWhisper");

            GoToUsePowerPhase(Thri);

            DestroyNonCharacterVillainCards();
            PlayCard("RuleOfThree");
            AssertPhaseActionCount(2);

            Card[] targets = new Card[] { PlayCard("MobileDefensePlatform", 0), PlayCard("MobileDefensePlatform", 1) };

            QuickHPStorage(targets);
            UsePower(equip);
            QuickHPCheck(-1, -1);
            UsePower(equip);
            QuickHPCheck(-1, -1);
            UsePower(equip);
            QuickHPCheck(-1 - 2, -1 - 2);
            UsePower(equip);
            QuickHPCheck(-1, -1);
        }

        [Test()]
        public void TestOngoingFindTheShot()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Thri", "Bunker", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();
            DestroyNonCharacterVillainCards();
            DiscardAllCards(Thri);

            Card power = PlayCard("FindTheShot");
            Card equip = PlayCard("LoadoutWhisper");

            GoToStartOfTurn(Thri);

            UsePower(equip);

            Card discard = PutInHand("CallTheShot");
            DecisionSelectCard = discard;


            QuickHPStorage(baron);
            QuickHandStorage(Thri);
            UsePower(power);
            AssertInTrash(discard);
            QuickHPCheck(-2 - 3); // Base power used. Third power, so confirm increase.
            QuickHandCheck(0); // Draw 1, Discard 1. 
        }


        [Test()]
        public void TestOngoingCallTheShot()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Thri", "Bunker", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();
            DestroyNonCharacterVillainCards();

            Card power = PlayCard("CallTheShot");

            QuickHPStorage(baron);
            QuickHandStorage(bunker);
            UsePower(power);
            UsePower(Thri);
            QuickHPCheck(-2 - 3); // Confirm bonus damage from default confirmed
            QuickHandCheck(1);    // and Bunker used initialize.
        }
        #endregion Ongoing Tests

        #region One-Shot Tests
        [Test()]
        public void TestOneShotToolsOfTheTrade()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Thri", "Bunker", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();
            DestroyNonCharacterVillainCards();


            Card equip = PutInDeck("LoadoutWhisper");
            DecisionSelectCards = new Card[] { baron.CharacterCard, null, equip, null };

            QuickHPStorage(baron);
            PlayCard("ToolsOfTheTrade");
            AssertIsInPlay(equip);
            QuickHPCheck(-1); // 1 damage effect used.
        }

        [Test()]
        public void TestOneShotGearedUp()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Thri", "Parse", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();
            DestroyNonCharacterVillainCards();

            Card equip = PutInHand("LoadoutAnchor");
            PutInHand("LoadoutSlick"); // Make it a decision.
            DecisionSelectCards = new Card[] { baron.CharacterCard, null, equip, baron.CharacterCard, null };

            QuickHPStorage(baron);
            QuickHandStorage(parse);
            PlayCard("GearedUp");
            AssertIsInPlay(equip); // Play card by default.
            QuickHandCheck(1); // Parse has no equip, so will draw.
            QuickHPCheck(-1 - 2); // Default power and anchor on-play used.
        }

        [Test()]
        public void TestOneShotFromTheHip()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Thri", "Parse", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();
            DestroyNonCharacterVillainCards();

            DiscardAllCards(Thri);
            Card equip = PutOnDeck("RuleOfThree");

            QuickHPStorage(baron);
            PlayCard("FromTheHip");
            AssertIsInPlay(equip); // Drew and played card.
            QuickHPCheck(-2); // Default power used.
        }


        [Test()]
        public void TestOneShotSynchronizeStrike()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Thri", "Ra", "Fanatic", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();
            DestroyNonCharacterVillainCards();

            QuickHPStorage(baron);
            PlayCard("SynchronizeStrike");
            QuickHPCheck(-2 - 2); // Two 2 damage powers.
        }


        [Test()]
        public void TestOneShotThirdTimesTheCharm()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Thri", "Ra", "Fanatic", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();
            DestroyNonCharacterVillainCards();
            DiscardAllCards(Thri);

            QuickHandStorage(Thri);
            QuickHPStorage(Thri, baron);
            PlayCard("ThirdTimesTheCharm");
            QuickHandCheck(3);
            QuickHPCheck(-3, -2 - 2 - 2 - 3);
        }

        #endregion One-Shot Tests

        #region MoveUnderCards

        [Test()]
        public void TestUnderCardCoveringFire()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Thri", "Ra", "Fanatic", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();
            DestroyNonCharacterVillainCards();

            QuickHPStorage(Thri, ra, fanatic);
            PlayCard("CoveringFire");
            DealDamage(Thri, Thri, 4, DamageType.Projectile);
            DealDamage(Thri, ra, 4, DamageType.Projectile);
            DealDamage(Thri, ra, 4, DamageType.Projectile);
            DealDamage(Thri, fanatic, 4, DamageType.Projectile);
            DealDamage(Thri, fanatic, 4, DamageType.Projectile);
            QuickHPCheck(0, 0, 0);

            DealDamage(baron, Thri, 4, DamageType.Projectile);
            DealDamage(baron, ra, 1, DamageType.Projectile);
            DealDamage(baron, fanatic, 5, DamageType.Projectile);

            QuickHPCheck(-4 + 2, -1 + 1, -5 + 4);

            AssertNumberOfCardsInTrash(Thri, 1);
            AssertNumberOfCardsInTrash(ra, 2);
            AssertNumberOfCardsInTrash(fanatic, 2);
        }


        [Test()]
        public void TestUnderCardLeadTheTarget()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Thri", "Ra", "Fanatic", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();
            DestroyNonCharacterVillainCards();

            PlayCard("LeadTheTarget");
            UsePower(Thri);

            QuickHPStorage(baron);
            DealDamage(fanatic, baron, 2, DamageType.Fire);
            QuickHPCheck(-2);
            DealDamage(ra, baron, 2, DamageType.Fire);
            QuickHPCheck(-2 - 1);
            AssertNumberOfCardsInTrash(ra, 1);
            DealDamage(ra, baron, 2, DamageType.Fire);
            QuickHPCheck(-2);
        }
        #endregion MoveUnderCards

    }
}