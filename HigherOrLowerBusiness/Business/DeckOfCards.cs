using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigherOrLowerData.Business
{
    public  class DeckOfCards
    {
        public const int NumberOfCards = 52;
        

        private Card[] cards;

        public DeckOfCards(IRandomNumbers randomNumbers)
        {
            this.cards = CreateShuffledDeck(GenerateRandomSequence(randomNumbers));
        }

        public DeckOfCards(string textCards)
        {
            this.cards = Deserialize(textCards);
        }

        public string getCardsAsText()
        {
            return SerializeCards(this.cards);
        }

        public Card getCard(int iCardPosition)
        {
            return this.cards[iCardPosition];
        }



        private string SerializeCards(Card[] cards)
        {
            //  (A -> 9: 1 digit ) + (10 -> King: 2 digits) + (13 suits: 1 digit each)  9*1 + 4*2  + 13*1 = 9 + 8 + 13 = 30 digits  x 4 suits  ==> 120
            //  Each card: 1 space + 1 comma : 2*52 = 104
            StringBuilder sb = new StringBuilder(250);

            foreach (var card in cards)
            {
                if (card is null)
                    continue;

                sb.Append($",{(int)card.Value} {(int)card.Suit}");
            }

            
            sb.Remove(0, 1);

            return sb.ToString();
        }

        private Card[] Deserialize(string textCards)
        {
            Card[] tmpCards = new Card[52];
            var numberCards = textCards.Split(',');
            int i = 0;
            foreach (var textCard in numberCards)
            {
                var eachValue = textCard.Split(' ');
                tmpCards[i++] = new Card(Int32.Parse(eachValue[0]), Int32.Parse(eachValue[1]));
            }

            return tmpCards;

        }

        private Card[] CreateOrderedDeck()
        {
            return new Card[NumberOfCards]
               {
                new Card(Card.CardValues.Ace, Card.Suits.Clubs),        //  0
                new Card(Card.CardValues.Two, Card.Suits.Clubs),        //  1
                new Card(Card.CardValues.Three, Card.Suits.Clubs),      //  2
                new Card(Card.CardValues.Four, Card.Suits.Clubs),
                new Card(Card.CardValues.Five, Card.Suits.Clubs),
                new Card(Card.CardValues.Six, Card.Suits.Clubs),
                new Card(Card.CardValues.Seven, Card.Suits.Clubs),      //  6
                new Card(Card.CardValues.Eight, Card.Suits.Clubs),
                new Card(Card.CardValues.Nine, Card.Suits.Clubs),
                new Card(Card.CardValues.Ten, Card.Suits.Clubs),        //  9
                new Card(Card.CardValues.Jack, Card.Suits.Clubs),       // 10
                new Card(Card.CardValues.Queen, Card.Suits.Clubs),      // 11
                new Card(Card.CardValues.King, Card.Suits.Clubs),       // 12

                new Card(Card.CardValues.Ace, Card.Suits.Diamonds),     // 13
                new Card(Card.CardValues.Two, Card.Suits.Diamonds),
                new Card(Card.CardValues.Three, Card.Suits.Diamonds),
                new Card(Card.CardValues.Four, Card.Suits.Diamonds),
                new Card(Card.CardValues.Five, Card.Suits.Diamonds),
                new Card(Card.CardValues.Six, Card.Suits.Diamonds),
                new Card(Card.CardValues.Seven, Card.Suits.Diamonds),
                new Card(Card.CardValues.Eight, Card.Suits.Diamonds),
                new Card(Card.CardValues.Nine, Card.Suits.Diamonds),
                new Card(Card.CardValues.Ten, Card.Suits.Diamonds),     // 22
                new Card(Card.CardValues.Jack, Card.Suits.Diamonds),
                new Card(Card.CardValues.Queen, Card.Suits.Diamonds),
                new Card(Card.CardValues.King, Card.Suits.Diamonds),    // 25


                new Card(Card.CardValues.Ace, Card.Suits.Hearts),       // 26
                new Card(Card.CardValues.Two, Card.Suits.Hearts),       // 27 
                new Card(Card.CardValues.Three, Card.Suits.Hearts),
                new Card(Card.CardValues.Four, Card.Suits.Hearts),
                new Card(Card.CardValues.Five, Card.Suits.Hearts),
                new Card(Card.CardValues.Six, Card.Suits.Hearts),
                new Card(Card.CardValues.Seven, Card.Suits.Hearts),
                new Card(Card.CardValues.Eight, Card.Suits.Hearts),
                new Card(Card.CardValues.Nine, Card.Suits.Hearts),
                new Card(Card.CardValues.Ten, Card.Suits.Hearts),
                new Card(Card.CardValues.Jack, Card.Suits.Hearts),
                new Card(Card.CardValues.Queen, Card.Suits.Hearts),
                new Card(Card.CardValues.King, Card.Suits.Hearts),      // 38

                new Card(Card.CardValues.Ace, Card.Suits.Spades),       // 39
                new Card(Card.CardValues.Two, Card.Suits.Spades),
                new Card(Card.CardValues.Three, Card.Suits.Spades),     // 41
                new Card(Card.CardValues.Four, Card.Suits.Spades),
                new Card(Card.CardValues.Five, Card.Suits.Spades),
                new Card(Card.CardValues.Six, Card.Suits.Spades),
                new Card(Card.CardValues.Seven, Card.Suits.Spades),
                new Card(Card.CardValues.Eight, Card.Suits.Spades),
                new Card(Card.CardValues.Nine, Card.Suits.Spades),
                new Card(Card.CardValues.Ten, Card.Suits.Spades),
                new Card(Card.CardValues.Jack, Card.Suits.Spades),
                new Card(Card.CardValues.Queen, Card.Suits.Spades),
                new Card(Card.CardValues.King, Card.Suits.Spades)       // 51

               };
        }



        private Card[] CreateShuffledDeck(int[] randomSequence)
        {
            Card?[] tmpDeck = CreateOrderedDeck();
            Card[] shuffledDeck = new Card[tmpDeck.Length];

            // The loop will pick up all available cards (randomly)
            for (int iAvailableCount = 0; iAvailableCount < NumberOfCards; iAvailableCount++)
            {
                // choose the Nth available card 
                int nextAvailableSelected = randomSequence[iAvailableCount]; 
                Card? nextCard = null;

                // Find the nth available card (null element will not be counted.
                for (int j = 0; j < tmpDeck.Length; j++)
                {
                    if (tmpDeck[j] != null) {

                        if (nextAvailableSelected <= 0)
                        {
                            nextCard = tmpDeck[j];
                            tmpDeck[j] = null;
                            break;
                        }

                        nextAvailableSelected--;
                    }
                }

                if (nextCard == null)
                    throw new Exception("NULL CARD " + IntArrayToString(randomSequence));

                shuffledDeck[iAvailableCount] = nextCard;
                
            }
            return shuffledDeck;
        }


        private int[] GenerateRandomSequence(IRandomNumbers randomNumbers)
        {
            int[] randomSequence = new int[NumberOfCards];

            for (int iAvailableCount = NumberOfCards; iAvailableCount > 0; iAvailableCount--)
            {
                // choose the Nth available card 
                randomSequence[NumberOfCards - iAvailableCount] = randomNumbers.NextRandom(iAvailableCount);
            }
            return randomSequence;
        }


        //private string CardArrayToString(Card[] cards)
        //{
        //    StringBuilder sb = new StringBuilder();

        //    foreach (var card in cards)
        //    {
        //        sb.Append("{");
        //        sb.Append(card == null ? "NULL " : card.ToString());
        //        sb.Append("} ");
        //    }

        //    return sb.ToString();
        //}

        private string IntArrayToString( int[] numbers)
        {
            return String.Join(", ",numbers.Select(i => i.ToString()).ToArray());            
        }
    }
}
