using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    void Start() {
        Position = transform.position;
    }
    public Vector3 Position;
    public int SpotId = -10;
    public int RelationalReferenceId = -10;

    public BattlefieldPosition GetInfo() {
        return new BattlefieldPosition(Position, SpotId, RelationalReferenceId);
    }
}
