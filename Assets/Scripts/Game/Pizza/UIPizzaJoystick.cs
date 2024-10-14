using UnityEngine;

public class UIPizzaJoystick : UIPizzaBase
{
    [SerializeField] private PizzaJoystick joystick;
    public PizzaJoystick PizzaJoystick => joystick;
}