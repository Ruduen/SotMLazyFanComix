using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using LazyFanComix.Conflux;
using NUnit.Framework;

namespace LazyFanComixTest
{
  [TestFixture]
  public class ConfluxTest : BaseTest
  {
    [OneTimeSetUp]
    public void DoSetup()
    {
      // Tell the engine about our mod assembly so it can load up our code.
      // It doesn't matter which type as long as it comes from the mod's assembly.
      //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
      ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(ConfluxCharacterCardController))); // replace with your own namespace
    }

    protected HeroTurnTakerController Conflux
    { get { return FindHero("Conflux"); } }

    [Test(Description = "Basic Setup and Health")]
    public void TestModWorks()
    {
      SetupGameController("BaronBlade", "LazyFanComix.Conflux", "Megalopolis");

      Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

      Assert.IsNotNull(Conflux);
      Assert.IsInstanceOf(typeof(HeroTurnTakerController), Conflux);
      Assert.IsInstanceOf(typeof(ConfluxCharacterCardController), Conflux.CharacterCardController);

      Assert.AreEqual(22, Conflux.CharacterCard.HitPoints);
      AssertNumberOfCardsInDeck(Conflux, 36);
      AssertNumberOfCardsInHand(Conflux, 4);
    }


    #region Innate Tests


    [Test()]
    public void TestInnatePowerBase()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      QuickHPStorage(baron);
      UsePower(Conflux);
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

      SelectFromBoxForNextDecision("LazyFanComix.ConfluxCharacter", "LazyFanComix.Conflux");
      QuickHPStorage(baron);
      PlayCard("CalledToJudgement");
      QuickHPCheck(-2);

      UsePower(FindCardInPlay("ConfluxCharacter"));
      QuickHPCheck(-2);
    }

    [Test()]
    public void TestInnatePowerTeam()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux/LazyFanComix.ConfluxTeamCharacter", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      QuickHPStorage(Conflux, bunker, baron);

      // 1 Type, Protects Baron
      UsePower(Conflux);
      DealDamage(Conflux, baron, 1, DamageType.Psychic);
      DealDamage(Conflux, Conflux, 1, DamageType.Psychic);
      DealDamage(Conflux, bunker, 1, DamageType.Psychic);
      QuickHPCheck(-1, -1, -1);

      // 2 Types, Protect Baron and Conflux
      UsePower(Conflux);
      DealDamage(Conflux, baron, 1, DamageType.Psychic);
      DealDamage(Conflux, Conflux, 1, DamageType.Psychic);
      DealDamage(Conflux, bunker, 1, DamageType.Psychic);
      QuickHPCheck(0, -1, 0);

      GoToStartOfTurn(Conflux);
      PlayCard("LivingForceField");
      // 0 Types, Living Force Field. Check text.
      AssertNextMessage("Team Conflux has not dealt damage this turn, so no targets can be selected.");
      UsePower(Conflux);
      AssertExpectedMessageWasShown();
    }


    [Test()]
    public void TestInnatePowerTeamTribunal()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "Tempest", "Guise", "Legacy/AmericasGreatestLegacyCharacter", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      // Tempest based power
      SelectFromBoxForNextDecision("LazyFanComix.ConfluxTeamCharacter", "LazyFanComix.Conflux");
      QuickHPStorage(baron);
      PlayCard("CalledToJudgement");
      DealDamage(tempest, baron, 1, DamageType.Lightning);
      QuickHPCheck(-1);

      // Env based power
      GoToStartOfTurn(tempest);
      UsePower(FindCardInPlay("ConfluxCharacter"));
      DealDamage(tempest, baron, 1, DamageType.Lightning);
      QuickHPCheck(-1);

      // Env based power
      GoToStartOfTurn(env);
      UsePower(FindCardInPlay("ConfluxCharacter"));
      DealDamage(tempest, baron, 1, DamageType.Lightning);
      QuickHPCheck(-1);


    }

    [Test()]
    public void TestInnatePowerChibi()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux/LazyFanComix.ConfluxChibiCharacter", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      QuickHandStorage(Conflux);
      QuickHPStorage(baron);

      // No discards. 
      UsePower(Conflux);
      QuickHandCheck(1);
      QuickHPCheck(0);

      // One discard.
      DealDamage(Conflux, Conflux, 1, DamageType.Cold);
      UsePower(Conflux);
      QuickHandCheck(1 - 1);
      QuickHPCheck(-1);

      // 2ne discard. (From damage.) 
      UsePower(Conflux);
      QuickHandCheck(1 - 2);
      QuickHPCheck(-2);
    }


    [Test()]
    public void TestInnatePowerChibiTribunal()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "Tempest", "Guise", "Legacy/AmericasGreatestLegacyCharacter", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      // Tempest based power
      SelectFromBoxForNextDecision("LazyFanComix.ConfluxChibiCharacter", "LazyFanComix.Conflux");
      QuickHPStorage(baron);
      QuickHandStorage(tempest);
      PlayCard("CalledToJudgement");
      QuickHPCheck(0);
      QuickHandCheck(1);

      // Env based power should gracefully fail due to lack of hand/discard.
      AssertNextMessage("Chibi Conflux does not have a hand, so no cards can be drawn or discarded.");
      UsePower(FindCardInPlay("ConfluxCharacter"));
      QuickHPCheck(0);
      AssertExpectedMessageWasShown();
    }
    #endregion Innate Tests
    #region Incap Tests
    [Test()]
    public void TestIncapBase()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      DestroyCard(Conflux);

      // Bunker plays card.
      DiscardAllCards(bunker);
      Card play = PutInHand("AdhesiveFoamGrenade");
      UseIncapacitatedAbility(Conflux, 0);
      AssertInTrash(play);

      QuickHandStorage(bunker);
      UseIncapacitatedAbility(Conflux, 1);
      QuickHandCheck(1);

      QuickHPStorage(baron, bunker);
      UseIncapacitatedAbility(Conflux, 2);
      DealDamage(bunker, bunker, 1, DamageType.Cold);
      DealDamage(baron, baron, 1, DamageType.Cold);
      QuickHPCheck(-2, -2);

      GoToStartOfTurn(Conflux);
      DealDamage(bunker, bunker, 1, DamageType.Cold);
      DealDamage(baron, baron, 1, DamageType.Cold);
      QuickHPCheck(-1, -1);
    }

    [Test()]
    public void TestIncapTeam()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux/LazyFanComix.ConfluxTeamCharacter", "Bunker", "TheCourtOfBlood"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      DestroyCard(Conflux);

      // Bunker plays card.
      DiscardAllCards(bunker);
      Card play = PutInHand("AdhesiveFoamGrenade");
      UseIncapacitatedAbility(Conflux, 0);
      AssertInTrash(play);


      QuickHPStorage(baron, bunker);
      UseIncapacitatedAbility(Conflux, 1);
      DealDamage(bunker, bunker, 1, DamageType.Cold);
      DealDamage(baron, baron, 1, DamageType.Cold);
      QuickHPCheck(-1, 0);

      GoToStartOfTurn(Conflux);
      DealDamage(bunker, bunker, 1, DamageType.Cold);
      DealDamage(baron, baron, 1, DamageType.Cold);
      QuickHPCheck(-1, -1);

      PutIntoPlay("UnhallowedHalls");

      // Damaged is changed and therefore not prevented.
      QuickHPStorage(baron, bunker);
      UseIncapacitatedAbility(Conflux, 2);
      DealDamage(bunker, bunker, 1, DamageType.Radiant);
      DealDamage(baron, baron, 1, DamageType.Radiant);
      QuickHPCheck(-1, -1);

      GoToStartOfTurn(Conflux);
      DealDamage(bunker, bunker, 1, DamageType.Radiant);
      DealDamage(baron, baron, 1, DamageType.Radiant);
      QuickHPCheck(0, 0);
    }

    [Test()]
    public void TestIncapChibi()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux/LazyFanComix.ConfluxChibiCharacter", "Bunker", "TheCourtOfBlood"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      DestroyCard(Conflux);

      // Bunker plays card.
      DiscardAllCards(bunker);
      Card play = PutInHand("AdhesiveFoamGrenade");
      UseIncapacitatedAbility(Conflux, 0);
      AssertInTrash(play);

      Card boom = PlayCard("LivingForceField");
      UseIncapacitatedAbility(Conflux, 1);
      AssertInTrash(boom);

      Card envTarget = PutIntoPlay("HunterFulepet");
      QuickHPStorage(bunker.CharacterCard, baron.CharacterCard, envTarget);
      UseIncapacitatedAbility(Conflux, 2);
      QuickHPCheck(-2, -2, -10);

    }
    #endregion Incap Tests
    #region One-Shot Tests

    [Test()]
    public void TestOneShotFlarebolt()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      Card[] flarebolts = this.FindCardsWhere((Card c) => c.Title == "Flarebolt").ToArray();

      MoveAllCardsFromHandToDeck(Conflux);

      MoveAllCards(Conflux, Conflux.TurnTaker.Deck, Conflux.TurnTaker.OutOfGame);

      DealDamage(Conflux, Conflux, 10, DamageType.Cold);

      MoveCards(Conflux, flarebolts, Conflux.TurnTaker.Deck);

      // Three bolts. Conflux heals, baron takes one more hit. For this chain to occur, needed to have drawn.
      QuickHPStorage(Conflux, baron);
      PlayCard(flarebolts[0]);
      QuickHPCheck(0, -2 - 2 - 2 - 3);

      AssertInTrash(flarebolts);

    }

    [Test()]
    public void TestOneShotNegabolt()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();
      DestroyNonCharacterVillainCards();

      Card destroy = PlayCard("BacklashField");
      Card[] bolts = this.FindCardsWhere((Card c) => c.Title == "Negabolt").ToArray();

      DiscardAllCards(Conflux);

      PutInHand(bolts);

      QuickHPStorage(baron);
      PlayCard(bolts[0]);
      QuickHPCheck(-2 - 2 - 2 - 3);
      AssertInTrash(destroy);

    }

    [Test()]
    public void TestOneShotSnowbolt()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      Card[] snowbolts = this.FindCardsWhere((Card c) => c.Title == "Snowbolt").ToArray();

      DiscardAllCards(Conflux);

      PutInHand(snowbolts);

      DealDamage(Conflux, Conflux, 10, DamageType.Cold);

      // Three snowbolts. Conflux heals, baron takes one more hit. 
      QuickHPStorage(Conflux, baron);
      PlayCard(snowbolts[0]);
      QuickHPCheck(+2 + 2 + 2, -2 - 2 - 2 - 3);

    }

    [Test()]
    public void TestOneShotLightningAndThunder()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DiscardAllCards(Conflux);

      QuickHandStorage(Conflux);


      Card mdp = this.FindCardInPlay("MobileDefensePlatform");
      DecisionSelectTarget = mdp;
      DealDamage(mdp, mdp, 6, DamageType.Cold);

      // MDP at 4 health - destroy MDP, check baron.
      QuickHPStorage(baron);
      QuickHandStorage(Conflux);
      PlayCard("LightningAndThunder");
      QuickHPCheck(-1);
      QuickHandCheck(2);
      AssertInTrash(mdp);

    }

    [Test()]
    public void TestOneShotPreyAndSpray()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DiscardAllCards(Conflux);

      QuickHandStorage(Conflux);


      Card mdp = this.FindCardInPlay("MobileDefensePlatform");
      DecisionSelectTargets = new Card[] { mdp, mdp, baron.CharacterCard };
      DealDamage(mdp, mdp, 6, DamageType.Cold);

      // MDP at 4 health - destroy MDP, check baron.
      AssertNumberOfUsablePowers(Conflux, 1);
      QuickHPStorage(baron);
      PlayCard("PreyAndSpray");
      // 1 from damage, 2 from innate power.
      QuickHPCheck(-1 - 2);
      AssertInTrash(mdp);
      AssertNumberOfUsablePowers(Conflux, 0);

    }

    #endregion One-Shot Tests

    #region Persistent Tests


    [Test()]
    public void TestPersistentPowerCrystal()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "WagnerMarsBase"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      DealDamage(Conflux, baron, 1, DamageType.Cold);

      Card crystal = PlayCard("PowerCrystal");

      QuickHPStorage(Conflux, baron);
      DealDamage(Conflux, baron, 1, DamageType.Cold);
      QuickHPCheck(0, -2);
      DealDamage(Conflux, baron, 1, DamageType.Toxic);
      QuickHPCheck(0, -2);
      DealDamage(Conflux, baron, 1, DamageType.Toxic);
      QuickHPCheck(0, -3);

      DecisionYesNo = true;
      QuickHandStorage(Conflux);
      GoToStartOfTurn(Conflux);
      QuickHandCheck(-2);
      AssertIsInPlay(crystal);
      // Plus from psychic damage.
      DealDamage(Conflux, baron, 1, DamageType.Cold);
      QuickHPCheck(0, -1);
      DealDamage(Conflux, baron, 1, DamageType.Toxic);
      QuickHPCheck(0, -2);
      DealDamage(Conflux, baron, 1, DamageType.Toxic);
      QuickHPCheck(0, -3);


      DiscardAllCards(Conflux);
      DrawCard(Conflux);
      GoToStartOfTurn(Conflux);
      AssertInHand(crystal);
      AssertNumberOfCardsInHand(Conflux, 1);

      DecisionSelectCards = new Card[] { null };

      PlayCard(crystal);
      GoToStartOfTurn(Conflux);
      AssertNumberOfCardsInHand(Conflux, 1);
      AssertInHand(crystal);


    }

    [Test()]
    public void TestPersistentEchoedBlast()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      Card[] echoes = this.FindCardsWhere((Card c) => c.Title == "Echoed Blast").ToArray();

      QuickHPStorage(baron);
      PlayCard(echoes[0]);
      UsePower(Conflux);
      QuickHPCheck(-2 - 2);
      PlayCard(echoes[1]);
      UsePower(Conflux);
      QuickHPCheck(-2 - 2);

      GoToStartOfTurn(Conflux);
      UsePower(Conflux);
      QuickHPCheck(-2 - 2 - 2);

      PutInHand(echoes[0]);
      PlayCard(echoes[0]);
      UsePower(Conflux);
      QuickHPCheck(-2 - 2);

    }

    [Test()]
    public void TestPersistentChannelEssence()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "Legacy", "Ra", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      GoToStartOfTurn(Conflux);

      Card essence = PlayCard("ChannelEssence");
      QuickHandStorage(Conflux, bunker, legacy, ra);
      DealDamage(Conflux, Conflux, 2, DamageType.Cold);
      GoToEndOfTurn(Conflux);
      // 1 damage type prior to play, 1 draw.
      QuickHandCheck(1, 0, 0, 0);

      GoToStartOfTurn(Conflux);
      DealDamage(Conflux, Conflux, 2, DamageType.Cold);
      DealDamage(Conflux, Conflux, 2, DamageType.Cold);
      DealDamage(Conflux, Conflux, 1, DamageType.Fire);
      DealDamage(Conflux, Conflux, 0, DamageType.Lightning);
      GoToEndOfTurn(Conflux);
      QuickHandCheck(1, 1, 0, 0);


      GoToStartOfTurn(Conflux);
      DealDamage(Conflux, Conflux, 1, DamageType.Cold);
      DealDamage(Conflux, Conflux, 1, DamageType.Fire);
      DealDamage(Conflux, Conflux, 1, DamageType.Lightning);
      GoToEndOfTurn(Conflux);
      QuickHandCheck(1, 1, 1, 0);

    }

    [Test()]
    public void TestPersistentInterceptingShot()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      Card[] interceptions = new Card[] {
        PlayCard("InterceptingShot", 0),
        PlayCard("InterceptingShot", 1)
      };


      QuickHPStorage(Conflux, baron);
      DealDamage(baron, Conflux, 2, DamageType.Cold);
      QuickHPCheck(0, -2);
      AssertInTrash(interceptions[0]);
      AssertIsInPlay(interceptions[1]);
      GoToStartOfTurn(Conflux);
      DealDamage(baron, Conflux, 6, DamageType.Cold);
      QuickHPCheck(0, -2);
      AssertInTrash(interceptions);
    }


    [Test()]
    public void TestPersistentEyeOfTheVortex()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      Card eye = PlayCard("EyeOfTheVortex");
      Card[] destroy = new Card[] {
        PlayCard("LivingForceField"),
        PlayCard("BacklashField")
      };

      QuickHandStorage(Conflux);
      UsePower(eye);
      QuickHandCheck(2);
      AssertInTrash(eye);
      AssertInTrash(destroy);

      PlayCard(eye);
      PlayCards(destroy);
      DestroyCard(eye);
      QuickHandCheck(0);
      AssertInTrash(destroy);
    }


    [Test()]
    public void TestPersistentExtraArms()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      Card arms = PlayCard("ExtraArms", 0);
      GoToUsePowerPhase(Conflux);
      AssertPhaseActionCount(2);
      PlayCard("ExtraArms", 1);
      AssertPhaseActionCount(3);
      Card draw = PutOnDeck("PowerCrystal");

      QuickHandStorage(Conflux);
      QuickHPStorage(baron);
      UsePower(arms, 0);
      QuickHPCheck(-1);
      UsePower(arms, 1);
      QuickHPCheck(-1);
      UsePower(arms, 2);
      QuickHPCheck(0);
      QuickHandCheck(1 - 1);
    }


    [Test()]
    public void TestPersistentOverwhelmingBlast()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      Card boom = PlayCard("OverwhelmingBlast");
      PlayCard("LivingForceField");

      QuickHPStorage(baron);
      UsePower(boom);
      QuickHPCheck(-3);
    }



    [Test()]
    public void TestPersistentGatheredVigor()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();
      MoveAllCardsFromHandToDeck(Conflux);

      Card vigor = PlayCard("GatheredVigor");

      UsePower(Conflux, 1);
      UsePower(Conflux, 1);
      UsePower(bunker, 1);

      // Two in play, Conflux bounces one in hand due to limited rule.
      Assert.AreEqual(FindCardsWhere((Card c) => c.IsInPlay && c.DoKeywordsContain("limited")).Count(), 2);
      Assert.AreEqual(FindCardsWhere((Card c) => c.Location == Conflux.HeroTurnTaker.Hand && c.DoKeywordsContain("limited")).Count(), 1);
    }


    [Test()]
    public void TestPersistentOverchargedCrystal()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      DealDamage(baron, bunker, 5, DamageType.Cold);
      DealDamage(baron, Conflux, 5, DamageType.Cold);
      DealDamage(baron, baron, 5, DamageType.Cold);

      Card power = PlayCard("OverchargedCrystal");
      Card boom = PlayCard("GatheredVigor");
      DecisionDestroyCard = boom;

      QuickHPStorage(baron, bunker, Conflux);
      QuickHandStorage(Conflux);

      UsePower(power);
      QuickHPCheck(-2, 2, 2);
      QuickHandCheck(2);
      AssertInTrash(boom);

      UsePower(power);
      QuickHPCheck(-2, 2, 2);
      QuickHandCheck(2);
      AssertInTrash(power);
    }
    #endregion Persistent Tests
  }
}