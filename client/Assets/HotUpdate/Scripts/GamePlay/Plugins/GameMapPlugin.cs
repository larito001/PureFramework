using System.Collections.Generic;
using UnityEngine;
using YOTO;


public class BlockInfo
{
    public Vector3 center;
    public float radio;
}

public class GameMapPlugin : LogicPluginBase
{
    #region 单例和生命周期

    public static GameMapPlugin Instance;
    private GameObject stone;

    protected override void OnInstall()
    {
        base.OnInstall();
        DrawMap();
        stone = GameObject.Find("SpecialStone");
        YOTOFramework.timeMgr.LoopCall(CheckStone, 1);
    }

    private void CheckStone()
    {
        SetBlock(new BlockInfo()
        {
            center = stone.transform.position,
            radio = 5f
        });
    }

    protected override void OnUninstall()
    {
        base.OnUninstall();
    }

    public GameMapPlugin()
    {
        Instance = this;
    }

    #endregion

    public class MapInfo
    {
        public ObjectBase content; // 当前格子上的物体（可以为空）
        public Vector2Int gridPosition; // 格子坐标
    }

    public Vector3 center = new Vector3(-80, 0, -50); // 地图左下角的世界坐标
    public int column = 6; // 列数（x轴方向）
    public int row = 6; // 行数（z轴方向）
    public float cellSize = 6f; // 每个格子的大小

    private List<List<MapInfo>> map = new List<List<MapInfo>>();

    /// <summary>
    /// 初始化并绘制地图
    /// </summary>
    public void DrawMap()
    {
        map.Clear();

        for (int y = 0; y < row; y++)
        {
            var rowList = new List<MapInfo>();
            for (int x = 0; x < column; x++)
            {
                rowList.Add(new MapInfo
                {
                    gridPosition = new Vector2Int(x, y),
                    content = null
                });
            }

            map.Add(rowList);
        }

        // ✅ 在每个格子生成一个物体测试
        for (int y = 0; y < row; y++)
        {
            for (int x = 0; x < column; x++)
            {
                // 创建 Cube

                // var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                // obj.transform.localScale = Vector3.one * (cellSize * 0.8f); // 缩小一点以防重叠
                //
                // // 添加脚本
                // var item = obj.AddComponent<GameMapItem>();
                Vector2Int grid = new Vector2Int(x, y);
                Vector3 worldPos = GridToWorld(grid);
                var item = TowerManager.Instance.GenerateTowerBase(worldPos);
                PutThingOnMap(worldPos, item);
            }
        }
    }

    public void SetBlock(BlockInfo info)
    {
        for (int y = 0; y < map.Count; y++)
        {
            for (int x = 0; x < map[y].Count; x++)
            {
                if (map[y][x].content == null) continue;
                Vector2Int grid = new Vector2Int(x, y);
                Vector3 gridCenter = GridToWorld(grid);
                float distance = Vector3.Distance(gridCenter, info.center);

                if (distance <= info.radio)
                {
                    var tower = map[y][x].content as TowerBaseEntity;
                    tower.AddPower();
                }
                else
                {
                    var tower = map[y][x].content as TowerBaseEntity;
                    tower.RemovePower();
                }
            }
        }
    }


    /// <summary>
    /// 世界坐标转网格坐标
    /// </summary>
    private Vector2Int WorldToGrid(Vector3 pos)
    {
        int x = Mathf.FloorToInt((pos.x - center.x) / cellSize);
        int y = Mathf.FloorToInt((pos.z - center.z) / cellSize);
        return new Vector2Int(x, y);
    }

    /// <summary>
    /// 网格坐标转世界坐标（格子中心点）
    /// </summary>
    private Vector3 GridToWorld(Vector2Int grid)
    {
        float worldX = center.x + grid.x * cellSize + cellSize * 0.5f;
        float worldZ = center.z + grid.y * cellSize + cellSize * 0.5f;
        return new Vector3(worldX, 0f, worldZ);
    }

    /// <summary>
    /// 在地图上放置一个 GameMapItem
    /// </summary>
    public void PutThingOnMap(Vector3 pos, ObjectBase obj)
    {
        Vector2Int grid = WorldToGrid(pos);
        if (!IsValidGrid(grid)) return;

        var info = map[grid.y][grid.x];
        if (info.content == null)
        {
            info.content = obj;
            obj.Location = GridToWorld(grid);
        }
    }

    /// <summary>
    /// 从地图上移除一个 GameMapItem
    /// </summary>
    public ObjectBase RemoveThingOnMap(Vector3 pos)
    {
        Vector2Int grid = WorldToGrid(pos);
        if (!IsValidGrid(grid)) return null;

        var info = map[grid.y][grid.x];
        ObjectBase item = info.content;
        if (item != null)
        {
            info.content = null;
        }

        return item;
    }

    /// <summary>
    /// 判断网格坐标是否合法
    /// </summary>
    private bool IsValidGrid(Vector2Int grid)
    {
        return grid.x >= 0 && grid.x < column && grid.y >= 0 && grid.y < row;
    }
}