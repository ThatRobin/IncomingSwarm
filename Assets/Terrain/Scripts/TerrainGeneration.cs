using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TerrainGeneration : MonoBehaviour {

    public class Cell {
        public bool visited = false;
        public bool[] status = new bool[4];
    }

    [System.Serializable]
    public class Rule {
        public GameObject room;
        public Vector2Int minPosition;
        public Vector2Int maxPosition;

        public bool obligatory;

        public int ProbabilityOfSpawning(int x, int y) {
            if (x >= minPosition.x && x <= maxPosition.x && y >= minPosition.y && y <= maxPosition.y) { // if the requested coordinate is inside the position limits
                return obligatory ? 2 : 1; // set the probability up
            }
            return 0; // it cannot spawn
        }

    }

    public Vector2Int size;
    public int startPos = 0;
    public GameObject startingRoom;
    public Rule[] rooms;
    public Vector2 offset;

    List<Cell> board;
    List<Cell> bossRooms;

    public void InitializeGeneration(int seed) {
        Random.InitState(seed); // Set up the random seed
        LevelGeneration(); // generate the level
        foreach (NavMeshSurface navMesh in transform.GetComponentsInChildren<NavMeshSurface>()) { // for each nav mesh surface
            navMesh.BuildNavMesh(); // build the nav mesh
        }
        foreach (EnemySpawner navMesh in transform.GetComponentsInChildren<EnemySpawner>()) { // for each spawner
            navMesh.SpawnEnemies(); // spawn the enemies
        }
        QuestManager.ResetQuest(); // reset the quest
    }

    void GenerateDungeon() {
        List<RoomBehaviour> roomBehaviours = new List<RoomBehaviour>();
        for (int i = 0; i < size.x; i++) { // for each room in the grid
            for (int j = 0; j < size.y; j++) {
                Cell currentCell = board[(i + j * size.x)]; // get the current cell of the board
                if (currentCell.visited) { // if that cell has been visited
                    int randomRoom = -1;
                    List<int> availableRooms = new List<int>();

                    for (int k = 0; k < rooms.Length; k++) { // for each room type
                        int p = rooms[k].ProbabilityOfSpawning(i, j); // get the probabilty of this room spawning

                        if (p == 2) { // if this room must spawn
                            randomRoom = k; // set the random room to this rooms value
                            break; // break from the loop
                        } else if (p == 1) { 
                            availableRooms.Add(k); // add this room to the room options
                        }
                    }

                    if (randomRoom == -1) { // if the room has not been set
                        if (availableRooms.Count > 0) { // if there are available rooms
                            randomRoom = availableRooms[Random.Range(0, availableRooms.Count)]; // get a random room from the available rooms
                        } else {
                            randomRoom = 0; // set the random room to the first room in the array
                        }
                    }
                    // create a new room using the generated values and the offsets.
                    var newRoom = Instantiate(rooms[randomRoom].room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.Euler(0f,180f,0f), transform);
                    if (bossRooms.Contains(currentCell)) { // if the current room is a boss room
                        newRoom.GetComponentInChildren<EnemySpawner>().SetBossRoom(true); // set it to be a boss room
                    }
                    RoomBehaviour behaviour = newRoom.GetComponent<RoomBehaviour>(); // get the room behaviour
                    behaviour.UpdateRoom(board[Mathf.FloorToInt(i + j * size.x)].status); // update the room to include its wall status
                    roomBehaviours.Add(behaviour); // add this room to the list of room behaviours

                    if (this.startingRoom == null) this.startingRoom = newRoom; // set the starting room to the first generated room
                    newRoom.name += " " + i + "-" + j; // name the room according to its co-ordinate

                }
            }
        }

    }

    void LevelGeneration() {
        board = new List<Cell>();
        bossRooms = new List<Cell>();

        for (int i = 0; i < size.x; i++) { // iterate over the x and y values of the grid size
            for (int j = 0; j < size.y; j++) {
                board.Add(new Cell()); // add a new cell to the board
            }
        }

        int currentCell = startPos; // set the current cell index to be the starting position index

        Stack<int> path = new Stack<int>();

        int k = 0;

        while (k < 1000) { // while k is less than 1000
            k++; // increment k

            board[currentCell].visited = true; // visit the current cell

            if (currentCell == board.Count - 1) { // if the current cell is equal the the last cell on the board
                break; // stop the loop
            }

            //Check the cell's neighbors
            List<int> neighbors = CheckNeighbours(currentCell); // get a list of neighbour index values

            if (neighbors.Count == 0) { // if there are no neighbours
                if (path.Count == 0) { // if the path has no where to go
                    break; // stop the loop
                } else {
                    if (CheckNeighbours(path.Peek()).Count != 0) { // check the neighbours of the next in the path
                        bossRooms.Add(board[path.Peek()]); // add this room to the boss rooms
                    }
                    currentCell = path.Pop(); // set the current cell to the paths location, and remove it from the stack.
                    
                }
            } else {
                path.Push(currentCell); // add the current cell to the stack

                int newCell = neighbors[Random.Range(0, neighbors.Count)]; // get a random one of the neighbours

                if (newCell > currentCell) { // if the new cell is later in the list than the current cell
                    // down or right
                    if (newCell - 1 == currentCell) {
                        board[currentCell].status[2] = true; // set the walls to be true
                        currentCell = newCell;
                        board[currentCell].status[3] = true; // set the walls to be true
                    } else {
                        board[currentCell].status[1] = true; // set the walls to be true
                        currentCell = newCell;
                        board[currentCell].status[0] = true; // set the walls to be true
                    }
                } else {
                    // up or left
                    if (newCell + 1 == currentCell) {
                        board[currentCell].status[3] = true; // set the walls to be true
                        currentCell = newCell;
                        board[currentCell].status[2] = true; // set the walls to be true
                    } else {
                        board[currentCell].status[0] = true; // set the walls to be true
                        currentCell = newCell;
                        board[currentCell].status[1] = true; // set the walls to be true
                    }
                }

            }
        }
        bossRooms.Add(board[currentCell]); // add the last room to the boss rooms
        GenerateDungeon(); // generate the actual dungeon tiles
    }

    List<int> CheckNeighbours(int cell) {
        List<int> neighbors = new List<int>();

        //check up neighbor
        if (cell - size.x >= 0 && !board[(cell - size.x)].visited) {
            neighbors.Add((cell - size.x));
        }

        //check down neighbor
        if (cell + size.x < board.Count && !board[(cell + size.x)].visited) {
            neighbors.Add((cell + size.x));
        }

        //check right neighbor
        if ((cell + 1) % size.x != 0 && !board[(cell + 1)].visited) {
            neighbors.Add((cell + 1));
        }

        //check left neighbor
        if (cell % size.x != 0 && !board[(cell - 1)].visited) {
            neighbors.Add((cell - 1));
        }

        return neighbors; // return all neighbour cells as their index on the baord.
    }

}
