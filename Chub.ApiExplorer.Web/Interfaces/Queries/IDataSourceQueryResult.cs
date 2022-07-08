namespace Chub.ApiExplorer.Web.Interfaces.Queries
{
    using Stylelabs.M.Sdk.Contracts.Base;
    using Stylelabs.M.Sdk.Contracts.Querying;
    using Stylelabs.M.Sdk.Contracts.Querying.Generic;

    public interface IDataSourceQueryResult : IQueryResult<IDataSource>, IQueryResult
    {
    }
}
