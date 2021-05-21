using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace UsAcRe.Web.Server.Tests {
	public class FakeDbSet<TEntity> : DbSet<TEntity>, IQueryable where TEntity : class {
		readonly List<TEntity> items;
		readonly Func<TEntity, object[], bool> predicate;
		public FakeDbSet(Func<TEntity, object[], bool> predicate) {
			items = new List<TEntity>();
			this.predicate = predicate;
		}

		public override EntityEntry<TEntity> Add(TEntity entity) {
			items.Add(entity);
			return entity as EntityEntry<TEntity>;
		}

		public override ValueTask<TEntity> FindAsync(params object[] keyValues) {
			return ValueTask.FromResult(items.FirstOrDefault(x => predicate(x, keyValues)));
		}

		class QueryProvider : IQueryProvider {
			public IQueryable CreateQuery(Expression expression) {
				throw new NotImplementedException();
			}

			public IQueryable<TElement> CreateQuery<TElement>(Expression expression) {
				throw new NotImplementedException();
			}

			public object Execute(Expression expression) {
				throw new NotImplementedException();
			}

			public TResult Execute<TResult>(Expression expression) {
				throw new NotImplementedException();
			}
		}


		IQueryProvider queryProvider = new QueryProvider();
		IQueryProvider IQueryable.Provider { get { return queryProvider; } }
		Type IQueryable.ElementType { get { return typeof(TEntity); } }
		Expression IQueryable.Expression { get { return null; } }
	}
}
