#nullable enable 
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorWasmLife.Shared
{
    /// <summary>
    /// an abstract class that partially implements the ILifeBoard interface
    /// </summary>
    abstract public class LifeBoardBase : ILifeBoard
    {
        public virtual int MaxRows { get; }
        public virtual int MaxColumns { get; }

        public int RowCount { get; set; } = 5;
        public int ColumnCount { get; set; } = 5;
        public int GenerationCount { get; set; } = 0;

        abstract public bool this[int row, int col] { get; set; }

        abstract public ILifeBoard GetCellMatrix(int rowCount, int columnCount,
                                                int generationCount);

        abstract public ILifeBoard NextGeneration(ILifeBoard initial);

        virtual public ILifeBoard SetInitialCells(params string[] initialRows)
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

            if (rowCount > MaxRows || colCount > MaxColumns)
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


        virtual protected int CountNeighbors(ILifeBoard cellValues, int i, int j)
        {
            // wrap to previous row
            int prevRow = i == 0 ? (cellValues.RowCount - 1) : (i - 1);
            int prevCol = j == 0 ? (cellValues.ColumnCount - 1) : (j - 1);
            int nextRow = i == (cellValues.RowCount - 1) ? 0 : (i + 1);
            int nextCol = j == (cellValues.ColumnCount - 1) ? 0 : (j + 1);

            int count = 0;
            count = (cellValues[prevRow, prevCol] ? 1 : 0) +
                (cellValues[prevRow, j] ? 1 : 0) +
                (cellValues[prevRow, nextCol] ? 1 : 0) +
                (cellValues[i, prevCol] ? 1 : 0) +
                (cellValues[i, nextCol] ? 1 : 0) +
                (cellValues[nextRow, prevCol] ? 1 : 0) +
                (cellValues[nextRow, j] ? 1 : 0) +
                (cellValues[nextRow, nextCol] ? 1 : 0);
            return count;
        }
    }

    public class LifeBoardBaseConverter<C> : JsonConverter<C>
    {
        protected const string RowCountName = "rowCount";
        protected const string ColumnCountName = "columnCount";
        protected const string GenerationCountName = "generationCount";
        protected const string CellsName = "cells";

        protected static readonly JsonEncodedText _rowCountName = JsonEncodedText.Encode(RowCountName, encoder: null);
        protected static readonly JsonEncodedText _columnCountName = JsonEncodedText.Encode(ColumnCountName, encoder: null);
        protected static readonly JsonEncodedText _generationCountName = JsonEncodedText.Encode(GenerationCountName, encoder: null);
        protected static readonly JsonEncodedText _cellsName = JsonEncodedText.Encode(CellsName, encoder: null);

        public override C Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, C value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        protected T ReadProperty<T>(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            T k;

            if (typeToConvert != typeof(object) &&
                (options?.GetConverter(typeToConvert) is JsonConverter<T> keyConverter))
            {
                reader.Read();
                k = keyConverter.Read(ref reader, typeToConvert, options);
            }
            else
            {
                k = JsonSerializer.Deserialize<T>(ref reader, options);
            }

            return k;
        }

        protected void WriteProperty<T>(Utf8JsonWriter writer, T value, JsonEncodedText name, JsonSerializerOptions? options)
        {
            Type typeToConvert = typeof(T);

            writer.WritePropertyName(name);

            if (typeToConvert != typeof(object) &&
                (options?.GetConverter(typeToConvert) is JsonConverter<T> keyConverter))
            {
                keyConverter.Write(writer, value, options);
            }
            else
            {
                JsonSerializer.Serialize<T>(writer, value, options);
            }
        }
    }

}
