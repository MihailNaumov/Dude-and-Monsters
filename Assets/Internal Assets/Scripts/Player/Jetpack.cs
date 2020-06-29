using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jetpack : MonoBehaviour {

	private Rigidbody2D rb;
	private PlayerController player;
	[Header("Полет")]
	public float _flySpeed;
	private float flySpeed;
	public float FlySpeed { get { return flySpeed; } set { flySpeed = value; } }
	public float _startMoveTime;
	private float startMoveTime;
	public float StartMoveTime { get { return startMoveTime; } set { startMoveTime = value; } }
	public float _stopMoveTime;
	private float stopMoveTime;
	public float StopMoveTime { get { return stopMoveTime; } set { stopMoveTime = value; } }
	[Header("Вращение")]

	[Range(0f, 10f)]
	public float _rotationSpeed;
	private float rotationSpeed;
	public float RotationSpeed { get { return rotationSpeed; } set { rotationSpeed = value; } }
	[Range(0f, 10f)]
	public float _normalizeRotationSpeed;
	private float normalizeRotationSpeed;
	public float NormalizeRotationSpeed { get { return normalizeRotationSpeed; } set { normalizeRotationSpeed = value; } }

	[Header("Падение")]
	[Range(1f, 2f)]
	public float _gravityCof;
	private float gravityCof;
	[Range(0f, 2f)]
	public float _flyFallMultiplier;
	private float flyFallMultiplier;

	[Header("Топливо")]
	private float maxFuel;
	private float fuel;
	public float Fuel { get { return fuel; } set { fuel = value; } }
	public float _fuelReduction;
	private float fuelReduction;
	public float _fuelReplenishment;
	private float fuelReplenishment;


	private bool canFly = false;
	[HideInInspector]
	public bool isFly = false;

	public FuelBar fuelBar;

	void Start()
	{
		player = GetComponent<PlayerController>();
		rb = GetComponent<Rigidbody2D>();
		//test stuff
		flySpeed = _flySpeed;
		startMoveTime = _startMoveTime;
		stopMoveTime = _stopMoveTime;
		rotationSpeed = _rotationSpeed;
		normalizeRotationSpeed = _normalizeRotationSpeed;
		gravityCof = _gravityCof;
		flyFallMultiplier = _flyFallMultiplier;
		fuelReduction = _fuelReduction;
		fuelReplenishment = _fuelReplenishment;
		//
		maxFuel = 100f;
		fuel = maxFuel;
		fuelBar.SetMaxFuel(maxFuel);

		curGrav = player.Gravity;
		curFlyCof = player.FallMultiplier;
	}

	// Update is called once per frame
	void Update() {
		//test stuff
		flySpeed = _flySpeed;
		startMoveTime = _startMoveTime;
		stopMoveTime = _stopMoveTime;
		rotationSpeed = _rotationSpeed;
		normalizeRotationSpeed = _normalizeRotationSpeed;
		gravityCof = _gravityCof;
		flyFallMultiplier = _flyFallMultiplier;
		fuelReduction = _fuelReduction;
		fuelReplenishment = _fuelReplenishment;
		//
	}

	private float curGrav;
	private float curFlyCof;
	private float zRotation;
	public void Fly(float flySpeed)
	{
		if (fuel > 0 && !player.isJump)
		{
			canFly = true;
		}
		else if (fuel <= 0)
		{
			isFly = false;
			canFly = false;
			/*player._fallMultiplier = curFlyCof; // изменить на приватный fallMultiplier*/
			player.Gravity = curGrav;
		}

		if(Input.GetButton("Jetpack") && player.isJump)
		{
			rb.velocity = Vector2.zero;
		}

		if (Input.GetButtonDown("Jetpack") && !player.isJump && !player.CanJump && canFly)
		{
			rb.velocity = Vector2.zero;
			if (player.Gravity == curGrav)
			{
				player.Gravity /= gravityCof;
				/*player._fallMultiplier = flyFallMultiplier; // изменить на приватный fallMultiplier*/
			}
		}
		if (Input.GetButton("Jetpack") && !player.isJump && canFly)
		{
			isFly = true;
			Vector2 fly = Vector2.up * flySpeed;
			/*rb.AddForce(transform.rotation.normalized * fly, ForceMode2D.Impulse);*/
			
		}

		if(Input.GetButtonUp("Jetpack"))
		{
			isFly = false;
		}


	}

	public void FlyRotation(float rotationSpeed, float normalizeRotationSpeed)
	{
		zRotation = Mathf.Clamp(zRotation - player.Movement * rotationSpeed, -10, 10);

		if (isFly && player.isMove)
		{
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0.0f, 0.0f, zRotation)), rotationSpeed * Time.deltaTime);
		}

		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, normalizeRotationSpeed * Time.deltaTime);

		if (player.IsGrounded || fuel <= 0)
		{
			transform.rotation = Quaternion.identity;
		}
	}

	public float SetFuel(float fuel)
	{
		if(isFly)
		{
			if(fuel > 0)
			{
				fuel -= fuelReduction * Time.deltaTime;
				fuelBar.SetFuel(fuel);
			}
			else
			{
				fuel = 0;
			}
		}
		else
		{
			if(fuel < maxFuel && player.IsGrounded)
			{
				fuel += fuelReplenishment * Time.deltaTime;
				fuelBar.SetFuel(fuel);
			}
		
		}
		return fuel;
	}
	
}
