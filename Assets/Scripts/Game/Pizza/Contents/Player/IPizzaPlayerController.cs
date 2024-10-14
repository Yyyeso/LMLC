using UnityEngine;

public interface IPizzaPlayerController
{
    public bool OnCollision { get; set; }
    public bool IsFreeze { get; set; }

    public void SetCharacter(int idx);

    public IPizzaPlayerController Setup(int idx);

    public Vector3 Move(float speed);

    public void Fallen(); // float force

    public void GameOver();
}