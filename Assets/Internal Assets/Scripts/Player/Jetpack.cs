using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jetpack : MonoBehaviour {

	private Rigidbody2D rb;
	[Header("Полет")]
	[SerializeField] private float flySpeed;
	public float FlySpeed { get { return flySpeed; } set { flySpeed = value; } }
	[SerializeField]  private float startMoveTime;
	public float StartMoveTime { get { return startMoveTime; } set { startMoveTime = value; } }
	[SerializeField] private float stopMoveTime;
	public float StopMoveTime { get { return stopMoveTime; } set { stopMoveTime = value; } }
	[Header("Вращение")]

	[Range(0f, 10f)]
	[SerializeField] private float rotationSpeed;
	public float RotationSpeed { get { return rotationSpeed; } set { rotationSpeed = value; } }
	[Range(0f, 10f)]
	[SerializeField]  private float normalizeRotationSpeed;
	public float NormalizeRotationSpeed { get { return normalizeRotationSpeed; } set { normalizeRotationSpeed = value; } }

	[Header("Падение")]
	[Range(1f, 2f)]
	[SerializeField] private float gravityCof;

	[Range(0f, 2f)]
	[SerializeField] private float flyFallMultiplier;

	[Header("Топливо")]
	private float maxFuel;
	private float fuel;
	public float Fuel { get { return fuel; } set { fuel = value; } }
	[SerializeField] private float fuelReduction;
	[SerializeField] private float fuelReplenishment;


	private bool canFly = false;
	[HideInInspector]
	public bool isFly = false;

	

	[Header("Scripts")]
	public FuelBar fuelBar;
	private PlayerController playerController;
	private PlayerStates playerStates;


	void Start()
	{
		playerController = GetComponent<PlayerController>();
		playerStates = PlayerStates.Instance;
		rb = GetComponent<Rigidbody2D>();

		maxFuel = 100f;
		fuel = maxFuel;
		fuelBar.SetMaxFuel(maxFuel);

		curGrav = playerController.Gravity;
		curFlyCof = playerController.FallMultiplier;
	}

	private float curGrav;
	private float curFlyCof;
	private float zRotation;
	public void Fly(float flySpeed)
	{
		var curMove = playerStates.curState;

		if (fuel > 0 )
		{
			canFly = true;
		}
		else if (fuel <= 0)
		{
			isFly = false;
			canFly = false;
			/*player._fallMultiplier = curFlyCof; // изменить на приватный fallMultiplier*/
			playerController.Gravity = curGrav;
		}

		if(Input.GetButton("Jetpack"))
		{
			rb.velocity = Vector2.zero;
		}

		if (Input.GetButtonDown("Jetpack") /*&& !playerController.isJump && !playerController.CanJump*/ && canFly)
		{
			rb.velocity = Vector2.zero;
			if (playerController.Gravity == curGrav)
			{
				playerController.Gravity /= gravityCof;
				/*player._fallMultiplier = flyFallMultiplier; // изменить на приватный fallMultiplier*/
			}
		}
		if (Input.GetButton("Jetpack") /*&& !playerController.isJump*/ && canFly)
		{
			isFly = true;
			Vector2 fly = Vector2.up * flySpeed;
			rb.AddForce(transform.rotation.normalized * fly, ForceMode2D.Impulse);
			
		}

		if(Input.GetButtonUp("Jetpack"))
		{
			isFly = false;
		}


	}

	public void FlyRotation(float rotationSpeed, float normalizeRotationSpeed)
	{
		/*zRotation = Mathf.Clamp(zRotation - player.Movement * rotationSpeed, -10, 10);

		if (isFly && player.isMove)
		{
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0.0f, 0.0f, zRotation)), rotationSpeed * Time.deltaTime);
		}

		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, normalizeRotationSpeed * Time.deltaTime);

		if (player.IsGrounded || fuel <= 0)
		{
			transform.rotation = Quaternion.identity;
		}*/
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
			if(fuel < maxFuel && playerController.IsGrounded)
			{
				fuel += fuelReplenishment * Time.deltaTime;
				fuelBar.SetFuel(fuel);
			}
		
		}
		return fuel;
	}
	
}
