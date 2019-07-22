using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

//eternal APIs
using Proyecto26;
using FullSerializer;
using UnityEditor;
using UnityEngine.SocialPlatforms;

public class Player_Scores : MonoBehaviour
{
    //random func for the player score
    private System.Random random = new System.Random();

    //required to activate the FS API
    public static fsSerializer Serializer = new fsSerializer();

    public Text scoreText; //score text field
    public Text worldNumberText; //World Number text field
    public InputField getScoreText;
    public InputField playerInput; //ref to inputted player name
    public InputField usernameText; 
    public InputField emailText;
    public InputField passwordText;
    public InputField worldIdText;


    public Text OnlineText; //online text field
    //button references
    public Button loginButton;
    public Button registerButton;
    public Button worldSelectButton;
    public Button submitScoreButton;
    public Button rollNewScoreButton;
    public Button worldConfirmButton;
    
    //static vars to be passed through to the User.cs
    public static int playerScore;
    public static string playerName;
    //public static string playerPassword;
    public static int playerOnline;
    public static int worldNumber;
    public int rollNewScore1; //score counter.

    public static string localId;
    private string tokenId;
    private string getLocalId; //gets the local id for accessing the retrieve from database func


    //database URL's
    private string databaseURL = "https://madg-ba4-s2.firebaseio.com/users";
    //private string authURL = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key=";
    //private string loginURL = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=";
    //public string webApiKey = ""; //<<<<<<<<<<<<<<please input webApiKey here----------------------------------------------------------------------------------------------<<<<<<<<<<<<<<<<

    //creates a new user from the User Class
    User user = new User();

    //create a list to hold list of online names
    List<string> onlineUsers = new List<string>();


    private void Start()
    {
        playerScore = random.Next(0, 101);
        //worldNumber = 0;
        scoreText.text = "Score: " + playerScore;
        Debug.Log("Start");
        OnlineText.text = string.Empty;
        //enable/disable buttons at start
        loginButton.interactable = true;
        registerButton.interactable = true;
        worldSelectButton.interactable = false;
        submitScoreButton.interactable = false;
        rollNewScoreButton.interactable = false;
        worldConfirmButton.interactable = false;

    }

    public void OnSubmit()
    {
        playerName = usernameText.text;
        //Online();
        Debug.Log("Try to convert Score to Int and Store in playerScore...");

        //playerScore = user.userScore; //sets playerscore as the user saved score from the database - has issues.
        user.userScore = playerScore;
        Debug.Log(playerScore);

        Debug.Log("Submitting");
        PostToDatabase();
    }

    public void worldSelect()
    {
        //sets playerName 
        playerName = usernameText.text;

        //make sure that the player online value has been set
        //this is wrong and is causing the misclick issue...
        if (playerOnline == 0)
        {
            Online();
            UpdateDatabase();
        }
        else
        {


            //worldNumber = Convert.ToInt32(worldIdText.text);
            if (Int32.TryParse(worldIdText.text, out worldNumber))
            {
                Debug.Log("Convert to World Number Entry success" + worldNumber);
                GetOnlineUsers();
                PostToDatabase();
                //EditorUtility.DisplayDialog("Joining World", $"Connecting {playerName} into World {worldNumber}", "Continue");
                worldConfirmButton.interactable = true;
            }
            else
            {
                Debug.Log("Failed to Convert " + worldIdText.text);
            }

            worldNumberText.text = "World Selected: " + worldNumber;
        }
    }

    public void worldConfirm() //hack solution to the online users issue
    {
        GetOnlineUsers(); //repost the online list with the now set online user
        EditorUtility.DisplayDialog("Joining World", $"Connecting {playerName} into World {worldNumber}", "Continue");
        worldConfirmButton.interactable = false;
    }

    public void GetScore()
    {
        GetLocalId();
        Debug.Log("Retrieving");
        RetrieveFromDatabase();
    }

    public void DeletePlayer()
    {
        playerName = playerInput.text;
        DeleteEntry();
    }

    public void RollScore()
    {
        Debug.Log("Reroll Score");
        playerScore = random.Next(0, 101);
        scoreText.text = "Score: " + playerScore;

        //I would've liked to have this stored on the database and the compared with a last logged in timestamp
        //but sadly I could not full implement this before submission.
        var rollScoreCount2 = 3;//comparison var here
        rollNewScore1++; //increment rollNewScore1

        if (rollNewScore1 == rollScoreCount2)
        {
            rollNewScoreButton.interactable = false;
        }

        Debug.Log(rollNewScore1);
    }

    public void LogOut()
    {
        //playerName = playerInput.text;
        Debug.Log("Logout");
        Offline(); //change to offline

        //post update to database
        PostToDatabase();

        EditorUtility.DisplayDialog("Logout Success", $"User {usernameText.text} was successfully logged out.", "Ok");

        Debug.Log("Clearing Fields..."); //clear form fields
        playerInput.text = string.Empty;
        emailText.text = string.Empty;
        passwordText.text = string.Empty;
        worldIdText.text = string.Empty;
        OnlineText.text = string.Empty;
        worldNumberText.text = string.Empty;

        //buttons interactable
        loginButton.interactable = true;
        registerButton.interactable = true;
        worldSelectButton.interactable = false;
        submitScoreButton.interactable = false;
        rollNewScoreButton.interactable = false;
        rollNewScore1 = 0; //reset counter upon logout
    }

    public void Online()
    {
        Debug.Log("Player Online");
        playerOnline = 1;
    }

    public void Offline()
    {
        Debug.Log("Player Offline");
        playerOnline = 0;
        worldNumber = 0;
    }

    public void PrintOnUsers()
    {
        OnlineText.text = string.Empty;

        foreach (var onlineUser in onlineUsers)
        {
            OnlineText.text += onlineUser + "\n";
            Debug.Log("Printing Users:" + onlineUser);
        }
    }

    //calls to function Register
    public void RegisterUserButton()
        {
            Debug.Log("Register Request");
            //try to register user
            try
            {
                DatabaseService.Instance.Register<LoginResponse>(emailText.text, passwordText.text, response =>
                {
                    localId = response.localId;
                    tokenId = response.tokenId;

                    playerName = usernameText.text; //set username as input
                    PostToDatabase(true); //post with score set to 0

                }); //send to Database Accessor

            }
            catch (Exception e) //catch error
            {
                Debug.Log(e.Message + ": Register Void Error");

                EditorUtility.DisplayDialog("Unsuccessful Registration", "Could not register this user.  Perhaps account is already created.", "Ok");
            }

            EditorUtility.DisplayDialog("Registration Complete", "This Username, Email and Password were successfully registered to the database.  Please remember to login to continue with this User.", "Ok");

        }

    //calls to function Login
    public void LoginUserButton()
    {
        Debug.Log("Login Request");

        //login attempt
        try
        {
            DatabaseService.Instance.Login<LoginResponse>(emailText.text, passwordText.text, response =>
            {            
                //set local ID and token ID 
                localId = response.localId;
                tokenId = response.tokenId;
                //Get Player entered username
                GetUsername();
                //Online();

            }); //send values to the Database Accessor
            
        }catch (Exception e) //catch error
        {
            Debug.Log(e.Message + ": Login Error");
        }
        
        GetLocalId();
        Online(); //set to online
        UpdateDatabase(); //update database with online.
        GetOnlineUsers(); //add to online list
        //RetrieveFromDatabase(); //update the playerscore with value from database

        try
        {
            DatabaseService.databaseRetrival(getLocalId, response =>
            {
                playerName = response.userName;
                playerOnline = response.userOnline;
                playerScore = response.userScore;

                UpdateScore();
            });
        }
        catch (Exception e)
        {
            Debug.Log(e.Message + ": Update Database Error");
        }

        Debug.Log(playerName);
        
        //login UI response
        EditorUtility.DisplayDialog("Successfully Logged In", $"Welcome back {usernameText.text}.  \nPlease connect to a World.", "Ok");
        //buttons interactable
        loginButton.interactable = false;
        registerButton.interactable = false;
        worldSelectButton.interactable = true;
        submitScoreButton.interactable = true;
        rollNewScoreButton.interactable = true;
    }


    private void RetrieveFromDatabase()
    {

        //get from the database the information parsed from the getLocalId Func
        RestClient.Get<User>(databaseURL +"/"+ getLocalId + ".json").Then(response =>
        {

            Debug.Log("User Found");
            //password get in here?

            user = response;
            //calls private func
            UpdateScore();
            //Online();
        });
    }

    private void GetLocalId()
    {
        RestClient.Get(databaseURL + ".json").Then(response =>
        {
            var username = getScoreText.text;

            //takes userData and tries to parse into the user array
            fsData userData = fsJsonParser.Parse(response.Text);
            Debug.Log("Tries to Parse...");

            //simple array doesn't work as need to reference
            //User[] users = null; //an array of users to check through 

            Dictionary<string, User> users =  null; //user dictionary key referencing
            Serializer.TryDeserialize(userData, ref users);

            foreach (var user in users.Values)
            {
                if (user.userName == username)
                {
                    getLocalId = user.localId; //set the found user local id to the local id field
                    RetrieveFromDatabase(); //call the retrieve after finding the user

/*                    try
                    {
                        DatabaseService.databaseRetrival(getLocalId, callback =>
                        {
                            user = callback;
                            
                            UpdateScore();

                        });
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message + " new try/catch retrieval");
                    }*/

                    break;
                }
            }

        }).Catch(error =>
        {
            Debug.Log("Get Local ID Error"); //catch any errors
        });
    }


    //sets the score text
    private void UpdateScore()
    {
        Debug.Log("Update Score Called...");
        playerScore = user.userScore;
        scoreText.text = "Score: " + playerScore;
    }

    private void PostToDatabase(bool emptyScore = false)
    {
        Debug.Log("Post to Database");
        User user = new User();

        if (emptyScore == true)//sets new player score and world number as 0
        {
            user.userScore = 0;
            user.userworld = 0;
        }

        //uses the RestAPI to post to the Database the User class defined param.
        RestClient.Put(databaseURL +"/" + localId + ".json",user); //sets the main folder as the local id

        Debug.Log("Put in database");
    }

    private void DeleteEntry()
    {
        RestClient.Delete(databaseURL + playerInput.text + ".json");
    }

    private void UpdateDatabase()
    {
        //RestClient.Put(databaseURL + playerInput + ".json", playerName);
        RestClient.Put(databaseURL + playerOnline + ".json", playerOnline);
    }

    private void GetUsername()
    {
        //get from the database the information on the playerInput then throw that to the other field
        RestClient.Get<User>(databaseURL + "/" + localId + ".json" + tokenId).Then(response =>
        {
            Debug.Log("Retrieved Username...");
            playerName = response.userName;
            //UpdateScore();

        });
    }

    private void GetOnlineUsers()
    {
        onlineUsers.Clear(); //clear list
        
        RestClient.Get(databaseURL + ".json").Then(response =>
        {
            //var usersOnline;

            //takes userData and tries to parse into the user array
            fsData userData = fsJsonParser.Parse(response.Text);
            Debug.Log("Tries to Parse...");
            
            Dictionary<string, User> users = null; //user dictionary key referencing
            Serializer.TryDeserialize(userData, ref users);
            
            foreach (var usersValue in users.Values.Where(o => o.userOnline == 1))
            {
                RetrieveFromDatabase(); //call the retrieve after finding the user

                //add to list
                onlineUsers.Add(usersValue.userName);

                    //debug list
                    Debug.Log(usersValue.userName);
            }

            PrintOnUsers(); //print users

        }).Catch(error =>
        {
            Debug.Log("Get Local ID Error"); //catch any errors
        });
    }
}