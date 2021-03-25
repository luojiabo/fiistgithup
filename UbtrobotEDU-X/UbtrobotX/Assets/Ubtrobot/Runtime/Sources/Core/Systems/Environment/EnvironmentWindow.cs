using System;
using Loki;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Text;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Ubtrobot
{
	public class EnvironmentWindow : UComponent
	{
		[AutoSerializeField]
		public Slider tempSlider;
		[AutoSerializeField]
		public Slider humiditySlider;
		[AutoSerializeField]
		public Slider luminanceSlider;

		private Environment activeEnv;

		private void SetEnvironment(Environment env)
		{
			activeEnv = env;
			OnEnvironmentChanged();
		}

		private new void Awake()
		{
			base.Awake();

			RegisterSlider(tempSlider, OnTempChanged);
			RegisterSlider(humiditySlider, OnHumidityChanged);
			RegisterSlider(luminanceSlider, OnLuminanceChanged);

			OnEnvironmentChanged();
		}

		private void Update()
		{
			if (activeEnv == null)
			{
				var env = EnvironmentSystem.GetOrAlloc().defaultEnvironment;
				SetEnvironment(env);
			}
		}

		private void RegisterSlider(Slider slider, UnityAction<float> onValueChanged)
		{
			if (slider != null)
			{
				slider.onValueChanged.AddListener(onValueChanged);
			}
		}

		private void UnregisterSlider(Slider slider, UnityAction<float> onValueChanged)
		{
			if (slider != null)
			{
				slider.onValueChanged.RemoveListener(onValueChanged);
			}
		}

		private void OnEnvironmentChanged()
		{
			UpdateTempSlider();
			UpdateHumiditySlider();
			UpdateLuminanceSlider();
			if (activeEnv == null)
			{
				if (tempSlider != null)
					tempSlider.value = 0.0f;
				if (humiditySlider != null)
					humiditySlider.value = 0.0f;
				if (luminanceSlider != null)
					luminanceSlider.value = 0.0f;
				return;
			}

			if (tempSlider != null)
				tempSlider.value = activeEnv.temperature;
			if (humiditySlider != null)
				humiditySlider.value = activeEnv.humidity;
			if (luminanceSlider != null)
				luminanceSlider.value = activeEnv.luminance;
		}

		private void UpdateSlider(Slider slider, string titleContent)
		{
			if (slider == null)
				return;

			var title = slider.GetComponent<Text>("Title");
			title.text = titleContent;
		}

		private void UpdateSliderRange(Slider slider, Type type, string fieldName)
		{
			if (slider == null)
				return;

			FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
			var rangeAttr = fieldInfo.GetCustomAttribute<RangeAttribute>();

			slider.minValue = rangeAttr.min;
			slider.maxValue = rangeAttr.max;
			slider.wholeNumbers = false;
		}

		private void UpdateTempSlider()
		{
			if (activeEnv == null)
				return;

			UpdateSliderRange(tempSlider, activeEnv.GetType(), "temperature");
			UpdateSlider(tempSlider, string.Format("温度 [{0}C°]", activeEnv.temperature.ToString("F1")));
		}

		private void OnTempChanged(float value)
		{
			if (activeEnv == null)
				return;
			activeEnv.temperature = value;
			UpdateTempSlider();
		}

		private void UpdateHumiditySlider()
		{
			if (activeEnv == null)
				return;
			UpdateSliderRange(humiditySlider, activeEnv.GetType(), "humidity");
			UpdateSlider(humiditySlider, string.Format("相对湿度 [{0}%]", activeEnv.humidity.ToString("F1")));
		}

		private void OnHumidityChanged(float value)
		{
			if (activeEnv == null)
				return;
			activeEnv.humidity = value;
			UpdateHumiditySlider();
		}

		private void UpdateLuminanceSlider()
		{
			if (activeEnv == null)
				return;
			UpdateSliderRange(luminanceSlider, activeEnv.GetType(), "luminance");
			UpdateSlider(luminanceSlider, string.Format("光度 [{0}]", activeEnv.luminance.ToString("F1")));
		}

		private void OnLuminanceChanged(float value)
		{
			if (activeEnv == null)
				return;
			activeEnv.luminance = value;
			UpdateLuminanceSlider();
		}
	}
}
