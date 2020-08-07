using System.Collections.Generic;

class JsonLevelData
{
    public List<ObjectData> objects = new List<ObjectData>();
    public List<BarbedWireData> barbedWires = new List<BarbedWireData>();
    public List<DoorData> doors = new List<DoorData>();
    public List<BarrierData> barriers = new List<BarrierData>();
    public List<SpotlightData> spotlights = new List<SpotlightData>();
    public List<EnemyData> enemies = new List<EnemyData>();
    public List<OfficerData> officers = new List<OfficerData>();
    public PlayerData player = new PlayerData();
    public List<NodeData> nodes = new List<NodeData>();
    public List<BushData> bushes = new List<BushData>();
    public List<BuildingBlockData> blocks = new List<BuildingBlockData>();
}
