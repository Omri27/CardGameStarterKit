using BlackJack.CardGameFramework;
using System.Collections.Generic;
using System.Linq;

namespace BlackJack
{
    public class BlackJackLogic
    {
        private static Card isFlush(List<Card> h)
        {
            int counter = 0;
            
          for(int i = h.Count-1; i >0; i--)
            {
                
                for(int j=i-1; j >=0; j--)
                {
                    if (h[i].Suit == h[j].Suit)
                    {
                        
                        counter++;
                    }
                }
                if (counter >= 4)
                {
                    return h[i];
                }
                    
                counter = 0;
            }
            return null;
            
        }

        // make sure the rank differs by one
        // we can do this since the Hand is 
        // sorted by this point
        private static Card isStraight(List<Card> h)
        {
            int counter = 0;
            for (int i = h.Count - 1; i >0; i--)
            {
                counter = 0;
                for (int j = i - 1; j >= 0; j--)
                {
                    if (h[j].FaceVal == h[j + 1].FaceVal - 1)
                        counter++;
                    else
                        break;
                }
                if (counter >= 4)
                    return h[i];
                
            }
            return null;
        }

        // must be flush and straight and
        // be certain cards. No wonder I have
        private static bool isRoyalFlush(List<Card> h)
        {
            Card c = isStraight(h);
            Card f = isFlush(h);
            if (c == null || f == null)
                return false;
            if (c.FaceVal == FaceValue.Ace && f.FaceVal == FaceValue.Ace)
                return true;
            return false;
        }

        private static bool isStraightFlush(List<Card> h)
        {
            Card c = isStraight(h);
            Card f = isFlush(h);
            if (c == null || f == null)
                return false;
            if (c.CompareTo(f)==0)
                return true;
            return false;
        }

        /*
         * Two choices here, the first four cards
         * must match in rank, or the second four
         * must match in rank. Only because the hand
         * is sorted
         */
        private static Card isFourOfAKind(List<Card> h)
        {
            int counter = 0;
            for (int i = h.Count - 1; i > 0; i--)
            {
                counter = 0;
                for (int j = i - 1; j >= 0; j--)
                {
                    if (h[i].FaceVal == h[j].FaceVal)
                    {
                        counter++;
                    }
                    
                }
                if (counter == 3)
                    return h[i];

            }
            return null;
        }

        /*
         * two choices here, the pair is in the
         * front of the hand or in the back of the
         * hand, because it is sorted
         */
        private static Card isFullHouse(List<Card> h)
        {
            Card triple = isThreeOfAKind(h);
            List<Card> l = null;
            l = h.Where(x => x != triple).ToList();
            Card pair = isPair(l);
            
            if (triple == null || pair == null)
                return null;

            if (pair.CompareTo(triple) != 0)
                return triple;
            return null;
        }

        /*
         * three choices here, first three cards match
         * middle three cards match or last three cards
         * match
         */
        private static Card isThreeOfAKind(List<Card> h)
        {
            int counter = 0;
            for (int i = h.Count - 1; i > 0; i--)
            {
                counter = 0;
                for (int j = i - 1; j >= 0; j--)
                {
                    if (h[i].FaceVal == h[j].FaceVal)
                    {
                        counter++;
                    }

                }
                if (counter == 2)
                    return h[i];

            }
            return null;
        }

        /*
         * three choices, two pair in the front,
         * separated by a single card or
         * two pair in the back
         */
        private static Card isTwoPair(List<Card> h)
        {
            List<Card> l = null;
            Card c = isPair(h);
            if (c != null) {
                l = h.Where(x => x != c).ToList();
                if (isPair(l) != null)
                    return c;
                    }
            return null;
        }
        private static Card isPair(List<Card> h)
        {
           
            for (int i = h.Count - 1; i > 0; i--)
            {
              
                for (int j = i - 1; j >= 0; j--)
                {
                    if (h[i].FaceVal == h[j].FaceVal)
                    {
                        return h[i];
                    }

                }
               
            }
            return null;
        }
       
      
        public static int compareHands(BlackJackHand hand1, BlackJackHand hand2, Hand.HandValue hand)
        {
            switch (hand)
            {
                case Hand.HandValue.STRAIGHT_FLUSH:
                    return isStraight(hand1.AllCards).CompareTo(isStraight(hand2.AllCards));                 
                case Hand.HandValue.FLUSH:
                    return isFlush(hand1.AllCards).CompareTo(isFlush(hand2.AllCards));
                case Hand.HandValue.QUAD:
                    return isFourOfAKind(hand1.AllCards).CompareTo(isFourOfAKind(hand2.AllCards));
                case Hand.HandValue.STRAIGHT:
                    return isStraight(hand1.AllCards).CompareTo(isStraight(hand2.AllCards));
                case Hand.HandValue.TRIPLE:
                    return isThreeOfAKind(hand1.AllCards).CompareTo(isThreeOfAKind(hand2.AllCards));
                case Hand.HandValue.TWO_PAIR:
                    return isTwoPair(hand1.AllCards).CompareTo(isTwoPair(hand2.AllCards));
                case Hand.HandValue.PAIR:
                    return isPair(hand1.AllCards).CompareTo(isPair(hand2.AllCards));
                case Hand.HandValue.HIGHCARD:
                    return hand1.AllCards[hand1.AllCards.Count - 1].CompareTo(hand2.AllCards[hand2.AllCards.Count - 1]);
            }
            return 0;

        }
        // must be in order of hands and must be
        // mutually exclusive choices
        public static Hand.HandValue score(List<Card> h)
        {
            if (isRoyalFlush(h))
                return Hand.HandValue.ROYAL_FLUSH;
            else if (isStraightFlush(h))
                return Hand.HandValue.STRAIGHT_FLUSH;
            else if (isFourOfAKind(h)!=null)
                return Hand.HandValue.QUAD;
            else if (isFullHouse(h)!=null)
                return Hand.HandValue.FULL_HOUSE;
            else if (isFlush(h) != null)
                return Hand.HandValue.FLUSH;
            else if (isStraight(h) != null)
                return Hand.HandValue.STRAIGHT;
            else if (isThreeOfAKind(h)!=null)
                return Hand.HandValue.TRIPLE;
            else if (isTwoPair(h)!=null)
                return Hand.HandValue.TWO_PAIR;
            else if (isPair(h)!=null)
                return Hand.HandValue.PAIR;
            else
                return Hand.HandValue.HIGHCARD;
        }
    }
}
