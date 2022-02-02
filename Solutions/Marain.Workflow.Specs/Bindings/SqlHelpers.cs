// <copyright file="SqlHelpers.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Bindings
{
    using Microsoft.Data.SqlClient;
    using Microsoft.SqlServer.Dac;
    using Microsoft.SqlServer.Management.Common;
    using Microsoft.SqlServer.Management.Smo;

    /// <summary>
    /// Helpers for setting up SQL databases.
    /// </summary>
    public static class SqlHelpers
    {
        /// <summary>
        /// Set up a database for a given <see cref="SqlConnection"/> using a DACPAC.
        /// </summary>
        /// <param name="connectionString">The sql connection string for the server.</param>
        /// <param name="targetDatabaseName">The target database name for the server.</param>
        /// <param name="dacpacPath">The path to the DACPAC.</param>
        public static void SetupDatabaseFromDacPac(string connectionString, string targetDatabaseName, string dacpacPath)
        {
            var services = new DacServices(connectionString);
            using var package = DacPackage.Load(dacpacPath);
            services.Publish(package, targetDatabaseName, new PublishOptions { DeployOptions = new DacDeployOptions { CreateNewDatabase = true }, GenerateDeploymentReport = false });
        }

        /// <summary>
        /// Delete a database if it exists.
        /// </summary>
        /// <param name="connectionString">The connection string to the server.</param>
        /// <param name="database">The database to delete if it exists.</param>
        public static void DeleteDatabase(string connectionString, string database)
        {
            using var connection = new SqlConnection(connectionString);
            var serverConnection = new ServerConnection(connection);
            var server = new Server(serverConnection);

            Database databaseInstance = server.Databases[database];
            if (databaseInstance != null)
            {
                server.KillDatabase(database);
            }
        }
    }
}