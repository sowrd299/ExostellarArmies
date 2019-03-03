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
		
        dlC.AddCard(new UnitCard(2, "Exostellar Snipers", Faction.CARTH, "Carthan Elite Ranging Infantry Unit", "Illuminate with Muzzle Flare!", 3, 1, 30), 1);
		dlC.AddCard(new UnitCard(1, "Exostellar Vanshield Marines", Faction.CARTH, "Carthan Elite Infantry Unit", "“First into the Night!”", 1, 3, 50), 1);
		dlC.AddCard(new UnitCard(2, "Exostellar Marine Squad", Faction.CARTH, "Carthan Elite Infantry Unit", "Boldly into the Night!", 2, 2, 40), 2);
		dlC.AddCard(new UnitCard(1, "Cmdr. Yos Lorth", Faction.CARTH, "Unique; Ranged Shield 1\rFront Line: Allied Elites on the Front Line have +1M and Ranged Shield 1.\nCarthan Elite Command Infantry Unit", "Exostellar Champion", 2, 2, 50), 2);
		dlC.AddCard(new UnitCard(2, "Ancillary Medical Officer", Faction.CARTH, "Support Carthan Infantry: Heal this Front Line 2. (when this is deployed behind Carthan Infantry, activate this ability)\rCarthan Infantry Unit", "Healer of Carth", 1, 2, 30), 2);
		dlC.AddCard(new UnitCard(2, "Autonomous Range Finder", Faction.CARTH, "Support Carthan: Give this Front Line +3R this turn. (when this is deployed behind a Carthan, activate this ability)\r", "56413", 0, 1, 30), 1);
        dlC.AddCard(new UnitCard(3, "Adv. Infantry Support System", Faction.CARTH, "Melee Shield 1\rFront or Back Line: At the start of your turn, generate an extra resource.\rCarthan Command Vehicle Unit", "Mobile Command Center", 0, 3, 60), 1);
        dlC.AddCard(new UnitCard(0, "Emergancy Med Drop", Faction.CARTH, "Deploy: Heal this Front Line 2 and each adjacent Front Line 1.\rCarthan Medical Drone Unit", "Forever Alive in the Dark!", 0, 0, 10), 2);
        dlC.AddCard(new UnitCard(2, "Adv. Supply Drone", Faction.CARTH, "Support Carthan: Heal this Front Line 2 and gain 3 Resources.\rCarthan Medical Drone Unit", "762491", 0, 1, 20), 2);
        dlC.AddCard(new UnitCard(3, "Exostellar Rover Marine", Faction.CARTH, "Front Line: If this has a Back Line, this has +2M, else this has +1R.\rCarthan Elite Infantry Unit", "In the Darkness, Everywhere!", 1, 1, 40), 2);
        dlC.AddCard(new UnitCard(4, "Techrositioner Marine", Faction.CARTH, "Front Line Command: Discard a card to gain 1 Resource and give this +2R.\rCarthan Elite Engineering Infantry Unit", "“Lock; Mark; Two; Fire!”", 2, 0, 50), 2);
        dlC.AddCard(new UnitCard(2, "Hover-Shield Projector", Faction.CARTH, "Support Carthan Infantry: Give this Front Line Ranged Shield 3 this turn. (when this is deployed behind Carthan Infantry, activate this ability)\rCarthan Drone Unit", "98432", 0, 1, 40), 2);


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
          myMana.Add(2);
          gameManager.Players[0].GetDeployPhaseDeltas()[0].Apply();
////        enemyMana.Add(2);
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
            listOfProperties = createCardProperties(myName, "TYPE", flavorText, mainText, cost, hp, meleeAttack, rangedAttack);
            CardFrontEnd cardFront = new CardFrontEnd(listOfProperties);
            ans.Add(cardFront);
        }
        return ans;
    }

    public List<CardFrontEnd> loadFrontEnd2()
    {
        List<CardFrontEnd> ans = new List<CardFrontEnd>();
        foreach (Lane lane in gameManager.Lanes)
        {
            for (int play = 0; play < lane.Units.GetLength(0); play++)
            {
                for (int pos = 0; pos < lane.Units.GetLength(1); pos++)
                {
                    if (lane.Units[play, pos] != null)
                    {
                        Unit u = lane.Units[play, pos];
                        string myName = u.Card.Name;
                        string flavorText = u.Card.FlavorText;
                        string mainText = u.Card.MainText;
                        int cost = u.Card.DeployCost;
                        int meleeAttack = u.Card.MeleeAttack;
                        int rangedAttack = u.Card.RangedAttack;
                        int hp = u.Card.HealthPoints;
                        CardProperties[] listOfProperties = new CardProperties[9];
                        listOfProperties = createCardProperties(myName, "TYPE", flavorText, mainText, cost, hp, meleeAttack, rangedAttack);
                        CardFrontEnd cardFront = new CardFrontEnd(listOfProperties);
                        ans.Add(cardFront);
                    }
                }
            }
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
                // handled by UI
                break;
			case Phase.COMBAT:
				//Debug.Log("LeftFront " + (gameManager.Lanes[0].Units[0, 0]==null));
				//Debug.Log("LeftBack " + (gameManager.Lanes[0].Units[0, 1] == null));
				//Debug.Log("MiddleFront " + (gameManager.Lanes[1].Units[0, 0] == null));
				//Debug.Log("MiddleBack " + (gameManager.Lanes[1].Units[0, 1] == null));
				//Debug.Log("RightFront " + (gameManager.Lanes[2].Units[0, 0] == null));
				//Debug.Log("RightBack " + (gameManager.Lanes[2].Units[0, 0].HealthPoints));

				gameManager.CombatPhase();
				
                //Debug.Log("LeftFront After" + (gameManager.Lanes[0].Units[0, 0] == null));
                //Debug.Log("LeftBack After" + (gameManager.Lanes[0].Units[0, 1] == null));
                //Debug.Log("MiddleFront After" + (gameManager.Lanes[1].Units[0, 0] == null));
                //Debug.Log("MiddleBack After" + (gameManager.Lanes[1].Units[0, 1] == null));
                //Debug.Log("RightFront After" + (gameManager.Lanes[2].Units[0, 0] == null));
                //Debug.Log("RightBack After" + (gameManager.Lanes[2].Units[0, 0].HealthPoints));

                List<CardFrontEnd> c = loadFrontEnd2();
                List<CardUI> cu = manager.loadCardUI();
                Debug.Log("Lengths==" + (c.Count == cu.Count));
                for (int i = 0; i < c.Count; i++)
                {
                    cu[i].LoadCard(c[i]);
                }

                //Debug.Log("HP:" + gameManager.Lanes[0].Units[0, 0].HealthPoints + "R:" + gameManager.Lanes[0].Units[0, 0].RangedAttack + "R:" + gameManager.Lanes[0].Units[0, 0].MeleeAttack);
				phase = gameManager.Over ? Phase.DONE : Phase.DRAW;
                StartCoroutine(manager.damageAnims());
				break;
			case Phase.DONE:
				print("Game Over: Player " + (gameManager.Players[0].Lives==0?2:1) + " Wins!");
				break;
		}
	}

	public void printField() {
		int[][] loop = {
			new int[2] { 1, 1 },
			new int[2] { 1, 0 },
			new int[2] { 0, 0 },
			new int[2] { 0, 1 },
		};

		foreach(int[] play_pos in loop) {
			string s = "";
			foreach(Lane l in gameManager.Lanes) {
				Unit u = l.Units[play_pos[0], play_pos[1]];
				s += (u!=null ? ""+u.HealthPoints : "X") + " ";
			}
			Debug.Log(s);
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