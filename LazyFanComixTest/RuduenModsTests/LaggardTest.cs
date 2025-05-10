using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using LazyFanComix.Laggard;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LazyFanComixTest
{
  [TestFixture]
  public class LaggardTest : BaseTest
  {
    [OneTimeSetUp]
    public void DoSetup()
    {
      // Tell the engine about our mod assembly so it can load up our code.
      // It doesn't matter which type as long as it comes from the mod's assembly.
      //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
      ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(LaggardCharacterCardController))); // replace with your own namespace
    }

    protected HeroTurnTakerController Laggard
    { get { return FindHero("Laggard"); } }

    [Test(Description = "Basic Setup and Health")]
    public void TestModWorks()
    {
      SetupGameController("BaronBlade", "LazyFanComix.Laggard", "Megalopolis");

      Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

      Assert.IsNotNull(Laggard);
      Assert.IsInstanceOf(typeof(HeroTurnTakerController), Laggard);
      Assert.IsInstanceOf(typeof(LaggardCharacterCardController), Laggard.CharacterCardController);

      Assert.AreEqual(27, Laggard.CharacterCard.HitPoints);
      AssertNumberOfCardsInDeck(Laggard, 36);
      AssertNumberOfCardsInHand(Laggard, 4);
    }

    #region Innate Powers
    [Test()]
    public void TestInnatePowerBase()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Laggard", "Legacy", "Megalopolis"
            };
      SetupGameController(setupItems);

      StartGame();

      Card mdp = GetCardInPlay("MobileDefensePlatform");
      DecisionSelectTarget = mdp;

      QuickHPStorage(mdp);
      UsePower(Laggard);
      QuickHPCheck(-1);

      PutIntoPlay("LostAndFound");
      UsePower(Laggard);
      QuickHPCheck(-2);
    }
    #endregion Innate Powers

    #region Cards 1-5
    [Test()]
    public void TestCard1LateBreakingNews()
    {
      SetupGameController("BaronBlade", "LazyFanComix.Laggard", "TheCelestialTribunal");

      StartGame();

      Card mdp = GetCardInPlay("MobileDefensePlatform");
      DecisionSelectCard = mdp;
      Card[] inPlay = { PutOnDeck("LivingForceField"), PutOnDeck("LostAndFound"), PutOnDeck("CelestialExecutioner") };

      PlayCard("LateBreakingNews");
      AssertIsInPlay(inPlay);
      AssertNotInPlay(mdp);
    }

    [Test()]
    public void TestCard2Postdiction()
    {
      SetupGameController("BaronBlade", "TheWraith", "LazyFanComix.Laggard", "TheCelestialTribunal");

      StartGame();

      DestroyNonCharacterVillainCards();

      PlayCard("Postdiction");
      QuickHPStorage(baron);
      QuickHandStorage(wraith);

      DecisionSelectCard = wraith.CharacterCard;
      PlayCard("AtLeastWereWarned");
      QuickHandCheck(1);

      DecisionSelectCard = baron.CharacterCard;
      PlayCard("Hindseen");
      QuickHPCheck(-2);
    }


    [Test()]
    public void TestCard3ForegoneConclusion()
    {
      SetupGameController("BaronBlade", "TheWraith", "LazyFanComix.Laggard", "TheCelestialTribunal");

      StartGame();

      Card played = PutOnDeck("LivingForceField");
      Card testCard = PlayCard("ForegoneConclusion");

      QuickHandStorage(Laggard);
      UsePower(Laggard);
      QuickHandCheck(1);
      SelectYesNoForNextDecision(new bool[] { false, true });
      UsePower(testCard);
      QuickHandCheck(1);
      AssertNotInPlay(played);
      UsePower(testCard);
      QuickHandCheck(1);
      AssertIsInPlay(played);
    }


    [Test()]
    public void TestCard4LateToTheParty()
    {
      SetupGameController("BaronBlade", "TheWraith", "LazyFanComix.Laggard", "TheCelestialTribunal");

      StartGame();

      DiscardAllCards(Laggard);
      Card[] hindsights = { PutOnDeck("LostAndFound"), PutOnDeck("Hindseen"), PutOnDeck("SeekVisions") };
      PutOnDeck("Again");
      QuickHandStorage(Laggard);
      PlayCard("LateToTheParty");
      AssertIsInPlay(hindsights);
      QuickHandCheck(1); // Draw 4, play 3

    }

    [Test()]
    public void TestCard5Hindseen()
    {
      SetupGameController("BaronBlade", "Ra", "Legacy", "LazyFanComix.Laggard", "TheCelestialTribunal");

      StartGame();

      Card mdp = GetCardInPlay("MobileDefensePlatform");
      DecisionSelectCard = mdp;
      DecisionSelectTarget = mdp;
      Card card = PlayCard("Hindseen");
      QuickHPStorage(mdp);
      DealDamage(mdp, Laggard, 1, DamageType.Melee);
      QuickHPCheck(-2);

      GoToNextTurn();
      DealDamage(mdp, ra, 1, DamageType.Melee);
      QuickHPCheck(-2);

      DestroyCard(mdp);
      AssertInPlayArea(baron, card);
      GoToStartOfTurn(Laggard);
      AssertInTrash(card);
    }
    #endregion

    #region Cards 6-10
    [Test()]
    public void TestCard6SeekVisions()
    {
      SetupGameController("BaronBlade", "TheWraith", "Legacy", "LazyFanComix.Laggard", "TheCelestialTribunal");

      StartGame();
      Card card = PutInTrash("UtilityBelt");
      DecisionSelectCard = wraith.CharacterCard;
      DecisionYesNo = true;
      PlayCard("SeekVisions");
      GoToStartOfTurn(Laggard);
      AssertIsInPlay(card);

      Card mdp = GetCardInPlay("MobileDefensePlatform");
      DestroyCard(mdp);
      DecisionSelectCard = baron.CharacterCard;
      DecisionYesNo = false;
      PlayCard("SeekVisions");
      GoToStartOfTurn(Laggard);
      AssertAtLocation(mdp, baron.TurnTaker.Deck);
    }


    [Test()]
    public void TestCard7Again()
    {
      SetupGameController("BaronBlade", "TheWraith", "Legacy", "LazyFanComix.Laggard", "TheCelestialTribunal");

      StartGame();

      Card playFromTrash = PutInTrash("SeekVisions");
      Card play = PutInTrash("Again");

      QuickHandStorage(Laggard);
      PlayCard(play);
      QuickHandCheck(2);
      AssertIsInPlay(playFromTrash);
    }


    [Test()]
    public void TestCard8AtLeastWereWarned()
    {
      SetupGameController("BaronBlade", "TheWraith", "Legacy", "LazyFanComix.Laggard", "TheCelestialTribunal");

      StartGame();
      Card mdp = GetCardInPlay("MobileDefensePlatform");

      DecisionSelectCard = wraith.CharacterCard;
      DecisionYesNo = true;
      Card played = PlayCard("AtLeastWereWarned");

      AssertIsInPlay(played);

      Card play = PutInHand("ThrowingKnives");
      DecisionSelectCard = play;
      DecisionSelectPower = play;
      QuickHPStorage(mdp);
      GoToStartOfTurn(Laggard);
      AssertInTrash(played);
      AssertIsInPlay(play);
      QuickHPCheck(-1); // Damage dealt by throwing knives.
    }

    [Test()]
    public void TestCard9GlimpseOfTheUnknown()
    {
      SetupGameController("BaronBlade", "TheWraith", "Legacy", "LazyFanComix.Laggard", "TheCelestialTribunal");

      StartGame();
      DestroyNonCharacterVillainCards();

      Card destroy = PlayCard("LivingForceField");
      Card card = PlayCard("GlimpseOfTheUnknown");

      QuickHPStorage(baron);
      UsePower(card);
      AssertInTrash(destroy, card);
      QuickHPCheck(-1);


    }


    [Test()]
    public void TestCard10DirectMethod()
    {
      SetupGameController("BaronBlade", "TheWraith", "Legacy", "LazyFanComix.Laggard", "TheCelestialTribunal");

      StartGame();
      DestroyNonCharacterVillainCards();

      Card[] hindsights = FindCardsWhere((Card c) => c.DoKeywordsContain("hindsight")).ToArray();
      Card[] nonSights = FindCardsWhere((Card c) => !c.DoKeywordsContain("hindsight") && c.Owner == Laggard.TurnTaker).ToArray();
      DiscardAllCards(Laggard);
      PutInHand(hindsights);
      QuickHPStorage(baron);
      PlayCard("DirectMethod");
      QuickHPCheck(-12);

      DiscardAllCards(Laggard);
      PlayCard("DirectMethod");
      QuickHPCheck(-2);

      PutInHand(hindsights[0]);
      PutInHand(hindsights[1]);
      PutInHand(hindsights[2]);
      PutInHand(nonSights[0]);
      PutInHand(nonSights[1]);
      PlayCard("DirectMethod");
      QuickHPCheck(-10);

    }
    #endregion

    #region Cards 11-15
    [Test()]
    public void TestCard11SlowChaseSequence()
    {
      SetupGameController("BaronBlade", "TheWraith", "Legacy", "LazyFanComix.Laggard", "TheCelestialTribunal");

      StartGame();

      Card mdp = GetCardInPlay("MobileDefensePlatform");

      DecisionSelectTarget = mdp;

      PlayCard("SlowChaseSequence");
      AssertOnTopOfDeck(mdp);

      PlayCard(mdp);
      SetHitPoints(mdp, 2);
      PlayCard("SlowChaseSequence");
      AssertInTrash(mdp);

      DecisionSelectTarget = baron.CharacterCard;
      QuickHPStorage(baron);
      PlayCard("SlowChaseSequence");
      QuickHPCheck(-3);
    }

    [Test()]
    public void TestCard12FashionablyLate()
    {
      SetupGameController("BaronBlade", "TheWraith", "Legacy", "LazyFanComix.Laggard", "TheCelestialTribunal");

      StartGame();
      DestroyNonCharacterVillainCards();

      Card card = PlayCard("FashionablyLate");

      DecisionSelectPower = card;

      DecisionSelectFunction = 0;
      QuickHPStorage(baron);
      UsePower(card);
      QuickHPCheck(-3);

      GoToEndOfTurn(Laggard);
      QuickHPCheck(-2);

      PlayCard("LivingForceField");
      UsePower(card);
      QuickHPCheck(-3 + 1);

    }

    [Test()]
    public void TestCard13ChronicleOfUnknownScenes()
    {
      SetupGameController("BaronBlade", "TheWraith", "Legacy", "LazyFanComix.Laggard", "TheCelestialTribunal");

      StartGame();
      DestroyNonCharacterVillainCards();

      Card card = PlayCard("ChronicleOfUnknownScenes");

      DecisionSelectFunction = 1;

      DiscardAllCards(Laggard);
      PutInHand("FashionablyLate");
      Card hindsight = PutInHand("Hindseen"); // Next to Baron
      UsePower(card);
      AssertIsInPlay(hindsight);

      DecisionSelectCard = wraith.CharacterCard;
      PlayCard("LostAndFound"); // Next to Wraith
      PlayCard("SeekVisions"); // Next to Wraith

      QuickHPStorage(baron, wraith, Laggard);

      DealDamage(baron, baron, 3, DamageType.Melee);
      DealDamage(wraith, wraith, 3, DamageType.Melee);
      DealDamage(Laggard, Laggard, 3, DamageType.Melee);

      QuickHPCheck(-3 - 1, -3 + 1, -3);
    }


    [Test()]
    public void TestCard14LostAndFound()
    {
      SetupGameController("BaronBlade", "TheWraith", "Legacy", "LazyFanComix.Laggard", "TheCelestialTribunal");

      StartGame();

      Card mdp = GetCardInPlay("MobileDefensePlatform");
      DecisionSelectCard = mdp;
      Card play = PlayCard("LostAndFound");
      AssertNextToCard(play, mdp);

      QuickHPStorage(mdp);
      GoToStartOfTurn(Laggard);
      QuickHPCheck(-6);
      AssertInTrash(play);

      PlayCard(play);
      GoToStartOfTurn(Laggard);
      AssertInTrash(mdp, play);

      PlayCard(mdp);
      PlayCard(play);
      DestroyCard(mdp);
      AssertInPlayArea(baron, play);
      GoToStartOfTurn(Laggard);
      AssertInTrash(play);
    }


    [Test()]
    public void TestCard15StallTactics()
    {
      SetupGameController("BaronBlade", "TheWraith", "Legacy", "LazyFanComix.Laggard", "TheCelestialTribunal");

      StartGame();

      Card mdp = GetCardInPlay("MobileDefensePlatform");
      Card play = PlayCard("StallTactics");

      QuickHPStorage(mdp, Laggard.CharacterCard);
      DealDamage(mdp, Laggard.CharacterCard, 5, DamageType.Melee);
      DealDamage(Laggard.CharacterCard, mdp, 5, DamageType.Melee);
      QuickHPCheck(-4, -4);

      DecisionYesNo = false;
      GoToStartOfTurn(Laggard);
      QuickHPCheck(0, 0);
      AssertIsInPlay(play);

      DecisionYesNo = true;
      GoToStartOfTurn(Laggard);
      QuickHPCheck(0, 3);
      AssertInTrash(play);

    }
    #endregion
  }
}