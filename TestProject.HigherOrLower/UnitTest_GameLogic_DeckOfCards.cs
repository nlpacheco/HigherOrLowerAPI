using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HigherOrLowerData.Business;
using NUnit.Framework;
using TestProject.HigherOrLower.Mocking;

namespace TestProject.HigherOrLower
{
    public class UnitTest_GameLogic_DeckOfCards
    {
        private readonly string fakeRandomCardAsText = "3 3,2 2,10 1,10 0,12 2,13 2,0 2,2 3,9 2,10 2,0 3,8 3,7 3,12 3,4 2,11 0,5 1,10 3,6 1,2 1,5 0,6 2,0 0,9 3,9 1,3 2,6 3,7 0,11 3,11 2,13 3,0 1,9 0,8 0,5 3,3 1,8 2,2 0,11 1,12 1,8 1,4 3,7 2,4 0,4 1,7 1,13 1,6 0,3 0,5 2,13 0,12 0";
        private readonly Card fakeCard0 = new Card(Card.CardValues.Three, Card.Suits.Spades);
        private readonly Card fakeCard1 = new Card(Card.CardValues.Two, Card.Suits.Hearts);


        [Test]
        public void Test_Shuffle()
        {

            Mocking.FakeRandomNumbers randomNumbers = new Mocking.FakeRandomNumbers();
            DeckOfCards deckOfCards = new DeckOfCards(randomNumbers);

            TestContext.Out.WriteLine("Cards: " + deckOfCards.getCardsAsText());
            TestContext.Out.WriteLine("FakeCards: " + fakeRandomCardAsText);

            Assert.AreEqual(deckOfCards.getCardsAsText(), fakeRandomCardAsText);
       
        }

        [Test]
        public void Test_LoadCards()
        {

            Mocking.FakeRandomNumbers randomNumbers = new Mocking.FakeRandomNumbers();
            DeckOfCards deckOfCards = new DeckOfCards(fakeRandomCardAsText);

            TestContext.Out.WriteLine("Card 0: " + deckOfCards.getCard(0));
            TestContext.Out.WriteLine("Card 1: " + deckOfCards.getCard(1));

            Assert.AreEqual(deckOfCards.getCard(0).ToString(), fakeCard0.ToString()) ;
            Assert.AreEqual(deckOfCards.getCard(1).ToString(), fakeCard1.ToString());

        }

    }
}
