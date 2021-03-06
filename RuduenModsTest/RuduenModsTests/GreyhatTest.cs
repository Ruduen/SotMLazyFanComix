using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using RuduenWorkshop.Greyhat;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RuduenModsTest
{
    [TestFixture]
    public class GreyhatTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
            ModHelper.AddAssembly("RuduenWorkshop", Assembly.GetAssembly(typeof(GreyhatCharacterCardController))); // replace with your own namespace
        }

        protected HeroTurnTakerController Greyhat { get { return FindHero("Greyhat"); } }

        [Test(Description = "Basic Setup and Health")]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.Greyhat", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(Greyhat);
            Assert.IsInstanceOf(typeof(HeroTurnTakerController), Greyhat);
            Assert.IsInstanceOf(typeof(GreyhatCharacterCardController), Greyhat.CharacterCardController);

            Assert.AreEqual(23, Greyhat.CharacterCard.HitPoints);
            //AssertNumberOfCardsInDeck(Greyhat, 36);
            //AssertNumberOfCardsInHand(Greyhat, 4);
        }

        [Test()]
        public void TestInnatePowerDialUp()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Greyhat", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            QuickHandStorage(Greyhat);
            UsePower(Greyhat);
            QuickHandCheck(0);
            AssertNumberOfCardsInTrash(Greyhat, 1);
        }

        #region Devices

        [Test()]
        public void TestDeviceDeployedRelay()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Greyhat", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card allyLink = PutInHand("DigitalUplink");
            Card enemyLink = PutInTrash("CoercedUplink");

            DecisionSelectCards = new Card[] { allyLink, enemyLink, baron.CharacterCard };

            Card device = PlayCard("DeployedRelay");
            UsePower(device);
            AssertIsInPlay(allyLink);
            GoToEndOfTurn(Greyhat);
            AssertIsInPlay(enemyLink);
        }

        [Test()]
        public void TestDeviceInstantFirewall()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Greyhat", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            Card mdp = FindCardInPlay("MobileDefensePlatform");
            Card device = PlayCard("InstantFirewall");

            DecisionSelectTarget = mdp;
            UsePower(device);
            AssertHitPoints(mdp, 7);
            GoToEndOfTurn(Greyhat);
            AssertHitPoints(mdp, 6);
        }

        [Test()]
        public void TestDeviceProxyPod()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Greyhat", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            GoToPlayCardPhase(Greyhat);

            Card mdp = FindCardInPlay("MobileDefensePlatform");
            Card device = PlayCard("ProxyPod");
            Card ongoing = PlayCard("LivingForceField");

            UsePower(device);
            AssertInTrash(ongoing);

            DealDamage(mdp, device, 3, DamageType.Fire);

            QuickHPStorage(device);
            GoToEndOfTurn(Greyhat);
            QuickHPCheck(2);

        }

        #endregion Devices

        #region UsesLinkCards

        [Test()]
        public void TestPlayDirectControl()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Greyhat", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            DestroyCard(FindCardInPlay("MobileDefensePlatform"));
            PlayCard("DigitalUplink");
            PlayCard("CoercedUplink");
            GoToPlayCardPhase(Greyhat);

            QuickHPStorage(baron);
            PlayCard("DirectControl");
            QuickHPCheck(-2);
            AssertNumberOfUsablePowers(Greyhat, 0);
            AssertNumberOfUsablePowers(legacy, 0);
            // TODO: If necessary, add tests for bounce cases. This is using the same code as other similar areas.
        }

        [Test()]
        public void TestPlayShockTherapy()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Greyhat", "Legacy", "TheScholar", "CaptainCosmic","Unity", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            PlayCard("MortalFormToEnergy");
            PlayCard("BeeBot");
            DecisionSelectCard = Greyhat.CharacterCard;
            Card siphon = PlayCard("DynamicSiphon");
            Card device = PlayCard("DeployedRelay");
            // TODO: TEST CONVOLUTED CASE OF LINK REMOVAL (Scholar healing destroying bee-bot destroying link, Schollar healing trigger construct play) 
            Card[] uplinks = new Card[] { PutInHand(Greyhat, "DigitalUplink", 0), PutInHand(Greyhat, "DigitalUplink", 1), PutInHand(Greyhat, "DigitalUplink", 2), PutInHand(Greyhat, "DigitalUplink", 3) };

            DecisionSelectCard = scholar.CharacterCard;
            PlayCard(uplinks[0]);

            DealDamage(baron, (Card c) => c.IsCharacter, 10, DamageType.Cold);

            ResetDecisions();
            DecisionSelectCards = new Card[] { siphon, uplinks[1], cosmic.CharacterCard };
            DecisionSelectPower = device;
            PlayCard("ShockTherapy");

            // TODO: Test convoluted case of damage dealing (Link played midway, link removed midway). 
        }

        #endregion UsesLinkCards

        #region Ungrouped Cards

        [Test()]
        public void TestPlayCoercedUplink()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Greyhat", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card mdp = FindCardInPlay("MobileDefensePlatform");
            DecisionSelectCard = mdp;

            QuickHPStorage(mdp);
            Card link = PlayCard("CoercedUplink");
            QuickHPCheck(-1);
            AssertNextToCard(link, mdp);
        }

        [Test()]
        public void TestPlayDigitalUplink()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Greyhat", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            Card link = PlayCard("DigitalUplink");
            AssertNextToCard(link, legacy.CharacterCard);
            AssertNumberOfUsablePowers(legacy, 0); // Power was used.
        }

        #endregion Ungrouped Cards
    }
}