using MediatR;
using Microsoft.Extensions.Logging;
using Core.CrossCuttingConcerns.Monitoring;

namespace Core.Application.Pipelines.Performance;

public class PerformanceMonitorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly PerformanceMonitor _performanceMonitor;
    private readonly ILogger<PerformanceMonitorBehavior<TRequest, TResponse>> _logger;

    public PerformanceMonitorBehavior(
        PerformanceMonitor performanceMonitor,
        ILogger<PerformanceMonitorBehavior<TRequest, TResponse>> logger)
    {
        _performanceMonitor = performanceMonitor;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;
        _logger.LogInformation("Performance monitoring started for {RequestName}", requestName);
        
        return await _performanceMonitor.MeasureAsync(requestName, async () => await next());
    }
} 