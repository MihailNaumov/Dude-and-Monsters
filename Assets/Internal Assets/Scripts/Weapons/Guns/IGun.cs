using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGun {
	Vector2 mousePos { get; set; }
	Transform FirePoint { get; set; }
	GameObject BulletPrefab { get; set; }
	float Damage { get; set; }

	Vector2 BulletSize { get; set; }

	
}
