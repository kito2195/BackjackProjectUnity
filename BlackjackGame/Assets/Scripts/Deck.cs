using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck
{
    private List<string> deck = new List<string>();

    public Deck()
    {
        makeDeck();
        shuffleDeck();
    }

    public List<string> getDeck()
    {
        return this.deck;
    }

    /**
    * This methos creates a new logical deck with this structure: material+suit+number
    */
    private List<string> makeDeck()
    {

        int cardNum = 1;
        int cardSuit = 1;

        for (int i = 0; i < 52; i++)
        {
            //new beggining for each suit
            if (cardNum > 13)
            {
                cardNum = 1;
                cardSuit++;
            }

            string newCard = i.ToString() + " " + cardSuit.ToString() + " " + cardNum.ToString() + " ";
            this.deck.Add(newCard);
            cardNum++;
        }
        return deck;
    }

    /**
     * This Method shuffle an array with a simple algorithm
     *
     * The algorithm consists of going through a list of cards and change the actual position with a random number
     * This random number is always within in the range (total cards in the list - 1)
     */
    public void shuffleDeck()
    {
        int randomNumber = 0;

        for (int i = 0; i < deck.Count; i++)
        {
            //make a random between 0 and 51
            randomNumber = Random.Range(0, 52);

            //current card in position i
            string card1 = deck.ElementAt(i);
            //current card in random position
            string card2 = deck.ElementAt(randomNumber);

            //clear the current card in the random position before inserting the new element
            this.deck.RemoveAt(randomNumber);
            //insert the current card in position i in the random position
            this.deck.Insert(randomNumber, card1);
            //insert the random element card in the position i and then eliminate the card in position i+1
            this.deck.Insert(i, card2);
            this.deck.RemoveAt(i + 1);
        }
    }
}
