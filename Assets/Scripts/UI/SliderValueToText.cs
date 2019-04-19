using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Slider))]
public class SliderValueToText : MonoBehaviour {
	[SerializeField]
	private Text sliderText;
	//TODO proper string formatting
	[SerializeField]
	private string formatPrefix;
	[SerializeField]
	private string formatSuffix;
	[SerializeField]
	private float valueMultiplier = 1;
	[SerializeField]
	private float valueModifier = 0;


	private Slider thisSlider;

	private void Awake() {
		this.thisSlider = this.GetComponent<Slider> ();
	}

	private void Update() {
		this.sliderText.text = this.formatPrefix +
		                       (this.thisSlider.value * this.valueMultiplier + this.valueModifier) +
		                       this.formatSuffix;
	}
}
