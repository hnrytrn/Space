using UnityEngine;
using System.Collections;

public class Enemy_2 : Enemy {
	//Enemy 2 uses a Sin wave to modify a 2-point linear interpolation
	public Vector3[] points;
	public float birthTime;
	public float lifeTime = 10;
	//determines how much the sine wave will affect movement
	public float sinEccentricity = 0.6f;

	void Start () {
		points = new Vector3[2];

		Vector3 cbMin = Utils.camBounds.min;
		Vector3 cbMax = Utils.camBounds.max;

		Vector3 v = Vector3.zero;

		v.x = cbMin.x - Main.S.enemySpawnPadding;
		v.y = Random.Range (cbMin.y, cbMax.y);
		points [0] = v;

		v = Vector3.zero;
		v.x = cbMax.x + Main.S.enemySpawnPadding;
		v.y = Random.Range (cbMin.y, cbMax.y);
		points [1] = v;

		//possibly swap sides
		if (Random.value < 0.5f) {
			points [0].x *= -1;
			points [1].x *= -1;
		}

		birthTime = Time.time;
	}
	
	public override void Move() {
		float u = (Time.time - birthTime) / lifeTime;

		if (u > 1) {
			Destroy (this.gameObject);
			return;
		}

		//Adjust u by adding an easing curve
		u = u + sinEccentricity * (Mathf.Sin (u * Mathf.PI * 2));
		//Interpolate the two linear interpolation points
		pos = (1 - u) * points [0] + u * points [1];
	}
}
