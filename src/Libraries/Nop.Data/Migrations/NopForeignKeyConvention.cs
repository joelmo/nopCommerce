﻿using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Model;
using FluentMigrator.Runner.Conventions;

namespace Nop.Data.Migrations
{
    /// <summary>
    /// Convention for the default naming of a foreign key
    /// </summary>
    public class NopForeignKeyConvention : IForeignKeyConvention
    {
        #region Fields

        private readonly INopDataProvider _dataProvider;
        private readonly IMigrationContext _context;

        #endregion

        #region Ctor

        public NopForeignKeyConvention(INopDataProvider dataProvider, IMigrationContext context)
        {
            _dataProvider = dataProvider;
            _context = context;
        }

        #endregion

        #region Utils

        /// <summary>
        /// Gets the default name of a foreign key
        /// </summary>
        /// <param name="foreignKey">The foreign key definition</param>
        /// <returns>Name of a foreign key</returns>
        private string GetForeignKeyName(ForeignKeyDefinition foreignKey)
        {
            var foreignColumns = string.Join('_', foreignKey.ForeignColumns);
            var primaryColumns = string.Join('_', foreignKey.PrimaryColumns);

            var keyName = _dataProvider.GetForeignKeyName(foreignKey.ForeignTable, foreignColumns, foreignKey.PrimaryTable, primaryColumns);
            
            if (_context.QuerySchema.ConstraintExists(foreignKey.ForeignTableSchema, foreignKey.ForeignTable, keyName))
                keyName = _dataProvider.GetForeignKeyName(foreignKey.ForeignTable, foreignColumns, foreignKey.PrimaryTable, primaryColumns, false);

            return keyName;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Applies a convention to a FluentMigrator.Expressions.IForeignKeyExpression
        /// </summary>
        /// <param name="expression">The expression this convention should be applied to</param>
        /// <returns>The same or a new expression. The underlying type must stay the same</returns>
        public IForeignKeyExpression Apply(IForeignKeyExpression expression)
        {
            if (string.IsNullOrEmpty(expression.ForeignKey.Name))
                expression.ForeignKey.Name = GetForeignKeyName(expression.ForeignKey);

            return expression;
        }

        #endregion
    }
}