namespace Chub.ApiExplorer.Web.Models.Queries
{
    using System.Collections.Generic;
    using Chub.ApiExplorer.Web.Interfaces.Queries;
    using Stylelabs.M.Sdk.Contracts.Base;
    using Stylelabs.M.Sdk.Contracts.Querying;
    using Stylelabs.M.Sdk.Contracts.Querying.Generic;
    using Stylelabs.M.Sdk.Models.Querying;

    public class DataSourceQueryResult : QueryResultBase<IDataSource>, IDataSourceQueryResult, IQueryResult<IDataSource>, IQueryResult
    {
        public DataSourceQueryResult(IEnumerable<IDataSource> items, long totalItems, long offset)
            : base(items, totalItems, offset)
        {
        }
    }
}
