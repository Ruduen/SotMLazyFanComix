using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using LazyFanComix.Greyhat;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LazyFanComixTest
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
                "Omnitron", "LazyFanComix.Greyhat", "MrFixer", "ChronoRanger", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            QuickHandStorage(Greyhat);
            UsePower(Greyhat);
            QuickHandCheck(1);
            AssertNumberOfCardsInTrash(Greyhat, 1);
        }

        [Test()]
        public void TestInnatePowerDialUpTribunal()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "Legacy", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();

            SelectFromBoxForNextDecision("LazyFanComix.GreyhatCharacter", "LazyFanComix.Greyhat");
            PlayCard("CalledToJudgement");

            UsePower(FindCardInPlay("GreyhatCharacter"));
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

        [Test()]
        public void TestInnateBurstNoiseTribunal()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "Legacy", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();


            SelectFromBoxForNextDecision("LazyFanComix.GreyhatBurstNoiseCharacter", "LazyFanComix.Greyhat");
            PlayCard("CalledToJudgement");

            UsePower(FindCardInPlay("GreyhatCharacter"));
        }

        #region Link Cards
        [Test()]
        public void TestLinkAllMultiPlay()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "Infinitor", "LazyFanComix.Greyhat", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();

            // Confirm two plays. 
            DiscardAllCards(Greyhat);
            Card[] cards = new Card[] { PutInHand("CoercedUplink"), PutInHand("CommunicationRelay") };
            PlayCard(cards[0]);
            AssertIsInPlay(cards);

            // Confirm post bounce, replay of same card is fine. 
            cards = new Card[] { PutInHand("CoercedUplink"), PutInHand("CommunicationRelay") };
            PlayCard(cards[0]);
            AssertIsInPlay(cards[0]);
            AssertInHand(cards[1]);

            // Villain played first. 
            cards = new Card[] { PutInHand("CoercedUplink"), PutInHand("CommunicationRelay") };
            GoToStartOfTurn(Greyhat);
            Card swarm = PlayCard("OcularSwarm");
            PlayCard(cards[0]);
            AssertIsInPlay(cards[0]);
            AssertInHand(cards[1]);

            // Interruption via destroyed Ocular Swarm.
            cards = new Card[] { PutInHand("CoercedUplink"), PutInHand("CommunicationRelay") };
            SetHitPoints(swarm, 1);
            DecisionNextToCard = swarm;
            PutOnDeck("LambentReaper");
            GoToStartOfTurn(env);
            PlayCard(cards[0]);
            // Ocular swarm interrupts by playing, but 'first' should be set, so a play should be available.
            AssertIsInPlay(cards);

        }

        [Test()]
        public void TestLinkCoercedUplink()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "Omnitron", "LazyFanComix.Greyhat", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();

            DiscardAllCards(Greyhat);

            QuickHPStorage(omnitron);
            PlayCard("CoercedUplink");
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestLinkDigitalUplink()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "Omnitron", "LazyFanComix.Greyhat", "Legacy", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();

            DiscardAllCards(Greyhat);

            QuickHandStorage(legacy);
            PlayCard("DigitalUplink");
            QuickHandCheck(1);
        }

        [Test()]
        public void TestLinkDigitalUplinkTribunal()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "Omnitron", "LazyFanComix.Greyhat", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();

            DiscardAllCards(Greyhat);

            SelectFromBoxForNextDecision("LegacyCharacter", "Legacy");
            PlayCard("CalledToJudgement");

            PlayCard("DigitalUplink");
        }

        [Test()]
        public void TestLinkFlareRelay()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "Omnitron", "LazyFanComix.Greyhat", "Legacy", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();

            DiscardAllCards(Greyhat);

            QuickHPStorage(Greyhat, omnitron);
            PlayCard("FlareRelay");
            QuickHPCheck(-1, -1);
            UsePower(Greyhat, 1);
            QuickHPCheck(0, -3);
        }

        [Test()]
        public void TestLinkProxyRelay()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "Omnitron", "LazyFanComix.Greyhat", "Legacy", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();

            DiscardAllCards(Greyhat);

            DealDamage(Greyhat, Greyhat, 5, DamageType.Fire);
            QuickHPStorage(Greyhat);
            QuickHandStorage(Greyhat);
            PlayCard("ProxyRelay");
            QuickHPCheck(2);

            UsePower(Greyhat, 1);
            QuickHandCheck(1);
            DealDamage(Greyhat, Greyhat, 5, DamageType.Fire);
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestLinkCommunicationRelay()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "Omnitron", "LazyFanComix.Greyhat", "Legacy", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();

            DiscardAllCards(Greyhat);
            Card destroyCard = PlayCard("InterpolationBeam");

            PlayCard("CommunicationRelay");
            Card play = PutInHand("DigitalUplink");
            AssertInTrash(destroyCard);
            UsePower(Greyhat, 1);
            AssertIsInPlay(play);

        }

        #endregion Link Cards

        #region UsesLinkCards

        [Test()]
        public void TestNetworkAllMultiPlay()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "Infinitor", "LazyFanComix.Greyhat", "RealmOfDiscord"
            };
            SetupGameController(setupItems);

            StartGame();

            int draws = 2;

            // Confirm Draw 2.
            Card play = PutInTrash("DirectControl");

            QuickHandStorage(Greyhat);
            PlayCard(play);
            QuickHandCheck(draws);

            // Confirm replay of same card is fine.
            PlayCard(play);
            QuickHandCheck(0);

            // Villain played first. 
            GoToStartOfTurn(Greyhat);
            Card swarm = PlayCard("OcularSwarm");
            PlayCard(play);
            QuickHandCheck(0);

            // Interruption via destroyed Ocular Swarm.
            DecisionNextToCard = swarm;
            DiscardAllCards(Greyhat);
            PutOnDeck("LambentReaper");
            PlayCard("CoercedUplink");
            SetHitPoints(swarm, 1);
            PutOnDeck("LambentReaper");
            GoToStartOfTurn(env);
            QuickHandStorage(Greyhat);
            PlayCard(play);
            // Ocular swarm interrupts by playing, but 'first' should be set, so a play should be available.
            QuickHandCheck(draws);

        }

        [Test()]
        public void TestNetworkDirectControl()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Greyhat", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            DestroyCard(FindCardInPlay("MobileDefensePlatform"));
            DiscardAllCards(Greyhat);

            PlayCard("DigitalUplink");
            PlayCard("CoercedUplink");

            QuickHPStorage(baron);
            PlayCard("DirectControl");
            QuickHPCheck(-2);
            AssertNumberOfUsablePowers(Greyhat, 1);
            AssertNumberOfUsablePowers(legacy, 0);
        }

        [Test()]
        public void TestNetworkShockTherapy()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "Omnitron", "LazyFanComix.Greyhat", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            DiscardAllCards(Greyhat);

            PlayCard("DigitalUplink");
            PlayCard("CoercedUplink");
            PlayCard("FlareRelay");

            DealDamage(legacy, legacy, 10, DamageType.Fire);
            DealDamage(legacy, Greyhat, 10, DamageType.Fire);
            DealDamage(legacy, omnitron, 10, DamageType.Fire);

            QuickHPStorage(Greyhat, legacy, omnitron);
            PlayCard("ShockTherapy");
            QuickHPCheck(2, 2, -2);
        }

        //[Test()]
        //public void TestNetworkShockTherapyWeirdEventually()
        //{
        //    // TO CHECK: SEED 210116367
        //    IEnumerable<string> setupItems = new List<string>()
        //    {
        //        "BaronBlade", "LazyFanComix.Greyhat", "Legacy", "TheScholar", "CaptainCosmic", "Unity", "Megalopolis"
        //    };
        //    SetupGameController(setupItems);

        //    StartGame();

        //    // CONVOLUTED CASE OF LINK REMOVAL (Scholar healing destroying bee-bot destroying link, Scholar healing trigger construct play)
        //    PlayCard("MortalFormToEnergy");
        //    PlayCard("BeeBot");
        //    DecisionSelectCards = new Card[] { Greyhat.CharacterCard, null, scholar.CharacterCard, null };
        //    Card siphon = PlayCard("DynamicSiphon");
        //    Card playPower = PlayCard("CommunicationRelay");
        //    Card[] uplinks = new Card[] { PutInHand(Greyhat, "DigitalUplink", 0), PutInHand(Greyhat, "DigitalUplink", 1), PutInHand(Greyhat, "DigitalUplink", 2), PutInHand(Greyhat, "DigitalUplink", 3) };
        //    PlayCard(uplinks[0]);

        //    ResetDecisions();
        //    DealDamage(baron, (Card c) => c.IsCharacter, 10, DamageType.Cold);
        //    PutOnDeck("CosmicCrest"); // Put on so Captain's power doesn't break.
        //    PutOnDeck("DDoS"); // Put on top so potential draw doesn't affect.
        //    DiscardAllCards(Greyhat);

        //    DecisionSelectCards = new Card[] { Greyhat.CharacterCard, siphon, uplinks[1], cosmic.CharacterCard };
        //    DecisionSelectPower = playPower;
        //    PlayCard("ShockTherapy");

        //    // TODO: Test convoluted case of damage dealing (Link played midway, link removed midway).
        //}


        [Test()]
        public void TestPlayDataTransfer()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "Omnitron", "LazyFanComix.Greyhat", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            DiscardAllCards(Greyhat);
            PlayCard("DigitalUplink");
            PlayCard("CoercedUplink");

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
            DiscardAllCards(Greyhat);
            PlayCard("DigitalUplink", 0);
            PlayCard("DigitalUplink", 1);
            PlayCard("CoercedUplink");

            QuickHandStorage(Greyhat, sentinels);
            PlayCard("DataTransfer");
            QuickHandCheck(1, 1);
        }

        [Test()]
        public void TestNetworkDataTransferSentinelsVaried()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Greyhat", "TheSentinels", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            DestroyCard(FindCardInPlay("MobileDefensePlatform"));
            DiscardAllCards(Greyhat);
            DecisionNextToCard = medico;
            PlayCard("DigitalUplink", 0);
            DecisionNextToCard = mainstay;
            PlayCard("DigitalUplink", 1);
            PlayCard("CoercedUplink");

            QuickHandStorage(Greyhat, sentinels);
            PlayCard("DataTransfer");
            QuickHandCheck(1, 2);
        }

        [Test()]
        public void TestNetworkDDoS()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Greyhat", "TheSentinels", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            DiscardAllCards(Greyhat);
            DestroyCard(FindCardInPlay("MobileDefensePlatform"));
            Card target = PlayCard("BladeBattalion");

            DecisionNextToCard = medico;
            PlayCard("DigitalUplink", 0);
            DecisionNextToCard = mainstay;
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

        #endregion UsesLinkCards

        #region UsesLinkCardsOngoing


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
            DecisionSelectCards = new Card[] { baron.CharacterCard };
            PlayCard("CoercedUplink");
            PlayCard("BandwidthRestriction");

            ResetDecisions();

            QuickHPStorage(Greyhat, ra);
            DealDamage(baron, Greyhat, 3, DamageType.Fire);
            DealDamage(baron, ra, 3, DamageType.Fire);
            QuickHPCheck(-2, -3);
            DealDamage(mdp, Greyhat, 3, DamageType.Fire);
            DealDamage(mdp, ra, 3, DamageType.Fire);
            QuickHPCheck(-3, -3);
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
            DecisionSelectCards = new Card[] { baron.CharacterCard };
            PlayCard("CoercedUplink");
            PlayCard("OverclockSystems");

            ResetDecisions();

            QuickHPStorage(baron.CharacterCard, prt);
            DealDamage(Greyhat, baron, 1, DamageType.Fire);
            QuickHPCheck(-2, 0);
            DealDamage(Greyhat, prt, 1, DamageType.Fire);
            QuickHPCheck(0, -1);
            DealDamage(ra, baron, 1, DamageType.Fire);
            QuickHPCheck(-1, 0);
            DealDamage(ra, prt, 1, DamageType.Fire);
            QuickHPCheck(0, -1);
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
            PlayCard("CoercedUplink");
            PlayCard("AutoRedirect");

            ResetDecisions();

            QuickHPStorage(Greyhat, baron);
            DecisionRedirectTarget = baron.CharacterCard;
            DealDamage(ra, Greyhat, 1, DamageType.Fire);
            DealDamage(ra, Greyhat, 1, DamageType.Fire);
            QuickHPCheck(-1, -1);
        }

        #endregion UsesLinkCardsOngoing

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