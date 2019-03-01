using SFB.Game;
using SFB.Game.Content;
using SFB.Game.Management;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour {
    public static Driver instance = null;
    public GameManager gameManager;
	public Phase phase;
 
    [SerializeField]
    private Element[] elemList;
    public bool deployDone = false;

    public Manager manager;
    public ResourcePool myMana;
    private ResourcePool enemyMana;
    public Lane[] myLanes=null;

    public int resoureCount = 0;
    public int dropCostSum = 0;

    private List<CardFrontEnd> listofUIOnTable = new List<CardFrontEnd>();
    public  List<CardFrontEnd> ListofUIOnTable
    {
        get
        {
            return listofUIOnTable;
        }
    }
    private List<CardFrontEnd> listofEnemyUI = new List<CardFrontEnd>();
    public List<CardFrontEnd> ListofEnemyUI
    {
        get
        {
            return listofEnemyUI;
        }
    }

    void Awake()
    {
		if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

		DeckList dlC = new DeckList();
        dlC.AddCard(new UnitCard(3, "Exostellar Snipers", Faction.CARTH, "Carthan Elite Ranging Infantry Unit", "Illuminate with Muzzle Flare!", 3, 1, 3), 1);
		dlC.AddCard(new UnitCard(3, "Exostellar Vanshield Marines", Faction.CARTH, "Carthan Elite Infantry Unit", "“First into the Night!”", 1, 3, 5), 1);
		dlC.AddCard(new UnitCard(3, "Exostellar Marine Squad", Faction.CARTH, "Carthan Elite Infantry Unit", "Boldly into the Night!", 2, 2, 4), 2);
		dlC.AddCard(new UnitCard(4, "Cmdr. Yos Lorth", Faction.CARTH, "Unique; Ranged Shield 1\rFront Line: Allied Elites on the Front Line have +1M and Ranged Shield 1.\nCarthan Elite Command Infantry Unit", "Exostellar Champion", 2, 2, 5), 2);
		dlC.AddCard(new UnitCard(2, "Ancillary Medical Officer", Faction.CARTH, "Support Carthan Infantry: Heal this Front Line 2. (when this is deployed behind Carthan Infantry, activate this ability)\rCarthan Infantry Unit", "Healer of Carth", 1, 2, 3), 2);
		dlC.AddCard(new UnitCard(2, "Autonomous Range Finder", Faction.CARTH, "Support Carthan: Give this Front Line +3R this turn. (when this is deployed behind a Carthan, activate this ability)\r", "56413", 0, 1, 3), 1);
        dlC.AddCard(new UnitCard(5, "Adv. Infantry Support System", Faction.CARTH, "Melee Shield 1\rFront or Back Line: At the start of your turn, generate an extra resource.\rCarthan Command Vehicle Unit", "Mobile Command Center", 0, 3, 6), 1);
        dlC.AddCard(new UnitCard(0, "Emergancy Med Drop", Faction.CARTH, "Deploy: Heal this Front Line 2 and each adjacent Front Line 1.\rCarthan Medical Drone Unit", "Forever Alive in the Dark!", 0, 0, 1), 2);
        dlC.AddCard(new UnitCard(2, "Adv. Supply Drone", Faction.CARTH, "Support Carthan: Heal this Front Line 2 and gain 3 Resources.\rCarthan Medical Drone Unit", "762491", 0, 1, 2), 2);
        dlC.AddCard(new UnitCard(3, "Exostellar Rover Marine", Faction.CARTH, "Front Line: If this has a Back Line, this has +2M, else this has +1R.\rCarthan Elite Infantry Unit", "In the Darkness, Everywhere!", 1, 1, 4), 2);
        dlC.AddCard(new UnitCard(4, "Techrositioner Marine", Faction.CARTH, "Front Line Command: Discard a card to gain 1 Resource and give this +2R.\rCarthan Elite Engineering Infantry Unit", "“Lock; Mark; Two; Fire!”", 2, 0, 5), 2);
        dlC.AddCard(new UnitCard(2, "Hover-Shield Projector", Faction.CARTH, "Support Carthan Infantry: Give this Front Line Ranged Shield 3 this turn. (when this is deployed behind Carthan Infantry, activate this ability)\rCarthan Drone Unit", "98432", 0, 1, 4), 2);

        DeckList dlJ = new DeckList();
		dlJ.AddCard(new UnitCard(2, "Dominion Pikeman Squad", Faction.JIRNOR, "Jirnorn Infantry Unit", "“For Jirnor that Will Be!”", 0, 4, 3), 4);
		dlJ.AddCard(new UnitCard(2, "Scions of Radiation", Faction.JIRNOR, "Jirnorn Mutant Infantry Unit", "We Have Nothing to Lose!", 3, 2, 3), 3);
		dlJ.AddCard(new UnitCard(2, "Defenders of the Losthome", Faction.JIRNOR, "Jirnorn Infantry Unit", "For Jirnor that Was!", 2, 1, 3), 3);
		dlJ.AddCard(new UnitCard(3, "Hellfire Bringers", Faction.JIRNOR, "Jirnorn Elite Infantry Unit", "Deliver the Doom of Jirnor!", 2, 1, 3), 3);
		dlJ.AddCard(new UnitCard(4, "Deathspew Battery", Faction.JIRNOR, "Jirnorn Mutant Artillery Unit", "No Remorse for the Unrepentant!", 3, 0, 5), 2);
		dlJ.AddCard(new UnitCard(2, "Vanguards of the Dominion", Faction.JIRNOR, "Jirnorn Mutant Infantry Unit", "After Jirnor, We have Nothing!", 1, 2, 1), 1);
        dlJ.AddCard(new UnitCard(3, "Losthome’s Radio Riders", Faction.JIRNOR, "Jirnorn Mutant Fast Vehicle Unit", "For the Losthome! For Jirnor!", 2, 2, 3), 2);
        dlJ.AddCard(new UnitCard(4, "Salvage Truck", Faction.JIRNOR, "Jirnorn Engineering Vehicle Unit", "Waist Nothing; Leave Nothing", 2, 3, 5), 2);


        // Each deck has 16 cards in it
        DeckList[] deckLists = new DeckList[] { dlC, dlJ };
		gameManager = new GameManager(deckLists);
        myLanes = gameManager.Lanes;
 //       Debug.Log("MuLanes:" + (myLanes == null));
 //       Debug.Log("MuLanes1:" + (myLanes[0] == null));
 //       Debug.Log("MuLanes2:" + (myLanes[1] == null));
 //       Debug.Log("MuLanes3:" + (myLanes[2] == null));

        phase = Phase.DRAW;
        myMana = gameManager.Players[0].Mana;
        resoureCount = myMana.Count;
        //loadWhenDraw();
    }

    public CardProperties[] createCardProperties(string name,string type,string flavor, string ability,int cost,int hp,int melee, int range)
    {
        CardProperties[] listOfProp = new CardProperties[9];
        for (int i=0; i<listOfProp.Length; i++)
        {
            CardProperties cardProp = new CardProperties();
            cardProp.element = elemList[i];
            listOfProp[i] = cardProp;
        }
        listOfProp[0].stringValue = name;
        listOfProp[1].stringValue = type;
        listOfProp[2].stringValue = flavor;
        listOfProp[3].stringValue = ability;
        listOfProp[5].intValue = cost;
        listOfProp[6].intValue = hp;
        listOfProp[7].intValue = melee;
        listOfProp[8].intValue = range;
        return listOfProp;

    }

    public void drawCards()
    {
//        Player us = gameManager.Players[0];
//        us.Hand.DrawFrom(us.Deck);
//        us.Hand.DrawFrom(us.Deck);
//        us.Hand.DrawFrom(us.Deck);
//        Player enemy = gameManager.Players[1];
//        enemy.Hand.DrawFrom(enemy.Deck);
//        enemy.Hand.DrawFrom(enemy.Deck);
//        enemy.Hand.DrawFrom(enemy.Deck);

//        //loadPlayerCards(us);
//        //loadPlayerCards(enemy);
          myMana.Add(2);
          gameManager.Players[0].GetDeployPhaseDeltas()[0].Apply();
////        enemyMana.Add(2);
    }

    //public void loadPlayerCards(Player p)
    //{
    //    for (int i = 0; i < p.HandSize; i++)
    //    {
    //        Debug.Log("NAME:: " + p.Hand[i].Name);
    //        string myName = p.Hand[i].Name;
    //        string flavorText = p.Hand[i].FlavorText;
    //        string mainText = p.Hand[i].MainText;
    //        int cost = p.Hand[i].DeployCost;
    //        UnitCard uc = p.Hand[i] as UnitCard;
    //        int meleeAttack = uc.MeleeAttack;
    //        int rangedAttack = uc.RangedAttack;
    //        int hp = uc.HealthPoints;
    //        CardProperties[] listOfProperties = new CardProperties[9];
    //        listOfProperties = createCardProperties(myName, "TYPE", flavorText, mainText, cost, rangedAttack, meleeAttack, hp);
    //        CardFrontEnd cardFront = new CardFrontEnd(listOfProperties);
    //        if (p == gameManager.Players[0])
    //            listofUI.Add(cardFront);
    //        else if (p == gameManager.Players[1])
    //            listofEnemyUI.Add(cardFront);
    //    }
    //}

    public void updateUIonTable()
    {
        List<CardUI> l = new List<CardUI>();
        for (int i = 0; i < manager.myCardHolders.Length; i++)
        {
            if (manager.myCardHolders[i].transform.childCount>0)
            {
                CardUI c = manager.myCardHolders[i].transform.GetChild(0).GetComponent<CardUI>();
                l.Add(c);
            }
        }
        for (int i = 0; i < listofUIOnTable.Count; i++)
        {
            for (int m = 0; m < l.Count; m++)
            {
                if (l[m].card == listofUIOnTable[i])
                    l[m].LoadCard(listofUIOnTable[i]);
            }
        }
    }


    public List<CardFrontEnd> loadFrontEnd(Player p)
    {
        List<CardFrontEnd> ans = new List<CardFrontEnd>();
        for (int i = 0; i < p.HandSize; i++)
        {
//            Debug.Log("NAME:: " + p.Hand[i].Name);
            string myName = p.Hand[i].Name;
            string flavorText = p.Hand[i].FlavorText;
            string mainText = p.Hand[i].MainText;
            int cost = p.Hand[i].DeployCost;
            UnitCard uc = p.Hand[i] as UnitCard;
            int meleeAttack = uc.MeleeAttack;
            int rangedAttack = uc.RangedAttack;
            int hp = uc.HealthPoints;
            CardProperties[] listOfProperties = new CardProperties[9];
            listOfProperties = createCardProperties(myName, "TYPE", flavorText, mainText, cost, rangedAttack, meleeAttack, hp);
            CardFrontEnd cardFront = new CardFrontEnd(listOfProperties);
            ans.Add(cardFront);
        }
        return ans;
    }

    void Update() {
        //updateUI();
        resoureCount = myMana.Count;
        switch (phase) {
            case Phase.DRAW:
				gameManager.DrawPhase();
				//phase = Phase.PLACEMENT;
				break;
			case Phase.PLACEMENT:
                manager.enemyPlay();
                // handled by UI
                break;
			case Phase.COMBAT:
				gameManager.CombatPhase();
                updateUIonTable();
				phase = gameManager.Over ? Phase.DONE : Phase.DRAW;
				break;
			case Phase.DONE:
				print("Game Over: Player " + (gameManager.Players[0].Lives==0?2:1) + " Wins!");
				break;
		}
	}

	internal void PlayUnitCardAction(UnitCard c, Lane l, int play, int pos) {
		foreach(Delta d in new PlayUnitCardAction(c, l, play, pos).GetDeltas(gameManager.Players[play]))
			d.Apply();
	}
}

public enum Phase {
	DRAW, PLACEMENT, COMBAT, DONE
}