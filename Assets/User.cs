using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class User
{
    //sets vars for use in class
    public string userName;
    public int userScore;
    public string localId;

    public int userOnline;
    public int userworld;

    //public string userPassword;
    //public int userAttempts;
    //public string lastLogin;

    //construc for this class
    public User()
    {
        //sets vars with those passed from the Player_Scores script
        userName = Player_Scores.playerName;
        userScore = Player_Scores.playerScore;
        localId = Player_Scores.localId;
        userOnline = Player_Scores.playerOnline;
        userworld = Player_Scores.worldNumber;
        //userPassword = Player_Scores.playerPassword;
        //userAttempts = Player_Scores.playerAttempts
    }
}
