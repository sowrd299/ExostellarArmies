using SFB.Game;
using SFB.Game.Content;
using SFB.Game.Management;
using SFB.Net.Client;
using System;
using System.Xml;
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
    }

	public void ProcessTurnStartDeltas(XmlDocument doc, CardLoader loader) {
		string phase = "X";
		//X:		misc deploy phase stuff
		//R, M, T:	ranged, melee, tower
		//D:		draw
		List<Delta> deltasOfPhase = new List<Delta>();

		List<Type> combatPhases = new List<Type>{ typeof(UnitDelta), typeof(TowerDelta) };

		foreach(XmlElement element in doc.GetElementsByTagName("delta")) {
			Debug.Log($"Sorting Turn Start Delta:\n'{element.OuterXml}'");

			Delta d = Delta.FromXml(element, loader);
			
			string phaseOfDelta = "X";
			if(combatPhases.Contains(Type.GetType((element?.Attributes["type"]?.Value))))
				phaseOfDelta = element?.Attributes["dmgType"]?.Value;
			else if(phase != "X")
				phaseOfDelta = "D";
			
			
			if(phase != phaseOfDelta) {
				// apply all deltas of phase that just ended
				foreach(Delta dta in deltasOfPhase) {
					Debug.Log($"Applying Turn Start Delta:\n'{dta}'");
					dta.Apply();
					// TODO ui stuff
				}
				deltasOfPhase.Clear();

				// next phase
				phase = phaseOfDelta;
			}

			deltasOfPhase.Add(d);
		}

		// apply deltas of last phase
		foreach(Delta dta in deltasOfPhase) {
			Debug.Log($"Applying Turn Start Delta:\n'{dta}'");
			dta.Apply();
		}
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
		manager.LockUnitsOnField();

		for (int sideIndex = 0; sideIndex < gameManager.Players.Length; sideIndex++)
		{
			for (int laneIndex = 0; laneIndex < gameManager.Lanes.Length; laneIndex++)
			{
				Lane lane = gameManager.Lanes[laneIndex];

				for (int positionIndex = 0; positionIndex < lane.Units.GetLength(1); positionIndex++)
				{
					Unit unit = lane.Units[sideIndex, positionIndex];
					
					if (unit != null && unit.HealthPoints > 0)
					{
						CardUI unitUI = manager.FindUnitAt(sideIndex, laneIndex, positionIndex);
						unitUI.LoadHp(unit.HealthPoints);
					}
					else
					{
						manager.RemoveUnitAt(sideIndex, laneIndex, positionIndex);
					}
				}
			}
		}
    }


    void Update() {
		Client.instance.Update();
	}
}

public enum Phase {
	DRAW, PLACEMENT, COMBAT, DONE
}