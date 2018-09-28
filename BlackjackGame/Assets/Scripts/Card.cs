
public class Card
{
    private int value;
    private string symbol; // handles de Ace, Jack, Queen and King
    private string suit;
    private string imagePath;

    public Card(int pValue, string pSymbol, string pSuit)
    {
        this.value = pValue;
        this.symbol = pSymbol;
        this.suit = pSuit;
    }

    public Card(int pValue, string pSuit)
    {
        this.value = pValue;
        this.symbol = "None";
        this.suit = pSuit;
    }

    public int getValue()
    {
        return this.value;
    }

    public string getSymbol()
    {
        return this.symbol;
    }

    public string getSuit()
    {
        return this.suit;
    }

    public void setSuit(string pSuit)
    {
        this.suit = pSuit;
    }

    public void setValue(int pValue)
    {
        this.value = pValue;
    }

    public void setSymbol(string pSymbol)
    {
        this.symbol = pSymbol;
    }

    public void changeValue(int pValue) // to change the Ace value
    {
        this.value = pValue;
    }

    public void setImagePath(string cardName)
    {
        this.imagePath = cardName;
    }

    public string getImagePath()
    {
        return this.imagePath;
    }

    public string getCardName()
    {
        string res = "";
        switch (this.symbol)
        {
            case "None": //Is not an Ace, then
                res = this.value + " of " + this.suit;
                break;
            default: // is an Ace
                res = this.symbol + " of " + this.suit;
                break;
        }
        return res;
    }
}
