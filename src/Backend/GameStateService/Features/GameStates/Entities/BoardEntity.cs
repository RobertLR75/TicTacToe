namespace GameStateService.Features.GameStates.Entities;

public sealed class Board
{
    private readonly CellEntity[,] _grid;

    public Board()
    {
        _grid = new CellEntity[3, 3];
        for (int r = 0; r < 3; r++)
            for (int c = 0; c < 3; c++)
                _grid[r, c] = new CellEntity(r, c, PlayerMark.None);
    }

    public bool IsEmpty(int row, int col) => _grid[row, col].Mark == PlayerMark.None;

    public CellEntity GetCell(int row, int col) => _grid[row, col];

    public void SetCell(int row, int col, PlayerMark mark)
    {
        _grid[row, col] = new CellEntity(row, col, mark);
    }

    public IEnumerable<CellEntity> GetAllCells()
    {
        for (int r = 0; r < 3; r++)
            for (int c = 0; c < 3; c++)
                yield return _grid[r, c];
    }
}

