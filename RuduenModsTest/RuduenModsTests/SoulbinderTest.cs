using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using RuduenWorkshop.Soulbinder;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RuduenModsTest
{
    [TestFixture]
    public class SoulbinderTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
            ModHelper.AddAssembly("RuduenWorkshop", Assembly.GetAssembly(typeof(SoulbinderCharacterCardController))); // replace with your own namespace
        }

        protected HeroTurnTakerController Soulbinder { get { return FindHero("Soulbinder"); } }

        protected Card[] Soulshards { get { return new Card[] { GetCard("SoulshardOfLightningCharacter"), GetCard("SoulshardOfMercuryCharacter"), GetCard("SoulshardOfIronCharacter") }; } }

        protected Card SoulbinderMortal { get { return GetCard("SoulbinderMortalFormCharacter"); } }

        [Test(Description = "Basic Setup and Health")]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.Soulbinder", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(Soulbinder);
            Assert.IsInstanceOf(typeof(HeroTurnTakerController), Soulbinder);
            Assert.IsInstanceOf(typeof(SoulbinderCharacterCardController), Soulbinder.CharacterCardController);

            // No character cards, so just assert the four exist. 
            AssertNumberOfCardsInHand(Soulbinder, 4);
        }

        #region Basic Setup Tests

        [Test]
        public void TestSetupBasic()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.Soulbinder", "Megalopolis");

            DecisionSelectCard = Soulshards[0];

            StartGame(false);

            AssertNotInPlay(SoulbinderMortal);
            AssertIsInPlay(Soulshards);

            AssertNotFlipped(Soulshards[0]);
            AssertFlipped(Soulshards[1], Soulshards[2]);

            AssertNotIncapacitatedOrOutOfGame(Soulbinder);
        }

        [Test]
        public void TestSetupMortal()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.Soulbinder/SoulbinderMortalCharacter", "Megalopolis");

            DecisionSelectCard = Soulshards[0];

            StartGame(false);

            AssertIsInPlay(SoulbinderMortal);
            AssertIsInPlay(Soulshards);

            AssertNotFlipped(Soulshards[0]);
            AssertFlipped(Soulshards[1], Soulshards[2]);

            AssertNotIncapacitatedOrOutOfGame(Soulbinder);
        }

        #endregion Basic Setup Tests

        #region Multi-Character and Incap Tests

        [Test]
        public void TestIncapBasicOne()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.Soulbinder/SoulbinderMortalCharacter", "Megalopolis");

            DecisionSelectCards = Soulshards;

            StartGame(false);

            AssertIsInPlay(Soulshards);

            AssertNotFlipped(Soulshards[0]);
            AssertFlipped(Soulshards[1], Soulshards[2]);

            // First is destroyed, second flips to take its place.
            DestroyCard(Soulshards[0]);
            AssertOutOfGame(Soulshards[0]);
            AssertNotFlipped(Soulshards[1]);

            AssertNotIncapacitatedOrOutOfGame(Soulbinder);
            GoToPlayCardPhase(Soulbinder);
        }

        [Test]
        public void TestIncapMortalOne()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.Soulbinder/SoulbinderMortalCharacter", "Megalopolis");

            DecisionSelectCards = Soulshards;

            StartGame(false);

            AssertIsInPlay(SoulbinderMortal);
            AssertIsInPlay(Soulshards);

            AssertNotFlipped(Soulshards[0]);
            AssertFlipped(Soulshards[1], Soulshards[2]);

            // First is destroyed, second flips to take its place.
            DestroyCard(Soulshards[0]);
            AssertOutOfGame(Soulshards[0]);
            AssertNotFlipped(Soulshards[1]);

            AssertNotIncapacitatedOrOutOfGame(Soulbinder);
            GoToPlayCardPhase(Soulbinder);
        }


        [Test]
        public void TestIncapBasicAll()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.Soulbinder", "Legacy", "Megalopolis");

            DecisionSelectCards = Soulshards;

            StartGame(false);

            AssertIsInPlay(Soulshards);

            AssertNotFlipped(Soulshards[0]);
            AssertFlipped(Soulshards[1], Soulshards[2]);

            // First is destroyed, second flips to take its place.
            DestroyCard(Soulshards[0]);
            AssertOutOfGame(Soulshards[0]);
            AssertNotFlipped(Soulshards[1]);

            DestroyCard(Soulshards[1]);
            AssertOutOfGame(Soulshards[1]);
            AssertNotFlipped(Soulshards[2]);

            DestroyCard(Soulshards[2]);
            AssertFlipped(Soulshards[2]);
            AssertIsInPlay(Soulshards[2]);

            AssertIncapacitated(Soulbinder);

            GoToUseIncapacitatedAbilityPhase(Soulbinder);
        }

        [Test]
        public void TestIncapBasicAllRemoved()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.Soulbinder", "Legacy", "TheFinalWasteland");

            DecisionSelectCards = Soulshards;

            StartGame(false);

            AssertIsInPlay(Soulshards);

            AssertNotFlipped(Soulshards[0]);
            AssertFlipped(Soulshards[1], Soulshards[2]);

            // First is destroyed, second flips to take its place.
            DestroyCard(Soulshards[0]);
            AssertOutOfGame(Soulshards[0]);
            AssertNotFlipped(Soulshards[1]);

            DestroyCard(Soulshards[1]);
            AssertOutOfGame(Soulshards[1]);
            AssertNotFlipped(Soulshards[2]);
            
            Card card=PlayCard("UnforgivingWasteland");
            DealDamage(card, Soulshards[2], 100, DamageType.Melee);
            AssertOutOfGame(Soulshards[2]);

            AssertIncapacitated(Soulbinder);

            //GoToUseIncapacitatedAbilityPhase(Soulbinder);
            // THIS SHOULD CAUSE A UNIT TEST TO FAIL! 
        }

        [Test]
        public void TestIncapMortalAll()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.Soulbinder/SoulbinderMortalCharacter", "Legacy", "Megalopolis");

            DecisionSelectCards = Soulshards;

            StartGame(false);

            AssertIsInPlay(SoulbinderMortal);
            AssertIsInPlay(Soulshards);

            AssertNotFlipped(Soulshards[0]);
            AssertFlipped(Soulshards[1], Soulshards[2]);

            // First is destroyed, second flips to take its place.
            DestroyCard(Soulshards[0]);
            AssertOutOfGame(Soulshards[0]);
            AssertNotFlipped(Soulshards[1]);

            DestroyCard(Soulshards[1]);
            AssertOutOfGame(Soulshards[1]);
            AssertNotFlipped(Soulshards[2]);

            DestroyCard(Soulshards[2]);
            AssertIsInPlay(Soulshards[2]);
            AssertIsInPlay(Soulshards[2]);
            AssertNotFlipped(SoulbinderMortal);

            AssertNotIncapacitatedOrOutOfGame(Soulbinder);

            DestroyCard(SoulbinderMortal);
            AssertIncapacitated(Soulbinder);

            GoToUseIncapacitatedAbilityPhase(Soulbinder);
        }

        [Test]
        public void TestIncapMortalFirst()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.Soulbinder/SoulbinderMortalCharacter", "Legacy", "Megalopolis");

            DecisionSelectCards = Soulshards;

            StartGame(false);

            AssertIsInPlay(SoulbinderMortal);
            AssertIsInPlay(Soulshards);

            AssertNotFlipped(Soulshards[0]);
            AssertFlipped(Soulshards[1], Soulshards[2]);


            DestroyCard(SoulbinderMortal);

            AssertOutOfGame(Soulshards);
            AssertIsInPlay(SoulbinderMortal);

            AssertIncapacitated(Soulbinder);

            GoToUseIncapacitatedAbilityPhase(Soulbinder);
        }

        [Test]
        public void TestIncapMortalRemoved()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.Soulbinder/SoulbinderMortalCharacter", "Legacy", "TheFinalWasteland");

            DecisionSelectCards = Soulshards;

            StartGame(false);

            AssertIsInPlay(SoulbinderMortal);
            AssertIsInPlay(Soulshards);

            AssertNotFlipped(Soulshards[0]);
            AssertFlipped(Soulshards[1], Soulshards[2]);

            Card card = PlayCard("UnforgivingWasteland");
            DealDamage(card, SoulbinderMortal, 100, DamageType.Melee);
            AssertOutOfGame(SoulbinderMortal);

            // Still in play - one soulshard left!
            AssertNotIncapacitatedOrOutOfGame(Soulbinder);

            // First is destroyed, second flips to take its place.
            DestroyCard(Soulshards[0]);
            AssertOutOfGame(Soulshards[0]);
            AssertNotFlipped(Soulshards[1]);

            DestroyCard(Soulshards[1]);
            AssertOutOfGame(Soulshards[1]);
            AssertNotFlipped(Soulshards[2]);

            DestroyCard(Soulshards[2]);
            AssertIsInPlay(Soulshards[2]);
            AssertIsInPlay(Soulshards[2]);

            AssertIncapacitated(Soulbinder);

            GoToUseIncapacitatedAbilityPhase(Soulbinder);
        }


        #endregion Multi-Character and Incap Tests
    }
}