using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForceCharacterController : MonoBehaviour
{
    float mass = 3.0f; // defines the character mass
    Vector3 impact = Vector3.zero;
    CharacterController character;


    void Awake()
    {
        character = GetComponent<CharacterController>();
    }

    public void AddImpact(Vector3 _dir, float _force)
    {
        _dir.Normalize();
        if (_dir.y < 0) _dir.y = -_dir.y; // reflect down force on the ground
        impact += _dir.normalized * _force / mass;
    }
    void Update()
    {
        // apply the impact force:
        if (impact.magnitude > 0.2) character.Move(impact * Time.deltaTime);

        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
    }
}
