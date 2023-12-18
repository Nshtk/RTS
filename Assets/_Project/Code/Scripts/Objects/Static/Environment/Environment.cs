using UnityEngine;

namespace Environment 
{
	public partial class Environment : MonoBehaviour	//REVIEW to Global 
	{
		public TimeCycle time_cycle;
		[SerializeField] private Light _sun_prefab;
		[SerializeField] private Light _moon_prefab;
		[SerializeField] private ParticleSystem _stars_prefab;

		private void Awake()
		{

		}
		private void Start()
		{

		}
		private void Update()
		{
			time_cycle.update();
		}
		private void OnValidate()
		{
			//time_cycle.update();
		}
	}
}

