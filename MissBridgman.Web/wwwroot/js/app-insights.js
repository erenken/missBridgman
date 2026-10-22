(() => {
    const connectionString = document.querySelector(
        'meta[name="application-insights-connection-string"]')?.content;
    const ApplicationInsights = window.Microsoft?.ApplicationInsights?.ApplicationInsights;

    if (!connectionString || !ApplicationInsights) return;

    const appInsights = new ApplicationInsights({
        config: {
            connectionString,
            enableAutoRouteTracking: true,
            enableCorsCorrelation: true,
            distributedTracingMode: 2
        }
    });

    appInsights.loadAppInsights();
    appInsights.trackPageView();
    window.appInsights = appInsights;
})();
