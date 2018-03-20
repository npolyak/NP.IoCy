using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyIoCContainerTest
{
    class TypeMapCell
    {
        public Type BaseType { get; set; }

        public List<Type> TargetTypes { get; } = new List<Type>();

        public Type GetTargetType() =>
            TargetTypes.LastOrDefault();
    }

    public class IoCContainer
    {
        Dictionary<Type, TypeMapCell> _typeMap = new Dictionary<Type, TypeMapCell>();

        public void Register(Type baseType, Type targetType)
        {
            if (!targetType.SelfOrSuperTypeMatches(baseType))
            {
                throw new Exception($"Cannot assign type {targetType.Name} to {baseType.Name}");
            }

            if (!_typeMap.TryGetValue(baseType, out TypeMapCell typeMapCell))
            {
                typeMapCell = new TypeMapCell();
            }

            typeMapCell.TargetTypes.Add(targetType);
            _typeMap[baseType] = typeMapCell;
        }

        public void Unregister(Type baseType, Type targetType)
        {
            if (_typeMap.TryGetValue(baseType, out TypeMapCell typeMapCell))
            {
                typeMapCell.TargetTypes.Remove(targetType);

                if (typeMapCell.TargetTypes.Count == 0)
                {
                    _typeMap.Remove(baseType);
                }
            }
        }

        public void Unregister(Type baseType)
        {
            _typeMap.Remove(baseType);
        }

        public void Register<TBase, TTarget>()
            where TTarget : TBase
        {
            Register(typeof(TBase), typeof(TTarget));
        }

        public void Unregister<TBase, TTarget>()
        {
            Unregister(typeof(TBase), typeof(TTarget));
        }

        public void Unregister<TBase>() =>
            Unregister(typeof(TBase));

        public TResult GetTargetTypeImpl<TBase, TResult>(Func<IEnumerable<Type>, TResult> resultGetter)
        {
            Type baseType = typeof(TBase);

            if (baseType.IsConstructedGenericType)
            {
                baseType = baseType.GetGenericTypeDefinition();
            }

            if (_typeMap.TryGetValue(baseType, out TypeMapCell cell))
            {
                return resultGetter(cell.TargetTypes);
            }

            return default(TResult);
        }


        public Type GetTargetType<TBase>() =>
            GetTargetTypeImpl<TBase, Type>(targetTypes => targetTypes.Last());

        public IEnumerable<Type> GetAllTargetTypes<TBase>() =>
            GetTargetTypeImpl<TBase, IEnumerable<Type>>(targetTypes => targetTypes);

        public TBase GetTargetObj<TBase>(bool force = false, params object[] args)
        {
            Type targetType = GetTargetType<TBase>();

            if (targetType == null)
            {
                if (force)
                {
                    targetType = typeof(TBase);
                }
                else
                {
                    return default(TBase);
                }
            }
            else
            {

            }

            return (TBase) Activator.CreateInstance(targetType, args);
        }

        public IEnumerable<TBase> GetAllTargetObjs<TBase>()
        {
            IEnumerable<Type> allTargetTypes = GetAllTargetTypes<TBase>();

            IList<TBase> results = new List<TBase>();

            if (allTargetTypes == null)
                return results;

            foreach(Type type in allTargetTypes)
            {
                TBase obj = (TBase)Activator.CreateInstance(type);

                results.Add(obj);
            }

            return results;
        }
    }
}
