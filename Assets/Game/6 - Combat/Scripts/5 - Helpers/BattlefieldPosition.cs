using UnityEngine;

public class BattlefieldPosition
{
    public BattlefieldPosition(Vector3 position, int spotId, int relationalReferenceId)
    {
        Position = position;
        SpotId = spotId;
        RelationalReferenceId = relationalReferenceId;
    }
    public Vector3 Position;
    public int SpotId = -10;
    public int RelationalReferenceId = -10;
}
