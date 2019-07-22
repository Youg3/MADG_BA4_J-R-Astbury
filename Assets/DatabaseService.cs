using System;
using System.Collections.Generic;
using FullSerializer;
using Proyecto26;

public class DatabaseService
{
    //database accessors/keys
    private static readonly string databaseURL = "https://madg-ba4-s2.firebaseio.com/users";
    private string authURL = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key=";
    private string loginURL = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=";
    public string webApiKey = "";//<<<<<<<<<<<<<<please input webApiKey here----------------------------------------------------------------------------------------------<<<<<<<<<<<<<<<<

    private static fsSerializer serializer = new fsSerializer();

    public delegate void getUserCallback(User user);

    public delegate void getUsersCallback(Dictionary<string, User> users);

    //create a new singleton instance
    public static DatabaseService Instance = new DatabaseService();

    //login interaction with database
    public void Login<T>(string email, string password, Action<LoginResponse> callback)
    {
        //LoginResponse loginResponse = null;

        var userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";

        //post to database
        RestClient.Post<LoginResponse>(loginURL + webApiKey, userData).Then(response => callback(response));

        //return loginResponse; //return response
    }

    //register interaction with database
    public void Register<T>(string email, string password, Action<LoginResponse> callback)
    {

        var userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        //post to database
        RestClient.Post<LoginResponse>(authURL + webApiKey, userData).Then(response => callback(response));
    }

//    public LoginResponse updateDatabase()
//    {
//        RestClient.Put(databaseURL + playerInput + "json", OnlineText);
//    }

    public static void databaseRetrival(string localId, getUserCallback callback)
    {
        //LoginResponse loginResponse = null;
        
        //get from the database the information parsed from the getLocalId Func
        RestClient.Get<User>(databaseURL + "/" + localId + ".json").Then(user => callback(user));

        //return loginResponse;
    }

/*    public static void GetUsers(getUsersCallback callback) //gets all users
    {
        RestClient.Get(databaseURL + ".json").Then(response =>
        {
            var responseJson = response.Text;

            var data = fsJsonParser.Parse(responseJson);
            object deserialised = null;
            serializer.TryDeserialize(data, typeof(Dictionary<string, User>), ref deserialised);

            var users = deserialised as Dictionary<string, User>;

            callback(users);
        });
    }*/


}