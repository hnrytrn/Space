using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	public float speed = 10f; //speed in m/s
	public float fireRate = 0.3f; // Seconds/shot (unused)
	public float health = 10;
	public int score = 100; //points earned for destroying this

	public int showDamageForFrames = 2; // # of frames to show damage
	public float powerUpDropChance = 1f;//Chance to drop a power-up

	public bool ____________;

	public Color[] originalColors;
	public Material[] materials;//All the Materials of this & its children
	public int remainingDamageFrames = 0; // damage frames left

	public Bounds bounds; 
	public Vector3 boundsCenterOffset; // dist of bounds.center from position

	void Awake() {
		materials = Utils.GetAllMaterials (gameObject);
		originalColors = new Color[materials.Length];
		for (int i = 0; i < materials.Length; i++) {
			originalColors [i] = materials [i].color;
		}
		InvokeRepeating ("CheckOffscreen", 0f, 2f);
	}

	// Update is called once per frame
	void Update () {
		Move ();
		if (remainingDamageFrames > 0) {
			remainingDamageFrames--;
			if (remainingDamageFrames == 0) {
				UnShowDamage ();
			}
		}
	}

	public virtual void Move() {
		Vector3 tempPos = pos;
		tempPos.y -= speed * Time.deltaTime;
		pos = tempPos;
	}

	//property
	public Vector3 pos {
		get { 
			return (this.transform.position);
		}
		set { 
			this.transform.position = value;
		}
	}

	void CheckOffscreen() {
		//If bounds are still their default value
		if (bounds.size == Vector3.zero) {
			bounds = Utils.CombineBoundsofChildren (this.gameObject);
			boundsCenterOffset = bounds.center - transform.position;
		}

		//update bounds to current position
		bounds.center = transform.position + boundsCenterOffset;

		//Check to see whether the bounds are completely offscreen
		Vector3 off = Utils.ScreenBoundsCheck(bounds,BoundsTest.offScreen);
		if (off != Vector3.zero) {
			//destry enemy object if it has gone off the bottom of the screen
			if (off.y < 0) {
				Destroy (this.gameObject);
			}
		}
	}

	void OnCollisionEnter(Collision coll) {
		GameObject other = coll.gameObject;

		switch (other.tag) {
		case "ProjectileHero":
			Projectile p = other.GetComponent<Projectile> ();
			//prevent players from shooting enemies before they are visible
			bounds.center = transform.position + boundsCenterOffset;
			if (bounds.extents == Vector3.zero || Utils.ScreenBoundsCheck (bounds, BoundsTest.offScreen) != Vector3.zero) {
				Destroy (other);
				break;
			}
			//damage the enemy
			ShowDamage();
			health -= Main.W_DEFS [p.type].damageOnHit;
			if (health <= 0) {
				//Tell the Main singleton that this ship has been destoyed
				Main.S.ShipDestroyed(this);
				Destroy (this.gameObject);
			}
			Destroy (other);
			break;
		}
	}

	void ShowDamage() {
		foreach (Material m in materials) {
			m.color = Color.red;
		}
		remainingDamageFrames = showDamageForFrames;
	}

	void UnShowDamage() {
		for (int i = 0; i < materials.Length; i++) {
			materials [i].color = originalColors [i];
		}
	}
}
