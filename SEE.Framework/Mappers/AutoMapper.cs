using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEE.Framework.Mappers
{
    public interface ICustomMapper<TContract, TEntity>
    {
        void Map(TContract contract, TEntity entity);
    }
    public static class AutoMapper<TContract, TEntity>
    {
        public static void Map(TContract contract, TEntity entity)
        {
            var mapper = new Mapper();
            mapper.Map(contract, entity);
        }

        public static void Map(TContract contract, TEntity entity, ICustomMapper<TContract, TEntity> customMapper)
        {
            var mapper = new Mapper();
            mapper.Map(contract, entity);
            customMapper.Map(contract, entity);
        }

        public static void Map(TContract contract, TEntity entity, Action<TContract, TEntity> action)
        {
            var mapper = new Mapper();
            mapper.Map(contract, entity);
            action(contract, entity);
        }

    }
    public static class AutoMapper<TCustomMapper, TContract, TEntity> where TCustomMapper : ICustomMapper<TContract, TEntity>, new()
    {
        public static void Map(TContract contract, TEntity entity)
        {
            new Mapper().Map(contract, entity);
            var mapper = new TCustomMapper();
            mapper.Map(contract, entity);
        }
    }
}
