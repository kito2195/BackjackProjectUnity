using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

class Moves : MonoBehaviour
{
    public Button askForACardButton;
    public Button bet21Button;
    public Button doubleBetButton;
    public Button passButton;

    void Start()
    {
        FindButtons();
    }

    private void FindButtons()
    {
        askForACardButton = GameObject.Find("AskForACardButton").GetComponent<Button>();
        askForACardButton.onClick.AddListener(AskForExtraCard);
        askForACardButton.interactable = false;
        bet21Button = GameObject.Find("Bet21Button").GetComponent<Button>();
        bet21Button.onClick.AddListener(BetFor21);
        bet21Button.interactable = false;
        doubleBetButton = GameObject.Find("DoubleBetButton").GetComponent<Button>();
        doubleBetButton.onClick.AddListener(AskFordoubleBet);
        doubleBetButton.interactable = false;
        passButton = GameObject.Find("PassButton").GetComponent<Button>();
        passButton.onClick.AddListener(PassTurn);
        passButton.interactable = false;
    }

    private void activateButtons()
    {
        passButton.GetComponent<Button>().interactable = true;
        askForACardButton.GetComponent<Button>().interactable = true;
        int playerCoinsLeft = int.Parse(PlayersFields.playersCoins[Player.PlayerPosition].text);
        int playerBet = Player.CoinsInGame;

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

    //Asks the server for an extra card  
    void AskForExtraCard()
    {
        this.askForACard.Invoke();
    }

    /*
     * This method lets one player to bet that the house or casino has blackjack in its initial hand
     * This method is only allowed when the face up card of the house is a 10, J, Q, K or an A
     */
    public void BetFor21()
    {
        hideBlackjackBetWindow = false;
    }

    //Asks the server for a new card, double the bet and pass the turn
    public void AskFordoubleBet()
    {
        this.doubleBetEvent.Invoke();
    }

    public void PassTurn()
    {
        deactivateButtons();
        this.passTurnEvent.Invoke();
    }


}

