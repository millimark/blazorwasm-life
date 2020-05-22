#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorWasmLife.Shared
{
    [JsonConverter(typeof(LifeBoardBoolConverter))]
    public class LifeBoardBool : LifeBoardBase, ILifeBoard
    {
        private const int MAX_ROWS = 32;
        private const int MAX_COLS = 32;
        /// <summary>
        /// naive representation of a matrix using a jagged array of bool
        /// with row number being the first and column number being the
        /// second index
        /// </summary>
        internal bool[][] Cells { get; set; }

        public override int MaxRows { get; } = MAX_ROWS;
        public override int MaxColumns { get; } = MAX_COLS;
        public static int GetMaxRows() { return MAX_ROWS; }
        public static int GetMaxColumns() { return MAX_COLS; }

        /// <summary>
        /// compute the next generation of the matrix
        /// </summary>
        /// <param name="initial">current generation</param>
        /// <returns>next generation after applying the rules</returns>
        public override ILifeBoard NextGeneration(ILifeBoard initial)
        {
            if (initial == null)
            {
                throw new ArgumentNullException(nameof(initial));
            }

            initial.GenerationCount++;

            ILifeBoard newCells = new LifeBoardBool(initial.RowCount, initial.ColumnCount, initial.GenerationCount, null);
            for (int i = 0; i < initial.RowCount; i++)
            {
                for (int j = 0; j < initial.ColumnCount; j++)
                {
                    int countNeighbors = CountNeighbors(initial, i, j);

                    newCells[i, j] = initial[i, j] ?
                                     (countNeighbors == 2 || countNeighbors == 3)
                                                : (countNeighbors == 3);
                }
            }

            return newCells;
        }

        public LifeBoardBool(int rowCount, int columnCount, int generationCount,
            bool[][]? cells)
        {

            if (rowCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rowCount));
            }
            if (columnCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(columnCount));
            }

            RowCount = rowCount;
            ColumnCount = columnCount;
            GenerationCount = generationCount;
            Cells = new bool[RowCount][];
            for (int i = 0; i < RowCount; i++)
            {
                Cells[i] = new bool[ColumnCount];
            }

            if (cells != null)
            {
                for (int i = 0; i < RowCount; i++)
                {
                    for (int j = 0; j < ColumnCount; j++)
                    {
                        Cells[i][j] = cells[i][j];
                    }
                }
            }
        }

        public LifeBoardBool()
        {
            Cells = new bool[RowCount][];
            for (int i = 0; i < RowCount; i++)
            {
                Cells[i] = new bool[ColumnCount];
            }
        }

        override public ILifeBoard GetCellMatrix(int rowCount, int colCount, int generationCount)
        {
            return new LifeBoardBool(rowCount, colCount, generationCount, null);
        }

        static public ILifeBoard FromPattern(IEnumerable<string> initialRows)
        {
            if (initialRows == null)
            {
                throw new ArgumentNullException(nameof(initialRows));
            }

            if (initialRows.Any(l => l == null))
            {
                throw new ArgumentOutOfRangeException(nameof(initialRows));
            }

            int rowCount = initialRows.Count();
            if (rowCount == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialRows));
            }

            int colCount = initialRows.Max(l => l.Length);
            if (colCount == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialRows));
            }

            if (rowCount > GetMaxRows() || colCount > GetMaxColumns())
            {
                throw new ArgumentOutOfRangeException(nameof(initialRows));
            }

            ILifeBoard cells = new LifeBoardBool(rowCount, colCount, 0, null);

            int rowNum = 0;
            foreach (var row in initialRows)
            {
                var l = row;
                if (l.Length < colCount)
                {
                    l = l + new string('0', colCount - l.Length);
                }

                for (int c = 0; c < colCount; c++)
                {
                    cells[rowNum, c] = l[c] == '1' || l[c] == 'X';
                }
                rowNum++;
            }
            return cells;
        }

        override public bool this[int row, int col]
        {
            get
            {
                if (row < 0 || row >= RowCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(row));
                }
                if (col < 0 || col >= ColumnCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(col));
                }
                return Cells[row][col];
            }
            set
            {
                if (row < 0 || row >= RowCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(row));
                }
                if (col < 0 || col >= ColumnCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(col));
                }
                Cells[row][col] = value;
            }
        }
    }
    public class LifeBoardBoolConverter : LifeBoardBaseConverter<LifeBoardBool>
    {

        public override LifeBoardBool Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            // Get the first property.
            bool rowCountSet = false;
            bool columnCountSet = false;
            bool generationCountSet = false;
            bool cellsSet = false;
            int r = 0, c = 0, g = 0;
            bool[][]? e = null;

            while (!rowCountSet || !columnCountSet ||
                    !generationCountSet || !cellsSet)
            {
                reader.Read();
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                string propertyName = reader.GetString()!;
                switch (propertyName)
                {
                    case RowCountName:
                        r = ReadProperty<int>(ref reader, typeToConvert, options);
                        rowCountSet = true;
                        break;
                    case ColumnCountName:
                        c = ReadProperty<int>(ref reader, typeToConvert, options);
                        columnCountSet = true;
                        break;
                    case GenerationCountName:
                        g = ReadProperty<int>(ref reader, typeToConvert, options);
                        generationCountSet = true;
                        break;
                    case CellsName:
                        e = ReadProperty<bool[][]>(ref reader, typeToConvert, options);
                        cellsSet = true;
                        break;
                    default:
                        throw new JsonException();
                }

            }

            reader.Read();

            if (reader.TokenType != JsonTokenType.EndObject)
            {
                throw new JsonException();
            }

            return new LifeBoardBool(r, c, g, e);

        }

        public override void Write(
            Utf8JsonWriter writer,
            LifeBoardBool value,
            JsonSerializerOptions options)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            writer.WriteStartObject();
            WriteProperty(writer, value.RowCount, _rowCountName, options);
            WriteProperty(writer, value.ColumnCount, _columnCountName, options);
            WriteProperty(writer, value.GenerationCount, _generationCountName, options);
            WriteProperty(writer, value.Cells, _cellsName, options);
            writer.WriteEndObject();
        }
    }
}
