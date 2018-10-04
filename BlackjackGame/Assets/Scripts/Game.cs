using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//Event that tells the client that this player's initial bet is ready
[System.Serializable]
public class InitialBetReady : UnityEvent<int>
{
}

//Event that tells the client that this player's blackjack bet is ready
[System.Serializable]
public class BlackjckBetReady : UnityEvent<int>
{
}

public class Game : MonoBehaviour
{
    //public fields
    public float moveSpeed;
    public Transform[] fieldsPosition;
    public GameObject cardPrefab;
  

    public InitialBetReady betReady;
    public UnityEvent passTurnEvent;
    public UnityEvent askForACard;
    public UnityEvent doubleBetEvent;
    public BlackjckBetReady blackjackBetReady;

    //private fields
    private int currentCard;
    private int totalPlayers = 4;
    private List<GameObject> cardsToDeal; 
    private const int totalCards = 52;
    private int currentPlayerPosition;
    private List<Card> cardsDealed = new List<Card>();
    private List<int> casinoCards = new List<int>();
    private List<int> player1Cards = new List<int>();
    private List<int> player2Cards = new List<int>();
    private List<int> player3Cards = new List<int>();

    //tmp new functionality
    private List<Card> cardToInstante = new List<Card>(); // pile with the 52 cards

    // Use this for initialization
    void Start()
    {
        cardsToDeal = new List<GameObject>();
        currentCard = 0;
    }

    // Update is called once per frame
    void Update()
    {

        //To give every player 2 cards, when everyone make their initial bets
        if (cardsToDeal.Count == totalPlayers * 2)
        {
            if (currentCard < cardsToDeal.Count)
            {
                if (cardsToDeal[currentCard].transform.position == fieldsPosition[currentCard].position)
                {
                    if (currentCard > 0)
                    {
                        Vector3 newRotation = new Vector3(180, 0, 0);
                        cardsToDeal[currentCard].transform.Rotate(newRotation * 5f);
                    }
                    currentCard++;
                }
                else
                {
                    cardsToDeal[currentCard].transform.position = Vector3.MoveTowards(cardsToDeal[currentCard].transform.position, fieldsPosition[currentCard].position, moveSpeed * Time.deltaTime);

                }
            }
        }

        //To give one extra card to player 1
        else if (cardsToDeal.Count > totalPlayers * 2)
        {
            if (currentCard < cardsToDeal.Count)
            {
                if (cardsToDeal[currentCard].transform.position == fieldsPosition[currentCard].position)
                {
                    Vector3 newRotation = new Vector3(180, 0, 0);
                    cardsToDeal[currentCard].transform.Rotate(newRotation * 5f);
                    currentCard++;
                }
                else
                {
                    cardsToDeal[currentCard].transform.position = Vector3.MoveTowards(cardsToDeal[currentCard].transform.position, fieldsPosition[currentCard].position, moveSpeed * Time.deltaTime);
                }
            }
        }    
    }

    public void PutAPlayerInTheTable(int position, string remotePlayer)
    {
        //Puts the local player in the table
        if (remotePlayer == null)
        {
            this.currentPlayerPosition = position;
            this.playerNames[position].text = Player.Nickname;
            this.playersCoins[position].text = Player.CoinsInGame.ToString();
        }
        //Puts a remote player in the table
        else
        {
            this.playerNames[position].text = remotePlayer;
        }
    }

    //Puts an initial bet from another player in this bet
    public void putRemoteInitialBet(int position, string bet)
    {
        this.playersBets[position].text = "Bet: " + bet;
        DealCoins(position, int.Parse(bet));
    }

    //Recieves a logical card with format material+suit+number and turns them into Card objects and put them in the table
    public void putInitialCards(List<string> cards)
    {
        int i = 0;
        foreach (string logicalCard in cards)
        {
            if (i == 0 || i == 1)
            {
                createCard(logicalCard, -1);
            }
            else if (i == 2 || i == 3)
            {
                createCard(logicalCard, 0);
            }
            else if (i == 4 || i == 5)
            {
                createCard(logicalCard, 1);
            }
            else if (i == 6 || i == 7)
            {
                createCard(logicalCard, 2);
            }
            i++;
        }
        DealCards();
    }

    //creates a card from a logical card, and asign that card to one player
    public void createCard(string logicalCard, int player)
    {

        Card newCard = new Card(logicalCard);
        if (player == 0)
        {
            this.player1Cards.Add(newCard.Value);
        }
        else if (player == 1)
        {
            this.player2Cards.Add(newCard.Value);
        }
        else if (player == 2)
        {
            this.player3Cards.Add(newCard.Value);
        }
        else if (player == -1)
        {
            this.casinoCards.Add(newCard.Value);
        }
        cardToInstante.Add(newCard);

    }

    public void turnChange(string name, int number)
    {
        updatePlayersHandsCount(false);
        this.turnText.text = name + "'s Turn";
        //Determines if this client is the next in turn
        // If current player has the turn, every buttons are activated
        if (name == Player.Nickname)
        {
            activateButtons();
        }
    }

    //Makes the casino play
    public void casinoPlay(List<string> casinoExtraCards)
    {
        this.turnText.text = "Casino Turn";
        //Turns the hidden card, so everyone can see it
        Vector3 newRotation = new Vector3(180, 0, 0);
        cardsToDeal[0].transform.Rotate(newRotation * 5f);
        updatePlayersHandsCount(true);
        if (casinoExtraCards == null)
        {
            determineWinners();
        }
        //If the casino ask for extra cards
        else
        {
            StartCoroutine(putCasinoExtraCards(casinoExtraCards));
        }
    }

    //Puts the casino extra cards in the table
    public IEnumerator putCasinoExtraCards(List<string> casinoExtraCards)
    {
        foreach (string card in casinoExtraCards)
        {
            RecieveExtraCard(-1, card, false);
            updatePlayersHandsCount(true);
            yield return new WaitForSeconds(2);
        }
        determineWinners();
    }

    //Determines the winners and losers of this match
    public void determineWinners()
    {
        int casinoPunctuation = getPlayerPunctuation(this.casinoCards);
        int player1Punctuation = getPlayerPunctuation(this.player1Cards);
        int player2Punctuation = getPlayerPunctuation(this.player2Cards);
        int player3Punctuation = getPlayerPunctuation(this.player3Cards);
        List<int> playersGain = new List<int>();
        playersGain.Add(comparePunctuations(casinoPunctuation, player1Punctuation, 0, this.player1Cards) + checkForBlackjackBet(0));
        playersGain.Add(comparePunctuations(casinoPunctuation, player2Punctuation, 1, this.player2Cards) + checkForBlackjackBet(1));
        playersGain.Add(comparePunctuations(casinoPunctuation, player3Punctuation, 2, this.player3Cards) + checkForBlackjackBet(2));
        int currentPlayerCoins = int.Parse(this.playersCoins[this.currentPlayerPosition].text);
        currentPlayerCoins += playersGain[this.currentPlayerPosition];
        this.playersCoins[this.currentPlayerPosition].text = currentPlayerCoins.ToString();
        this.hideAnotherRoundWindow = false;
    }

    /*
     * Compares the punctuations between the casino and one player
     * Returns the gain of the player
     */
    private int comparePunctuations(int casinoPunctuation, int playerPunctuation, int player, List<int> playerCards)
    {
        int playerBet = getPlayerBet(player);
        int gain = 0;
        if (casinoPunctuation > 21)
        {
            if (playerPunctuation > 21)
            {
                gain += playerBet;
                this.playersBets[player].text = "Tie: + " + playerBet.ToString();
            }
            else if (playerPunctuation == 21 && this.player1Cards.Count == 2)
            {
                playerBet = playerBet + ((playerBet / 2) * 3);
                gain += playerBet;
                this.playersBets[player].text = "Winner: + " + playerBet.ToString();
            }
            else
            {
                playerBet = playerBet * 2;
                gain += playerBet;
                this.playersBets[player].text = "Winner: + " + playerBet.ToString();
            }
        }
        else
        {
            if (playerPunctuation > 21)
            {
                this.playersBets[player].text = "Loser: - " + playerBet.ToString();
            }
            else if (playerPunctuation == casinoPunctuation)
            {
                if (playerPunctuation == 21)
                {
                    if(playerCards.Count == 2 && casinoCards.Count > 2)
                    {
                        playerBet = playerBet + ((playerBet / 2) * 3);
                        gain += playerBet;
                        this.playersBets[player].text = "Winner: + " + playerBet.ToString();
                    }
                    else if(playerCards.Count > 2 && casinoCards.Count == 2)
                    {
                        this.playersBets[player].text = "Loser: - " + playerBet.ToString();
                    }
                    else
                    {
                        gain += playerBet;
                        this.playersBets[player].text = "Tie: + " + playerBet.ToString();
                    }
                }
                else
                {
                    gain += playerBet;
                    this.playersBets[player].text = "Tie: + " + playerBet.ToString();
                }
            }
            else if (playerPunctuation > casinoPunctuation)
            {
                if (playerPunctuation == 21 && playerCards.Count == 2)
                {
                    playerBet = playerBet + ((playerBet / 2) * 3);
                    gain += playerBet;
                    this.playersBets[player].text = "Winner: + " + playerBet.ToString();
                }
                else
                {
                    playerBet = playerBet * 2;
                    gain += playerBet;
                    this.playersBets[player].text = "Winner: + " + playerBet.ToString();
                }
            }
            else if (playerPunctuation < casinoPunctuation)
            {
                this.playersBets[player].text = "Loser: - " + playerBet.ToString();
            }
        }
        return gain;
    }

    /*
     *checks for the players that made a secondary bet (if the casio has blackjack)
     *returns the total gain of the player
     */
     private int checkForBlackjackBet(int player)
    {
        int gain = 0;
        if (playersBlackjackBet[player].text != "")
        {
            int card1 = this.cardsDealed[0].Value;
            int card2 = this.cardsDealed[1].Value;
            int playerBlackjackBet = getPlayerBlackjackBet(player);
            if ((card1 + card2) == 21)
            {
                playerBlackjackBet = playerBlackjackBet * 3;
                gain += playerBlackjackBet;
                this.playersBlackjackBet[player].text = "BJ Winner: +" + playerBlackjackBet;
            }
            else
            {
                this.playersBlackjackBet[player].text = "BJ Loser: -" + playerBlackjackBet;
            }
        }
        return gain;
    }

    //Returns the sum of every card of the player
    private int getPlayerPunctuation(List<int> playerCards)
    {
        int total = 0;
        foreach (int card in playerCards)
        {
            total += card;
        }
        if (total > 21)
        {
            for (int i = 0; i < playerCards.Count; i++)
            {
                if (playerCards[i] == 11)
                {
                    total -= 10;
                    playerCards[i] = 1;
                    break;
                }
            }
        }
        return total;
    }

    //Updates every player hand count
    private void updatePlayersHandsCount(bool casinoHand)
    {
        int player1Punctuation = getPlayerPunctuation(this.player1Cards);
        int player2Punctuation = getPlayerPunctuation(this.player2Cards);
        int player3Punctuation = getPlayerPunctuation(this.player3Cards);
        this.playersHandCount[0].text = "Hand Count: " + player1Punctuation.ToString();
        this.playersHandCount[1].text = "Hand Count: " + player2Punctuation.ToString();
        this.playersHandCount[2].text = "Hand Count: " + player3Punctuation.ToString();
        if (casinoHand)
        {
            int casinoPunctuation = getPlayerPunctuation(this.casinoCards);
            this.playersHandCount[3].text = "Casino Count: " + casinoPunctuation.ToString();
        }
    }

   
  
    

    void DealCards()
    {
        //put the initial cards to deal in the deck position
        int i = 0;
        while (i < totalPlayers * 2)
        {
            //reference the position of the deck
            Transform deckPosition = GameObject.Find("Deck").GetComponent<Transform>();
            GameObject card = (GameObject)Instantiate(Resources.Load("Prefabs/Card"), deckPosition.position, deckPosition.rotation);
            card.transform.Rotate(0, -90, 0);
            // generates the Card prefab
            ObjectCard tmpCard = card.GetComponent<ObjectCard>();
            ReduceThicknessOfDeck();
            // set the logicalCard values to the prefab object   
            Card removed = cardToInstante[0];
            //delete the card from the array
            cardToInstante.RemoveAt(0);
            tmpCard.gameObject.GetComponent<ObjectCard>().SetCard(removed);
            //add the current to an a list to manage the card in the game 
            cardsDealed.Add(removed);
            //add the prefab to move the card from deckposition to cardposition
            cardsToDeal.Add(card);
            i++;
        }
        //clear the pile to avoid any error
        cardToInstante.Clear();
    }

    private void ReduceThicknessOfDeck() // animation of taking a card from the deck (reduces the Scale Y of the deck)
    {
        GameObject objectDeck = GameObject.Find("Deck");
        objectDeck.transform.localScale = (objectDeck.transform.localScale - new Vector3(0, 0.00943f, 0));
    }


   

    //Recieves an extra card for one player and put it in the table
    public void RecieveExtraCard(int player, string logicalCard, bool turnChange)
    {
        this.bet21Button.GetComponent<Button>().interactable = false;
        this.doubleBetButton.GetComponent<Button>().interactable = false;
        createCard(logicalCard, player);

        //reference the position of the deck
        Transform deckPosition = GameObject.Find("Deck").GetComponent<Transform>();
        GameObject card = (GameObject)Instantiate(Resources.Load("Prefabs/Card"), deckPosition.position, deckPosition.rotation);
        card.transform.Rotate(0, -90, 0);
        // generates the Card prefab
        ObjectCard tmpCard = card.GetComponent<ObjectCard>();
        ReduceThicknessOfDeck();
        // set the logicalCard values to the prefab object   
        Card removed = cardToInstante[0];
        //delete the card from the array
        cardToInstante.RemoveAt(0);
        tmpCard.gameObject.GetComponent<ObjectCard>().SetCard(removed);
        //add the current to an a list to manage the card in the game 
        cardsDealed.Add(removed);
        //add the prefab to move the card from deckposition to cardposition
        cardsToDeal.Add(card);
        //clear the pile to avoid any error
        cardToInstante.Clear();

        List<Transform> variable = fieldsPosition.ToList();
        int positionExtraCard = 3 + (player * 2);
        variable.Add(fieldsPosition[positionExtraCard]);
        fieldsPosition = variable.ToArray<Transform>();
        if(player == -1)
        {
            fieldsPosition[currentCard].position = new Vector3(fieldsPosition[positionExtraCard].position.x - 1, fieldsPosition[positionExtraCard].position.y, fieldsPosition[positionExtraCard].position.z);
        }
        else
        {
            fieldsPosition[currentCard].position = new Vector3(fieldsPosition[positionExtraCard].position.x - 0.70f, fieldsPosition[positionExtraCard].position.y + 0.01f, fieldsPosition[positionExtraCard].position.z - 0.50f);
        }
        int currentPlayerPunctuation = 0;
        updatePlayersHandsCount(false);
        if (player == 0)
        {
            currentPlayerPunctuation = getPlayerPunctuation(this.player1Cards);
        }
        if (player == 1)
        {
            currentPlayerPunctuation = getPlayerPunctuation(this.player2Cards);
        }
        if (player == 2)
        {
            currentPlayerPunctuation = getPlayerPunctuation(this.player3Cards);
        }
        if (turnChange && currentPlayerPunctuation > 21)
        {
            if (player == this.currentPlayerPosition)
            {
                StartCoroutine(waitForTurnChange());
            }
        }
    }

    private Texture GetCardTexture(string cardValue, int cardSuit)
    {
        string[] suits = { "hearts", "clubs", "diamonds", "spades" };

        // temp texture path
        string cardImage = (cardValue + "_of_" + suits[cardSuit-1]);
        // get the texture from assets
        return Resources.Load("Textures/AllCards/" + cardImage, typeof(Texture)) as Texture;
    }

    //Waits one second and then, change turn
    private IEnumerator waitForTurnChange()
    {
        yield return new WaitForSeconds(3);
        passTurn();
    }

    

    public void recieveRemoteBlackjackBet(int player, string blackjackBet)
    {
        this.playersBlackjackBet[player].text = "BJ Bet: " + blackjackBet;
        DealCoins(player + 3,int.Parse(blackjackBet));
    }

    

    //Doubles de bet of the player and changes the turn
    public void doubleBet(int player, string card)
    {
        int bet = getPlayerBet(player);
        bet = bet * 2;
        this.playersBets[player].text = "Bet: " + bet.ToString();
        RecieveExtraCard(player, card, false);
        if(this.currentPlayerPosition == player)
        {
            int currentCoins = int.Parse(this.playersCoins[player].text);
            currentCoins = currentCoins - (bet / 2);
            this.playersCoins[player].text = currentCoins.ToString();
        }
        deactivateButtons();
    }

    //This function search a player initial bet from the table
    private int getPlayerBet(int player)
    {
        string stringNumber = "";
        for (int i = 5; i < this.playersBets[player].text.Length; i++)
        {
            stringNumber = stringNumber + this.playersBets[player].text[i];
        }
        int finalNumber = int.Parse(stringNumber);
        return finalNumber;
    }

    //This function returns the value of the secondary bet of one player
    private int getPlayerBlackjackBet(int player)
    {
        string stringNumber = "";
        for (int i = 8; i < this.playersBlackjackBet[player].text.Length; i++)
        {
            stringNumber = stringNumber + this.playersBlackjackBet[player].text[i];
        }
        int finalNumber = int.Parse(stringNumber);
        return finalNumber;
    }
}