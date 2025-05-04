using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Core.CrossCuttingConcerns.Monitoring;

public class PerformanceMonitor
{
    private readonly ILogger<PerformanceMonitor> _logger;
    private readonly Dictionary<string, Stopwatch> _watches = new();
    
    public PerformanceMonitor(ILogger<PerformanceMonitor> logger)
    {
        _logger = logger;
    }
    
    public void StartMeasure(string operation)
    {
        var watch = new Stopwatch();
        watch.Start();
        _watches[operation] = watch;
    }
    
    public void EndMeasure(string operation)
    {
        if (_watches.TryGetValue(operation, out var watch))
        {
            watch.Stop();
            
            _logger.LogInformation(
                "Performance: {Operation} completed in {ElapsedMs}ms",
                operation,
                watch.ElapsedMilliseconds);
                
            _watches.Remove(operation);
        }
    }
    
    public async Task<T> MeasureAsync<T>(string operation, Func<Task<T>> action)
    {
        try
        {
            StartMeasure(operation);
            return await action();
        }
        finally
        {
            EndMeasure(operation);
        }
    }

    public void MeasureSync(string operation, Action action)
    {
        try
        {
            StartMeasure(operation);
            action();
        }
        finally
        {
            EndMeasure(operation);
        }
    }

    public T MeasureSync<T>(string operation, Func<T> function)
    {
        try
        {
            StartMeasure(operation);
            return function();
        }
        finally
        {
            EndMeasure(operation);
        }
    }
} 