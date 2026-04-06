using UnityEngine;

public class BattleTimer : MonoBehaviour
{
    public float timeRemaining = 180f;
    public void StartTimer(float duration) { timeRemaining = duration; }
    public bool IsTimeUp() { return timeRemaining <= 0f; }
}
