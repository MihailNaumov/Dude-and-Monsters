using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FuelBar : MonoBehaviour {

	public Slider slider;
	public Gradient gradient;
	public Image fill;
	// Use this for initialization
	public void SetMaxFuel(float fuel)
	{
		slider.highValue = fuel;
		slider.value = fuel;

		fill.tintColor = gradient.Evaluate(1f);
	}
	
	public void SetFuel(float fuel)
	{
		slider.value = fuel;
		fill.tintColor = gradient.Evaluate(slider.value);
	}
}
