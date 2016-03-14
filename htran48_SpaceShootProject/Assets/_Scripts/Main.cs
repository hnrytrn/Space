using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {
	static public Main S;
	static public Dictionary<WeaponType, WeaponDefinition> W_DEFS;

	public GameObject[] prefabEnemies;
	public float enemySpawnPerSecond = 0.3f;
	public float enemySpawnPadding = 1.5f;//padding for position
	public WeaponDefinition[] weaponDefinitions;
	public GameObject prefabPowerUp;
	public WeaponType[] powerUpFrequency = new WeaponType[] {WeaponType.blaster, WeaponType.blaster, WeaponType.spread, WeaponType.shield};

	public bool ___________;

	public WeaponType[] activeWeaponTypes;
	public float enemySpawnRate;//delay between enemy spawns
	public Text scoreGT;

	void Awake () {
		S = this;

		Utils.SetCameraBounds (this.GetComponent<Camera>());

		enemySpawnRate = 1f / enemySpawnPerSecond;
		//invoke after 2 second delay
		Invoke("SpawnEnemy", enemySpawnRate);

		W_DEFS = new Dictionary<WeaponType, WeaponDefinition> ();
		foreach (WeaponDefinition def in weaponDefinitions) {
			W_DEFS [def.type] = def;
		}
	}

	static public WeaponDefinition GetWeaponDefinition (WeaponType wt) {
		//check if key exists in the Dictionary
		if (W_DEFS.ContainsKey (wt)) {
			return (W_DEFS [wt]);
		}
		//otherwise it has failed to find the WeaponDefinition
		return (new WeaponDefinition());
	}

	void Start() {
		//set score
		GameObject scoreGO = GameObject.Find("Score");
		scoreGT = scoreGO.GetComponent<Text> ();
		scoreGT.text = "0";

		activeWeaponTypes = new WeaponType[weaponDefinitions.Length];
		for (int i = 0; i < weaponDefinitions.Length; i++) {
			activeWeaponTypes [i] = weaponDefinitions [i].type;
		}
	}

	public void SpawnEnemy() {
		//Pick a random Enemy preab to instantiate
		int ndx = Random.Range (0, prefabEnemies.Length);
		GameObject go = Instantiate (prefabEnemies [ndx]) as GameObject;

		//Position the enemy above the screen with a random x position
		Vector3 pos = Vector3.zero;
		float xMin = Utils.camBounds.min.x + enemySpawnPadding;
		float xMax = Utils.camBounds.max.x - enemySpawnPadding;
		pos.x = Random.Range (xMin, xMax);
		pos.y = Utils.camBounds.max.y + enemySpawnPadding;
		go.transform.position = pos;
		Invoke ("SpawnEnemy", enemySpawnRate);
	}

	public void DelayedRestart(float delay) {
		//Invoke the Restart() method in delay seconds
		Invoke("Restart",delay);
	}

	public void Restart() {
		SceneManager.LoadScene ("_Scene_0");
	}

	public void ShipDestroyed (Enemy e) {
		//Potentially generate a PowerUp
		if (Random.value <= e.powerUpDropChance) {
			int ndx = Random.Range (0, powerUpFrequency.Length);
			WeaponType puType = powerUpFrequency [ndx];

			GameObject go = Instantiate (prefabPowerUp) as GameObject;
			PowerUp pu = go.GetComponent<PowerUp> ();
			pu.SetType (puType);

			pu.transform.position = e.transform.position;
		}

		//add to score
		int score = int.Parse(scoreGT.text);
		score += 100;
		scoreGT.text = score.ToString();
	}
}
