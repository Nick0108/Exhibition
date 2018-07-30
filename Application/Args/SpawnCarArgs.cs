using UnityEngine;
class SpawnCarArgs
{
    public int CarID;
    public bool isInARScene;
   public SpawnCarArgs(int carID)
    {
        CarID = carID;
        isInARScene = false;

    }
    public SpawnCarArgs(int carID,bool pIsInARScene)
    {
        CarID = carID;
        isInARScene = pIsInARScene;
    }

}
