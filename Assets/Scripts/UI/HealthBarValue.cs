using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

//TODO Merge slider value scripts using custom inspector

[RequireComponent(typeof(Slider))]
public class HealthBarValue : MonoBehaviour {
	[SerializeField]
	private CharacterData healthData;
	[SerializeField, Tooltip("Used to display current health value through UI.")]
	private Text healthText;

	[SerializeField]
	private string formatPrefix;
	[SerializeField]
	private string formatSuffix;
	[SerializeField]
	private float valueMultiplier = 1;
	[SerializeField]
	private float valueModifier = 0;

	private Slider healthSlider;

	private void Awake() {
		if (this.healthData == null) {
			Debug.LogWarning("HealthBarValue: A HealthTracker component was not assigned for health tracking.", this);
		}

		this.healthSlider = this.GetComponent<Slider>();
	}

	[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
	private void Update() {
		if (this.healthData == null) {
			return;
		}

		//Runs if Slider max value does not equal player's max stamina
		if (this.healthSlider.maxValue != this.healthData.maxHealth) {
			//Updates max value for Slider to correspond to player's max stamina
			this.healthSlider.maxValue = this.healthData.maxHealth;

			//If Slider value is at max Value, updates Text to match current max value
			if (this.healthText != null && this.healthSlider.value == this.healthSlider.maxValue) {
				this.healthText.text = this.healthSlider.maxValue.ToString(CultureInfo.CurrentCulture);
			}
		}

		//Updates Slider value to match player's current stamina
		if (this.healthSlider.value != this.healthData.currentHealth) {
			this.healthSlider.value = this.healthData.currentHealth;
		}
	}

	public void ChangeDisplayValue(float currentHealth) {
		if (this.healthData != null && this.healthText != null) {
			this.healthText.text = this.formatPrefix +
			(this.healthSlider.value * this.valueMultiplier + this.valueModifier) +
			this.formatSuffix;
		}
	}
}
