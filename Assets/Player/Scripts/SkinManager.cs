using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Rendering;

public class SkinManager : MonoBehaviour {
	public Material material;
	public Texture2D skinTexture;
	public Sprite sprite;
	public Image image;

	private void Awake() {
		GameObject tempObject = GameObject.Find("PlayerIcon"); // get the player icon object
		if (tempObject != null) { // if the icon is not null
			image = tempObject.GetComponent<Image>(); // get the image from the icon
		}
	}

	public void UpdateMaterial() {
		material = new Material(material); // create a new instance of the current material
		this.GetComponent<PlayerHandler>().SetMaterial(material); // set the material in PlayerHandler to this new instance
	}

	public void SetUsername(string username) {
		material = new Material(material); // create a new instance of the current material
		GameObject tempObject = GameObject.Find("PlayerIcon"); // get the player icon object
		if (tempObject != null) { // if the icon is not null
			image = tempObject.GetComponent<Image>(); // get the image from, the icon
		}
		StartCoroutine(DownloadImage(username)); // get the icon for this username
	}

	public static void RecursiveFindChild(Transform parent, string childTag, Material material) {
        foreach (Transform child in parent) { // for each child of the parent object
            if (child.CompareTag(childTag)) {// if the tag is equal to the tag being checked for
                child.gameObject.GetComponent<MeshRenderer>().material = material; // set the material to the provided material
            } else {
                RecursiveFindChild(child, childTag, material); // run this method using the child object
            }
        }
    }

	/// <summary>
	/// Uses numerous minecraft related APIs to gather data about the given username in order to get the Icon and Skin of the username.
	/// These are then used to set the assets in game for the player.
	/// </summary>
	IEnumerator DownloadImage(string username) {
		string playerid = "";

		UnityWebRequest idRequest = UnityWebRequest.Get("https://playerdb.co/api/player/minecraft/" + username);
		yield return idRequest.SendWebRequest();
		if (idRequest.result == UnityWebRequest.Result.ProtocolError) {
			Debug.Log(idRequest.error);
		} else {
			SkinRoot playerData = JsonConvert.DeserializeObject<SkinRoot>(idRequest.downloadHandler.text);
			playerid = playerData.data.player.id;
		}

		UnityWebRequest textureRequest = UnityWebRequestTexture.GetTexture("https://crafatar.com/skins/" + playerid);
		yield return textureRequest.SendWebRequest();
		if (textureRequest.result == UnityWebRequest.Result.ProtocolError) {
			Debug.Log(textureRequest.error);
		} else {
			skinTexture = ((DownloadHandlerTexture)textureRequest.downloadHandler).texture;
			if (skinTexture.height < 64) {
				Texture2D texture = new Texture2D(64, 64);
				Color[] colours = skinTexture.GetPixels(0, 0, 64, 32);
				texture.SetPixels(0, 32, 64, 32, colours);
				texture.Apply();
				skinTexture = texture;
            }
			skinTexture.filterMode = FilterMode.Point;
			//skinTexture.alphaIsTransparency = true;
			skinTexture.EncodeToPNG();
			material.SetTexture("_BaseMap", skinTexture);
			SkinManager.RecursiveFindChild(transform, "Reskin", material);
			this.GetComponent<PlayerHandler>().SetMaterial(material);
		}

		UnityWebRequest iconRequest = UnityWebRequestTexture.GetTexture("https://mc-heads.net/head/" + playerid);
		yield return iconRequest.SendWebRequest();
		if (iconRequest.result == UnityWebRequest.Result.ProtocolError) {
			Debug.Log(iconRequest.error);
		} else {
			Texture2D iconTexture = ((DownloadHandlerTexture)iconRequest.downloadHandler).texture;
			iconTexture.filterMode = FilterMode.Point;
			//iconTexture.alphaIsTransparency = true;
			iconTexture.EncodeToPNG();
			if (image != null) {
				image.sprite = Sprite.Create(iconTexture, new Rect(0.0f, 0.0f, iconTexture.width, iconTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
			}
		}
	}

	

	/// <summary>
	/// This section is a set of internal classes used for handling the conversion of the JSON data retrieved from the API
	/// It is just getters and setters that load data according to what each API call returns.
	/// </summary>
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


}
