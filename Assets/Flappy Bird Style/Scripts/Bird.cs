using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Bird : MonoBehaviour 
{


	public float sensitivity = 100;
	public float loudness = 0;
	public AudioSource audioSource;
	float timeSinceRestart = -0.75f;

	public float upForce;					
	private bool isDead = false;			

	private Animator anim;					
	private Rigidbody2D rb2d;	
	Slider sensitivitySlider;


	void Start()
	{
		anim = GetComponent<Animator> ();
		rb2d = GetComponent<Rigidbody2D>();

		sensitivitySlider = GameObject.Find("SensitivitySlider").GetComponent<Slider>();   

		timeSinceRestart = Time.time;
		audioSource.clip = Microphone.Start(null, true, 300, 44100);
		//wait until microphone position is found (?)
		while (!(Microphone.GetPosition(null) > 0))
		{
		}

		audioSource.Play(); // Play the audio source
	}

	float GetAveragedVolume()
	{
		float[] data = new float[256];
		float a = 0;
		audioSource.GetOutputData(data, 0);
		foreach(float s in data)
		{
			a += Mathf.Abs(s);
		}
		return a / 256;
	}

	void Update()
	{

		if (Time.time - timeSinceRestart > 300)// && !Microphone.IsRecording(null))
		{
			timeSinceRestart = Time.time;
			audioSource.clip = Microphone.Start(null, true, 300, 44100);
			//wait until microphone position is found (?)
			while (!(Microphone.GetPosition(null) > 0))
			{
			}

			audioSource.Play(); 

		}

		loudness = GetAveragedVolume() * sensitivity;

		sensitivitySlider.value = Mathf.Clamp(sensitivitySlider.value, 0.01f, 1);
		float minVal = 1/sensitivitySlider.value * 5;
		float maxVal = 1/sensitivitySlider.value * 10;


		if (loudness > minVal) {
			if (isDead == false) 
			{

				rb2d.velocity = Vector2.zero;
				rb2d.AddForce(new Vector2(0, loudness));


				anim.SetTrigger("Flap");
				//rb2d.velocity = Vector2.zero;
				//rb2d.AddForce(new Vector2(0, upForce));
			}	
		
		}

	}

	void OnCollisionEnter2D(Collision2D other)
	{
		rb2d.velocity = Vector2.zero;
		isDead = true;
		anim.SetTrigger ("Die");
		GameControl.instance.BirdDied ();
	}
}
