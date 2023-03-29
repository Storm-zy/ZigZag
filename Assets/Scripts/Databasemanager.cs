using Firebase;
using Firebase.Database;
using Firebase.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;


public class Databasemanager : MonoBehaviour
{
    public InputField Name;
    public TMP_Text highscore;
    private DatabaseReference DBreference;
    private string userId;
    public DependencyStatus dependencyStatus;
    public Button SaveBtn;
    public Button GetBtn;
    public Button userNameBtn;
    public Button clearScores;
    public GameObject cleartable;
    public GameObject getbtn;
    public Button homeBtn;

    public List<User> userList = new List<User>();
    private float tempHeight = -10f;

    public TMP_InputField playerUsernameInput;
    string userName = " ";
    public Transform entryTemplate; // The prefab for the user entry UI element
    public Transform entrycontainer; // The container for the user entry UI elements
    private int hs;
     

    void Awake(){
         // Initialize Firebase
       FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Enable offline persistence
                FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(true);
                DBreference = FirebaseDatabase.DefaultInstance.RootReference;

                // Debug.Log(DBreference);
                Debug.Log("Firebase Initialized");
            }
            else
            {
                Debug.LogError(System.String.Format(
                "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }


    private void Start()
    {   
        cleartable.SetActive(false);
        
        SaveBtn.onClick.AddListener(async () => await CheckUserExists());
        GetBtn.onClick.AddListener(RetrieveAllUsers);
        userNameBtn.onClick.AddListener(onUsersaveBtnClicked);
        clearScores.onClick.AddListener(clearList);
        homeBtn.onClick.AddListener(goHome);


        userId = SystemInfo.deviceUniqueIdentifier;
        highscore.text = "High Score: " + PlayerPrefs.GetInt("highscore").ToString();
        hs = PlayerPrefs.GetInt("highscore");
        userName = PlayerPrefs.GetString("name"); 

        Debug.Log(PlayerPrefs.GetString("name"));

        if(PlayerPrefs.HasKey("name")){
            string playerName = PlayerPrefs.GetString("name");
            playerUsernameInput.text = playerName;
        }
    }


    public void goHome(){
       SceneManager.LoadScene(0);   
    }



 public void onUsersaveBtnClicked()
    {
        string name = playerUsernameInput.text;

        // Check if the player has entered a name
        if (string.IsNullOrEmpty(name))
        {
            highscore.text = "Please enter a name ";
            Debug.Log("Please enter a name");
            return;
        }

        // Save the player's name to PlayerPrefs
        PlayerPrefs.SetString("name", name);
        PlayerPrefs.Save();

        // Notify the player that their name has been saved
        Debug.Log($"Your name has been saved as {name}!");
        highscore.text = "Your name has been saved ";

        StartCoroutine(DelayedTextUpdate());




    }

    IEnumerator DelayedTextUpdate()
    {
        yield return new WaitForSeconds(2f);
         highscore.text = "High Score: " + PlayerPrefs.GetInt("highscore").ToString();
    }



  private async Task CheckUserExists()
    {
        userName = playerUsernameInput.text;
        
        // Check if user exists with the given user name
        await DBreference.Child("users").OrderByChild("name").EqualTo(userName).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to retrieve data: " + task.Exception);
                return;
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    Debug.Log("User already exists.");
                // playerUsernameInput.text = "player Name Exists choose another";
                }
                else
                {
                    Debug.Log("User does not exist.");
                    CreateUser();
                }
            }
        });
    }

  
    private void CreateUser()
    {
      // Get the saved name and high score from PlayerPrefs
       
        // If the name is not yet saved in PlayerPrefs, save it now
        // if (!PlayerPrefs.HasKey("name"))
        // {
        //     PlayerPrefs.SetString("name", name);
        // }
         
        string name = userName ;
        int highScore = hs;

        // Create a new user in the database
        var user = new User(name, highScore);
        string json = JsonUtility.ToJson(user);
        DBreference.Child("users").Child(userId).SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Data saved successfully");
                highscore.text = "Your information has been saved ";
                
               // PlayerPrefs.SetString("name", userName);
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Failed to save data: " + task.Exception);
            }
        });
    }

 
private void RetrieveAllUsers()
{
    getbtn.SetActive(false);
    cleartable.SetActive(true);
    StartCoroutine(LoadUserData());
}


private void clearList(){
    cleartable.SetActive(false);
    getbtn.SetActive(true);
     // Get the container transform and its child count
    Transform container = entrycontainer.transform;
    int childCount = container.childCount;

    // If there are less than 2 children, do nothing
    if (childCount < 2)
    {
        return;
    }

    // Loop through all but the first child and destroy them
    for (int i = childCount - 1; i > 0; i--)
    {
        Destroy(container.GetChild(i).gameObject);
    }
}

private IEnumerator LoadUserData()
{
    //Get the User Data
    var DBTask = DBreference.Child("users").GetValueAsync();

    yield return new WaitUntil(() => DBTask.IsCompleted);

    if (DBTask.Exception != null)
    {
        Debug.LogWarning($"Failed to register task with {DBTask.Exception}");
    }
    else if (DBTask.Result.Value == null)
    {
        Debug.Log("No data");
    }
    else
    {
        //Data has been retrieved
        DataSnapshot snapshot = DBTask.Result;
        //Debug.Log(snapshot.ChildrenCount);//
        string items = snapshot.GetRawJsonValue();
        //Debug.Log(snapshot.GetRawJsonValue().GetType());

          //Sort the children by "highscore" value in descending order
        var sortedChildren = snapshot.Children.OrderByDescending(c => int.Parse(c.Child("highscore").Value.ToString()));


        int index = 0;
        foreach (var child in sortedChildren)
        {
            Transform entryTransform = Instantiate(entryTemplate, entrycontainer);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, -tempHeight * index);
            entryTransform.gameObject.SetActive(true);

            entryTransform.Find("Name").GetComponent<Text>().text = child.Child("name").Value.ToString();
            entryTransform.Find("HighScore").GetComponent<Text>().text = child.Child("highscore").Value.ToString();
              
            index++;
        }
   
    }
}


//function to print list 
// public void DebugUserList(List<User> userList)
//     {
//         if (userList == null)
//         {
//             Debug.LogError("User list is null");
//             return;
//         }

//         if (userList.Count == 0)
//         {
//             Debug.LogWarning("User list is empty");
//             return;
//         }

//         foreach (User user in userList)
//         {
//             Debug.Log("User: " + user.name + ", Highscore: " + user.highscore);
//         }
//     }

    public void SaveData() 
    {
        User newUser = new User("hello", 123);
        string json = JsonUtility.ToJson(newUser);
        DBreference.Child("users").Child(userId).SetRawJsonValueAsync(json);
    }


}

public class User
{
    public string name;
    public int highscore;

    public User(string name, int highscore)
    {
        this.name = name;
        this.highscore = highscore;
    }
}
