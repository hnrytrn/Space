using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {
	static public Hero S;//Singleton

	public float gameRestartDelay = 2f;

	//Fields to control the movement of the ship
	public float speed = 30;
	public float rollMult = -45;
	public float pitchMult = 30;

	//Ship status info
	[SerializeField]
	private float _shieldLevel = 1;
	//Weapon fields
	public Weapon[] weapons = new Weapon[2];
	public bool ________________;
	public Bounds bounds;

	public delegate void WeaponFireDelegate ();
	public WeaponFireDelegate fireDelegate;

	void Awake() {
		S = this;//set the Singleton	
		bounds = Utils.CombineBoundsofChildren(this.gameObject);
	}

	void Start() {
		//Reset the weapons to start _Hero with 1 blaster
		ClearWeapons();
		weapons[0].SetType(WeaponType.blaster);
	}
			
	// Update is called once per frame
	void Update () {
		//Pull in information from the Input class
		float xAxis = Input.GetAxis("Horizontal");
		float yAxis = Input.GetAxis ("Vertical");

		//Change transform.position based on the axes
		Vector3 pos = transform.position;
		pos.x += xAxis * speed * Time.deltaTime;
		pos.y += yAxis * speed * Time.deltaTime;
		transform.position = pos;

		bounds.center = transform.position;

		//keep the ship constrained to the screen bounds
		Vector3 off = Utils.ScreenBoundsCheck(bounds,BoundsTest.onScreen);
		if (off != Vector3.zero) {
			pos -= off;
			transform.position = pos;
		}

		//Rotate the ship to make it feel more dynamic
		transform.rotation = Quaternion.Euler(yAxis*pitchMult, xAxis*rollMult,0);

		//use fireDelegate to fire Weaponds
		if (Input.GetAxis("Jump") == 1 && fireDelegate != null) {
			fireDelegate ();
		}
	}

	//reference to the last triggering GameObeject
	public GameObject lastTriggerGo = null;

	void OnTriggerEnter(Collider other) {
		//Find the tag
		GameObject go = Utils.FindTaggedParent (other.gameObject);

		if (go != null) {
			//check if its the same triggering go as lasttime
			if (go == lastTriggerGo) {
				return;
			}
			lastTriggerGo = go;

			if (go.tag == "Enemy") {
				//If the shield was triggered by an enemy decrease the level of the sield by 1
				shieldLevel--;
				//destry the enemy
				Destroy (go);
			} else if (go.tag == "PowerUp") {
				AbsorbPowerUp (go);
			} else {
				print ("Triggered: " + go.name);
			}
		} else {
			print ("Triggered: " + other.gameObject.name);
		}
	}

	public void AbsorbPowerUp(GameObject go) {
		PowerUp pu = go.GetComponent<PowerUp>();
		switch (pu.type) {
		case WeaponType.shield: 
			shieldLevel++;
			break;
		
		default://any other weapon PowerUp
			//Check current Weapon Type
			if (pu.type == weapons [0].type) {
				//increase the number of weapons of this type
				Weapon w = GetEmptyWeaponSlot ();//find available weapon
				if (w != null) {
					w.SetType (pu.type);
				}
			} else {//change weapon type if its different
				ClearWeapons ();
				weapons [0].SetType (pu.type);
			}
			break;
		}
		pu.AbsorbedBy (this.gameObject);
	}

	Weapon GetEmptyWeaponSlot() {
		for (int i = 0; i < weapons.Length; i++) {
			if (weapons [i].type == WeaponType.none) {
				return (weapons [i]);
			}
		}
		return (null);
	}

	void ClearWeapons() {
		foreach (Weapon w in weapons) {
			w.SetType (WeaponType.none);
		}
	}
	public float shieldLevel {
		get { 
			return (_shieldLevel);
		}
		set { 
			_shieldLevel = Mathf.Min (value, 4);
			if (value < 0) {
				Destroy(this.gameObject);
				//restart the game after a delay
				Main.S.DelayedRestart(gameRestartDelay);
			}
		}
	}
}
