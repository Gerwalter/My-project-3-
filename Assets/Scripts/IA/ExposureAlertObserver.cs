using UnityEngine;

public class ExposureAlertObserver : IAlertSystemObserver
{
    private PatrollingNPC npc;

    public ExposureAlertObserver(PatrollingNPC npcRef)
    {
        npc = npcRef;
    }

    public void Notify(float currentAlert, float maxAlert)
    {
        // Este observer solo existe para la suscripción, la lógica está en UpdateExposure()
    }
}