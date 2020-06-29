using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : RifleGun, IGun {


	void Start()
	{
		isPlayer = IsPlayerGun();
		objectPooler = ObjectPooler.Instance;
		SetBulletsValue(bulParent);

		if (isPlayer)
		{
			playerCollider = transform.root.GetComponent<BoxCollider2D>();
		}

		Camera[] cam = FindObjectsOfType<Camera>();
		foreach (Camera c in cam)
		{
			if (c.gameObject.name == "PlayerCamera")
			{
				playerCam = c;
				break;
			}
		}
	}

	void FixedUpdate()
	{
		if(fire && isPlayer)
		{
			fire = false;
			Fire();
		}
		
	}
	void Update()
	{
		if (isPlayer)
		{
			Aiming(playerCam, mousePos, aimHand, transform.root);
			if (Input.GetButtonDown("Fire1"))
			{
				fire = true;
			}
		}
	}
	

}
