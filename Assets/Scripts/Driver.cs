using SFB.Game;
using SFB.Game.Content;
using SFB.Game.Management;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour {
    public static Driver instance = null;
    public GameManager gameManager;
	Phase phase;

    private List<CardFrontEnd> listofUI = new List<CardFrontEnd>();
    public  List<CardFrontEnd> ListofUI
    {
        get
        {
            return listofUI;
        }
    }

    void Awake()
    {
		if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }


    [SerializeField]
    private Element[] elemList;



    void Start() {
		DeckList dlC = new DeckList();
		dlC.AddCard(new UnitCard(1, "Carth1Cost", Faction.CARTH,"Main","Flavor", 1, 1, 1), 4);
		dlC.AddCard(new UnitCard(2, "Carth2Cost#1", Faction.CARTH, "Main", "Flavor", 1, 1, 1), 3);
		dlC.AddCard(new UnitCard(2, "Carth2Cost#2", Faction.CARTH, "Main", "Flavor", 1, 1, 1), 3);
		dlC.AddCard(new UnitCard(3, "Carth3Cost", Faction.CARTH, "Main", "Flavor", 1, 1, 1), 3);
		dlC.AddCard(new UnitCard(4, "Carth4Cost", Faction.CARTH, "Main", "Flavor", 1, 1, 1), 2);
		dlC.AddCard(new UnitCard(5, "Carth5Cost", Faction.CARTH, "Main", "Flavor", 1, 1, 1), 1);

		DeckList dlJ = new DeckList();
		dlJ.AddCard(new UnitCard(1, "Jirnor1Cost", Faction.JIRNOR,"Main", "Flavor", 1, 1, 1), 4);
		dlJ.AddCard(new UnitCard(2, "Jirnor2Cost#1", Faction.JIRNOR, "Main", "Flavor", 1, 1, 1), 3);
		dlJ.AddCard(new UnitCard(2, "Jirnor2Cost#2", Faction.JIRNOR, "Main", "Flavor", 1, 1, 1), 3);
		dlJ.AddCard(new UnitCard(3, "Jirnor3Cost", Faction.JIRNOR, "Main", "Flavor", 1, 1, 1), 3);
		dlJ.AddCard(new UnitCard(4, "Jirnor4Cost", Faction.JIRNOR, "Main", "Flavor", 1, 1, 1), 2);
		dlJ.AddCard(new UnitCard(5, "Jirnor5Cost", Faction.JIRNOR, "Main", "Flavor", 1, 1, 1), 1);


		// Each deck has 16 cards in it
		DeckList[] deckLists = new DeckList[] { dlC, dlJ };


		gameManager = new GameManager(deckLists);

        phase = Phase.DRAW;

        //Loading UI 
        for (int i = 0; i < 3; i++)
        {
            CardProperties[] listOfProperties = new CardProperties[9];
            listOfProperties = createCardProperties(9, "NAME", "TYPE", "FLAVOR", "ABILITY", 5, 5, 5, 5);
            CardFrontEnd cardFront = new CardFrontEnd(listOfProperties);
            listofUI.Add(cardFront);
        }
    }

    public CardProperties[] createCardProperties(int numberOfProperties, string name,string type,string flavor, string ability,int cost,int hp,int melee, int range)
    {
        CardProperties[] listOfProp = new CardProperties[numberOfProperties];
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

    void Update() {
		switch(phase) {
			case Phase.DRAW:
				gameManager.DrawPhase();
				phase = Phase.PLACEMENT;

				break;
			case Phase.PLACEMENT:
				// handled by UI
				break;
			case Phase.COMBAT:
				gameManager.CombatPhase();
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

enum Phase {
	DRAW, PLACEMENT, COMBAT, DONE
}