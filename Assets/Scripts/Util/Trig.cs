using System;

[Serializable]
public struct Trig
{
	public float verticalOffset, amplitude, phaseShift, angularFrequency;
	public Func<float, float> func;
	public Func<float> theta;

	public readonly float originalVerticalOffset, originalAmplitude, originalPhaseShift, originalAngularFrequency;
	public readonly Func<float, float> originalFunc;
	public readonly Func<float> originalTheta;

	public Trig(float verticalOffset, float amplitude, float phaseShift, float angularFrequency, Func<float, float> func, Func<float> theta)
	{
		this.verticalOffset = verticalOffset;
		this.amplitude = amplitude;
		this.phaseShift = phaseShift;
		this.angularFrequency = angularFrequency;
		this.func = func;
		this.theta = theta;

		originalVerticalOffset = verticalOffset;
		originalAmplitude = amplitude;
		originalPhaseShift = phaseShift;
		originalAngularFrequency = angularFrequency;
		originalFunc = func;
		originalTheta = theta;
	}

	public float Value()
	{
		return amplitude * func(angularFrequency * (theta() - phaseShift)) + verticalOffset;
	}
}
