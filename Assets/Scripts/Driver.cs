using SFB.Game;
using SFB.Game.Content;
using SFB.Game.Management;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour {
    public static Driver instance = null;
    public GameManager gameManager;
	public Phase phase;
 
    [SerializeField]
    private Element[] elemList;
    [SerializeField]
    private Element towerElem;
    public bool combatStarted = false;

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

    private List<int> lane1Damages = new List<int>();
    public List<int> Lane1Damages
    {
        get
        {
            return lane1Damages;
        }
    }

    private List<int> lane2Damages = new List<int>();
    public List<int> Lane2Damages
    {
        get
        {
            return lane2Damages;
        }
    }

    private List<int> lane3Damages = new List<int>();
    public List<int> Lane3Damages
    {
        get
        {
            return lane3Damages;
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
		
        dlC.AddCard(new UnitCard(2, "Exostellar Snipers", Faction.CARTH, "Carthan Elite Ranging Infantry Unit", "Illuminate with Muzzle Flare!", 3, 1, 3), 1);
		dlC.AddCard(new UnitCard(1, "Exostellar Vanshield Marines", Faction.CARTH, "Carthan Elite Infantry Unit", "“First into the Night!”", 1, 3, 5), 1);
		dlC.AddCard(new UnitCard(2, "Exostellar Marine Squad", Faction.CARTH, "Carthan Elite Infantry Unit", "Boldly into the Night!", 2, 2, 4), 2);
		dlC.AddCard(new UnitCard(1, "Cmdr. Yos Lorth", Faction.CARTH, "Unique; Ranged Shield 1\rFront Line: Allied Elites on the Front Line have +1M and Ranged Shield 1.\nCarthan Elite Command Infantry Unit", "Exostellar Champion", 2, 2, 5), 2);
		dlC.AddCard(new UnitCard(2, "Ancillary Medical Officer", Faction.CARTH, "Support Carthan Infantry: Heal this Front Line 2. (when this is deployed behind Carthan Infantry, activate this ability)\rCarthan Infantry Unit", "Healer of Carth", 1, 2, 3), 2);
		dlC.AddCard(new UnitCard(2, "Autonomous Range Finder", Faction.CARTH, "Support Carthan: Give this Front Line +3R this turn. (when this is deployed behind a Carthan, activate this ability)\r", "56413", 0, 1, 3), 1);
        dlC.AddCard(new UnitCard(3, "Adv. Infantry Support System", Faction.CARTH, "Melee Shield 1\rFront or Back Line: At the start of your turn, generate an extra resource.\rCarthan Command Vehicle Unit", "Mobile Command Center", 0, 3, 6), 1);
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

    public CardProperties createHpCardProperty(int hp)
    {
        CardProperties cardProp = new CardProperties();
        cardProp.element = elemList[6];
        cardProp.intValue = hp;
        return cardProp;
    }

    public TowerProperties[] createTowerProperties(int hp)
    {
        TowerProperties[] listOfProp = new TowerProperties[1];
        TowerProperties towerProp = new TowerProperties();
        towerProp.element = towerElem;
        listOfProp[0]=towerProp;
        listOfProp[0].intValue = hp;
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

    public List<TowerFrontEnd> loadTowerFrontEnd()
    {
        List<TowerFrontEnd> ans = new List<TowerFrontEnd>();
        foreach (Lane lane in gameManager.Lanes)
        {
            for (int play = 0; play < lane.Towers.Length; play++)
            {
                if (lane.Towers[play] != null)
                {
                    Tower t = lane.Towers[play];
                    int hp = t.HP;
                    TowerProperties[] tp = new TowerProperties[1];//For now 1, later will probably need Damage,Sprite
                    tp = createTowerProperties(hp);
                    TowerFrontEnd tf = new TowerFrontEnd(tp);
                    ans.Add(tf);
                }
            }
        }
        return ans;
    }

    public List<CardFrontEnd> loadNewHP()
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
                        int hp = u.HealthPoints;
                        CardProperties hpprop = createHpCardProperty(hp);
                        CardProperties[] listOfProperties = new CardProperties[1];
                        listOfProperties[0] = hpprop; 
                        CardFrontEnd cardFront = new CardFrontEnd(listOfProperties);
                        ans.Add(cardFront);
                    }
                }
            }
        }
        return ans;  
    }

    public void updateTowerUI()
    {
        List<TowerFrontEnd> t = loadTowerFrontEnd();
        List<TowerUI> tu = manager.loadTowerUI();
        for (int i = 0; i < t.Count; i++)
        {
            tu[i].LoadTower(t[i]);
        }
    }

    public void updateCardsOntable()
    {
        List<CardFrontEnd> c = loadNewHP();
        List<CardUI> cu = manager.loadCardUI();
        Debug.Log("Lengths==" + (c.Count).ToString() + cu.Count.ToString());
        for (int i = 0; i < c.Count; i++)
        {
            cu[i].loadHp(c[i]);
        }
        manager.cleanup();
    }



    void Update() {
        //updateUI();
        resoureCount = myMana.Count;
        switch (phase) {
            case Phase.DRAW:
                combatStarted = false;
                gameManager.DrawPhase();
				//phase = Phase.PLACEMENT;
				break;
			case Phase.PLACEMENT:
                // handled by UI
                break;
			case Phase.COMBAT:
                manager.makeDraggable(false);
                //gameManager.CombatPhase();
                //phase = gameManager.Over ? Phase.DONE : Phase.DRAW;
                //updateTowerUI();
                if (!combatStarted)
                {
                    combatStarted = true;
                    StartCoroutine(manager.damageAnims());
                }
				break;
			case Phase.DONE:
				print("Game Over: Player " + (gameManager.Players[0].Lives==0?2:1) + " Wins!");
				break;
		}
	}

	public void printField() {
		printField(unit => ""+unit.HealthPoints);
	}

	internal void printField(Func<Unit, string> func) {
		int[][] loop = {
			new int[2] { 1, 1 },
			new int[2] { 1, 0 },
			new int[2] { 0, 0 },
			new int[2] { 0, 1 },
		};
        string a = "T:";
        foreach (Lane lane in gameManager.Lanes)
        {
            a += (lane.Towers[1] != null ? ""+lane.Towers[1].HP : "X") + " ";
        }
        Debug.Log(a);

        foreach (int[] play_pos in loop) {
			string s = "";
			foreach(Lane l in gameManager.Lanes) {
				Unit u = l.Units[play_pos[0], play_pos[1]];
				s += (u!=null ? func(u) : "X") + " ";
			}
			Debug.Log(s);
		}

        string b = "T:";
        foreach (Lane lane in gameManager.Lanes)
        {
            b += (lane.Towers[0] != null ? "" + lane.Towers[0].HP : "X") + " ";
        }
        Debug.Log(b);
    }

	internal void PlayUnitCardAction(UnitCard c, Lane l, int play, int pos) {
		foreach(Delta d in new PlayUnitCardAction(c, l, play, pos).GetDeltas(gameManager.Players[play]))
			d.Apply();
	}
}

public enum Phase {
	DRAW, PLACEMENT, COMBAT, DONE
}