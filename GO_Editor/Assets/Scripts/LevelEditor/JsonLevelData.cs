using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class JsonLevelData
{
    public List<ObjectData> objects = new List<ObjectData>();
    public List<BarbedWireData> barbedWires = new List<BarbedWireData>();
    public List<DoorData> doors = new List<DoorData>();
    public List<BarrierData> barriers = new List<BarrierData>();
    public List<EnemyData> enemies = new List<EnemyData>();
    public PlayerData player = new PlayerData();
    public List<NodeData> nodes = new List<NodeData>();
    public List<BushData> bushes = new List<BushData>();
}
