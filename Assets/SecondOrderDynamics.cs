using System;
using UnityEngine;

/// <summary>
/// A Simulation of Second Order System.
/// References:
///     [EN](https://www.youtube.com/watch?v=KPoeNZZ6H4s)
///     [CN](https://www.bilibili.com/video/BV1wN4y1578b)
/// </summary>
public class SecondOrderDynamics
{
    // previous input
    private Vector3 _prevPos;

    // state variables
    private Vector3 _pos;
    private Vector3 _velocity;

    // dynamics constants
    private readonly float _w;
    private readonly float _z;
    private readonly float _d;
    private readonly float _k1;
    private readonly float _k2;
    private readonly float _k3;

    private readonly bool _isHighSpeedMovement;


    /// <summary>
    /// Second Order Dynamics.
    /// </summary>
    /// <param name="frequency">Describe the speed of response to changes in input.</param>
    /// <param name="dampingCoef">
    /// Describes how the system comes to settle at the target.
    /// Critical damping is 1, means no vibrate.
    /// </param>
    /// <param name="initialResponse">Initial response. Usually 2.</param>
    /// <param name="initialPos">Initial position.</param>
    /// <param name="isHighSpeedMovement">is high speed movement?</param>
    public SecondOrderDynamics(float frequency, float dampingCoef,
        float initialResponse, Vector3 initialPos, bool isHighSpeedMovement = false)
    {
        _isHighSpeedMovement = isHighSpeedMovement;

        // compute constants
        _w = 2 * Mathf.PI * frequency;
        _z = dampingCoef;
        _d = _w * Mathf.Sqrt(Mathf.Abs(dampingCoef * dampingCoef - 1));
        _k1 = dampingCoef / (Mathf.PI * frequency);
        _k2 = 1 / (_w * _w);
        _k3 = initialResponse * dampingCoef / _w;
        // initialize variables
        _prevPos = initialPos;
        _pos = initialPos;
        _velocity = Vector3.zero;
    }

    public Vector3 Update(float deltaTime, Vector3 targetPos, Vector3? velocity = null)
    {
        // estimate velocity
        if (velocity == null)
        {
            velocity = (targetPos - _prevPos) / deltaTime;
            _prevPos = targetPos;
        }

        float k2Stable;
        // clamp k2 to guarantee stability without jitter
        if (!_isHighSpeedMovement || _w * deltaTime < _z)
        {
            k2Stable = Mathf.Max(_k2, deltaTime * _k1,
                deltaTime * deltaTime / 2 + deltaTime * _k1 / 2);
        }
        // use pole matching when the system is very fast
        else
        {
            float t1 = Mathf.Exp(-_z * _w * deltaTime);
            float alpha = 2 * t1 * (_z <= 1 ? Mathf.Cos(deltaTime * _d) : (float)Math.Cosh(deltaTime * _d));
            float beta = t1 * t1;
            float t2 = deltaTime / (1 + beta - alpha);
            k2Stable = deltaTime * t2;
        }

        // integrate position be velocity
        _pos += deltaTime * _velocity;
        // integrate velocity be acceleration
        _velocity += deltaTime * (targetPos + _k3 * velocity.Value - _pos - _k1 * _velocity) / k2Stable;

        return _pos;
    }
}