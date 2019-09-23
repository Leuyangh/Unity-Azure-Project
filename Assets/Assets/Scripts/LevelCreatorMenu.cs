using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.UI;

static class Globals
{
    // global int 
    public static string Name;
    public static string Creator;
    public static string BgColor;
    public static string Objects;
    public static string Text;
    public static string WantedLevel;
    public static string WantedLevelName;
}
public class LevelCreatorMenu : MonoBehaviour
{
    [SerializeField]
    private InputField nameInputField;
    [SerializeField]
    private InputField wantedInputField;
    [SerializeField]
    private InputField nameField;
    [SerializeField]
    private Text outputField;
    // Start is called before the first frame update
    void Start()
    {
        Globals.Name = "Default";
        Globals.Creator = "Default";
        Globals.BgColor = "Default";
        Globals.Objects = "";
    }
    public void EnterNameButtonClicked()
    {
        Globals.Name = nameInputField.text;
        TextboxUpdate();
    }
    public void EnterCreatorNameButtonClicked()
    {
        Globals.Creator = nameInputField.text;
        TextboxUpdate();
    }
    public void EnterBackgroundColorButtonClicked()
    {
        Globals.BgColor = nameInputField.text;
        TextboxUpdate();
    }
    public void AddRandomObjectButtonClicked()
    {
        int type = UnityEngine.Random.Range(0, 10);
        float x = UnityEngine.Random.Range(-100.0f, 100.0f);
        float y = UnityEngine.Random.Range(-100.0f, 100.0f);
        float z = UnityEngine.Random.Range(-100.0f, 100.0f);
        float rot = UnityEngine.Random.Range(-360.0f, 360.0f);
        float scale = UnityEngine.Random.Range(0.0f, 3.0f);
        Globals.Objects = Globals.Objects + "[{" + type.ToString() + "};{" + x.ToString() + "," + y.ToString() + "," + z.ToString() + "};{" + rot.ToString() + "};{" + scale.ToString() + "}]";
        TextboxUpdate();
    }
    public void ExitButtonClicked()
    {
        Application.Quit();
    }
    public async void SaveButtonClicked()
    {
        try
        {
            var table = AzureMobileServiceClient.Client.GetTable<LevelInfo>();
            var allEntries = await TestToListAsync(table);
            var initialCount = allEntries.Count();
            Debug.Log("Got count");
            await table.InsertAsync(new LevelInfo { Name = Globals.Name, Creator = Globals.Creator, BgColor = Globals.BgColor, Objects = Globals.Objects });
            Debug.Log("Inserted: " + Globals.Name + " " + Globals.Creator + " " + Globals.BgColor + " " + Globals.Objects);
            allEntries = await TestToListAsync(table);
            var newCount = allEntries.Count();

            Debug.Assert(newCount == initialCount + 1, "InsertAsync failed!");
        }
        catch(Exception){
            throw;
        }
    }
    public async void findIDs()
    {
        Globals.WantedLevelName = nameField.text;
        Debug.Log("Getting Ids for " + Globals.WantedLevelName);
        nameField.text = "";
        try
        {
            var table = AzureMobileServiceClient.Client.GetTable<LevelInfo>();
            var allEntries = await TestToListAsync(table);
            Debug.Log("Got table");
            int found = 0;
            foreach (LevelInfo lvl in allEntries)
            {
                if (lvl.Name.Equals(Globals.WantedLevelName))
                {
                    nameField.text = nameField.text + (found + 1).ToString() + ".'" + Globals.WantedLevelName + "' By: " + lvl.Creator + ". ID = " + lvl.Id + '\n';
                    found++;
                }
            }
            if (found == 0)
            {
                nameField.text = "No levels of that name were found";
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async void DownloadButtonClicked()
    {
        Globals.WantedLevel = wantedInputField.text;
        Debug.Log("Searching for level: " + Globals.WantedLevel);
        try
        {
            var table = AzureMobileServiceClient.Client.GetTable<LevelInfo>();
            var allEntries = await TestToListAsync(table);
            Debug.Log("Got table. Searching...");
            foreach (LevelInfo lvl in allEntries)
            {
                if (lvl.Id.Equals(Globals.WantedLevel))
                {
                    Debug.Log("Found it");
                    Globals.Name = lvl.Name;
                    Globals.Creator = lvl.Creator;
                    Globals.BgColor = lvl.BgColor;
                    Globals.Objects = lvl.Objects;
                    TextboxUpdate();
                    return;
                }
            }
            wantedInputField.text = "Level ID not found";
            Debug.Log("Level ID was invalid");
            return;
        }
        catch(Exception)
        {
            throw;
        }
    }
    private async Task<List<LevelInfo>> TestToListAsync(IMobileServiceTable<LevelInfo> table)
    {
        try
        {
            var allEntries = await table.ToListAsync();
            Debug.Assert(allEntries != null, "ToListAsync failed!");
            return allEntries;
        }
        catch (Exception)
        {

            throw;
        }
    }
    public void TextboxUpdate(){
        outputField.text = "Current Level String: " + Globals.Name + " " + Globals.Creator + " " + Globals.BgColor + " " + Globals.Objects;
    }
}
