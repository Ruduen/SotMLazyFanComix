using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using LazyFanComix.Greyhat;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LazyFanComixText
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
            ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(GreyhatCharacterCardController))); // replace with your own namespace
        }

        protected HeroTurnTakerController Greyhat { get { return FindHero("Greyhat"); } }

        [Test(Description = "Basic Setup and Health")]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Greyhat", "Megalopolis");

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
                "BaronBlade", "LazyFanComix.Greyhat", "Legacy", "Megalopolis"
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
                "BaronBlade", "LazyFanComix.Greyhat/GreyhatBurstNoiseCharacter", "Legacy", "Megalopolis"
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
                "BaronBlade", "LazyFanComix.Greyhat/GreyhatBurstNoiseCharacter", "Legacy", "Megalopolis"
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
                "BaronBlade", "LazyFanComix.Greyhat/GreyhatBurstNoiseCharacter", "SkyScraper", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            DestroyCard(FindCardInPlay("MobileDefensePlatform"));
            PlayCard("MicroAssembler");
            QuickHPStorage(baron);
            UsePower(Greyhat);
            QuickHPCheck(-2);
        }

        #region UsesLinkCards

        [Test()]
        public void TestPlayDirectControl()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Greyhat", "Legacy", "Megalopolis"
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
            AssertNumberOfUsablePowers(Greyhat, 1);
            AssertNumberOfUsablePowers(legacy, 0);
            // TODO: If necessary, add tests for bounce cases. This is using the same code as other similar areas.
        }

        [Test()]
        public void TestPlayShockTherapy()
        {
            // TO CHECK: SEED 210116367
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Greyhat", "Legacy", "TheScholar", "CaptainCosmic", "Unity", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            PlayCard("MortalFormToEnergy");
            PlayCard("BeeBot");
            DecisionSelectCards = new Card[] { Greyhat.CharacterCard, null, scholar.CharacterCard, null };
            Card siphon = PlayCard("DynamicSiphon");
            Card playPower = PlayCard("CommunicationRelay");
            // TODO: TEST CONVOLUTED CASE OF LINK REMOVAL (Scholar healing destroying bee-bot destroying link, Scholar healing trigger construct play)
            Card[] uplinks = new Card[] { PutInHand(Greyhat, "DigitalUplink", 0), PutInHand(Greyhat, "DigitalUplink", 1), PutInHand(Greyhat, "DigitalUplink", 2), PutInHand(Greyhat, "DigitalUplink", 3) };
            PlayCard(uplinks[0]);

            ResetDecisions();
            DealDamage(baron, (Card c) => c.IsCharacter, 10, DamageType.Cold);
            PutOnDeck("CosmicCrest"); // Put on so Captain's power doesn't break.
            PutOnDeck("DDoS"); // Put on top so potential draw doesn't affect.
            DiscardAllCards(Greyhat);

            DecisionSelectCards = new Card[] { Greyhat.CharacterCard, siphon, uplinks[1], cosmic.CharacterCard };
            DecisionSelectPower = playPower;
            PlayCard("ShockTherapy");

            // TODO: Test convoluted case of damage dealing (Link played midway, link removed midway).
        }

        [Test()]
        public void TestPlayDataTransfer()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Greyhat", "Legacy", "Megalopolis"
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
            QuickHandCheck(1, 1);
        }

        [Test()]
        public void TestPlayDataTransferSentinelsStacked()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Greyhat", "TheSentinels", "Megalopolis"
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
            QuickHandCheck(1, 1);
        }

        [Test()]
        public void TestPlayDataTransferSentinelsVaried()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Greyhat", "TheSentinels", "Megalopolis"
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
            QuickHandCheck(1, 2);
        }

        [Test()]
        public void TestPlayDDoS()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Greyhat", "TheSentinels", "Megalopolis"
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
            QuickHPCheck(-2, -2);
        }

        [Test()]
        public void TestPlayBandwidthRestriction()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Greyhat", "Ra", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            Card mdp = (FindCardInPlay("MobileDefensePlatform"));

            DiscardAllCards(Greyhat);
            PlayCard("ProxyRelay");
            DecisionSelectCards = new Card[] { PutInHand("CoercedUplink"), baron.CharacterCard };
            PlayCard("BandwidthRestriction");

            ResetDecisions();

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
                "BaronBlade", "LazyFanComix.Greyhat", "Ra", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            DestroyCard(FindCardInPlay("MobileDefensePlatform"));
            Card prt = (PlayCard("PoweredRemoteTurret"));

            DiscardAllCards(Greyhat);
            PlayCard("ProxyRelay");
            DecisionSelectCards = new Card[] { PutInHand("CoercedUplink"), baron.CharacterCard };
            PlayCard("OverclockSystems");

            ResetDecisions();

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
                "BaronBlade", "LazyFanComix.Greyhat", "Ra", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            DestroyCard(FindCardInPlay("MobileDefensePlatform"));

            DiscardAllCards(Greyhat);
            PlayCard("ProxyRelay");
            DecisionSelectCard = PutInHand("CoercedUplink");
            PlayCard("AutoRedirect");

            ResetDecisions();

            QuickHPStorage(Greyhat, baron);
            DecisionRedirectTarget = baron.CharacterCard;
            DealDamage(ra, Greyhat, 1, DamageType.Fire);
            DealDamage(ra, Greyhat, 1, DamageType.Fire);
            QuickHPCheck(-1, -1);
        }

        #endregion UsesLinkCards

        #region Link Cards

        [Test()]
        public void TestPlayCoercedUplink()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Greyhat", "Legacy", "Megalopolis"
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
                "BaronBlade", "LazyFanComix.Greyhat", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            Card link = PlayCard("DigitalUplink");
            AssertNextToCard(link, legacy.CharacterCard);
            AssertNumberOfUsablePowers(legacy, 0); // Power was used.
        }




        [Test()]
        public void TestPlayFlareRelay()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Greyhat", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            DestroyCard(FindCardInPlay("MobileDefensePlatform"));

            QuickHPStorage(Greyhat, legacy, baron);
            Card link = PlayCard("FlareRelay");
            AssertNextToCard(link, Greyhat.CharacterCard);
            QuickHPCheck(-1, -1, -1);

            QuickHPStorage(baron);
            UsePower(Greyhat, 1);
            QuickHPCheck(-3);
        }





        [Test()]
        public void TestPlayProxyRelay()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Greyhat", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            DestroyCard(FindCardInPlay("MobileDefensePlatform"));

            DealDamage(Greyhat, Greyhat, 5, DamageType.Melee);
            QuickHPStorage(Greyhat);
            Card link = PlayCard("ProxyRelay");
            AssertNextToCard(link, Greyhat.CharacterCard);
            QuickHPCheck(2);

            QuickHPStorage(Greyhat);
            QuickHandStorage(Greyhat);
            UsePower(Greyhat, 1);
            DealDamage(Greyhat, Greyhat, 2, DamageType.Melee);
            QuickHPCheck(-1);
            QuickHandCheck(1);
        }



        [Test()]
        public void TestPlayCommunicationRelay()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Greyhat", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            DestroyCard(FindCardInPlay("MobileDefensePlatform"));

            Card ongoing = PlayCard("LivingForceField");
            Card link = PlayCard("CommunicationRelay");
            AssertNextToCard(link, Greyhat.CharacterCard);
            AssertInTrash(ongoing);

            Card play = PutInHand("DDoS");
            DecisionSelectCards = new Card[] { play, null };
            UsePower(Greyhat, 1);
            AssertInTrash(play);
        }

        #endregion Link Cards

        #region Ungrouped Cards
        [Test()]
        public void TestPlaySystemReboot()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Greyhat", "TheSentinels", "Megalopolis"
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
                "BaronBlade", "LazyFanComix.Greyhat", "TheSentinels", "Megalopolis"
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
                "BaronBlade", "LazyFanComix.Greyhat", "TheSentinels", "Megalopolis"
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