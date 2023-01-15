using NP.IoC.CommonImplementations;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NP.IoCy
{
    internal class ResolvingMultiObjCell : IResolvingCell
    {
        public ResolvingCellKind CellKind => ResolvingCellKind.Singleton;

        public Type CellType { get; }

        internal Type ResolvingType { get; }


        public IList Objects { get; }

        private IList<IResolvingCell> Cells { get; } = new List<IResolvingCell>();

        private bool _hasCreated = false;
        public object? GetObj(IObjComposer objectComposer)
        {
            if (!_hasCreated)
            {
                foreach (var subCell in Cells)
                {
                    object? subObj = subCell.GetObj(objectComposer);

                    if (subObj == null)
                        continue;

                    if (subObj is IEnumerable enumerable)
                    {
                        foreach (object? obj in enumerable)
                        {
                            if (obj != null)
                            {
                                Objects.Add(obj);
                            }
                        }
                    }
                    else
                    {
                        Objects.Add(subObj);
                    }
                }

                _hasCreated = true;
            }

            return Objects;
        }

        public ResolvingMultiObjCell(Type cellType)
        {
            CellType = cellType;

            var listResolvingType = typeof(List<>).MakeGenericType(CellType);

            ResolvingType = typeof(IEnumerable<>).MakeGenericType(CellType);

            Objects = (IList) Activator.CreateInstance(listResolvingType)!;
        }

        public void AddCell(IResolvingCell newCell, Type newCellResolvingType)
        {
            bool canAddCell = false;
            if (this.CellType.IsAssignableFrom(newCellResolvingType))
            {
                canAddCell = true;
            }
            else 
            {
                if (ResolvingType.IsAssignableFrom(newCellResolvingType))
                {
                    canAddCell = true;
                }
            }

            if (canAddCell)
            {
                Cells.Add(newCell);
            }
            else
            {
                throw new Exception($"Cannot add a cell with resolving type '{newCellResolvingType.Name}' to a multicell with cellType '{ResolvingType}' because of the type mismatch");
            }
        }
    }
}
