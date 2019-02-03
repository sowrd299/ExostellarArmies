using System.Xml;
using System.Collections.Generic;
using System.Text;
using SFB.Game.Management;
using SFB.Game.Content;

namespace SFB.Game {
// not in management, because more likely to be needed by gamelogi
// management will just treat is as a generic Delta

    // a class to represent removing the given card from the given index the give card from the given index
    public class CardListDelta<T> : TargetedDelta<T> where T : CardList {

        /* TODO: move this over to delta
        public Card Card{
            get{ return card; }
        }
        */

        public enum Mode { ADD, REMOVE }

        private int index;
        private Mode mode; // WHETHER TO ADD OR REMOVE THE CARD IN QUESITON

        public CardListDelta(T list, Card c, int i, Mode m)
            : base(list)
        {
            card = c;
            index = i;
            mode = m;
        }

        public CardListDelta(XmlElement from, IdIssuer<T> issuer, CardLoader cl)
                : base(from, issuer, cl)
        {
        }
        

        protected void addCard(){
            if(target.Count >= index){
                target.Insert(index, card);
            }else{
                throw new IllegalDeltaException("The deck does not contain the index you wish to insert at");
            }
        }

        protected void removeCard(){
            if(target.Count > index && target[index] == card){
                target.RemoveAt(index);
            }else{
                throw new IllegalDeltaException("The card you wish to remove does not exist at that index");
            }
        }

        internal override void Apply(){
            if(mode == Mode.ADD){
                addCard();
            }else{
                removeCard();
            }
        }

        internal override void Revert(){
            if(mode == Mode.ADD){
                removeCard();
            }
            else{
                removeCard();
            }
        }

    }

}