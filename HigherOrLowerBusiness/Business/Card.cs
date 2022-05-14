using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigherOrLowerData.Business
{
    public  class Card
    {

        public enum Suits
        {
            Clubs,
            Diamonds,
            Hearts,
            Spades
        }

        public enum CardValues
        {
            Ace,
            One,
            Two,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine,
            Ten,
            Jack,
            Queen,
            King
        }

        public CardValues Value { get; }
        public Suits Suit { get; }

        public Card(CardValues value, Suits suit)
        {
            Value = value;
            Suit = suit;
        }

        public Card(int value, int suit)
        {
            Value = (CardValues) value;
            Suit = (Suits) suit;
        }

        public override string ToString()
        {
            return (Value > CardValues.Ace && Value < CardValues.Jack ? $"{(int)Value}" : Value.ToString()) + " of " + Suit.ToString() ;
        }


    }
}
