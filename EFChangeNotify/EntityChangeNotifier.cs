using System;
using System.Data.Entity;
using System.Data.SqlClient;

namespace EFChangeNotify
{
    public class EntityChangeNotifier<TEntity, TDbContext>
        : IDisposable
        where TDbContext : DbContext, new()
        where TEntity : class
    {
        private DbContext _context;
        private String _sql;
        private string _connectionString;

        public event EventHandler<EntityChangeEventArgs<TEntity>> Changed;
        public event EventHandler<NotifierErrorEventArgs> Error;

        public EntityChangeNotifier(String sql)
        {
            _context = new TDbContext();
            _sql = sql;
            _connectionString = _context.Database.Connection.ConnectionString;

            SafeCountDictionary.IncrementConnectionString(_connectionString, x => { SqlDependency.Start(x); });

            RegisterNotification();
        }

        private void RegisterNotification()
        {
            _context = new TDbContext();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(_sql, connection))
                {
                    connection.Open();

                    var sqlDependency = new SqlDependency(command);
                    sqlDependency.OnChange += new OnChangeEventHandler(_sqlDependency_OnChange);

                    // NOTE: You have to execute the command, or the notification will never fire.
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                    }
                }
            }
        }


        private void _sqlDependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (_context == null)
                return;

            if (e.Type == SqlNotificationType.Subscribe || e.Info == SqlNotificationInfo.Error)
            {
                var args = new NotifierErrorEventArgs
                {
                    Reason = e,
                    Sql = _sql
                };

                OnError(args);
            }
            else
            {
                var args = new EntityChangeEventArgs<TEntity>
                {
                    ContinueListening = true
                };

                OnChanged(args);

                if (args.ContinueListening)
                {
                    RegisterNotification();
                }
            }
        }

        protected virtual void OnChanged(EntityChangeEventArgs<TEntity> e)
        {
            if (Changed != null)
            {
                Changed(this, e);
            }
        }

        protected virtual void OnError(NotifierErrorEventArgs e)
        {
            if (Error != null)
            {
                Error(this, e);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
