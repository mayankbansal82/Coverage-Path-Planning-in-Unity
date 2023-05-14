using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Diagnostics;

public class Grid : MonoBehaviour
{
    public GameObject gridPrefab;

    private int rows = 0, columns = 0, scale = 1;
    private int mergedMapHeight,mergedMapWidth;
    private Vector3 topLeftLocation = new Vector3(0,0,0);
    private Vector3[,] unity_coord;
    private int[,] map;
    private int[,] occMap;
    private List<int[]> edge = new List<int[]>();

    public GameObject floor;
    public Collider floorCollider;
    private Vector3 floorColliderSize;

    public SphereCollider gridPrefabCollider;
    private List<int[]> new_path;
    private int i = 0;
    // private float gridPrefabColliderSize;

    public LayerMask obstacleLayer;

    public float delayInSeconds = 1.0f;
    public GameObject robot;
    public GameObject marker;

    // public float smoothTime = 2000000000000000.0f;
    // Vector3 velocity;
    // public float speed = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        // renderer = GetComponent<MeshRenderer>();
        // rendererSize = renderer.bounds.size;
        // print("renderer size = " + rendererSize);
        
        floorCollider = floor.GetComponent<Collider>();
        floorColliderSize = floorCollider.bounds.size;

        gridPrefabCollider = gridPrefab.GetComponent<SphereCollider>();

        columns = (int)(floorColliderSize.x);
        // print(columns);
        rows = (int)(floorColliderSize.z);
        // print(rows);
        mergedMapHeight = columns/2;
        mergedMapWidth = rows/2;
        
        topLeftLocation.x = (-columns/2)+(float)gridPrefabCollider.radius;
        topLeftLocation.y = (float)gridPrefabCollider.radius;
        topLeftLocation.z = (rows/2)-(float)gridPrefabCollider.radius;

        // map = new int[rows, columns]; // stores 0s and 1s 

        unity_coord = new Vector3[rows, columns]; // stores unity co-ordinates
        map = new int[rows, columns]; // stores 2D map
        // print(rows);
        // print(columns);
        
        GenerateGrid();

        int[] start = {0,0};
        int[] goal = {4,4};
        int flag = 0;

        var path = CCP(start, goal);

        //occMap = map;
        //var path = STC(start);

        // List<int[]> new_path = new List<int[]>();
        // foreach (var p in path)
        // {
        //     print(p);
        //     int[] nodeA = new int[2];
        //     nodeA[0] = p[0];
        //     nodeA[1] = p[1];
        //     new_path.Add(nodeA);
        //     int[] nodeB = new int[2];
        //     nodeB[0] = p[2];
        //     nodeB[1] = p[3];
        //     new_path.Add(nodeB);
        // }

        // string p_now = "";
        // foreach(var p in path){
        //     p_now += p[0];
        //     p_now += " ";
        //     p_now += p[1];
        //     p_now += '\n';
        //     p_now += p[2];
        //     p_now += " ";
        //     p_now += p[3];
        //     p_now += '\n';
        // }
        //     // Debug.Log(p_now);
        //     print(p_now);
        //     print(new_path.Count);  

        StartCoroutine(MoveRobot(path));
        // MoveRobot(new_path);
    }

    // void Update()
    // {
    //     var count = new_path.Count();
    //     var p = new_path[i];
    //     int row = p[0];
    //     int col = p[1];

    //     Vector3 newPosition = new Vector3(unity_coord[row,col].x, 1, unity_coord[row,col].z);
    //     print(newPosition);

    //     // robot.transform.position = newPosition;

    //     // robot.transform.position = Vector3.SmoothDamp(robot.transform.position, newPosition, ref velocity, smoothTime);
    //     robot.transform.position = Vector3.Lerp(robot.transform.position, newPosition, speed * Time.deltaTime);

    //     // if (i == count - 1)
    //     // {
    //     //     i = count - 1;
    //     // }
    //     // else
    //     // {
    //     //     i = i + 1;
    //     // }

    // }

    IEnumerator MoveRobot(List<int[]>path)
    {
        foreach(var p in path)
        {
            int row = p[0];
            int col = p[1];

            Vector3 newPosition = new Vector3(unity_coord[row,col].x, 1, unity_coord[row,col].z);
            Vector3 markerPosition = new Vector3(unity_coord[row,col].x, 0, unity_coord[row,col].z);
            // print(newPosition);

            robot.transform.position = newPosition;

            // robot.transform.position = Vector3.SmoothDamp(robot.transform.position, newPosition, ref velocity, smoothTime);
            Instantiate(marker, markerPosition, Quaternion.identity);
            // robot.transform.position = Vector3.Lerp(robot.transform.position, newPosition, speed * Time.deltaTime);

            // print("robot has traveled to " + newPosition);

            yield return new WaitForSeconds(delayInSeconds);
            // yield return true;
        } 
    }

    // void MoveRobot(List<int[]>path)
    // {
    //     foreach(var p in path)
    //     {
    //         int row = p[0];
    //         int col = p[1];

    //         Vector3 newPosition = new Vector3(unity_coord[row,col].x, 1, unity_coord[row,col].z);

    //         // robot.transform.position = newPosition;

    //         robot.transform.position = Vector3.SmoothDamp(robot.transform.position, newPosition, ref velocity, smoothTime);

    //         // print("robot has traveled to " + newPosition);

    //         // yield return new WaitForSeconds(delayInSeconds);
    //         // yield return true;
    //     } 
    // }

    void GenerateGrid()
    {
        for(int i = 0; i < columns; i++)
        {
            for(int j = 0; j < rows; j++)
            {
                GameObject obj = Instantiate(gridPrefab, new Vector3(topLeftLocation.x+scale*j,topLeftLocation.y,topLeftLocation.z-scale*i), Quaternion.identity);

                unity_coord[i,j] = obj.transform.position;
                // print(unity_coord[i,j]);

                Collider[] colliders = Physics.OverlapSphere(obj.transform.position, gridPrefabCollider.radius, obstacleLayer);

                if (colliders.Length > 0)
                {
                    map[i,j] = 1;
                    // obj collides with obstacle layer 
                    // Debug.Log($"Object at ({i}, {j}) collided with an obstacle");
                    // return true; // uncomment if you want to return immediately upon first collision
                }
                else
                {
                    map[i,j] = 0;
                }
                // print("value: " + map[i,j] + " i: " + i + " j: " + j);
                // print("value: " + map[i,j] + " x: " + unity_coord[i,j].x + " y: " + unity_coord[i,j].y + " z: " + unity_coord[i,j].z);

                Destroy(obj);
            }  
        }
        // print(map);
        // print(unity_coord[0,0]);  
    }    

    public int[] neighbours_4(int[,] PCEmatrix, int[] cell, int[,] visited, int[] next_cell, int[,,] Parent)
    {
        // shifting to neighbours as required
        int[][] shift_4 = new int[][] { new int[] { 0, 1 }, new int[] { 1, 0 }, new int[] { 0, -1 }, new int[] { -1, 0 } };

        List<int[]> neighbours_4 = new List<int[]>();

        // 4 valid neighbours
        for (int i = 0; i < shift_4.Length; i++)
        {
            int[] neighbour = new int[] { cell[0] + shift_4[i][0], cell[1] + shift_4[i][1] };

            // exclude ones outside the map and with present obstacles
            if (neighbour[0] >= 0 && neighbour[1] >= 0 && neighbour[0] < columns && neighbour[1] < rows)// && PCEmatrix[neighbour[0], neighbour[1]] != 1)
            {
                neighbours_4.Add(neighbour);
            }
        }

        List<int[]> safe_coordinates = new List<int[]>();
        foreach (int[] jj in neighbours_4)
        {
            if (map[jj[0], jj[1]] == 0)
            {
                safe_coordinates.Add(new int[] { jj[0], jj[1] });
            }
        }

        List<int[]> Prio_V0 = new List<int[]>();
        List<int[]> Prio_V1 = new List<int[]>();
        List<int> Prio_V0_PCEvals_list = new List<int>();
        List<int> Prio_V1_PCEvals_list = new List<int>();
        int indexx = 0;
        int flag_V = 0;
        if (flag_V == 0)
        {
            foreach (int[] jj in safe_coordinates)
            {
                if (visited[jj[0], jj[1]] == 0)
                {
                    Prio_V0.Add(new int[] { jj[0], jj[1] });
                    Prio_V0_PCEvals_list.Add(PCEmatrix[jj[0], jj[1]]);
                }
            }

            if (Prio_V0_PCEvals_list.Count == 0)
            {
                indexx = 0;
            }
            else
            {
                indexx = Prio_V0_PCEvals_list.IndexOf(Prio_V0_PCEvals_list.Max());

                return Prio_V0[indexx];
            }
        }

        if (Prio_V0.Count == 0) 
        {
            flag_V = 1;
        }

        // bool flagg = true;
        if (flag_V == 1) 
        {
            if (safe_coordinates.Count == 1) 
            {
                foreach (int[] jj in safe_coordinates) 
                {
                    if (visited[jj[0], jj[1]] == 1) 
                    {
                        Prio_V1.Add(new int[] { jj[0], jj[1] });
                        Prio_V1_PCEvals_list.Add(PCEmatrix[jj[0], jj[1]]);
                    }
                }
            } 
            else 
            {
                foreach (int[] jj in safe_coordinates) 
                {
                    if (visited[jj[0], jj[1]] == 1) 
                    {
                        if (Parent[cell[0], cell[1], 0] != jj[0] || Parent[cell[0], cell[1], 1] != jj[1]) 
                        {
                            Prio_V1.Add(new int[] { jj[0], jj[1] });
                            Prio_V1_PCEvals_list.Add(PCEmatrix[jj[0], jj[1]]);
                        }
                    }
                }
            }

            indexx = Prio_V1_PCEvals_list.IndexOf(Prio_V1_PCEvals_list.Min());

            return Prio_V1[indexx];
        }
        return cell;
    }            

    public bool IsIdentical(int[,] visited) {
        int[,] completeMatrix = new int[columns, rows];
        for (int i = 0; i < columns; i++) {
            for (int j = 0; j < rows; j++) {
                completeMatrix[i, j] = 1;
            }
        }
        for (int i = 0; i < columns; i++) {
            for (int j = 0; j < rows; j++) {
                if (visited[i,j] != completeMatrix[i, j]) {
                    return false;
                }
            }
        }
        return true;
    }

    int argmax(List<int> values)
    {
        int maxIndex = -1;
        int maxValue = int.MinValue;
        for (int i = 0; i < values.Count; i++)
        {
            if (values[i] > maxValue)
            {
                maxIndex = i;
                maxValue = values[i];
            }
        }
        return maxIndex;
    }

    int argmin(List<int> values)
    {
        int minIndex = -1;
        int minValue = int.MaxValue;
        for (int i = 0; i < values.Count; i++)
        {
            if (values[i] < minValue)
            {
                minIndex = i;
                minValue = values[i];
            }
        }
        return minIndex;
    }
    
    public List<int[]> Neighbours(int[] cell)
    {
        int[][] shift_8 = new int[][] {
            new int[] {0, 1},
            new int[] {1, 1},
            new int[] {1, 0},
            new int[] {1, -1},
            new int[] {0, -1},
            new int[] {-1, -1},
            new int[] {-1, 0},
            new int[] {-1, 1}
        };

        List<int[]> neighbours_8 = new List<int[]>();
        
        // 8 valid neighbours
        for (int i = 0; i < shift_8.Length; i++)
        {
            int row8 = cell[0] + shift_8[i][0];
            int col8 = cell[1] + shift_8[i][1];
            int[] neighbour = new int[] {row8, col8};
            neighbours_8.Add(neighbour);

            //exclude ones outside the map and with present obstacles
            if (row8 < 0 || col8 < 0 || row8 >= columns || col8 >= rows || map[row8, col8] == 1)
            {
                neighbours_8.Remove(neighbour);
            }
        }
        
        return neighbours_8;
    }

    public int[,] GetICEMatrix(int[,] ICEmatrix)
    {
        int counter = 0;

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (map[x, y] == 1)
                {
                    ICEmatrix[x, y] = int.MaxValue;
                    continue;
                }
                else
                {
                    try
                    {
                        if (map[x, y+1] == 1 || y+1 < 0 || x < 0 || x >= columns || y+1 >= rows)
                            counter++;
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        counter++;
                    }

                    try
                    {
                        if (map[x+1, y] == 1 || y < 0 || x+1 < 0 || x+1 >= columns || y >= rows)
                            counter++;
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        counter++;
                    }

                    try
                    {
                        if (map[x, y-1] == 1 || y-1 < 0 || x < 0 || x >= columns || y-1 >= rows)
                            counter++;
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        counter++;
                    }

                    try
                    {
                        if (map[x-1, y] == 1 || y < 0 || x-1 < 0 || x-1 >= columns || y >= rows)
                            counter++;
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        counter++;
                    }

                    try
                    {
                        if (map[x-1, y+1] == 1 || y+1 < 0 || x-1 < 0 || x-1 >= columns || y+1 >= rows)
                            counter++;
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        counter++;
                    }

                    try
                    {
                        if (map[x-1, y-1] == 1 || y-1 < 0 || x-1 < 0 || x-1 >= columns || y-1 >= rows)
                            counter++;
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        counter++;
                    }

                    try
                    {
                        if (map[x+1, y+1] == 1 || y+1 < 0 || x+1 < 0 || x+1 >= columns || y+1 >= rows)
                            counter++;
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        counter++;
                    }

                    try
                    {
                        if (map[x+1, y-1] == 1 || y-1 < 0 || x+1 < 0 || x+1 >= columns || y-1 >= rows)
                            counter++;
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        counter++;
                    }

                    ICEmatrix[x, y] = counter;
                    counter = 0;
                }
            }
        }

        return ICEmatrix;
    }

    List<int[]> CCP(int[] start, int[] goal)
    {
        var path = new List<int[]>();
        var que = new List<int[]>();
        var steps = 0;

        int[,] visited = new int[columns, rows];        

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                if (map[i,j] == 1)
                {
                    visited[i,j] = 1;
                }
            }
        }

        int[,] ICEmatrix = new int[columns, rows];

        int[,,] Parents = new int[columns, rows, 2];
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    Parents[i, j, k] = -1;
                }
            }
        }

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        que.Add(start);
        int[] current_Cell = start;
        ICEmatrix = GetICEMatrix(ICEmatrix);
        int[,] PCEmatrix = (int[,])ICEmatrix.Clone();
        System.Console.WriteLine(PCEmatrix);
        var f_next_cell = start;

        while (!IsIdentical(visited)) {
            Vector2 queu = new Vector2((int)que[0][0],(int)que[0][1]);
            
            que.RemoveAt(0);

            Vector2 queu_copy = queu;


            steps = steps + 1;

            List<int[]> neighbours_8 = new List<int[]>();
            neighbours_8 = Neighbours(current_Cell);

            for (int i = 0; i < neighbours_8.Count; i++)
            {
            PCEmatrix[neighbours_8[i][0], neighbours_8[i][1]] += 1;
            }

            int[] next_cell = neighbours_4(PCEmatrix, current_Cell, visited, f_next_cell, Parents);
            
            current_Cell = next_cell;
            visited[(int)queu.x,(int)queu.y] =(int)1;

            que.Add(next_cell);
            path.Add(next_cell);

            Parents[next_cell[0], next_cell[1], 1] = (int)queu_copy.x;
            Parents[next_cell[0], next_cell[1], 0] = (int)queu_copy.y;
        }

        stopwatch.Stop();
        double runtime = stopwatch.Elapsed.TotalSeconds;
        print("Time taken: "+runtime);
        string p_now = "";
        foreach(var p in path){
            p_now += p[0];
            p_now += " ";
            p_now += p[1];
            p_now += '\n';
        }
            // Debug.Log(p_now);
            // print(p_now);
            return path;
    }



            ////////////////
            ///////STC//////
            ////////////////



    List<int[]> STC(int[] start)
    {
        int[,] visitTimes = new int[mergedMapHeight, mergedMapWidth];
        visitTimes[start[0], start[1]] = 1;

        List<int[]> route = new List<int[]>();

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        route = PerformSpanningTreeCoverage(start, visitTimes, route);

        List<int[]> path = new List<int[]>();
        for (int idx = 0; idx < route.Count - 1; idx++)
        {
            int dp = Math.Abs(route[idx][0] - route[idx + 1][0]) +
                    Math.Abs(route[idx][1] - route[idx + 1][1]);
            if (dp == 0)
            {
                path.Add(GetRoundTripPath(route[idx - 1], route[idx]));
            }
            else if (dp == 1)
            {
                path.Add(Move(route[idx], route[idx + 1]));
            }
            else if (dp == 2)
            {
                int[] midNode = GetIntermediateNode(route[idx], route[idx + 1]);
                path.Add(Move(route[idx], midNode));
                path.Add(Move(midNode, route[idx + 1]));
            }
            else
            {
                Console.WriteLine("Adjacent path node distance larger than 2");
                Environment.Exit(1);
            }
        }

        // List<int[]> main_path = new List<int[]>();
        // // List<int[]> path = new List<int[]>();

        // foreach (int[] coordinates in path)
        // {
        //     foreach (int[] cell in coordinates)
        //     {
        //         main_path.Add(cell);
        //     }
        // }


        List<int[]> main_path = new List<int[]>();
        List<int[]> adjusted_path = new List<int[]>();
        foreach (var p in path)
        {
            print(p);
            int[] nodeA = new int[2];
            nodeA[0] = p[0];
            nodeA[1] = p[1];
            main_path.Add(nodeA);
            int[] nodeB = new int[2];
            nodeB[0] = p[2];
            nodeB[1] = p[3];
            main_path.Add(nodeB);
        }


        for (int i = 0; i < main_path.Count - 1; i++)
        {
            adjusted_path.Add(main_path[i]);

            // // if (Math.Abs(main_path[i][0] - main_path[i + 1][0]) == 1 && Math.Abs(main_path[i][1] - main_path[i + 1][1]) == 1)
            // {
            //     // Console.WriteLine(main_path[i][0] + " " + main_path[i][1] + " " + main_path[i + 1][0] + " " + main_path[i + 1][1]);
            // }

            if ((main_path[i + 1][0] - main_path[i][0]) == 1 && (main_path[i + 1][1] - main_path[i][1]) == 1)
            {
                adjusted_path.Add(new int[] { main_path[i][0] + 1, main_path[i][1] });
            }
            else if ((main_path[i + 1][0] - main_path[i][0]) == -1 && (main_path[i + 1][1] - main_path[i][1]) == 1)
            {
                adjusted_path.Add(new int[] { main_path[i][0], main_path[i][1] + 1 });
            }
            else if ((main_path[i + 1][0] - main_path[i][0]) == -1 && (main_path[i + 1][1] - main_path[i][1]) == -1)
            {
                adjusted_path.Add(new int[] { main_path[i][0] - 1, main_path[i][1] });
            }
            else if ((main_path[i + 1][0] - main_path[i][0]) == 1 && (main_path[i + 1][1] - main_path[i][1]) == -1)
            {
                adjusted_path.Add(new int[] { main_path[i][0], main_path[i][1] - 1 });
            }
        }

        // List<int[]> path = new List<int[]>();

        // for (int i = 0; i < path.Count; i += 2)
        // {
        //     path.Add(new int[] { path[i][0], path[i][1], path[i + 1][0], path[i + 1][1] });
        // }

        // Console.WriteLine(adjusted_path);


        stopwatch.Stop();
        double runtime = stopwatch.Elapsed.TotalSeconds;
        print("Time taken: "+runtime);

        // string p_now = "";
        // foreach(var p in path){
        //     p_now += p[0];
        //     p_now += " ";
        //     p_now += p[1];
        //     p_now += '\n';
        //     p_now += p[2];
        //     p_now += " ";
        //     p_now += p[3];
        //     p_now += '\n';
        // }
        //     // Debug.Log(p_now);
        //     // print(p_now);
        return adjusted_path;
    }

    public List<int[]> PerformSpanningTreeCoverage(int[] currentNode, int[,] visitTimes, List<int[]> route)
    {
        // local function to check if a node is valid
        bool IsValidNode(int i, int j)
        {
            bool isIValidBounded = 0 <= i && i < mergedMapHeight;
            bool isJValidBounded = 0 <= j && j < mergedMapWidth;
            if (isIValidBounded && isJValidBounded)
            {
                // free only when the 4 sub-cells are all free
                return (occMap[2 * i, 2 * j]==0) &&
                    (occMap[2 * i + 1, 2 * j]==0) &&
                    (occMap[2 * i, 2 * j + 1]==0) &&
                    (occMap[2 * i + 1, 2 * j + 1]==0);
            }
            return false;
        }

        // counter-clockwise neighbor finding order
        int[][] order = new int[][] { new int[] {1, 0}, new int[] {0, 1}, new int[] {-1, 0}, new int[] {0, -1} };

        bool found = false;
        route.Add(currentNode);
        foreach (int[] inc in order)
        {
            int ni = currentNode[0] + inc[0];
            int nj = currentNode[1] + inc[1];
            if (IsValidNode(ni, nj) && visitTimes[ni, nj] == 0)
            {
                int[] neighborNode = {ni, nj};
                int[] temp_edge = new int[4];
                temp_edge[0] = currentNode[0];
                temp_edge[1] = currentNode[1]; 
                temp_edge[2] = neighborNode[0];
                temp_edge[3] = neighborNode[1];
                edge.Add(temp_edge);
                found = true;
                visitTimes[ni, nj] += 1;
                route = PerformSpanningTreeCoverage(neighborNode, visitTimes, route);
            }
        }

        // backtrace route from node with neighbors all visited
        // to first node with unvisited neighbor
        if (!found)
        {
            bool hasNodeWithUnvisitedNgb = false;
            for (int i = route.Count - 1; i >= 0; i--)
            {
                var node = route[i];
                // drop nodes that have been visited twice
                if (visitTimes[node[0], node[1]] == 2)
                    continue;

                visitTimes[node[0], node[1]] += 1;
                route.Add(node);

                foreach (int[] inc in order)
                {
                    int ni = node[0] + inc[0];
                    int nj = node[1] + inc[1];
                    if (IsValidNode(ni, nj) && visitTimes[ni, nj] == 0)
                    {
                        hasNodeWithUnvisitedNgb = true;
                        break;
                    }
                }

                if (hasNodeWithUnvisitedNgb)
                    break;
            }
        }

        return route;
    }

    public int[] Move(int[] p, int[] q) {
        string direction = GetVectorDirection(p, q);
        int[] newP = p;
        int[] newQ = q;
        // move east
        if (direction == "E") {
            newP = GetSubNode(p, "SE");
            newQ = GetSubNode(q, "SW");
        }
        // move west
        else if (direction == "W") {
            newP = GetSubNode(p, "NW");
            newQ = GetSubNode(q, "NE");
        }
        // move south
        else if (direction == "S") {
            newP = GetSubNode(p, "SW");
            newQ = GetSubNode(q, "NW");
        }
        // move north
        else if (direction == "N") {
            newP = GetSubNode(p, "NE");
            newQ = GetSubNode(q, "SE");
        }
        else {
            Console.WriteLine("move direction error...");
            Environment.Exit(1);
        }
        int[] output = new int[4];
        output[0] = newP[0];
        output[2] = newQ[0];
        output[1] = newP[1];
        output[3] = newQ[1];
        return output;
    }

    public int[] GetRoundTripPath(int[] last, int[] pivot) {
        string direction = GetVectorDirection(last, pivot);
        int[] output = new int[4];
        int[] newP = new int[2];
        int[] newQ = new int[2];
        if (direction == "E") {
            newP = GetSubNode1(pivot, "SE");
            newQ = GetSubNode1(pivot, "NE");

        }
        else if (direction == "S") {
            newP = GetSubNode1(pivot, "SW");
            newQ = GetSubNode1(pivot, "SE");
            
        }
        else if (direction == "W") {
            newP = GetSubNode1(pivot, "NW");
            newQ = GetSubNode1(pivot, "SW");
        }
        else if (direction == "N") {
            newP = GetSubNode1(pivot, "NE"); 
            newQ = GetSubNode1(pivot, "NW");
        }
        else {
            Console.WriteLine("get_round_trip_path: last->pivot direction error.");
            Environment.Exit(1);
            return null; // This line will never be executed, but is included to avoid a compiler error.
        }
        output[0] = newP[0];
        output[2] = newQ[0];
        output[1] = newP[1];
        output[3] = newQ[1];
        return output;
    }

    private string GetVectorDirection(int[] p, int[] q)
    {
        // East
        if (p[0] == q[0] && p[1] < q[1])
        {
            return "E";
        }
        // West
        else if (p[0] == q[0] && p[1] > q[1])
        {
            return "W";
        }
        // South
        else if (p[0] < q[0] && p[1] == q[1])
        {
            return "S";
        }
        // North
        else if (p[0] > q[0] && p[1] == q[1])
        {
            return "N";
        }
        else
        {
            Console.WriteLine("get_vector_direction: Only E/W/S/N direction supported.");
            Environment.Exit(1);
            return null; // This return statement is added to satisfy the return type requirement, but it won't be reached due to the preceding exit statement
        }
    }

    private int[] GetSubNode(int[] node, string direction)
    {
        int[] subNode = new int[2];
        if (direction == "SE")
        {
            subNode[0] = 2 * node[0] + 1;
            subNode[1] = 2 * node[1] + 1;
        }
        else if (direction == "SW")
        {
            subNode[0] = 2 * node[0] + 1;
            subNode[1] = 2 * node[1];
        }
        else if (direction == "NE")
        {
            subNode[0] = 2 * node[0];
            subNode[1] = 2 * node[1] + 1;
        }
        else if (direction == "NW")
        {
            subNode[0] = 2 * node[0];
            subNode[1] = 2 * node[1];
        }
        else
        {
            Console.WriteLine("get_sub_node: sub-node direction error.");
            Environment.Exit(1);
        }

        return subNode;
    }

    private int[] GetSubNode1(int[] node, string direction)
    {
        int[] subNode = new int[2];
        if (direction == "SE")
        {
            subNode[0] = 2 * node[0];
            subNode[1] = 2 * node[1];
        }
        else if (direction == "SW")
        {
            subNode[0] = 2 * node[0];
            subNode[1] = 2 * node[1];
        }
        else if (direction == "NE")
        {
            subNode[0] = 2 * node[0];
            subNode[1] = 2 * node[1];
        }
        else if (direction == "NW")
        {
            subNode[0] = 2 * node[0];
            subNode[1] = 2 * node[1];
        }
        else
        {
            Console.WriteLine("get_sub_node: sub-node direction error.");
            Environment.Exit(1);
        }

        return subNode;
    }

    private int[] GetIntermediateNode(int[] p, int[] q)
    {
        HashSet<int[]> pNgb = new HashSet<int[]>(new IntArrayEqualityComparer());
        HashSet<int[]> qNgb = new HashSet<int[]>(new IntArrayEqualityComparer());

        // int total_elements = edge.GetLength(0);

        foreach(int[] edge_node in edge){// new int[4];
            // edge_node[0] = edge[i,0];
            // edge_node[1] = edge[i,1];
            // edge_node[2] = edge[i,2];
            // edge_node[3] = edge[i,3];
            if (edge_node[0] == p[0] && edge_node[1] == p[1])
            {
                pNgb.Add(new int[] { edge_node[2], edge_node[3] });
            }
            if (edge_node[2] == p[0] && edge_node[3] == p[1])
            {
                pNgb.Add(new int[] { edge_node[0], edge_node[1] });
            }
            if (edge_node[0] == q[0] && edge_node[1] == q[1])
            {
                qNgb.Add(new int[] { edge_node[2], edge_node[3] });
            }
            if (edge_node[2] == q[0] && edge_node[3] == q[1])
            {
                qNgb.Add(new int[] { edge_node[0], edge_node[1] });
            }
        }

        HashSet<int[]> itsc = new HashSet<int[]>(pNgb);
        itsc.IntersectWith(qNgb);

        if (itsc.Count == 0)
        {
            Console.WriteLine("get_intermediate_node: no intermediate node between " + p[0] + "," + p[1] + " and " + q[0] + "," + q[1]);
            Environment.Exit(1);
            return null;
        }
        else if (itsc.Count == 1)
        {
            return itsc.First();
        }
        else
        {
            Console.WriteLine("get_intermediate_node: more than 1 intermediate node between " + p[0] + "," + p[1] + " and " + q[0] + "," + q[1]);
            Environment.Exit(1);
            return null; // unreachable
        }
    }

    private class IntArrayEqualityComparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[] x, int[] y)
        {
            return x[0] == y[0] && x[1] == y[1];
        }

        public int GetHashCode(int[] obj)
        {
            return obj[0].GetHashCode() ^ obj[1].GetHashCode();
        }
    } 
}