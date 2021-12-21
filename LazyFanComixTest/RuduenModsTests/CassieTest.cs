using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using LazyFanComix.Cassie;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LazyFanComixTest
{
    [TestFixture]
    public class CassieTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
            ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(CassieCharacterCardController))); // replace with your own namespace
        }

        protected HeroTurnTakerController Cassie { get { return FindHero("Cassie"); } }

        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Cassie", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(Cassie);
            Assert.IsInstanceOf(typeof(HeroTurnTakerController), Cassie);
            Assert.IsInstanceOf(typeof(CassieCharacterCardController), Cassie.CharacterCardController);

            Assert.AreEqual(27, Cassie.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestSetupWorks()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Cassie", "Megalopolis");

            StartGame();

            AssertNumberOfCardsInDeck(Cassie, 2); // Should start with 2 card in deck.
            AssertNumberOfCardsInHand(Cassie, 4); // And 4 cards in hand.
            AssertNumberOfCardsAtLocation(GetCard("Riverbank").UnderLocation, 4); // And 4 cards in the Riverbank.
            AssertNumberOfCardsAtLocation(Cassie.TurnTaker.FindSubDeck("RiverDeck"), 30); // And 30 cards in the River Deck.
        }

        [Test()]
        public void TestSetupOblivaeon()
        {
            SetupGameController("OblivAeon", "Legacy", "TheWraith", "Megalopolis", "TimeCataclysm");

            StartGame();

            SelectFromBoxForNextDecision("LazyFanComix.CassieCharacter", "LazyFanComix.Cassie");
            SelectYesNoForNextDecision(false);

            DestroyCard(legacy);
            GoToAfterEndOfTurn(legacy);
            RunActiveTurnPhase();
            GoToNextTurn();

            AssertNumberOfCardsInDeck(Cassie, 2); // Should start with 2 card in deck.
            AssertNumberOfCardsInHand(Cassie, 4); // And 4 cards in hand.
            AssertNumberOfCardsAtLocation(GetCard("Riverbank").UnderLocation, 4); // And 4 cards in the Riverbank.
            AssertNumberOfCardsAtLocation(Cassie.TurnTaker.FindSubDeck("RiverDeck"), 30); // And 30 cards in the River Deck.
        }

        [Test()]
        public void TestInnatePower()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Cassie", "Megalopolis");

            StartGame();

            AssertNumberOfCardsInDeck(Cassie, 2); // Should start with 2 card in deck.
            MoveCards(Cassie, (Card c) => c.Location == GetCard("Riverbank").UnderLocation, Cassie.TurnTaker.FindSubDeck("RiverDeck"), numberOfCards: 4, overrideIndestructible: true); // Move all cards back to the river deck just in case.
            Card cardToBuy = MoveCard(Cassie, "RushingWaters", GetCard("Riverbank").UnderLocation); // Move Storm Swell under so we definitely have something to purchase. (Cost 3.)

            DecisionMoveCard = cardToBuy;
            DecisionYesNo = true;

            UsePower(Cassie.CharacterCard, 0); // Default Innate. Cast.
            Assert.IsTrue(cardToBuy.Location == Cassie.TurnTaker.Trash || cardToBuy.Location == Cassie.TurnTaker.Deck || cardToBuy.Location == Cassie.HeroTurnTaker.Hand); // Bought.
            AssertNumberOfCardsInHand(Cassie, 3);
        }

        [Test()]
        public void TestInnatePowerNoAffordable()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Cassie", "Megalopolis");

            StartGame();

            AssertNumberOfCardsInDeck(Cassie, 2); // Should start with 2 card in deck.
            MoveCards(Cassie, (Card c) => c.Location == GetCard("Riverbank").UnderLocation, Cassie.TurnTaker.FindSubDeck("RiverDeck"), numberOfCards: 4, overrideIndestructible: true); // Move all cards back to the river deck just in case.
            Card cardToBuy = MoveCard(Cassie, "StormSwell", GetCard("Riverbank").UnderLocation); // Move Storm Swell under so we definitely have something to purchase. (Cost 3.)

            DecisionMoveCard = cardToBuy;
            DecisionYesNo = true;

            UsePower(Cassie.CharacterCard, 0); // Default Innate. Cast.

            AssertAtLocation(cardToBuy, GetCard("Riverbank").UnderLocation);
            AssertNumberOfCardsInHand(Cassie, 3);
        }

        [Test()]
        public void TestInnatePowerGuiseDangIt()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Cassie", "Guise", "Megalopolis");

            StartGame();

            AssertNumberOfCardsInDeck(Cassie, 2); // Should start with 2 card in deck.
            MoveCards(Cassie, (Card c) => c.Location == GetCard("Riverbank").UnderLocation, Cassie.TurnTaker.FindSubDeck("RiverDeck"), numberOfCards: 4, overrideIndestructible: true); // Move all cards back to the river deck just in case.
            Card cardToBuy = MoveCard(Cassie, "RushingWaters", GetCard("Riverbank").UnderLocation); // Move Rushing Water under so we definitely have something to purchase. (Cost 3.)

            DecisionMoveCard = cardToBuy;
            DecisionYesNo = true;
            DecisionSelectPower = Cassie.CharacterCard;

            HeroTurnTakerController guise = FindHero("Guise");

            PlayCard("ICanDoThatToo"); // Guise uses the innate power.

            // Even if Guise discards everything, he should fail to get the card due to all discarded cards having a total magic value of 0.
            Assert.IsTrue(cardToBuy.Location == GetCard("Riverbank").UnderLocation); // Not bought.
            // Guise redraws to 3.
            AssertNumberOfCardsInHand(guise, 3);
        }

        [Test()]
        public void TestInnatePowerGuiseDangItAgain()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Cassie", "Guise", "Megalopolis");

            StartGame();

            AssertNumberOfCardsInDeck(Cassie, 2); // Should start with 2 card in deck.
            MoveCards(Cassie, (Card c) => c.Location == GetCard("Riverbank").UnderLocation, Cassie.TurnTaker.FindSubDeck("RiverDeck"), numberOfCards: 4, overrideIndestructible: true); // Move all cards back to the river deck just in case.
            Card cardToBuy = GetCard("Retcon"); // Get guise's card...
            MoveCard(Cassie, cardToBuy, GetCard("Riverbank").UnderLocation); // Move Retcon into the riverback. Yes, it doesn't have a cost. That's the point.

            DecisionMoveCard = cardToBuy;
            DecisionYesNo = true;

            UsePower(Cassie.CharacterCard, 0); // Default Innate. Cast.
            Assert.IsTrue(cardToBuy.Location == GetCard("Riverbank").UnderLocation); // Not bought. Even if the card's available, the lack of cost means the interaction fails.
            AssertNumberOfCardsInHand(Cassie, 3);
        }

        [Test()]
        public void TestInnatePowerRepresentative()
        {
            SetupGameController("BaronBlade", "Guise", "TheCelestialTribunal");

            StartGame();
            AvailableHeroes = DeckDefinition.AvailableHeroes.Concat(new string[] { "LazyFanComix.Cassie" });
            SelectFromBoxForNextDecision("LazyFanComix.CassieCharacter", "LazyFanComix.Cassie");

            PlayCard("RepresentativeOfEarth");

            Card representative = FindCardInPlay("CassieCharacter");
            AssertIsInPlay(representative);

            UsePower(representative);
        }

        [Test()]
        public void TestEssenceFlowInnatePower()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Cassie/CassieEssenceFlowCharacter", "Megalopolis");

            StartGame();

            AssertNumberOfCardsInDeck(Cassie, 2); // Should start with 2 card in deck.
            MoveCards(Cassie, (Card c) => c.Location == GetCard("Riverbank").UnderLocation, Cassie.TurnTaker.FindSubDeck("RiverDeck"), numberOfCards: 4, overrideIndestructible: true); // Move all cards back to the river deck just in case.
            Card cardToBuy = MoveCard(Cassie, "RushingWaters", GetCard("Riverbank").UnderLocation); // Move Storm Swell under so we definitely have something to purchase. (Cost 3.)
            Card cardToSpend = PutInHand("Droplet");

            DecisionSelectCards = new Card[] { cardToSpend, cardToBuy };
            DecisionYesNo = true;

            QuickHandStorage(Cassie);
            UsePower(Cassie.CharacterCard, 0); // Default Innate. Cast. Any card we use should qualify, since they have a base cost of 1.
            Assert.IsTrue(cardToBuy.Location == Cassie.TurnTaker.Trash || cardToBuy.Location == Cassie.TurnTaker.Deck || cardToBuy.Location == Cassie.HeroTurnTaker.Hand); // Bought.
        }

        [Test()]
        public void TestEssenceFlowInnatePowerRepresentative()
        {
            SetupGameController("BaronBlade", "Guise", "TheCelestialTribunal");

            StartGame();
            AvailableHeroes = DeckDefinition.AvailableHeroes.Concat(new string[] { "LazyFanComix.Cassie" });
            SelectFromBoxForNextDecision("LazyFanComix.CassieEssenceFlowCharacter", "LazyFanComix.Cassie");

            PlayCard("CalledToJudgement");

            Card representative = FindCardInPlay("CassieCharacter");
            AssertIsInPlay(representative);

            UsePower(representative);
        }

        [Test()]
        public void TestEssenceFlowInnatePowerGuise()
        {
            SetupGameController("BaronBlade", "Guise", "LazyFanComix.Cassie/CassieEssenceFlowCharacter", "TheCelestialTribunal");

            StartGame();

            DecisionSelectPower = Cassie.CharacterCard;
            Card card = PutOnDeck("ICanDoThatToo");

            QuickHandStorage(guise);
            PlayCard(card);
            QuickHandCheck(-1);
        }

        [Test()]
        public void TestDropletWithMove()
        {
            // Most basic purchase equivalent!
            SetupGameController("BaronBlade", "LazyFanComix.Cassie", "Megalopolis");

            StartGame();

            DealDamage(Cassie, Cassie, 3, DamageType.Melee);

            List<Card> wasUnderCards = new List<Card>(GetCard("Riverbank").UnderLocation.Cards);

            DecisionYesNo = true;

            QuickHPStorage(Cassie);
            PlayCard("Droplet"); // Play the card.
            QuickHPCheck(1); // Damage dealt.
            AssertAtLocation(wasUnderCards, Cassie.TurnTaker.FindSubDeck("RiverDeck"));
            AssertNumberOfCardsAtLocation(GetCard("Riverbank").UnderLocation, 4); // 4 cards in the Riverbank.
        }

        [Test()]
        public void TestDropletNoMove()
        {
            // Most basic purchase equivalent!
            SetupGameController("BaronBlade", "LazyFanComix.Cassie", "Megalopolis");

            StartGame();

            DealDamage(Cassie, Cassie, 3, DamageType.Melee);

            List<Card> wasUnderCards = new List<Card>(GetCard("Riverbank").UnderLocation.Cards);

            DecisionYesNo = false;

            QuickHPStorage(Cassie);
            PlayCard("Droplet"); // Play the card.
            QuickHPCheck(1); // Damage dealt.
            AssertAtLocation(wasUnderCards, GetCard("Riverbank").UnderLocation);
        }

        [Test()]
        public void TestWaterSurge()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Cassie", "Legacy", "Megalopolis");

            StartGame();

            DealDamage(Cassie, Cassie, 3, DamageType.Melee);
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;
            DecisionSelectTargetFriendly = Cassie.CharacterCard;

            PlayCard("InspiringPresence"); // Use to boost damage by 1 to make sure character card is source.

            QuickHPStorage(mdp, Cassie.CharacterCard);
            PlayCard("WaterSurge"); // Play the card.
            QuickHPCheck(-3, 1);
        }

        [Test()]
        public void TestRushingWaters()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Cassie", "Megalopolis");

            StartGame();

            DealDamage(Cassie, Cassie, 3, DamageType.Melee);
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;
            DecisionSelectTargetFriendly = Cassie.CharacterCard;

            PlayCard("RushingWaters"); // Play the card.
            AssertNumberOfCardsInTrash(Cassie, 3); // Constant flow and two other played cards in trash.
        }

        [Test()]
        public void TestWaterlog()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Cassie", "Megalopolis");

            StartGame();

            Card ongoing = PlayCard("LivingForceField");
            DecisionDestroyCard = ongoing;

            PlayCard("Waterlog"); // Play the card.
            AssertInTrash(ongoing); // Ongoing destroyed.
        }

        //[Test()]
        //public void TestStreamSurge()
        //{
        //    SetupGameController("BaronBlade", "LazyFanComix.Cassie", "Megalopolis");

        //    StartGame();

        //    List<Card> targets = new List<Card>
        //    {
        //        FindCardInPlay("MobileDefensePlatform"),
        //        FindCardInPlay("BaronBladeCharacter"),
        //        Cassie.CharacterCard,
        //        FindCardInPlay("BaronBladeCharacter")
        //    };
        //    Card followUp = PutInHand("WaterSurge");
        //    DealDamage(Cassie, Cassie, 4, DamageType.Melee);
        //    DealDamage(Cassie, targets[0], 9, DamageType.Melee);

        //    DecisionSelectTargets = targets.ToArray();
        //    DecisionYesNo = true;
        //    DecisionSelectCardToPlay = followUp;

        //    QuickHPStorage(targets[1], Cassie.CharacterCard);
        //    PlayCard("StreamSurge"); // Play the card.
        //    AssertNumberOfCardsInTrash(Cassie, 2); // Constant flow and other played cards in trash.
        //    AssertInTrash(targets[0]); // MDP Destroyed.
        //    QuickHPCheck(-3, 1); // Blade took 3 hits total. Cassie damaged 1, healed 2.

        //}

        [Test()]
        public void TestRiverWornStone()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Cassie", "Legacy", "Megalopolis");

            StartGame();

            GoToUsePowerPhase(Cassie);
            MoveCard(Cassie, GetCard("RiverWornStone", 1), Cassie.HeroTurnTaker.Trash); // Move spare copy to the trash so draw 2 has two cards.

            Card power = PlayCard("RiverWornStone"); // Play the card.
            Card cost = PutInHand("WaterSurge");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;
            DecisionSelectCard = cost;

            PlayCard("InspiringPresence"); // Use to boost damage by 1 to make sure character card is source.

            QuickHPStorage(mdp);

            UsePower(power, 0);
            QuickHPCheck(-2); // 1 damage for cost, 1 for boost.
            AssertAtLocation(cost, Cassie.TurnTaker.FindSubDeck("RiverDeck")); // Card was moved into the river deck.
            // Discard all cards to clear things for additional tests.
            DiscardAllCards(Cassie);

            QuickHandStorage(Cassie);
            GoToDrawCardPhase(Cassie);
            RunActiveTurnPhase();
            QuickHandCheck(2); // Confirm drew 2 cards.
        }

        [Test()]
        public void TestRiverWornStoneGuiseDangIt()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Cassie", "Guise", "Megalopolis");

            StartGame();

            GoToUsePowerPhase(Cassie);
            MoveCard(Cassie, GetCard("RiverWornStone", 1), Cassie.HeroTurnTaker.Trash); // Move spare copy to the trash so draw 2 has two cards.

            Card power = PlayCard("RiverWornStone"); // Play the card.
            Card cost = PutInHand("BlatantReference");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;
            DecisionSelectCard = cost;
            DecisionSelectPower = power;

            PlayCard("GuiseTheBarbarian"); // Damage boost to make sure no 1 damage instance occurs.

            QuickHPStorage(mdp);

            PlayCard("LemmeSeeThat"); // Guise borrows the card.
            UsePower(power); // Guise uses that power. Does this work?

            QuickHPCheck(0); // No damage - card was moved into the river, but no spell value exists, so we try to deal null value, opposed to dealing 0 damage.
            AssertAtLocation(cost, Cassie.TurnTaker.FindSubDeck("RiverDeck")); // Card was moved into the river deck, necessitating the core 'Dang it Guise' case.
        }

        //[Test()]
        //public void TestDivergingWaters()
        //{
        //    SetupGameController("BaronBlade", "LazyFanComix.Cassie", "Megalopolis");

        //    StartGame();

        //    DiscardAllCards(Cassie); // Discard all cards so draw cards can pull an appropriate amount.
        //    Card mdp = FindCardInPlay("MobileDefensePlatform");
        //    Card followUp = PutInHand("WaterSurge");
        //    Card waters = PutInHand("DivergingWaters");

        //    DealDamage(Cassie, Cassie, 4, DamageType.Melee);

        //    DecisionSelectTarget = mdp;
        //    DecisionYesNo = true;
        //    DecisionSelectCardToPlay = followUp;

        //    QuickHPStorage(mdp, Cassie.CharacterCard);
        //    QuickHandStorage(Cassie);

        //    PlayCard(waters); // Play the card.

        //    QuickHPCheck(-2, 2); // MDP took one hit, Cassie took one hit.
        //    QuickHandCheck(1); // Two cards played/returned, 3 cards drawn. Net +1.
        //    AssertInTrash(followUp); // Used card in discard.
        //    AssertAtLocation(waters, Cassie.TurnTaker.FindSubDeck("RiverDeck"), true); // Card returned to river.

        //}

        [Test()]
        public void TestStreamShot()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Cassie", "Legacy", "Megalopolis");

            StartGame();

            GoToUsePowerPhase(Cassie);
            Card play = PutInHand(Cassie, "StreamShot");
            Card riverCard = MoveCard(Cassie, "Droplet", GetCard("Riverbank").UnderLocation); // Move droplet to the riverbank for reference.
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            int riverbankCount = GetCard("Riverbank").UnderLocation.Cards.Count();

            PlayCard("InspiringPresence"); // Use to boost damage by 1 to make sure character card is source.

            DecisionSelectCard = riverCard;
            DecisionSelectTarget = mdp;

            QuickHPStorage(mdp);
            PlayCard(play);
            QuickHPCheck(-2); // 1 damage for cost, 1 for boost.
            AssertAtLocation(riverCard, Cassie.TurnTaker.FindSubDeck("RiverDeck"));
            AssertNumberOfCardsUnderCard(GetCard("Riverbank"), 4); 
        }

        [Test()]
        public void TestStreamShotGuiseDangIt()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Cassie", "Guise", "Legacy", "Megalopolis");

            StartGame();

            GoToUsePowerPhase(Cassie);
            Card play = PutInHand(Cassie, "StreamShot"); // Move into hand to prevent triggering from beneath Riverbank.
            Card riverCard = MoveCard(Cassie, "Retcon", GetCard("Riverbank").UnderLocation); // Move Retcon to the riverbank.
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            int riverbankCount = GetCard("Riverbank").UnderLocation.Cards.Count();

            DecisionSelectCard = riverCard;
            DecisionSelectTarget = mdp;
            PlayCard("InspiringPresence"); // Use Legacy to give +1 boost as necessary.

            QuickHPStorage(mdp);
            PlayCard(play);
            QuickHPCheck(0); // 0 damage, since no magic number exists.
            AssertAtLocation(riverCard, Cassie.TurnTaker.FindSubDeck("RiverDeck"));
            AssertNumberOfCardsUnderCard(GetCard("Riverbank"), 4); 
        }

        [Test()]
        public void TestCondensedOrb()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Cassie", "Megalopolis");

            StartGame();

            GoToUsePowerPhase(Cassie);

            Card mdp = FindCardInPlay("MobileDefensePlatform");
            List<Card> targets = new List<Card>() { Cassie.CharacterCard, mdp };

            DealDamage(Cassie.CharacterCard, targets.AsEnumerable(), 5, DamageType.Melee);

            DecisionSelectCards = targets.ToArray();

            QuickHPStorage(Cassie.CharacterCard, mdp);
            PlayCard("CondensedOrb");
            QuickHPCheck(3, 3);
        }

        [Test()]
        public void TestStormSwell()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Cassie", "Megalopolis");

            StartGame();

            GoToUsePowerPhase(Cassie);

            List<Card> targets = new List<Card>() { FindCardInPlay("MobileDefensePlatform"), FindCardInPlay("BaronBladeCharacter") };

            DealDamage(Cassie.CharacterCard, targets[0], 7, DamageType.Melee);

            DecisionSelectCards = targets.ToArray();

            QuickHPStorage(targets[1]);
            PlayCard("StormSwell");
            AssertInTrash(targets[0]); // MDP destroyed.
            QuickHPCheck(-4); // BB 4 damage.
        }

        //[Test()]
        //public void TestShapeTheStream()
        //{
        //    // Most basic purchase equivalent!
        //    SetupGameController("BaronBlade", "LazyFanComix.Cassie", "Megalopolis");

        //    StartGame();

        //    AssertNumberOfCardsInDeck(Cassie, 2); // Should start with 2 card in deck.

        //    PlayCard("ShapeTheStream"); // Play the card.

        //    AssertNumberOfCardsInTrash(Cassie, 2); // Shape the stream and gained card should now be in trash.
        //    AssertNumberOfCardsAtLocation(GetCard("Riverbank").UnderLocation, 4); // And 4 cards in the Riverbank.
        //}

        //[Test()]
        //public void TestShapeTheStreamGuiseDangIt()
        //{
        //    // Most basic purchase equivalent!
        //    SetupGameController("BaronBlade", "LazyFanComix.Cassie", "Guise", "Megalopolis");

        //    StartGame();

        //    AssertNumberOfCardsInDeck(Cassie, 2); // Should start with 2 card in deck.
        //    Card retcon = MoveCard(Cassie, "Retcon", GetCard("Riverbank").UnderLocation); // Move Retcon into riverbank.

        //    DecisionMoveCard = retcon;

        //    PlayCard("ShapeTheStream"); // Play the card.

        //    AssertInTrash(Cassie, retcon); // Someone else's card is now in your trash. You monster.
        //    AssertNumberOfCardsAtLocation(GetCard("Riverbank").UnderLocation, 4); // And Riverbank has been reset.
        //}

        // TODO: More in-depth tests for what happens when you play pre-existing cards. Apparently we have Guise + Toolbox to go off of and Akash's seeds - probably just
        // goes to the original owner, which is fine.

        [Test()]
        public void TestRisingWaters()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Cassie", "Megalopolis");

            StartGame();

            GoToUsePowerPhase(Cassie);

            Card mdp = FindCardInPlay("MobileDefensePlatform");
            Card played = PlayCard("RisingWaters");

            QuickHPStorage(mdp);
            DealDamage(Cassie.CharacterCard, mdp, 1, DamageType.Melee);
            QuickHPCheck(-2); // Boosted damage.

            GoToStartOfTurn(Cassie);
            AssertInTrash(played); // Self-destructed due to no other cards.
        }

        [Test()]
        public void TestPerpetualFlow()
        {
            // Most basic purchase equivalent!
            SetupGameController("BaronBlade", "LazyFanComix.Cassie", "Megalopolis");

            StartGame();

            AssertNumberOfCardsInDeck(Cassie, 2); // Should start with 2 card in deck.
            MoveCards(Cassie, (Card c) => c.Location == GetCard("Riverbank").UnderLocation, Cassie.TurnTaker.FindSubDeck("RiverDeck"), numberOfCards: 4, overrideIndestructible: true); // Move all cards back to the river deck just in case.
            Card cardToBuy = MoveCard(Cassie, "StormSwell", GetCard("Riverbank").UnderLocation); // Move Storm Swell under so we definitely have something to play.
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            QuickHPStorage(mdp);
            Card played = PlayCard("PerpetualFlow"); // Play the card.

            AssertInTrash(played, cardToBuy);
            AssertNumberOfCardsAtLocation(GetCard("Riverbank").UnderLocation, 4); // And Riverbank has been reset.
            QuickHPCheck(-4); // MDP should've taken damage from Storm Swell.
        }

        [Test()]
        public void TestPerpetualFlowGuiseDangIt()
        {
            // Most basic purchase equivalent!
            SetupGameController("BaronBlade", "LazyFanComix.Cassie", "Guise", "Megalopolis");

            StartGame();
            Card destroy = PlayCard("LivingForceField");

            AssertNumberOfCardsInDeck(Cassie, 2); // Should start with 2 card in deck.
            MoveCards(Cassie, (Card c) => c.Location == GetCard("Riverbank").UnderLocation, Cassie.TurnTaker.FindSubDeck("RiverDeck"), numberOfCards: 4, overrideIndestructible: true); // Move all cards back to the river deck just in case.
            Card cardToBuy = GetCard("Retcon");
            MoveCard(Cassie, cardToBuy, GetCard("Riverbank").UnderLocation); // Move Retcon under.
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            QuickHPStorage(mdp);
            Card played = PlayCard("PerpetualFlow"); // Play the card.

            AssertInTrash(played, cardToBuy, destroy);
            AssertNumberOfCardsAtLocation(GetCard("Riverbank").UnderLocation, 4); // And Riverbank has been reset.
        }

        // TODO: More in-depth tests for what happens when you play pre-existing cards. Apparently we have Guise + Toolbox to go off of and Akash's seeds - probably just
        // goes to the original owner, which is fine.

        [Test()]
        public void TestMeetingTheOcean()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Cassie", "Guise", "Legacy", "Megalopolis");

            StartGame();

            DiscardAllCards(Cassie);

            // Pull 2, 3, and NA cards for testing.
            IEnumerable<Card> cards = GetCards("Waterlog", "RisingWaters", "Retcon");
            MoveCards(Cassie, cards, Cassie.HeroTurnTaker.Hand, overrideIndestructible: true);

            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;
            DecisionYesNo = true;

            PlayCard("InspiringPresence"); // Increase damage by 1 to check for null rather than 0.

            QuickHPStorage(mdp);
            QuickHandStorage(Cassie);

            PlayCard("MeetingTheOcean");
            UsePower("MeetingTheOcean");
            QuickHPCheck(-6); // Instance of 1 and 2, increased by 1.
            AssertNumberOfCardsInHand(Cassie, 3); // Drawn back up to 3.
        }

        // TODO: Add riverbank tests when the River deck has been emptied! Yes, it will stop drawing cards - but you have a full deck to play with already, so at that stage that's your own fault!


        #region Tribunal

        [Test()]
        public void TestCassieTribunalRepresentativePower()
        {
            SetupGameController("BaronBlade", "Guise", "TheCelestialTribunal");

            StartGame();
            AvailableHeroes = DeckDefinition.AvailableHeroes.Concat(new string[] { "LazyFanComix.Cassie" });
            SelectFromBoxForNextDecision("LazyFanComix.CassieCharacter", "LazyFanComix.Cassie");

            PlayCard("RepresentativeOfEarth");

            Card representative = FindCardInPlay("CassieCharacter");
            AssertIsInPlay(representative);

            UsePower(representative);
        }

        [Test()]
        public void TestCassieTribunalCalledPower()
        {
            SetupGameController("BaronBlade", "Guise", "TheCelestialTribunal");

            StartGame();
            AvailableHeroes = DeckDefinition.AvailableHeroes.Concat(new string[] { "LazyFanComix.Cassie" });
            SelectFromBoxForNextDecision("LazyFanComix.CassieCharacter", "LazyFanComix.Cassie");

            PlayCard("CalledToJudgement");

            Card representative = FindCardInPlay("CassieCharacter");
            AssertIsInPlay(representative);

            UsePower(representative);
        }

        [Test()]
        public void TestCassieTribunalCalledEssenceFlowPower()
        {
            SetupGameController("BaronBlade", "Guise", "TheCelestialTribunal");

            StartGame();
            AvailableHeroes = DeckDefinition.AvailableHeroes.Concat(new string[] { "LazyFanComix.Cassie" });
            SelectFromBoxForNextDecision("LazyFanComix.CassieEssenceFlowCharacter", "LazyFanComix.Cassie");
            DiscardAllCards(guise);
            QuickHandStorage(guise);

            PlayCard("CalledToJudgement");

            Card representative = FindCardInPlay("CassieCharacter");
            AssertIsInPlay(representative);

            UsePower(representative);

            QuickHandCheck(3);
        }


        #endregion Tribunal
    }
}