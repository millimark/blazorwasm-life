namespace BlazorWasmLife.Shared
{
    /// <summary>
    /// represents a 2-dimensional board representing the state of
    /// a life game
    /// </summary>
    public interface ILifeBoard
    {
        /// <summary>
        /// maximum number of rows this matrix can handle
        /// </summary>
        int MaxRows { get; }

        /// <summary>
        /// maximum number of columns this matrix can handle
        /// </summary>
        int MaxColumns { get; }

        /// <summary>
        /// number of rows in the matrix
        /// </summary>
        int RowCount { get; }
        /// <summary>
        /// number of columns in the matrix
        /// </summary>
        int ColumnCount { get; set; }
        /// <summary>
        /// number of generations this matrix has been through
        /// </summary>
        int GenerationCount { get; set; }

        /// <summary>
        /// returns the state of a cell in the matrix
        /// </summary>
        /// <param name="row">row of the cell, 0-based</param>
        /// <param name="col">column of the cell, 0-based</param>
        /// <returns>true if cell is alive, false if dead</returns>
        bool this[int row, int col] { get; set; }

        /// <summary>
        /// initializes the matrix with an initial configuration
        /// </summary>
        /// <param name="initialRows">array of strings, each representing row in the matrix. 
        /// Each string should consist of a 1 or X where a live cell should
        /// appear, and an space or 0 where a dead cell should appear.</param>
        /// <returns></returns>
        ILifeBoard SetInitialCells(params string[] initialRows);

        /// <summary>
        /// applies the game's rules to a matrix and returns the next
        /// generation
        /// </summary>
        /// <param name="initial">current configuration of a matrix</param>
        /// <returns>matrix with the next configuration after applying the rules</returns>
        ILifeBoard NextGeneration(ILifeBoard initial);

        /// <summary>
        /// creates a new matrix with the given number of rows, columns,
        /// and generation values
        /// </summary>
        /// <param name="rowCount">number of rows to allow</param>
        /// <param name="columnCount">number of columns to allow</param>
        /// <param name="generationCount">generation number</param>
        /// <returns>a new matrix with the given properties, and with all cells dead by default
        /// </returns>
        ILifeBoard GetCellMatrix(int rowCount, int columnCount, int generationCount);

    }
}
