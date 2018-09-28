using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{

    //elements of a card
    private string cardSymbol;
    private int cardValue;
    private string cardSuit;
    private Texture cardTexture;

    //constructor with parameters
    public Card(int value, int suit, Texture cardTexture)
    {
        if (value > 10)
        {
            value = 10;
        }
        if (value == 1)
        {
            value = 11;
        }
        this.cardValue = value;
        this.cardSymbol = makeSymbol(this.cardValue);
        this.cardSuit = makeSuit(suit);
        this.cardTexture = cardTexture;
    }

    public string makeSymbol(int value)
    {
        string newSymbol = "";

        if (value > 1 && value < 11)
        {
            newSymbol = "" + value;
        }
        else if (value == 1)
        {
            newSymbol = "A";
        }
        else if (value == 11)
        {
            newSymbol = "J";
        }
        else if (value == 12)
        {
            newSymbol = "Q";
        }
        else if (value == 13)
        {
            newSymbol = "K";
        }

        return newSymbol;
    }

    /*
    * This method receive an int (1, 2, 3, 4)
    * This numbers means each suit of a card
    * 2 = Clubs, 4 = Spades, 1 = Hearts and 3 = Diamonds
    * Finally, it returns a corresponding symbol for each suit
    */
    public string makeSuit(int suit)
    {
        string newSuit = "";

        if (suit == 1)
        {
            newSuit = "♥";
        }
        else if (suit == 2)
        {
            newSuit = "♣";
        }
        else if (suit == 3)
        {
            newSuit = "♦";
        }
        else if (suit == 4)
        {
            newSuit = "♠";
        }

        return newSuit;
    }

    //get method for the fields

    //get for cardSynbol
    public string CardSymbol
    {
        get
        {
            return this.cardSymbol;
        }
    }

    //get for cardValue
    public int CardValue
    {
        get
        {
            return this.cardValue;
        }
    }

    //get for cardSuit
    public string CardSuit
    {
        get
        {
            return this.cardSuit;
        }
    }

    //get for cardMaterial
    public Texture CardTexture
    {
        get
        {
            return this.cardTexture;
        }
    }


    public void setCardValue(int newValue)
    {
        this.cardValue = newValue;
    }
}
