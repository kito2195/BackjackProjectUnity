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
    public List<Text> playerNames;
    public List<Text> playersCoins;
    public List<Text> playersBets;
    public List<Text> playersHandCount;
    public List<Text> playersBlackjackBet;
    public Text turnText;
    public float moveSpeed;
    public Transform[] fieldsPosition;
    public GameObject cardPrefab;
    public GameObject askForACardButton;
    public GameObject bet21Button;
    public GameObject doubleBetButton;
    public GameObject passButton;
    public List<Transform> coinsPositions;
    public GameObject coinPrefab_100;
    public GameObject coinPrefab_500;
    public GameObject coinPrefab_1000;
    public InitialBetReady betReady;
    public UnityEvent passTurnEvent;
    public UnityEvent askForACard;
    public UnityEvent doubleBetEvent;
    public BlackjckBetReady blackjackBetReady;

    //private fields
    private string playerInitialBet;
    private int initialBetValue;
    private string playerBlackjackBet;
    private int blackjackBet;
    private int currentCard;
    private int totalPlayers = 4;
    private List<GameObject> cardsToDeal;
    private Rect initialBetWindow = new Rect(100, 100, 400, 200);
    private Rect blackjackBetWindow = new Rect(100, 100, 400, 200);
    private Rect anotherRoundWindow = new Rect(100, 100, 400, 200);
    private bool hideInitialBetWindow;
    private bool hideBlackjackBetWindow;
    private bool hideAnotherRoundWindow;
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
        playerInitialBet = "";
        playerBlackjackBet = "";
        cardsToDeal = new List<GameObject>();
        currentCard = 0;
        hideInitialBetWindow = true;
        hideBlackjackBetWindow = true;
        hideAnotherRoundWindow = true;
        askForACardButton.GetComponent<Button>().onClick.AddListener(AskForExtraCard);
        askForACardButton.GetComponent<Button>().interactable = false;
        bet21Button.GetComponent<Button>().onClick.AddListener(betFor21);
        bet21Button.GetComponent<Button>().interactable = false;
        doubleBetButton.GetComponent<Button>().onClick.AddListener(askFordoubleBet);
        doubleBetButton.GetComponent<Button>().interactable = false;
        passButton.GetComponent<Button>().onClick.AddListener(passTurn);
        passButton.GetComponent<Button>().interactable = false;
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

    public void putAPlayerInTheTable(int position, string remotePlayer)
    {
        //Puts the local player in the table
        if (remotePlayer == null)
        {
            this.currentPlayerPosition = position;
            this.playerNames[position].text = Player.Nickname;
            this.playersCoins[position].text = Player.GameCoins.ToString();
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

    public void activateInitialBet()
    {
        hideInitialBetWindow = false;
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

    private void activateButtons()
    {
        passButton.GetComponent<Button>().interactable = true;
        askForACardButton.GetComponent<Button>().interactable = true;
        int playerCoinsLeft = int.Parse(this.playersCoins[this.currentPlayerPosition].text);
        int playerBet = getPlayerBet(this.currentPlayerPosition);

        if (playerCoinsLeft >= playerBet)
        {
            doubleBetButton.GetComponent<Button>().interactable = true;
        }
        
        if (cardsDealed[1].Value == 11 || cardsDealed[1].Value == 10)
        {
            if (playerCoinsLeft > 0)
            {
                bet21Button.GetComponent<Button>().interactable = true;
            }
        }
    }

    private void deactivateButtons()
    {
        askForACardButton.GetComponent<Button>().interactable = false;
        doubleBetButton.GetComponent<Button>().interactable = false;
        passButton.GetComponent<Button>().interactable = false;
        bet21Button.GetComponent<Button>().interactable = false;
    }

    //to manage the pop up initial bet
    void OnGUI()
    {
        if (!hideInitialBetWindow)
        {
            initialBetWindow = GUI.Window(0, initialBetWindow, doInitialBetWindow, "Initial Bet");
        }
        if (!hideBlackjackBetWindow)
        {
            blackjackBetWindow = GUI.Window(1, blackjackBetWindow, doBlackjackBetWindow, "Blackjack Bet");
        }
        if (!hideAnotherRoundWindow)
        {
            anotherRoundWindow = GUI.Window(1, anotherRoundWindow, doAnotherRoundWindow, "Another round?");
        }

    }
    /*
     * This window asks for the player initial bet
     */

    void doInitialBetWindow(int windowID)
    {
        GUI.Label(new Rect(25, 20, 390, 60), "Please, insert your initial bet in the next field and press Start Button! to continue.");
        GUI.Label(new Rect(25, 65, 100, 30), "Initial Bet: ");
        playerInitialBet = GUI.TextField(new Rect(130, 65, 200, 25), playerInitialBet, 25);

        if (GUI.Button(new Rect(290, 170, 100, 20), "Start Button!"))
        {
            //validate if the input is a number
            if (int.TryParse(playerInitialBet, out initialBetValue))
            {
                int playerCoins = int.Parse(playersCoins[this.currentPlayerPosition].text);

                if (initialBetValue <= playerCoins)
                {

                    playersBets[this.currentPlayerPosition].text = "Bet: " + playerInitialBet;
                    playersCoins[this.currentPlayerPosition].text = (playerCoins - initialBetValue).ToString();
                    hideInitialBetWindow = true;
                    DealCoins(this.currentPlayerPosition, initialBetValue);
                    this.betReady.Invoke(initialBetValue);
                }
            }
        }
    }
    //Window that asks for the secondary bet (bet the house has blackjack)
    void doBlackjackBetWindow(int windowID)
    {
        GUI.Label(new Rect(25, 20, 390, 60), "Please, insert your secondary bet in the next field and press Start Button! to continue.");
        GUI.Label(new Rect(25, 65, 100, 30), "Secondary Bet: ");
        playerBlackjackBet = GUI.TextField(new Rect(130, 65, 200, 25), playerBlackjackBet, 25);
        if (GUI.Button(new Rect(290, 170, 100, 20), "Start Button!"))
        {
            //validate if the input is a number
            if (int.TryParse(playerBlackjackBet, out blackjackBet))
            {
                int playerCoins = int.Parse(playersCoins[this.currentPlayerPosition].text);

                if (blackjackBet <= (initialBetValue/2) && blackjackBet <= int.Parse(this.playersCoins[this.currentPlayerPosition].text))
                {
                    playersBlackjackBet[this.currentPlayerPosition].text = "BJ Bet: " + blackjackBet;
                    playersCoins[this.currentPlayerPosition].text = (playerCoins - blackjackBet).ToString();
                    hideBlackjackBetWindow = true;
                    DealCoins(this.currentPlayerPosition + 3, blackjackBet);
                    this.blackjackBetReady.Invoke(blackjackBet);
                    deactivateButtons();
                    activateButtons();
                }
            }
        }
    }

    void doAnotherRoundWindow(int windowID)
    {
        new Rect(25, 65, 100, 30);
        if (GUI.Button(new Rect(50, 65, 80, 30), "Yes"))
        {
            this.hideAnotherRoundWindow = true;
        }
        if (GUI.Button(new Rect(300, 65, 80, 30), "No"))
        {
            this.hideAnotherRoundWindow = true;
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


    //Asks the server for an extra card  
    void AskForExtraCard()
    {
        this.askForACard.Invoke();
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

    /*
     * This method lets one player to bet that the house or casino has blackjack in its initial hand
     * This method is only allowed when the face up card of the house is a 10, J, Q, K or an A
     */
    public void betFor21()
    {
        hideBlackjackBetWindow = false;
    }

    public void recieveRemoteBlackjackBet(int player, string blackjackBet)
    {
        this.playersBlackjackBet[player].text = "BJ Bet: " + blackjackBet;
        DealCoins(player + 3,int.Parse(blackjackBet));
    }

    //Asks the server for a new card, double the bet and pass the turn
    public void askFordoubleBet()
    {
        this.doubleBetEvent.Invoke();
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

    public void passTurn()
    {
        deactivateButtons();
        this.passTurnEvent.Invoke();
    }

	public void DealCoins(int positionOfCoins, int betPlayer){
        List<int> coinsOfPlayer = new List<int>();
		while (betPlayer > 0) {
            if (betPlayer >= 2000)
            {
                coinsOfPlayer.Add(1000);
                betPlayer -= 1000;
            }
            else if (betPlayer > 500)
            {
                coinsOfPlayer.Add(500);
                betPlayer -= 500;
            }
            else if(betPlayer >= 100)
            {
                coinsOfPlayer.Add(100);
                betPlayer -= 100;
            }
            else
            {
                betPlayer = 0;
            }
		}
		MakeCoins(positionOfCoins,coinsOfPlayer);
	}

	public void MakeCoins(int positionOfCoins, List<int> coinsOfPlayer){
		int i = 0;
		float inc_100 = 0.04f;
		float inc_500 = 0.04f;
		float inc_1000 = 0.04f;

		while (i < coinsOfPlayer.Count) {
            if (coinsOfPlayer [i] == 100) {
                Vector3 vectTemp_100 = new Vector3(this.coinsPositions[positionOfCoins].position.x, this.coinsPositions[positionOfCoins].position.y + inc_100, this.coinsPositions[positionOfCoins].position.z);
                Instantiate (coinPrefab_100,vectTemp_100, this.coinsPositions[positionOfCoins].rotation);
				inc_100 += 0.04f;
			} 
			else if (coinsOfPlayer [i] == 500) {
                Vector3 vectTemp_500 = new Vector3(this.coinsPositions[positionOfCoins].position.x - 0.40f, this.coinsPositions[positionOfCoins].position.y + inc_500, this.coinsPositions[positionOfCoins].position.z);
                GameObject coin = Instantiate (coinPrefab_500, vectTemp_500, this.coinsPositions[positionOfCoins].rotation);
                coin.transform.Rotate(new Vector3(180,0,0));
				inc_500 += 0.04f;
			}
			else if (coinsOfPlayer [i] == 1000) {
                Vector3 vectTemp_1000 = new Vector3(this.coinsPositions[positionOfCoins].position.x + 0.40f, this.coinsPositions[positionOfCoins].position.y + inc_1000, this.coinsPositions[positionOfCoins].position.z);
                Instantiate (coinPrefab_1000, vectTemp_1000, this.coinsPositions[positionOfCoins].rotation);
				inc_1000 += 0.04f;
			}
			i++;
		}
	}
}
