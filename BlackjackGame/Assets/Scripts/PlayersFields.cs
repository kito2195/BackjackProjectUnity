using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public static class PlayersFields
{
    public static List<Text> playerNames { get; private set; }
    public static List<Text> playersCoins { get; private set; }
    public static List<Text> playersBets { get; private set; }
    public static List<Text> playersHandCount { get; private set; }
    public static List<Text> playersBlackjackBet { get; private set; }
    public static Text turnText { get; private set; }

    private static void InitializeAttr()
    {
        playerNames = new List<Text>();
        playersCoins = new List<Text>(); ;
        playersBets = new List<Text>();
        playersHandCount = new List<Text>();
        playersBlackjackBet = new List<Text>();
        turnText = GameObject.Find("TurnText").GetComponent<Text>();
    }

    public static void FindFields()
    {   
        //first we need to initialize all the attributes in the class
        InitializeAttr();

        //find the players (and all the other texts) in the table
        int i = 0;
        while (i < 3)
        {
            playerNames[i] = GameObject.Find("Player" + (i + 1)).GetComponent<Text>();
            playersCoins[i] = GameObject.Find("Player" + (i + 1) + "Coins").GetComponent<Text>();
            playersBets[i] = GameObject.Find("Player" + (i + 1) + "Bet").GetComponent<Text>();
            playersHandCount[i] = GameObject.Find("Player" + (i + 1) + "Hand").GetComponent<Text>();
            playersBlackjackBet[i] = GameObject.Find("Player" + (i + 1) + "Blackjack").GetComponent<Text>();
        }



    }
}

