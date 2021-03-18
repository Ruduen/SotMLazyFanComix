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
            AssertNumberOfCardsInDeck(Greyhat, 36);
            AssertNumberOfCardsInHand(Greyhat, 4);
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

            PlayCard("DigitalUplink");
            QuickHandStorage(Greyhat);
            UsePower(Greyhat);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestInnatePowerBurstNoise()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Greyhat/GreyhatBurstNoiseCharacter", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            DestroyCard(FindCardInPlay("MobileDefensePlatform"));
            PlayCard("CoercedUplink");
            QuickHPStorage(baron);
            UsePower(Greyhat);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestInnatePowerBurstNoiseNoLink()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Greyhat/GreyhatBurstNoiseCharacter", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            DestroyCard(FindCardInPlay("MobileDefensePlatform"));
            QuickHPStorage(baron);
            UsePower(Greyhat);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestInnatePowerBurstNoiseSkyScraperCounts()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Greyhat/GreyhatBurstNoiseCharacter", "SkyScraper", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            DestroyCard(FindCardInPlay("MobileDefensePlatform"));
            PlayCard("MicroAssembler");
            QuickHPStorage(baron);
            UsePower(Greyhat);
            QuickHPCheck(-2);
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

            DecisionSelectCards = new Card[] { allyLink, null, enemyLink, baron.CharacterCard };

            Card device = PlayCard("DeployedRelay");
            UsePower(device);
            AssertIsInPlay(allyLink);
            GoToEndOfTurn(Greyhat);
            AssertIsInPlay(enemyLink);
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

            DealDamage(mdp, device, 3, DamageType.Fire);

            DecisionSelectTarget = mdp;

            QuickHPStorage(device, mdp);
            UsePower(device);
            AssertInTrash(ongoing);

            GoToEndOfTurn(Greyhat);
            QuickHPCheck(2, -1);
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
            DiscardAllCards(Greyhat);

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
            // TO CHECK: SEED 210116367
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Greyhat", "Legacy", "TheScholar", "CaptainCosmic", "Unity", "Megalopolis"
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

            DecisionSelectCards = new Card[] { scholar.CharacterCard, null };
            PlayCard(uplinks[0]);

            ResetDecisions();
            DealDamage(baron, (Card c) => c.IsCharacter, 10, DamageType.Cold);
            PutOnDeck("CosmicCrest"); // Put on so Captain's power doesn't break.
            PutOnDeck("DDoS"); // Put on top so potential draw doesn't affect.
            DiscardAllCards(Greyhat);

            DecisionSelectCards = new Card[] { Greyhat.CharacterCard, siphon, uplinks[1], cosmic.CharacterCard };
            DecisionSelectPower = device;
            PlayCard("ShockTherapy");

            // TODO: Test convoluted case of damage dealing (Link played midway, link removed midway).
        }

        [Test()]
        public void TestPlayDataTransfer()
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
            DiscardAllCards(Greyhat);

            QuickHandStorage(Greyhat, legacy);
            PlayCard("DataTransfer");
            QuickHandCheck(2, 1);
        }

        [Test()]
        public void TestPlayDataTransferSentinelsStacked()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Greyhat", "TheSentinels", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            DestroyCard(FindCardInPlay("MobileDefensePlatform"));
            PlayCard("DigitalUplink", 0);
            PlayCard("DigitalUplink", 1);
            PlayCard("CoercedUplink");
            GoToPlayCardPhase(Greyhat);
            DiscardAllCards(Greyhat);

            QuickHandStorage(Greyhat, sentinels);
            PlayCard("DataTransfer");
            QuickHandCheck(2, 1);
        }

        [Test()]
        public void TestPlayDataTransferSentinelsVaried()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Greyhat", "TheSentinels", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            DestroyCard(FindCardInPlay("MobileDefensePlatform"));
            DecisionSelectCards = new Card[] { medico, null, idealist, null };
            DiscardAllCards(Greyhat);
            PlayCard("DigitalUplink", 0);
            PlayCard("DigitalUplink", 1);
            PlayCard("CoercedUplink");
            DiscardAllCards(Greyhat);

            QuickHandStorage(Greyhat, sentinels);
            PlayCard("DataTransfer");
            QuickHandCheck(2, 2);
        }

        [Test()]
        public void TestPlayDDoS()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Greyhat", "TheSentinels", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            DestroyCard(FindCardInPlay("MobileDefensePlatform"));
            Card target = PlayCard("BladeBattalion");
            DecisionSelectCards = new Card[] { medico, null, idealist, null };
            PlayCard("DigitalUplink", 0);
            PlayCard("DigitalUplink", 1);

            ResetDecisions();
            DecisionSelectCards = new Card[] { baron.CharacterCard, target };
            PlayCard("CoercedUplink", 0);
            PlayCard("CoercedUplink", 1);

            RestoreToMaxHP(target);
            ResetDecisions();
            DiscardAllCards(Greyhat);

            QuickHPStorage(baron.CharacterCard, target);
            PlayCard("DDoS");
            QuickHPCheck(-3, -3);
        }

        [Test()]
        public void TestPlayBandwidthHub()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Greyhat", "Ra", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            Card mdp = (FindCardInPlay("MobileDefensePlatform"));

            DiscardAllCards(Greyhat);
            PlayCard("CoercedUplink");
            PlayCard("BandwidthHub");

            QuickHPStorage(Greyhat, ra);
            DealDamage(baron, Greyhat, 3, DamageType.Fire);
            DealDamage(baron, ra, 3, DamageType.Fire);
            DealDamage(mdp, Greyhat, 3, DamageType.Fire);
            DealDamage(mdp, ra, 3, DamageType.Fire);
            QuickHPCheck(-5, -6);
        }

        [Test()]
        public void TestPlayOverclockSystems()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Greyhat", "Ra", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            DestroyCard(FindCardInPlay("MobileDefensePlatform"));
            Card prt = (PlayCard("PoweredRemoteTurret"));

            DiscardAllCards(Greyhat);
            PlayCard("CoercedUplink");
            PlayCard("OverclockHub");

            QuickHPStorage(baron.CharacterCard, prt);
            DealDamage(Greyhat, baron, 1, DamageType.Fire);
            DealDamage(Greyhat, prt, 1, DamageType.Fire);
            DealDamage(ra, baron, 1, DamageType.Fire);
            DealDamage(ra, prt, 1, DamageType.Fire);
            QuickHPCheck(-3, -2);
        }

        [Test()]
        public void TestPlayAutoRedirect()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Greyhat", "Ra", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            DestroyCard(FindCardInPlay("MobileDefensePlatform"));

            DiscardAllCards(Greyhat);
            PlayCard("CoercedUplink");
            PlayCard("AutoRedirectHub");

            QuickHPStorage(Greyhat, baron);
            DecisionRedirectTarget = baron.CharacterCard;
            DealDamage(ra, Greyhat, 1, DamageType.Fire);
            DealDamage(ra, Greyhat, 1, DamageType.Fire);
            QuickHPCheck(-1, -1);
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
            QuickHPCheck(-2);
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

        [Test()]
        public void TestPlaySystemReboot()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Greyhat", "TheSentinels", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            DiscardAllCards(Greyhat);
            DestroyCard(FindCardInPlay("MobileDefensePlatform"));

            ResetDecisions();
            DecisionSelectCard = baron.CharacterCard;
            Card[] plays = new Card[] { PlayCard("CoercedUplink", 0), PlayCard("CoercedUplink", 1) };
            Card search = GetCard("CoercedUplink", 3);
            PutInDeck(search);

            ResetDecisions();

            // Avoid search, since that's not the important part.

            Card card = PutInHand("SystemReboot");
            GoToPlayCardPhase(Greyhat);
            DecisionSelectCards = new Card[] { search, plays[0], plays[1], plays[0], plays[1] };
            QuickHPStorage(baron.CharacterCard);
            PlayCard(card);
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestPlayPingSweep()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Greyhat", "TheSentinels", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            PlayCard("PingSweep");
            AssertNumberOfCardsInPlay((Card c) => c.IsLink, 2);
        }

        [Test()]
        public void TestPlayNodeCheck()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Greyhat", "TheSentinels", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            DestroyCard(FindCardInPlay("MobileDefensePlatform"));

            Card play = PutInHand("CoercedUplink");
            Card search = PutOnDeck("DigitalUplink", true);

            DecisionSelectCards = new Card[] { search, play };

            QuickHandStorage(Greyhat);
            PlayCard("NodeCheck");
            QuickHandCheck(1); // Draw 2, play 1
            AssertIsInPlay(play);
        }

        #endregion Ungrouped Cards
    }
}