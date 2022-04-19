using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using LazyFanComix.Orbit;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LazyFanComixTest
{
    [TestFixture]
    public class OrbitTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
            ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(OrbitCharacterCardController))); // replace with your own namespace
        }

        protected HeroTurnTakerController Orbit { get { return FindHero("Orbit"); } }

        [Test(Description = "Basic Setup and Health")]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Orbit", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(Orbit);
            Assert.IsInstanceOf(typeof(HeroTurnTakerController), Orbit);
            Assert.IsInstanceOf(typeof(OrbitCharacterCardController), Orbit.CharacterCardController);

            Assert.AreEqual(28, Orbit.CharacterCard.HitPoints);
            AssertNumberOfCardsInDeck(Orbit, 36);
            AssertNumberOfCardsInHand(Orbit, 4);
        }

        #region Innate Powers

        [Test()]
        public void TestInnatePowerBase()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Orbit", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            QuickHPStorage(mdp);
            UsePower(Orbit);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestInnatePowerSatellite()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Orbit/LazyFanComix.OrbitArtificialSatelliteCharacter", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            DecisionSelectTarget = Orbit.CharacterCard;
            QuickHPStorage(Orbit);
            UsePower(Orbit);
            QuickHPCheck(-1);

            QuickHPStorage(Orbit);
            DealDamage(Orbit, Orbit, 2, DamageType.Projectile);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestTribunalPowerBase()
        {
            SetupGameController("BaronBlade", "Guise", "TheCelestialTribunal");

            StartGame();
            AvailableHeroes = DeckDefinition.AvailableHeroes.Concat(new string[] { "LazyFanComix.Orbit" });
            SelectFromBoxForNextDecision("LazyFanComix.OrbitCharacter", "LazyFanComix.Orbit");
            DiscardAllCards(guise);
            PutInHand("UhYeahImThatGuy");

            Card mdp = FindCardInPlay("MobileDefensePlatform");
            DecisionSelectCards = new Card[] { guise.CharacterCard, mdp, guise.CharacterCard, baron.CharacterCard, mdp, guise.CharacterCard, baron.CharacterCard };
            QuickHPStorage(mdp);

            PlayCard("CalledToJudgement");

            Card representative = FindCardInPlay("OrbitCharacter");
            AssertIsInPlay(representative);

            UsePower(representative);
        }

        [Test()]
        public void TestTribunalPowerSatellite()
        {
            SetupGameController("BaronBlade", "Guise", "TheCelestialTribunal");

            StartGame();
            AvailableHeroes = DeckDefinition.AvailableHeroes.Concat(new string[] { "LazyFanComix.Orbit" });
            SelectFromBoxForNextDecision("LazyFanComix.OrbitArtificialSatelliteCharacter", "LazyFanComix.Orbit");
            DiscardAllCards(guise);

            PlayCard("CalledToJudgement");

            Card representative = FindCardInPlay("OrbitCharacter");
            AssertIsInPlay(representative);

            UsePower(representative);

        }
        #endregion Innate Powers

        #region Covers

        [Test()]
        public void TestCoverFracturedBackdrop()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Orbit", "TheCelestialTribunal");

            StartGame();

            Card cover = PlayCard("FracturedBackdrop");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionSelectTargets = new Card[] { mdp, baron.CharacterCard, Orbit.CharacterCard, cover, mdp, baron.CharacterCard, Orbit.CharacterCard, cover };


            QuickHPStorage(mdp);
            DealDamage(Orbit.CharacterCard, cover, 1, DamageType.Melee);
            QuickHPCheck(-2);
            DestroyCard(cover);
            QuickHPCheck(-1);

        }

        [Test()]
        public void TestCoverHeavyBlockade()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Orbit", "TheCelestialTribunal");

            StartGame();

            Card cover = PlayCard("HeavyBlockade");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionSelectTarget = mdp;

            QuickHPStorage(mdp);
            DealDamage(Orbit.CharacterCard, cover, 1, DamageType.Melee);
            QuickHPCheck(-4);
            DestroyCard(cover);
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestCoverVolatileBarricade()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Orbit", "TheCelestialTribunal");

            StartGame();

            Card cover = PlayCard("VolatileBarricade");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card[] ongoings = new Card[] { PlayCard("LivingForceField") };
            DecisionSelectTargets = new Card[] { mdp, baron.CharacterCard, mdp, baron.CharacterCard };

            QuickHPStorage(mdp);
            DealDamage(Orbit.CharacterCard, cover, 1, DamageType.Melee);
            QuickHPCheck(-2);
            DestroyCard(cover);
            AssertInTrash(ongoings);
            QuickHPCheck(-1);

        }
        #endregion Covers

        #region Orbitals

        [Test()]
        public void TestOrbitalCollisionCourse()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Orbit", "WagnerMarsBase");

            StartGame();
            PlayCard("VillainousWeaponry");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card target = PlayCard("BladeBattalion");
            DecisionSelectCard = mdp;
            DecisionSelectTarget = target;

            QuickHPStorage(mdp, target);
            Card orbital = PlayCard("CollisionCourse");
            QuickHPCheck(-2, 0);
            DealDamage(mdp, mdp, 0, DamageType.Melee);
            QuickHPCheck(-1, -3 - 1);
            AssertInTrash(orbital);
        }

        [Test()]
        public void TestOrbitalCollisionCourseSkip()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Orbit", "Legacy","WagnerMarsBase");

            StartGame();
            PlayCard("VillainousWeaponry");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card target = PlayCard("BladeBattalion");
            DecisionSelectCard = mdp;
            DecisionSelectTargets = new Card[] { null };

            QuickHPStorage(mdp, target);
            Card orbital = PlayCard("CollisionCourse");
            QuickHPCheck(-2, 0);
            DealDamage(mdp, mdp, 0, DamageType.Melee);
            QuickHPCheck(-1, 0);
            AssertIsInPlay(orbital);
        }

        [Test()]
        public void TestOrbitalGuideTheBlow()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Orbit", "WagnerMarsBase");

            StartGame();
            PlayCard("VillainousWeaponry");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card target = PlayCard("BladeBattalion");
            DecisionSelectCard = mdp;
            DecisionRedirectTarget = target;

            QuickHPStorage(mdp, target);
            Card orbital = PlayCard("GuideTheBlow");
            QuickHPCheck(-2, 0);
            DealDamage(mdp, Orbit, 2, DamageType.Melee);
            QuickHPCheck(0, -2 - 1);
            AssertInTrash(orbital);
        }

        [Test()]
        public void TestOrbitalGuideTheBlowSkip()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Orbit", "WagnerMarsBase");

            StartGame();
            PlayCard("VillainousWeaponry");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card target = PlayCard("BladeBattalion");
            DecisionSelectCard = mdp;

            QuickHPStorage(mdp, target, Orbit.CharacterCard);
            Card orbital = PlayCard("GuideTheBlow");
            QuickHPCheck(-2, 0, 0);
            DealDamage(mdp, Orbit, 2, DamageType.Melee);
            QuickHPCheck(0, 0, -2 - 1);
            AssertIsInPlay(orbital);
        }


        [Test()]
        public void TestOrbitalCorrectiveMotion()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Orbit", "WagnerMarsBase");

            StartGame();
            PlayCard("VillainousWeaponry");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionSelectCard = mdp;
            DecisionYesNo = true;

            QuickHPStorage(mdp);
            Card orbital = PlayCard("CorrectiveMotion");
            QuickHPCheck(-2);
            DealDamage(mdp, mdp, 2, DamageType.Melee);
            QuickHPCheck(-2 - 1 - 2);
            AssertInTrash(orbital);
        }

        [Test()]
        public void TestOrbitalCorrectiveMotionSkip()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Orbit", "WagnerMarsBase");

            StartGame();
            PlayCard("VillainousWeaponry");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionSelectCard = mdp;
            DecisionYesNo = false;

            QuickHPStorage(mdp);
            Card orbital = PlayCard("CorrectiveMotion");
            QuickHPCheck(-2);
            DealDamage(mdp, mdp, 2, DamageType.Melee);
            QuickHPCheck(-2 - 1);
            AssertIsInPlay(orbital);
        }
        #endregion Orbitals

        #region Limited

        [Test()]
        public void TestLimitedOrbitalSlingshot()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Orbit", "WagnerMarsBase");

            StartGame();

            DestroyCard("MobileDefensePlatform");
            Card orbital = PlayCard("GuideTheBlow");

            Card limited = PlayCard("OrbitalSlingshot");

            DecisionYesNo = true;
            DestroyCard(orbital);

            AssertAtLocation(orbital, limited.UnderLocation);

            QuickHPStorage(baron);
            GoToEndOfTurn(Orbit);
            QuickHPCheck(-3);


        }

        [Test()]
        public void TestLimitedSubtlePreparation()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Orbit", "WagnerMarsBase");

            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card[] plays = new Card[] { PutInHand("RapidFire"), PutInHand("WreckTheWreckage") };
            DecisionSelectCards = plays;
            Card limited = PutInHand("SubtlePreparation");

            QuickHPStorage(mdp);
            QuickHandStorage(Orbit);
            PlayCard(limited);
            DealDamage(Orbit, mdp, 2, DamageType.Melee);
            QuickHPCheck(0);
            QuickHandCheck(-3 + 2);
            AssertInTrash(plays);

            DecisionSelectFunction = 1;
            UsePower(limited);
            QuickHandCheck(1);

            GoToStartOfTurn(Orbit);
            AssertInTrash(limited);

            DealDamage(Orbit, mdp, 2, DamageType.Melee);
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestLimitedControlTheField()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Orbit", "WagnerMarsBase");

            StartGame();

            Card limited = PlayCard("ControlTheField");
            Card cover = PlayCard("HeavyBlockade");

            QuickHPStorage(Orbit);
            DealDamage(Orbit, Orbit, 3, DamageType.Melee);
            QuickHPCheck(-2);

            UsePower(limited);
            AssertInTrash(limited);
            AssertNumberOfCardsInPlay((Card c) => c.IsCover, 3);

            PlayCard(limited);
            DealDamage(Orbit, Orbit, 3, DamageType.Melee);
            QuickHPCheck(0);

            GoToStartOfTurn(Orbit);
            AssertNumberOfCardsInPlay((Card c) => c.IsCover, 1);

        }

        [Test()]
        public void TestLimitedObjectsInMotion()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Orbit", "Ra", "WagnerMarsBase");

            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card play = PutInHand("BlazingTornado");
            DecisionSelectTurnTaker = ra.TurnTaker;
            DecisionSelectCard = play;
            DecisionSelectTarget = mdp;

            QuickHPStorage(mdp);

            Card limited = PlayCard("ObjectsInMotion");
            DealDamage(mdp, mdp, 1, DamageType.Melee);
            QuickHPCheck(-1);

            GoToStartOfTurn(Orbit);

            DealDamage(Orbit.CharacterCard, mdp, 1, DamageType.Melee);
            QuickHPCheck(-1);

            DealDamage(mdp, mdp, 1, DamageType.Melee);
            UsePower(limited);
            QuickHPCheck(-2 - 1 - 1 - 1);
            AssertIsInPlay(play);
            AssertInTrash(limited);
        }

        [Test()]
        public void TestLimitedLandscapeAwareness()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Orbit", "Legacy", "WagnerMarsBase");

            StartGame();

            Card limited = PlayCard("LandscapeAwareness");
            DecisionSelectTarget = Orbit.CharacterCard;

            UsePower(legacy);
            QuickHPStorage(Orbit);
            DealDamage(Orbit, Orbit, 2, DamageType.Melee);
            QuickHPCheck(-2);

            QuickHandStorage(Orbit);
            UsePower(limited);
            QuickHandCheck(1);
            QuickHPCheck(-1);
        }

        #endregion Limited
        #region One-Shots

        [Test()]
        public void TestOneShotRedesignate()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Orbit", "Ra", "WagnerMarsBase");

            StartGame();

            Card play = PutInHand("Redesignate");
            DestroyCard("MobileDefensePlatform");

            Card[] orbitals = new Card[] { PutInHand("CollisionCourse"), PlayCard("GuideTheBlow"), PlayCard("CorrectiveMotion") };

            DecisionSelectFunction = 0;
            DecisionSelectCards = new Card[] { orbitals[1], orbitals[2], orbitals[0], baron.CharacterCard, orbitals[1], baron.CharacterCard };

            QuickHPStorage(baron);
            QuickHandStorage(Orbit);
            PlayCard(play);
            QuickHPCheck(-4);
            QuickHandCheck(1); // Play -1, Return +2, Draw +2, Play -2
        }

        [Test()]
        public void TestOneShotRapidFire()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Orbit", "Legacy", "WagnerMarsBase");

            StartGame();

            DestroyCard("MobileDefensePlatform");
            QuickHPStorage(baron);

            UsePower(legacy);

            QuickHPStorage(baron);
            PlayCard("RapidFire");
            QuickHPCheck(-1 - 1 - 2 - 1);
        }

        [Test()]
        public void TestOneShotWreckTheWreckage()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Orbit", "WagnerMarsBase");

            StartGame();

            DestroyCard("MobileDefensePlatform");
            QuickHPStorage(baron);
            Card cover = PutInTrash("HeavyBlockade");

            QuickHPStorage(baron);
            PlayCard("WreckTheWreckage");
            QuickHPCheck(-1 -1);
            AssertIsInPlay(cover);
        }
        #endregion One-Shots

    }
}