using UnityEngine;
class SpawnCarArgs
{
    public int CarID;
    public bool isRealBody;
   public SpawnCarArgs(int carID)
    {
        CarID = carID;
        isRealBody = true;

    }
    public SpawnCarArgs(int carID,bool isrealBody)
    {
        CarID = carID;
        isRealBody = isrealBody;
    }
}
