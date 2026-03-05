namespace GameStateService.Models;

public class Board
{
    private readonly Cell[,] _grid;

    public Board()
    {
        _grid = new Cell[3, 3];
        for (int r = 0; r < 3; r++)
            for (int c = 0; c < 3; c++)
                _grid[r, c] = new Cell(r, c, PlayerMark.None);
    }

    public bool IsEmpty(int row, int col) => _grid[row, col].Mark == PlayerMark.None;

    public Cell GetCell(int row, int col) => _grid[row, col];

    public void SetCell(int row, int col, PlayerMark mark)
    {
        _grid[row, col] = new Cell(row, col, mark);
    }

    public IEnumerable<Cell> GetAllCells()
    {
        for (int r = 0; r < 3; r++)
            for (int c = 0; c < 3; c++)
                yield return _grid[r, c];
    }
}


