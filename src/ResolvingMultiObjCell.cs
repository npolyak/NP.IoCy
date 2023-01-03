﻿using NP.IoC.CommonImplementations;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NP.IoCy
{
    internal class ResolvingMultiObjCell : IResolvingCell
    {
        public ResolvingCellType CellType => ResolvingCellType.Singleton;

        public Type ResolvingType { get; }

        public IList<object> Objects { get; } = new List<object>();

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

        public ResolvingMultiObjCell(Type resolvingType)
        {
            ResolvingType = resolvingType;
        }

        public void AddCell(IResolvingCell cell, Type resolvingType)
        {
            bool canAddCell = false;
            if (this.ResolvingType.IsAssignableFrom(resolvingType))
            {
                canAddCell = true;
            }
            else 
            {
                Type realType = 
                    typeof(IEnumerable<>).MakeGenericType(ResolvingType);

                if (realType.IsAssignableFrom(resolvingType))
                {
                    canAddCell = true;
                }
            }

            if (canAddCell)
            {
                Cells.Add(cell);
            }
            else
            {
                throw new Exception($"Cannot add a cell with resolving type '{resolvingType.Name}' to a multicell with resolvingType '{ResolvingType}' because of the type mismatch");
            }
        }
    }
}
