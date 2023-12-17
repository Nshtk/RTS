using System;

using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Environment
{
	public partial class Environment : MonoBehaviour
	{
		[Serializable]
		public class TimeCycle
		{
			[SerializeField]  private Environment _environment;
			[SerializeField, Range(0, 24)] private float _time_current;
			private float _time_normalised;

			[Header("Sun settigns")]
			[SerializeField] private float _sun_speed;
			[SerializeField] private float _sun_intensity=1f;
			[SerializeField] private AnimationCurve _sun_intensity_curve;
			[SerializeField] private AnimationCurve _sun_color_temperature_curve;

			[Header("Moon settigns")]
			[SerializeField] private float _moon_speed;
			[SerializeField] private float _moon_intensity = 1f;
			[SerializeField] private AnimationCurve _moon_intensity_curve;
			[SerializeField] private AnimationCurve _moon_color_temperature_curve;

			private HDAdditionalLightData _sun_additional_data;
			private HDAdditionalLightData _moon_additional_data;

			public float Time_Current
			{
				get { return _time_current; }
				private set
				{
					_time_current = value;
					if (_time_current>24)
						_time_current%=24;
				}
			}

			internal TimeCycle()
			{
				_sun_additional_data=_environment._sun_prefab.GetComponent<HDAdditionalLightData>();
				_moon_additional_data=_environment._moon_prefab.GetComponent<HDAdditionalLightData>();
			}

			internal void update()
			{
				//_sun_additional_data=_environment._sun_prefab.GetComponent<HDAdditionalLightData>();
				//_moon_additional_data=_environment._moon_prefab.GetComponent<HDAdditionalLightData>();
				Time_Current+=Time.deltaTime;
				_time_normalised=_time_current/24;
				updateLights();
			}
			internal void updateLights()
			{
				if (_time_current>6f && _time_current<10f)
					_sun_additional_data.EnableShadows(true);
				else
					_sun_additional_data.EnableShadows(false);
				_environment._sun_prefab.transform.rotation=Quaternion.Euler(Mathf.Lerp(-90, 270, _time_normalised)*_sun_speed, _environment._sun_prefab.transform.rotation.y, _environment._sun_prefab.transform.rotation.z);
				_environment._sun_prefab.intensity = _sun_intensity_curve.Evaluate(_time_normalised)*_sun_intensity;
				_environment._sun_prefab.colorTemperature=_sun_color_temperature_curve.Evaluate(_time_normalised)*10000f;

				/*if (_time_current>6f && _time_current<10f)
					_sun_additional_data.EnableShadows(true);
				else
					_sun_additional_data.EnableShadows(false);*/
				_environment._moon_prefab.transform.rotation=Quaternion.Euler(Mathf.Lerp(90, 450, _time_normalised), _environment._moon_prefab.transform.rotation.y, _environment._moon_prefab.transform.rotation.z);
				_environment._moon_prefab.intensity = _moon_intensity_curve.Evaluate(_time_normalised);
				_environment._moon_prefab.colorTemperature=_moon_color_temperature_curve.Evaluate(_time_normalised)*10000f;
			}
		}
	}
}
