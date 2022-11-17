using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path
    /// Note that you will probably need to add some helper functions
    /// from the startPos to the endPos
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>

    private Dictionary<Vector2Int, Node> nodeGrid = new Dictionary<Vector2Int, Node>();

    private List<Node> open = new List<Node>();
    private List<Node> closed = new List<Node>();

    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {

        nodeGrid.Clear();
        open.Clear();
        closed.Clear();

        for(int x = 0; x < grid.GetLength(0); x++) {
            for(int y = 0; y < grid.GetLength(1); y++) {
                Node node = new Node(grid[x,y].gridPosition, null, 0, 0);
                node.cell = grid[x,y];
                nodeGrid.Add(grid[x,y].gridPosition, node);
            }
        }

        List<Vector2Int> path = new List<Vector2Int>();

        open.Add(nodeGrid[startPos]);
        nodeGrid[startPos].HScore = GetScore(startPos, endPos);

        Node current = new Node();
        
        while(open.Count != 0) {

            current = GetNodeWithLowestFScore(open);
            open.Remove(current);
            closed.Add(current);

            if(current.position == endPos) {
                break;
            }

            Debug.Log(current.cell);

            foreach(Node neighbour in GetNodeNeighbours(current, grid)) {

                Debug.Log(neighbour.cell.gridPosition);

                if(closed.Contains(neighbour)) {
                    continue;
                }

                Vector2Int neighbourDir = current.position - neighbour.position;

                if(neighbourDir.Equals(new Vector2Int(1,0))) {
                    if(neighbour.cell.HasWall(Wall.RIGHT)) {
                        continue;
                    }
                }
                else if(neighbourDir.Equals(new Vector2Int(-1,0))) {
                    if(neighbour.cell.HasWall(Wall.LEFT)) {
                        continue;
                    }
                }
                else if(neighbourDir.Equals(new Vector2Int(0,1))) {
                    if(neighbour.cell.HasWall(Wall.UP)) {
                        continue;
                    }
                }
                else if(neighbourDir.Equals(new Vector2Int(0,-1))) {
                    if(neighbour.cell.HasWall(Wall.DOWN)) {
                        continue;
                    }
                }

                if(neighbour.FScore <= current.FScore || !open.Contains(neighbour)) {
                    neighbour.GScore = current.GScore + GetScore(neighbour.position, current.position);
                    neighbour.HScore = GetScore(neighbour.position, endPos);
                    neighbour.parent = current;
                    if(!open.Contains(neighbour)) {
                        open.Add(neighbour);
                    }
                }

            }

        }

        while(current.parent != null) {
            path.Add(current.position);
            current = current.parent;
        }

        path.Reverse();

        return path;

    }

    public Node GetNodeWithLowestFScore(List<Node> nodes) {

        Node node = nodes[0];

        for(int i = 0; i < nodes.Count; i++) {
            if(nodes[i].FScore <= node.FScore) {
                node = nodes[i];
            }
        }

        return node;

    }

    public List<Node> GetNodeNeighbours(Node node, Cell[,] grid) {

        List<Cell> cellNeighbours = node.cell.GetNeighbours(grid);
        List<Node> neighbours = new List<Node>();

        foreach(Cell c in cellNeighbours) {
            neighbours.Add(nodeGrid[c.gridPosition]);
        }

        return neighbours;
    }

    public int GetScore(Vector2Int pos, Vector2Int targetPos) {
        return (int) Mathf.Abs(pos.x - targetPos.x) + Mathf.Abs(pos.y - targetPos.y);
    }

    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public Cell cell;

        public float FScore { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
