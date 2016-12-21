using System;
using System.Collections.Generic;

namespace BlackJack.CardGameFramework
{
    public class Hand
    {
        // Creates a list of cards
        protected List<Card> cards = new List<Card>();
        public int NumCards { get { return cards.Count; } }
		public List<Card> Cards { get { return cards; } }
        public enum HandValue {HIGHCARD, PAIR,TWO_PAIR,TRIPLE,FULL_HOUSE,STRAIGHT,FLUSH,QUAD, STRAIGHT_FLUSH, ROYAL_FLUSH };
        
        /// <summary>
        /// Checks to see if the hand contains a card of a certain face value
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool ContainsCard(FaceValue item)
        {
            foreach (Card c in cards)
            {
                if (c.FaceVal == item)
                {
                    return true;
                }
            }
            return false;
        }

        
    }

    /// <summary>
    /// This class is game-specific.  Creates a BlackJack hand that inherits from class hand
    /// </summary>
    public class BlackJackHand : Hand,IComparable
    {
        private HandValue handValue ;
        public HandValue HandValue { get { return handValue; } }

        private List<Card> allCards;
        public List<Card> AllCards { get { return allCards; } }
        /// <summary>
        /// This method compares two BlackJack hands
        /// </summary>
        /// <param name="otherHand"></param>
        /// <returns></returns>

        public int CompareFaceValue(object otherHand)
        {
            BlackJackHand aHand = otherHand as BlackJackHand;
            if (aHand != null)
            {
                return this.GetSumOfHand().CompareTo(aHand.GetSumOfHand());
            }
            else
            {
                throw new ArgumentException("Argument is not a Hand");
            }
        }

        public int CompareTo(object o)
       
        {
            if (o is BlackJackHand)
            {
                BlackJackHand c = (BlackJackHand)o;
                if (BlackJackLogic.score(this.AllCards) < BlackJackLogic.score(c.AllCards))
                    return -1;
                else if (BlackJackLogic.score(this.AllCards) > BlackJackLogic.score(c.AllCards))
                    return 1;
                else return BlackJackLogic.compareHands(this, c, BlackJackLogic.score(this.AllCards));
            }
            throw new ArgumentException("Object is not a Card");
        }
    

        public int GetSumOfHand()
        {
            return 0;
        }

        /// <summary>
        /// Gets the total value of a hand from BlackJack values
        /// </summary>
        /// <returns>int</returns>
        public HandValue GetValueOfHand(List<Card> tableCards)
        {
            allCards = new List<Card>(Cards);
            foreach(Card c in tableCards)
            {
                if(c != null)
                    allCards.Add(c);
            }
            allCards.Sort();
            

            return BlackJackLogic.score(allCards);
        }
    }
}
