using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Gun : MonoBehaviour
{
	protected Camera playerCam;
	protected bool isPlayer = false;
	void GetDamage(float damage)
	{
	}
	
	void Reload()
	{
	}

	public virtual void Aiming(Camera cam, Vector2 shootPos, Transform aimingsHand, Transform shooter)
	{
		if (isPlayer)
		{
			shootPos = cam.ScreenToWorldPoint(Input.mousePosition);
		}
			Vector2 lookDir = (shootPos - (Vector2)shooter.position).normalized;
		float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

		if(angle > 90 ||angle < -90)
		{
			transform.localScale = new Vector3(-1, -1, 1);
		}
		else
		{
			transform.localScale = new Vector3(-1, 1, 1);
		}
			aimingsHand.eulerAngles = new Vector3(0, 0, angle);
		
		
	}
	protected void PrepareBullets(GameObject bulletPrefab, string tag, int size, Transform parent)
	{
		ObjectPooler.Instance.AddToPool(bulletPrefab, tag, size, parent.gameObject);
	}
	protected bool IsPlayerGun()
	{
		if (transform.root.gameObject.GetComponent<PlayerController>())
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}