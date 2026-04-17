using UnityEngine;

public class ExploreAreaTrigger : MonoBehaviour
{
    [SerializeField] private string areaID; // deve bater com targetID da quest

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        QuestManager.Instance.ReportAreaReached(areaID);
    }
}