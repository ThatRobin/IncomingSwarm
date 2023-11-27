using Newtonsoft.Json;
using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {

    public string username;
	public int seed = 0;
	public GameObject player;
	public GameObject clientPlayer;


	public GameObject hud;
	public GameObject pauseScreen;
	public GameObject deathScreen;
	public GameObject winScreen;

	void Awake() {
		username = PlayerPrefs.GetString("username", "");

		if (username != "") { // if there is a username set
			StartCoroutine(DetermineModelType(seed)); // get the model type for the skin
		} else {
			SetupWorld(seed, true); // set up the world with the default player skin
		}
	}

    private void Update() {
        if (QuestManager.IsQuestComplete()) { // if the quest is completed
			this.winScreen.SetActive(true); // set the win screen to be active
        }
    }

	public void PauseGame() {
		Time.timeScale = 0;
		pauseScreen.SetActive(true);
	}

	public void ResumeGame() {
		Time.timeScale = 1;
		pauseScreen.SetActive(false);
	}


	/// <summary>
	/// Uses numerous minecraft related APIs to gather data about the given username in order
	/// to get the Model type of the player. as some models use a slim model and others use 
	/// a thick model
	/// </summary>
	IEnumerator DetermineModelType(int seed) {
		string playerid = "";

		UnityWebRequest idRequest = UnityWebRequest.Get("https://playerdb.co/api/player/minecraft/" + username);
		yield return idRequest.SendWebRequest();
		if (idRequest.result == UnityWebRequest.Result.ProtocolError) {
			Debug.Log(idRequest.error);
		} else {
			SkinRoot playerData = JsonConvert.DeserializeObject<SkinRoot>(idRequest.downloadHandler.text);
			playerid = playerData.data.player.id;
		}

		bool slim = false;
		UnityWebRequest dataRequest = UnityWebRequest.Get("https://sessionserver.mojang.com/session/minecraft/profile/" + playerid);
		yield return dataRequest.SendWebRequest();
		if (dataRequest.result == UnityWebRequest.Result.ProtocolError) {
			Debug.Log(dataRequest.error);
		} else {
			SlimRoot slimData = JsonConvert.DeserializeObject<SlimRoot>(dataRequest.downloadHandler.text);
				foreach (Property property in slimData.properties) {
				if (property.name.Equals("textures")) {
					String jsonString = Encoding.UTF8.GetString(Convert.FromBase64String(property.value));
					Root root = JsonConvert.DeserializeObject<Root>(jsonString);
					if (root.textures.SKIN.metadata != null) {
						slim = root.textures.SKIN.metadata.model != null;
					}
				}
			}
		}
		SetupWorld(seed, slim);
		
	}

	void SetupWorld(int seed, bool slim) {
		SpawnPlayer(new Vector3(0f, 0.3f, 0f), slim); // spawn the player
	}

	void SpawnPlayer(Vector3 spawnPoint, bool slim) {
		if (player != null) { // if the player prefab exists
			player.GetComponent<PlayerHandler>().SetScreens(hud, deathScreen, winScreen, pauseScreen); // set the players screens
			clientPlayer = Instantiate(player, spawnPoint, Quaternion.identity); // instantiate a new player at the spawnpoint
			clientPlayer.SetActive(true); // set the player to be active
			clientPlayer.name = username != "" ? username : "Player"; // set the GameObjects name if it should have one.
			Camera.main.GetComponent<CameraFollow>().InitializeCamera(clientPlayer); // make the camera follow the player
			clientPlayer.GetComponent<SkinManager>().SetUsername(username); // set the username of the player
			clientPlayer.GetComponent<SkinManager>().UpdateMaterial(); // update the players material to be an instance
		}
		if (slim) { // if the model should be slim
			GameObject.Find("LowerArmL").gameObject.SetActive(false); // disable the thicker arm models
			GameObject.Find("LowerArmR").gameObject.SetActive(false);
			GameObject.Find("UpperArmL").gameObject.SetActive(false);
			GameObject.Find("UpperArmR").gameObject.SetActive(false);
		} else {
			GameObject.Find("LowerArmLSlim").gameObject.SetActive(false); // disable the thinner arm models
			GameObject.Find("LowerArmRSlim").gameObject.SetActive(false);
			GameObject.Find("UpperArmLSlim").gameObject.SetActive(false);
			GameObject.Find("UpperArmRSlim").gameObject.SetActive(false);
		}
	}

	public void RestartGame() {
		Destroy(clientPlayer); // destroy the player
		seed = Random.Range(int.MinValue, int.MaxValue); // create a new random seed
		if (username != "") { // if the username exists
			StartCoroutine(DetermineModelType(seed)); // determine the players model type
		} else {
			SetupWorld(seed, true); // set up the world using the default player skin
		}
	}

	/// <summary>
	/// This section is a set of internal classes used for handling the conversion of the JSON data retrieved from the API
	/// It is just getters and setters that load data according to what each API call returns.
	/// </summary>
	public class Property
	{
		public string name { get; set; }
		public string value { get; set; }
	}

	public class SlimRoot
	{
		public string id { get; set; }
		public string name { get; set; }
		public List<Property> properties { get; set; }
	}

	public class Data
	{
		public Player player { get; set; }
	}

	public class Meta
	{
		public int cached_at { get; set; }
	}

	public class Player
	{
		public Meta meta { get; set; }
		public string username { get; set; }
		public string id { get; set; }
		public string raw_id { get; set; }
		public string avatar { get; set; }
		public List<object> name_history { get; set; }
	}

	public class SkinRoot
	{
		public string code { get; set; }
		public string message { get; set; }
		public Data data { get; set; }
		public bool success { get; set; }
	}

	public class Metadata
	{
		public string model { get; set; }
	}

	public class Root
	{
		public long timestamp { get; set; }
		public string profileId { get; set; }
		public string profileName { get; set; }
		public Textures textures { get; set; }
	}

	public class SKIN
	{
		public string url { get; set; }
		public Metadata metadata { get; set; }
	}

	public class Textures
	{
		public SKIN SKIN { get; set; }
	}

}
