﻿using WMS.Backend.Repositories.Interfaces.Magister;
using WMS.Backend.UnitsOfWork.Interfaces.Magister;
using WMS.Share.DTOs;
using WMS.Share.Models.Magister;
using WMS.Share.Responses;

namespace WMS.Backend.UnitsOfWork.Implementations.Magister
{
    public class ProductTypeUnifOfWork : IProductTypeUnitOfWork
    {
        private readonly IProductTypeRepository _repos;
        public ProductTypeUnifOfWork(IProductTypeRepository repos)
        {
            _repos = repos;
        }
        public Task<ActionResponse<ProductType>> GetAsync(long id) => _repos.GetAsync(id);

        public Task<ActionResponse<IEnumerable<ProductType>>> GetAsync() => _repos.GetAsync();

        public Task<ActionResponse<IEnumerable<ProductType>>> GetAsync(PaginationDTO pagination) => _repos.GetAsync(pagination);

        public Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination) => _repos.GetTotalPagesAsync(pagination);

        public Task<ActionResponse<IEnumerable<ProductType>>> GetDeleteAsync() => _repos.GetDeleteAsync();

        public Task<ActionResponse<IEnumerable<ProductType>>> GetDeleteAsync(PaginationDTO pagination) => _repos.GetDeleteAsync(pagination);

        public Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination) => _repos.GetDeleteTotalPagesAsync(pagination);

        public Task<ActionResponse<IEnumerable<ProductType>>> DownloadAsync(PaginationDTO pagination) => _repos.DownloadAsync(pagination);

        public Task<ActionResponse<ProductType>> AddAsync(ProductType model, long Id_Local) => _repos.AddAsync(model, Id_Local);

        public Task<ActionResponse<List<ProductType>>> AddListAsync(List<ProductType> list, long Id_Local) => _repos.AddListAsync(list, Id_Local);

        public Task<ActionResponse<ProductType>> UpdateAsync(ProductType model, long Id_Local) => _repos.UpdateAsync(model, Id_Local);

        public Task<ActionResponse<ProductType>> ActiveAsync(long id, long Id_local) => _repos.ActiveAsync(id, Id_local);

        public Task<ActionResponse<ProductType>> DeleteAsync(long id, long Id_local) => _repos.DeleteAsync(id, Id_local);

        public Task<ActionResponse<ProductType>> DeleteFullAsync(long id) => _repos.DeleteFullAsync(id);
    }
}