using SFB.Game;
using SFB.Game.Content;
using SFB.Game.Management;
using SFB.Net.Client;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour {
	public static Driver instance = null;

	public Client client;

	public GameManager gameManager;
 
    [SerializeField]
    private Element[] elemList;
    [SerializeField]
    private Element towerElem;

    public UIManager manager;
	
    public int dropCostSum = 0;

    private List<CardPropertyMap> listofUIOnTable = new List<CardPropertyMap>();
    public  List<CardPropertyMap> ListofUIOnTable
    {
        get
        {
            return listofUIOnTable;
        }
    }
    private List<CardPropertyMap> listofEnemyUI = new List<CardPropertyMap>();
    public List<CardPropertyMap> ListofEnemyUI
    {
        get
        {
            return listofEnemyUI;
        }
    }

    [SerializeField]
    private Sprite exostellar;
    [SerializeField]
    private Sprite jirnorn;
    [SerializeField]
    private Sprite myxori;



    void Awake()
    {
		if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
		
		Client.SetDriver(this);
		/*
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
			
		myMana = gameManager.Players[0].Mana;
		//loadWhenDraw();
		*/
    }

	public CardProperty[] createCardProperties(string name,string type,string flavor, string ability,Sprite sp, int cost,int hp=-1,int melee=-1, int range=-1)
    {
        CardProperty[] listOfProp = new CardProperty[9];
        for (int i=0; i<listOfProp.Length; i++)
        {
            CardProperty cardProp = new CardProperty();
            cardProp.element = elemList[i];
            listOfProp[i] = cardProp;
        }
        listOfProp[0].stringValue = name;
        listOfProp[1].stringValue = type;
        listOfProp[2].stringValue = flavor;
        listOfProp[3].stringValue = ability;
        listOfProp[4].sprite = sp;
        listOfProp[5].intValue = cost;
        listOfProp[6].intValue = hp;
        listOfProp[7].intValue = melee;
        listOfProp[8].intValue = range;
        return listOfProp;
    }

	public CardProperty[] createCardProperties(Unit unit)
	{
		return createCardProperties(
			unit.Card.Name,
			"TYPE",
			unit.Card.FlavorText,
			unit.Card.MainText,
			GetSpriteForFaction(unit.Card.Faction),
			unit.Card.DeployCost,
			unit.HealthPoints,
			unit.MeleeAttack,
			unit.RangedAttack
		);
	}

	public CardProperty[] createCardProperties(Card card)
	{
		if (card is UnitCard)
		{
			UnitCard unitCard = card as UnitCard;

			return createCardProperties(
				card.Name,
				"TYPE",
				card.FlavorText,
				card.MainText,
				GetSpriteForFaction(card.Faction),
				card.DeployCost,
				unitCard.HealthPoints,
				unitCard.MeleeAttack,
				unitCard.RangedAttack
			);
		}
		else
		{
			return createCardProperties(
				card.Name,
				"TYPE",
				card.FlavorText,
				card.MainText,
				GetSpriteForFaction(card.Faction),
				card.DeployCost
			);
		}
	}

	private Sprite GetSpriteForFaction(Faction faction)
	{
		switch (faction)
		{
			case Faction.CARTH:
			case Faction.NONE:
			default:
				return exostellar;
			case Faction.JIRNOR:
				return jirnorn;
			case Faction.MYXOR:
				return myxori;
		}
	}

    public CardProperty createHpCardProperty(int hp)
    {
        CardProperty cardProp = new CardProperty();
        cardProp.element = elemList[6];
        cardProp.intValue = hp;
        return cardProp;
    }

    public TowerProperty[] createTowerProperties(int hp)
    {
        TowerProperty[] listOfProp = new TowerProperty[1];
        TowerProperty towerProp = new TowerProperty();
        towerProp.element = towerElem;
        listOfProp[0]=towerProp;
        listOfProp[0].intValue = hp;
        return listOfProp;
    }

    public void drawCards()
    {
		Client.instance.gameManager.Players[Client.instance.sideIndex].GetDeployPhaseDeltas()[0].Apply();
    }

    public List<CardPropertyMap> loadFrontEnd(Player p)
    {
		Debug.Log($"loadFrontEnd for {p.Name}");
        List<CardPropertyMap> ans = new List<CardPropertyMap>();
        for (int i = 0; i < p.HandSize; i++)
        {
			ans.Add(new CardPropertyMap(createCardProperties(p.Hand[i])));
        }
        return ans;
    }

    public List<TowerData> loadTowerFrontEnd()
    {
        List<TowerData> ans = new List<TowerData>();
        foreach (Lane lane in Client.instance.gameManager.Lanes)
        {
            for (int play = 0; play < lane.Towers.Length; play++)
            {
                if (lane.Towers[play] != null)
                {
                    Tower t = lane.Towers[play];
                    int hp = t.HP;
                    TowerProperty[] tp = new TowerProperty[1];//For now 1, later will probably need Damage,Sprite
                    tp = createTowerProperties(hp);
                    TowerData tf = new TowerData(tp);
                    ans.Add(tf);
                }
            }
        }
        return ans;
    }

	public List<CardPropertyMap> loadNewHP()
	{
		List<CardPropertyMap> ans = new List<CardPropertyMap>();
		foreach (Lane lane in Client.instance.gameManager.Lanes)
		{
			foreach (Unit unit in lane.Units)
			{
				if (unit == null) continue;

				int hp = unit.HealthPoints;
				CardProperty hpProp = createHpCardProperty(hp);
				CardProperty[] listOfProperties = new CardProperty[1];
				listOfProperties[0] = hpProp;
				CardPropertyMap cardFront = new CardPropertyMap(listOfProperties);
				ans.Add(cardFront);
			}
		}
		return ans;
	}

    public void updateTowerUI()
    {
        List<TowerData> t = loadTowerFrontEnd();
        List<TowerUI> tu = manager.loadTowerUI();
        for (int i = 0; i < t.Count; i++)
        {
            tu[i].LoadTower(t[i]);
        }
    }

    public void updateCardsOntable()
    {
		manager.ApplyEnemyUnits();

		Debug.Log($"There are {Client.instance.gameManager.Lanes.Select<Lane, int>(lane => lane.Units.Cast<Unit>().Where(unit => unit != null).Count()).Sum()} units total");
        List<CardPropertyMap> c = loadNewHP();
        List<CardUI> cu = manager.loadCardUI();
		Debug.Log($"CardProperties length {c.Count.ToString()}; CardUIs length {cu.Count.ToString()}");
        for (int i = 0; i < c.Count; i++)
        {
            cu[i].loadHp(c[i]);
        }
        manager.cleanup();
    }


    void Update() {
		Client.instance.Update();
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
        foreach (Lane lane in Client.instance.gameManager.Lanes)
        {
            a += (lane.Towers[1] != null ? ""+lane.Towers[1].HP : "X") + " ";
        }
        Debug.Log(a);

        foreach (int[] play_pos in loop) {
			string s = "";
			foreach(Lane l in Client.instance.gameManager.Lanes) {
				Unit u = l.Units[play_pos[0], play_pos[1]];
				s += (u!=null ? func(u) : "X") + " ";
			}
			Debug.Log(s);
		}

        string b = "T:";
        foreach (Lane lane in Client.instance.gameManager.Lanes)
        {
            b += (lane.Towers[Client.instance.sideIndex] != null ? "" + lane.Towers[Client.instance.sideIndex].HP : "X") + " ";
        }
        Debug.Log(b);
    }
}

public enum Phase {
	DRAW, PLACEMENT, COMBAT, DONE
}