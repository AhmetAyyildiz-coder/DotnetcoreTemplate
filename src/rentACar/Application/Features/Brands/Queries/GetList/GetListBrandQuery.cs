using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Pipelines.Caching;
using Core.Application.Requests;
using Core.Application.Responses;
using Core.CrossCuttingConcerns.Logging.Serilog;
using Core.Persistence.Paging;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Features.Brands.Queries.GetList;

public class GetListBrandQuery : IRequest<GetListResponse<GetListBrandListItemDto>>, ICachableRequest
{
    public PageRequest PageRequest { get; set; }

    public bool BypassCache { get; }

    public string CacheKey => $"GetListBrands({PageRequest.Page},{PageRequest.PageSize})";
    public string CacheGroupKey => "GetBrands";

    public TimeSpan? SlidingExpiration { get; }

    public class GetListBrandQueryHandler : IRequestHandler<GetListBrandQuery, GetListResponse<GetListBrandListItemDto>>
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;
        private readonly LoggerServiceBase _logger;

        public GetListBrandQueryHandler(IBrandRepository brandRepository, IMapper mapper, LoggerServiceBase logger)
        {
            _brandRepository = brandRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GetListResponse<GetListBrandListItemDto>> Handle(GetListBrandQuery request, CancellationToken cancellationToken)
        {
            
            _logger.Info("tedst");
            IPaginate<Brand> brands = await _brandRepository.GetListAsync(index: request.PageRequest.Page, size: request.PageRequest.PageSize
, cancellationToken: cancellationToken);
            var mappedBrandListModel = _mapper.Map<GetListResponse<GetListBrandListItemDto>>(brands);
            return mappedBrandListModel;
        }
    }
}
