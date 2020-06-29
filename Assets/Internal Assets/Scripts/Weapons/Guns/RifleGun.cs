using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RifleGun : Gun, IGun
{
	public int poolSize;
	public float damage;
	public float Damage { get { return damage; } set { damage = value; SetBulletsValue(bulParent); } }

	public float bulletSpeed;
	public float BulletSpeed { get { return bulletSpeed; } set { bulletSpeed = value; SetBulletsValue(bulParent); } }

	protected bool fire = false;
	
	public string poolTag;

	public GameObject bulletPrefab;
	public GameObject BulletPrefab { get { return bulletPrefab; } set { bulletPrefab = value;} }


	protected ObjectPooler objectPooler;

	public Vector2 mousePos { get; set; }

	public Vector2 bulletSize;
	public Vector2 BulletSize { get { return bulletSize; } set { bulletSize = value; SetBulletsValue(bulParent); } }


	public Transform aimHand;
	public Transform firePoint;
	public Transform bulParent;
	
	public Transform FirePoint { get { return firePoint; } set { firePoint = value; } }

	protected BoxCollider2D playerCollider;

	public void Fire()
	{
		GameObject bul = objectPooler.SpawnFromPool(poolTag, FirePoint.position, Quaternion.identity);
		Rigidbody2D bulRb = bul.GetComponent<Rigidbody2D>();
		
		if (isPlayer)
		{
			mousePos = playerCam.ScreenToWorldPoint(Input.mousePosition);
		}

		Vector2 shootDir = mousePos - (Vector2)FirePoint.position;
		bulRb.velocity = Vector2.zero;
		bulRb.AddForce(shootDir.normalized * BulletSpeed, ForceMode2D.Impulse);
	}
	protected void SetBulletsValue(Transform bulletParent)
	{
		for (int i = 0; i < bulletParent.childCount; i ++)
		{
			RifleBullet b = bulletParent.GetChild(i).GetComponent<RifleBullet>();
			
			if (isPlayer)
				{
					b.SetBullet(Damage, BulletSpeed, BulletSize, transform.root);
				}
		}
	}

	//test
	void OnValidate()
	{
		SetBulletsValue(bulParent);
	}
	//
}
