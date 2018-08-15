using System.Collections;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DealerUI : MonoBehaviour {

    public Text outputCard;

    private const int totalCartas = 52;
    private List<Card> deck = new List<Card>();

    // Use this for initialization
    void Start () {
        makeDeck();
        shuffleDeck();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void showCard()
    {
        if (deck.Count() > 0)
        {
            Card cardToShow = deck.ElementAt(deck.Count() - 1);
            outputCard.text = cardToShow.CardSymbol + cardToShow.CardSuit;
            deck.RemoveAt(deck.Count() - 1);
        }
        else
        {
            outputCard.text = "Deck vacío";
        }
    }

    public void preShuffleDeck()
    {
        deck = null;
        deck = new List<Card>();
        makeDeck();
        shuffleDeck();
        outputCard.text = "Output";
            
    }

    /**
     * This method make the 52 cards in deck, 13 card for each suit
     */
    public void makeDeck()
    {
        int cardNum = 1;
        int cardSuit = 1;

        for (int i = 0; i < totalCartas; i++)
        {
            //new beggining for each suit
            if (cardNum > 13)
            {
                cardNum = 1;
                cardSuit++;
            }

            Card newCard = new Card(cardNum, cardSuit);
            deck.Add(newCard);
            cardNum++;
        }
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

        for (int i = 0; i < deck.Count(); i++)
        {
            //make a random between 0 and 51
            randomNumber = Random.Range(0, 52);

            //current card in position i
            Card card1 = deck.ElementAt(i);
            //current card in random position
            Card card2 = deck.ElementAt(randomNumber);

            //clear the current card in the random position before inserting the new element
            deck.RemoveAt(randomNumber);
            //insert the current card in position i in the random position
            deck.Insert(randomNumber, card1);
            //insert the random element card in the position i and then eliminate the card in position i+1
            deck.Insert(i, card2);
            deck.RemoveAt(i + 1);
        }
    }



}
