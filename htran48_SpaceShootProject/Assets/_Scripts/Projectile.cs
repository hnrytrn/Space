using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	[SerializeField]
	private WeaponType _type;

	//masks the field _type & takes action when it is set
	public WeaponType type {
		get { 
			return (_type);
		}
		set { 
			SetType (value);
		}
	}

	void Awake () {
		//check to see if this has passed off screen every 2 seconds
		InvokeRepeating("CheckOffscreen", 2f,2f);
	}

	public void SetType(WeaponType eType) {
		//Set the _type
		_type = eType;
		WeaponDefinition def = Main.GetWeaponDefinition (_type);
		GetComponent<Renderer>().material.color = def.projectileColor;
	}

	void CheckOffscreen() {
		if (Utils.ScreenBoundsCheck (GetComponent<Collider>().bounds, BoundsTest.offScreen) != Vector3.zero) {
			Destroy (this.gameObject);
		}
	}
}
