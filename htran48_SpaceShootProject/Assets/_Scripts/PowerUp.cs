using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {
	//Vector2 - x holds a min value and y a mac value for a random range
	public Vector2 rotMinMax = new Vector2(15,90);
	public Vector2 driftMinMax = new Vector2(.25f, 2);
	public float lifeTime = 6f; //Seconds the PowerUp Exists
	public float fadeTime = 4f; //Seconds it will then fade
	public bool _______________;
	public WeaponType type; // the type of the PowerUp
	public GameObject cube; //Reference to the Cube child
	public TextMesh letter; //Reference to the Text mesh
	public Vector3 rotPerSecond; //Euler rotation speed
	public float birthTime;

	void Awake() {
		cube = transform.Find ("Cube").gameObject;
		letter = GetComponent<TextMesh> ();

		//set random velocity
		Vector3 vel = Random.onUnitSphere;
		vel.z = 0;
		vel.Normalize ();
		vel *= Random.Range (driftMinMax.x, driftMinMax.y);
		GetComponent<Rigidbody> ().velocity = vel;

		transform.rotation = Quaternion.identity;//no rotation

		rotPerSecond = new Vector3 (Random.Range (rotMinMax.x, rotMinMax.y), Random.Range (rotMinMax.x, rotMinMax.y), Random.Range (rotMinMax.x, rotMinMax.y));
	
		InvokeRepeating ("CheckOffscreen", 2f, 2f);

		birthTime = Time.time;
	}

	void Update () {
		//rotate the cube child
		cube.transform.rotation = Quaternion.Euler(rotPerSecond*Time.time);

		//Fade out the PowerUp over time
		float u = (Time.time - (birthTime+lifeTime));

		if (u >= 1) {
			Destroy(this.gameObject);
			return;
		}
		//Use u to determine the alpha value of the Cube & Letter
		if (u > 0) {
			Color c = cube.GetComponent<Renderer> ().material.color;
			c.a = 1f - u;
			cube.GetComponent<Renderer> ().material.color = c;

			c = letter.color;
			c.a = 1f - (u * 0.5f);
			letter.color = c;
		}
	}

	public void SetType(WeaponType wt) {
		WeaponDefinition def = Main.GetWeaponDefinition (wt);
		cube.GetComponent<Renderer>().material.color = def.color;
		letter.text = def.letter;
		type = wt;
	}

	//when Hero collects a powerUp
	public void AbsorbedBy (GameObject targe) {
		Destroy(this.gameObject);
	}

	//destroy Power Up if it drifts off the screen
	void CheckOffscreen() {
		if (Utils.ScreenBoundsCheck (cube.GetComponent<Collider>().bounds, BoundsTest.offScreen) != Vector3.zero) {
			Destroy (this.gameObject);
		}
	}
}
