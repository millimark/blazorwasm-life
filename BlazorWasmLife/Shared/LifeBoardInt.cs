#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorWasmLife.Shared
{
    /// <summary>
    /// an implementation of a life board using an unsigned integer for
    /// each row. Each bit of a row's integer represents a single cell in
    /// that row.
    /// </summary>
    [JsonConverter(typeof(LifeBoardIntConverter))]
    public class LifeBoardInt : LifeBoardBase, ILifeBoard
    {
        private const int MAX_ROWS = 32;
        private const int MAX_COLS = 32;

        internal UInt32[] Cells { get; set; }
        public override int MaxRows { get; } = MAX_ROWS;
        public override int MaxColumns { get; } = MAX_COLS;

        public static int GetMaxRows() { return MAX_ROWS; }
        public static int GetMaxColumns() { return MAX_COLS; }

        public LifeBoardInt(int rowCount, int columnCount, int generationCount,
            UInt32[]? cells)
        {
            if (rowCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rowCount));
            }
            if (columnCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(columnCount));
            }
            if (generationCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(generationCount));
            }
            RowCount = rowCount;
            ColumnCount = columnCount;
            GenerationCount = generationCount;
            Cells = new UInt32[MaxRows];
            if (cells != null)
            {
                var otherCount = cells.Length;
                for (int i = 0; i < MaxRows && i < otherCount; i++)
                {
                    Cells[i] = cells[i];
                }
            }
        }

        public LifeBoardInt()
        {
            Cells = new UInt32[MaxRows];
        }


        override public ILifeBoard GetCellMatrix(int rowCount, int colCount, int generationCount)
        {
            return new LifeBoardInt(rowCount, colCount, generationCount, null);
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

            ILifeBoard cells = new LifeBoardInt(rowCount, colCount, 0, null);

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


        override public ILifeBoard SetInitialCells(params string[] initialRows)
        {

            if (initialRows.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialRows));
            }

            if (initialRows.Any(l => l == null))
            {
                throw new ArgumentOutOfRangeException(nameof(initialRows));
            }

            int rowCount = initialRows.Length;
            int colCount = initialRows.Max(l => l.Length);
            if (colCount == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialRows));
            }

            if (rowCount > GetMaxRows() || colCount > GetMaxColumns())
            {
                throw new ArgumentOutOfRangeException(nameof(initialRows));
            }

            ILifeBoard cells = GetCellMatrix(rowCount, colCount, 0);

            for (int r = 0; r < rowCount; r++)
            {
                var l = initialRows[r];
                if (l.Length < colCount)
                {
                    l = l + new string('0', colCount - l.Length);
                }

                for (int c = 0; c < colCount; c++)
                {
                    cells[r, c] = l[c] == '1' || l[c] == 'X';
                }
            }
            return cells;
        }

        /// <summary>
        /// compute the next generation of the matrix
        /// </summary>
        /// <param name="initial">current generation</param>
        /// <returns>next generation after applying the rules</returns>
        override public ILifeBoard NextGeneration(ILifeBoard initial)
        {
            if (initial == null)
            {
                throw new ArgumentNullException(nameof(initial));
            }

            initial.GenerationCount++;
            ILifeBoard newCells = initial.GetCellMatrix(initial.RowCount,
                                                initial.ColumnCount,
                                                initial.GenerationCount);

            for (int i = 0; i < initial.RowCount; i++)
            {
                for (int j = 0; j < initial.ColumnCount; j++)
                {
                    int countNeighbors = CountNeighbors(initial, i, j);

                    newCells[i, j] =
                                initial[i, j]
                                ? (countNeighbors == 2 || countNeighbors == 3)
                                : (countNeighbors == 3);
                }
            }

            return newCells;
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
                return (Cells[row] & (1 << col)) != 0;
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
                Cells[row] = value ? (Cells[row] | (1u << col))
                                    : (Cells[row] & ~(1u << col));
            }
        }
    }

    public class LifeBoardIntConverter : LifeBoardBaseConverter<LifeBoardInt>
    {
        public override LifeBoardInt Read(
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
            uint[]? e = null;

            while (!rowCountSet || !columnCountSet || !generationCountSet || !cellsSet)
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
                        e = ReadProperty<uint[]>(ref reader, typeToConvert, options);
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

            return new LifeBoardInt(r, c, g, e);

        }

        public override void Write(
            Utf8JsonWriter writer,
            LifeBoardInt value,
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
