using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleBullet : Bullet
{
	private float speed;
	public float Speed
	{	get { return speed; }	set	{speed = value;}}

	public void SetBullet(float damage, float bulletSpeed, Vector2 bulletSize, Transform shooter)
	{
		SetBullet(damage, bulletSize, shooter);
		Speed = bulletSpeed;
	}
	public override void OnCollisionEnter2D(Collision2D col)
	{
		base.OnCollisionEnter2D(col);
	}
}
